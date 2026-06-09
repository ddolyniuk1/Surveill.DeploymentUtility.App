using Microsoft.Extensions.Caching.Memory;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Surveill.DeploymentUtility.App.Enums;
using Surveill.DeploymentUtility.App.Extensions;
using Surveill.DeploymentUtility.App.Settings;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Interfaces;

namespace Surveill.DeploymentUtility.App.Services;

public class DefaultAzureDevopsPipelineService : IDevopsPipelineService
{
    private readonly AppSettingsManager           _appSettingsManager;
    private readonly IMemoryCache                 _memoryCache;
    private readonly IObservableTaskRunnerService _observableTaskRunnerService;

    private readonly SemaphoreSlim          _workingSemaphore = new(1, 1);
    private          List<BuildDefinition>? _buildDefinitions;
    private          IObservableTask?       _currentFetchTask;

    public DefaultAzureDevopsPipelineService(AppSettingsManager appSettingsManager, IMemoryCache memoryCache, IObservableTaskRunnerService observableTaskRunnerService)
    {
        _appSettingsManager          = appSettingsManager          ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _memoryCache                 = memoryCache                 ?? throw new ArgumentNullException(nameof(memoryCache));
        _observableTaskRunnerService = observableTaskRunnerService ?? throw new ArgumentNullException(nameof(observableTaskRunnerService));

        _appSettingsManager.AppSettingsChanged += AppSettingsManagerOnAppSettingsChanged;
    }

    public async Task<PipelineState> GetPipelineStateForBranchAsync(GitRepositoryBranch repo) => repo.PipelineId != null ? await GetPipelineStatusAsync(repo.PipelineId!.Value) : PipelineState.Unknown;

    public async Task<int?> FindPipelineIdByRepoAsync(string repoName, string branch)
    {
        if (_buildDefinitions == null)
        {
            await ResolveAzureDevopsPipelineDefinitionsAsync();
        }

        return _buildDefinitions?
           .FirstOrDefault(pipeline =>
                               string.Equals(GitRepositoryExtensions.ParseRepoName(pipeline.Repository.Id), repoName)
                            && pipeline.Repository.DefaultBranch.Contains(branch))?.Id;
    }

    private void AppSettingsManagerOnAppSettingsChanged()
    {
        _memoryCache.Remove(nameof(ResolveAzureDevopsPipelineDefinitionsAsync));
    }

    public async Task<IObservableTask?> ResolveAzureDevopsPipelineDefinitionsAsync()
    {
        var isObtained = false;
        try
        {
            isObtained = await _workingSemaphore.WaitAsync(TimeSpan.FromSeconds(1));

            if (!isObtained)
            {
                return _currentFetchTask;
            }

            _currentFetchTask = _observableTaskRunnerService.RunTask(async cancellationToken =>
            {
                var                   azureDevopsPipelineConfiguration = _appSettingsManager.AppSettings!.PipelineConfiguration;
                using var buildClient                      = ResolveClient(azureDevopsPipelineConfiguration);

                _buildDefinitions = await buildClient.GetFullDefinitionsAsync(
                                        project: azureDevopsPipelineConfiguration.Project,
                                        cancellationToken: cancellationToken);
            }, "Fetching Build Definitions", "Resolving pipeline build definitions for auto mapping pipeline ids.");
            await _currentFetchTask.Task;
        }
        finally
        {
            if (isObtained)
            {
                _workingSemaphore.Release();
            }
        }

        return _currentFetchTask;
    }

    private Task<PipelineState> GetPipelineStatusAsync(int pipelineId)
        => _memoryCache.GetOrCreateAsync(nameof(GetPipelineStatusAsync) + pipelineId, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);

            var       azureDevopsPipelineConfiguration = _appSettingsManager.AppSettings!.PipelineConfiguration;
            using var buildClient                      = ResolveClient(azureDevopsPipelineConfiguration);

            var builds = await buildClient.GetBuildsAsync(
                             azureDevopsPipelineConfiguration.Project,
                             [pipelineId],
                             top: 1,
                             queryOrder: BuildQueryOrder.QueueTimeDescending);

            var build = builds.FirstOrDefault();

            if (build is null)
            {
                return PipelineState.Unknown;
            }

            return build.Status switch
            {
                BuildStatus.Completed when build.Result == BuildResult.Failed             => PipelineState.Failed,
                BuildStatus.Completed when build.Result == BuildResult.Canceled           => PipelineState.Canceled,
                BuildStatus.Completed when build.Result == BuildResult.Succeeded          => PipelineState.Succeeded,
                BuildStatus.Completed when build.Result == BuildResult.PartiallySucceeded => PipelineState.Succeeded, // or your own state
                BuildStatus.NotStarted                                                    => PipelineState.NotStarted,
                BuildStatus.InProgress                                                    => PipelineState.InProgress,
                _                                                                         => PipelineState.Unknown
            };
        });

    private static BuildHttpClient ResolveClient(AzureDevopsPipelineConfiguration azureDevopsPipelineConfiguration)
    {
        BuildHttpClient? buildClient = null;
        try
        {
            var creds      = new VssBasicCredential(string.Empty, azureDevopsPipelineConfiguration.Pat);
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{azureDevopsPipelineConfiguration.Organization}"), creds);

            buildClient = connection.GetClient<BuildHttpClient>();
            return buildClient;
        }
        catch
        {
            buildClient?.Dispose();
            throw;
        }
    }
}

public interface IDevopsPipelineService
{
    Task<PipelineState>  GetPipelineStateForBranchAsync(GitRepositoryBranch repo);
    Task<int?>           FindPipelineIdByRepoAsync(string                   repoName, string branch);
}