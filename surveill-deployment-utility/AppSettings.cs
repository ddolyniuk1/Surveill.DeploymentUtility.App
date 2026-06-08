using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Surveill.DeploymentUtility.App;

public class AppSettings
{
    public AzureDevopsPipelineConfiguration PipelineConfiguration { get; set; } = new();
    public GitRepository[]                  GitRepositories       { get; set; } = [];
    
    public string SprintVersion { get; set; }
}

public class GitRepository : INotifyPropertyChanged
{
    public string DisplayName
    {
        get;
        set => SetField(ref field, value);
    } = "Unknown";

    public string RepositoryName
    {
        get;
        set => SetField(ref field, value);
    } = "Unknown";

    public string Directory
    {
        get;
        set => SetField(ref field, value);
    } = string.Empty;

    public GitRepositoryBranch AlphaBranch { get; set; } = new()
    {
        BranchType = BranchType.Alpha,
        Name       = "experimental",
        PipelineId = null
    };

    public GitRepositoryBranch BetaBranch { get; set; } = new()
    {
        BranchType = BranchType.Beta,
        Name       = "master",
        PipelineId = null
    };

    public GitRepositoryBranch ReleaseBranch { get; set; } = new()
    {
        BranchType = BranchType.Release,
        Name       = "release",
        PipelineId = null
    };

    [JsonIgnore]
    public ImmutableDictionary<BranchType, GitRepositoryBranch> AllBranches => field ??= new Dictionary<BranchType, GitRepositoryBranch>
    {
        { BranchType.Alpha, AlphaBranch },
        { BranchType.Beta, BetaBranch },
        { BranchType.Release, ReleaseBranch }
    }.ToImmutableDictionary();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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

public class GitRepositoryBranch : INotifyPropertyChanged
{
    public BranchType BranchType
    {
        get;
        set => SetField(ref field, value);
    }

    public string Name
    {
        get;
        set => SetField(ref field, value);
    } = "default";

    public int? PipelineId
    {
        get;
        set => SetField(ref field, value);
    }

    public void Clone(GitRepositoryBranch other)
    {
        BranchType = other.BranchType;
        Name       = other.Name;
        PipelineId = other.PipelineId;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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

public class AzureDevopsPipelineConfiguration : INotifyPropertyChanged
{
    public string Organization
    {
        get;
        set => SetField(ref field, value);
    } = "edge360";

    public string Project
    {
        get => field;
        set => SetField(ref field, value);
    } = "E360VMS";

    public string? Pat
    {
        get;
        set => SetField(ref field, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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