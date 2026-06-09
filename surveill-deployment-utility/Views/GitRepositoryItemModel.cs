using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Surveill.DeploymentUtility.App.Enums;
using Surveill.DeploymentUtility.App.Services;
using Surveill.DeploymentUtility.App.Settings;

namespace Surveill.DeploymentUtility.App.Views;

public sealed class GitRepositoryItemModel(GitRepository repositoryModel, IDevopsPipelineService devopsPipelineService) : INotifyPropertyChanged
{
    private readonly IDevopsPipelineService _devopsPipelineService = devopsPipelineService ?? throw new ArgumentNullException(nameof(devopsPipelineService));

    public GitRepository Repository { get; } = repositoryModel       ?? throw new ArgumentNullException(nameof(repositoryModel));

    public ImmutableDictionary<BranchType, PipelineState> PipelineStates
    {
        get;
        set => SetField(ref field, value);
    } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task RefreshStates()
    {
        var imm = new Dictionary<BranchType, PipelineState>();
        foreach (var branchType in Enum.GetValues<BranchType>())
        {
            var branches = Repository.AllBranches;
            imm[branchType] = await _devopsPipelineService.GetPipelineStateForBranchAsync(branches[branchType]);
        }

        PipelineStates = imm.ToImmutableDictionary();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}