using Microsoft.Extensions.Caching.Memory;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Surveill.DeploymentUtility.App.Extensions;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Interfaces;

namespace Surveill.DeploymentUtility.App;

public class PipelineStatusService : IPipelineStatusService
{
    private readonly AppSettingsManager           _appSettingsManager;
    private readonly IMemoryCache                 _memoryCache;
    private readonly IObservableTaskRunnerService _observableTaskRunnerService;

    private readonly SemaphoreSlim         _workingSemaphore = new(1, 1);
    private          List<BuildDefinition> _buildDefinitions;
    private          IObservableTask       _currentFetchTask;

    public PipelineStatusService(AppSettingsManager appSettingsManager, IMemoryCache memoryCache, IObservableTaskRunnerService observableTaskRunnerService)
    {
        _appSettingsManager          = appSettingsManager          ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _memoryCache                 = memoryCache                 ?? throw new ArgumentNullException(nameof(memoryCache));
        _observableTaskRunnerService = observableTaskRunnerService ?? throw new ArgumentNullException(nameof(observableTaskRunnerService));

        _appSettingsManager.AppSettingsChanged += AppSettingsManagerOnAppSettingsChanged;
    }

    public async Task<PipelineState> GetPipelineStateForBranchAsync(GitRepository repo, BranchType branchType)
    {
        var id = await FindPipelineIdByRepoAsync(repo.RepositoryName, branchType switch
        {
            BranchType.Alpha   => repo.AlphaBranch.Name,
            BranchType.Beta    => repo.BetaBranch.Name,
            BranchType.Release => repo.ReleaseBranch.Name,
            _                  => throw new ArgumentOutOfRangeException(nameof(branchType), branchType, null)
        });
        return id != null ? await GetPipelineStatusAsync(id.Value) : PipelineState.Unknown;
    }

    public async Task<PipelineState> GetPipelineStateForBranchAsync(GitRepositoryBranch repo) => repo.PipelineId != null ? await GetPipelineStatusAsync(repo.PipelineId!.Value) : PipelineState.Unknown;

    public event Action? IsLoadingChanged;

    public bool IsLoading
    {
        get;
        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            IsLoadingChanged?.Invoke();
        }
    }

    public async Task<int?> FindPipelineIdByRepoAsync(string repoName, string branch)
    {
        // #todo terrible - redesign all of this
        if (_buildDefinitions == null)
        {
            var             task = await ResolveAzureDevopsPipelineDefinitionsAsync();
            await           task!.Task;
        }
        return _buildDefinitions?
              .FirstOrDefault(pipeline => string.Equals(GitRepositoryExtensions.ParseRepoName(pipeline.Repository.Id), repoName) && pipeline.Repository.DefaultBranch.Contains(branch))?.Id;
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

            IsLoading = true;

            _currentFetchTask = _observableTaskRunnerService.RunTask(async cancellationToken =>
            {
                var azureDevopsPipelineConfiguration = _appSettingsManager.AppSettings!.PipelineConfiguration;
                var creds                            = new VssBasicCredential(string.Empty, azureDevopsPipelineConfiguration.Pat);
                var connection                       = new VssConnection(new Uri($"https://dev.azure.com/{azureDevopsPipelineConfiguration.Organization}"), creds);

                using var buildClient = connection.GetClient<BuildHttpClient>();

                // Full definitions include the repository
                _buildDefinitions = await buildClient.GetFullDefinitionsAsync(
                                        project: azureDevopsPipelineConfiguration.Project,
                                        cancellationToken: cancellationToken);
            }, "Fetching Build Definitions", "Resolving pipeline build definitions for auto mapping pipeline ids.");
        }
        finally
        {
            if (isObtained)
            {
                _workingSemaphore.Release();

                IsLoading = false;
            }
        }

        return _currentFetchTask;
    }

    private Task<PipelineState> GetPipelineStatusAsync(int pipelineId)
        => _memoryCache.GetOrCreateAsync(nameof(GetPipelineStatusAsync) + pipelineId, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);

            var org     = _appSettingsManager.AppSettings!.PipelineConfiguration.Organization;
            var project = _appSettingsManager.AppSettings.PipelineConfiguration.Project;
            var pat     = _appSettingsManager.AppSettings.PipelineConfiguration.Pat;

            var creds      = new VssBasicCredential(string.Empty, pat);
            var connection = new VssConnection(new Uri($"https://dev.azure.com/{org}"), creds);

            using var buildClient = connection.GetClient<BuildHttpClient>();

            // Most recent run for this definition
            var builds = await buildClient.GetBuildsAsync(
                             project,
                             new[] { pipelineId },
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
}

public interface IPipelineStatusService
{
    bool                 IsLoading { get; set; }
    Task<PipelineState>  GetPipelineStateForBranchAsync(GitRepository repo, BranchType branchType);
    public event Action? IsLoadingChanged;
    Task<PipelineState>  GetPipelineStateForBranchAsync(GitRepositoryBranch repo);
    Task<int?>           FindPipelineIdByRepoAsync(string                   repoName, string branch);
}