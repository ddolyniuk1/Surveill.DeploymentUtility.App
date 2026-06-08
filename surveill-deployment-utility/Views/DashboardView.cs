using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Surveill.DeploymentUtility.App.Views.Dialogs;
using Terminal.Gui.App;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Extensions;
using Terminal.Gui.Reflect.Interfaces;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Surveill.DeploymentUtility.App.Views;

public class DashboardView : ViewController<FrameView, DashboardViewModel>
{
    private static readonly BranchType[] Branches =
        { BranchType.Alpha, BranchType.Beta, BranchType.Release };

    private readonly IApplication                 _application;
    private readonly AppSettingsManager           _appSettingsManager;
    private readonly IGitCliService               _gitCliService;
    private readonly IObservableTaskRunnerService _observableTaskRunnerService;
    private readonly IViewControllerFactory       _viewControllerFactory;
    private          DataTable                    _data = null!;

    private TableView         _repositories = null!;
    private Label             _statusBar    = null!;
    private TaskLoadingDialog _taskLoadingDialog;

    public DashboardView(AppSettingsManager appSettingsManager,
        IApplication                        application,
        IViewControllerFactory              viewControllerFactory,
        IObservableTaskRunnerService        observableTaskRunnerService,
        IGitCliService                      gitCliService)
    {
        _appSettingsManager          = appSettingsManager          ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _application                 = application                 ?? throw new ArgumentNullException(nameof(application));
        _viewControllerFactory       = viewControllerFactory       ?? throw new ArgumentNullException(nameof(viewControllerFactory));
        _observableTaskRunnerService = observableTaskRunnerService ?? throw new ArgumentNullException(nameof(observableTaskRunnerService));
        _gitCliService               = gitCliService               ?? throw new ArgumentNullException(nameof(gitCliService));
    }

    private GitRepositoryItemModel? Selected
    {
        get
        {
            var cells = _repositories.GetAllSelectedCells().ToImmutableArray();
            if (!cells.Any())
            {
                return null;
            }

            var row = cells.First().Y;
            return row >= 0 && row < ViewModel.Items.Count
                       ? ViewModel.Items[row]
                       : null;
        }
    }

    public override void InitializeComponents()
    {
        _taskLoadingDialog = _viewControllerFactory.Create<TaskLoadingDialog>();
        _taskLoadingDialog.DisposeWith(this);

        Root.Title  = "Deployment Utility — (Ctrl+Q to quit)";
        Root.Width  = Dim.Fill();
        Root.Height = Dim.Fill();

        var menu = new MenuBar
        {
            Menus =
            [
                new MenuBarItem("_Repos",
                [
                    new MenuItem("_Add Repo",    "", () => _ = OnAddAsync()) { Key                   = Key.A.WithCtrl },
                    new MenuItem("_Edit Repo",   "", () => _ = OnEditAsync()) { Key                  = Key.E.WithCtrl },
                    new MenuItem("_Remove Repo", "", () => _ = OnRemoveAsync()) { Key                = Key.D.WithCtrl },
                    new MenuItem("Re_fresh",     "", () => _ = RefreshRepositoryStatusAsync()) { Key = Key.R.WithCtrl }
                ]),
                new MenuBarItem("_Merge",
                [
                    new MenuItem("Alpha → _Beta (all)",   "", () => _ = OnMergeAllAsync(BranchType.Alpha, BranchType.Beta)),
                    new MenuItem("Beta → _Release (all)", "", () => _ = OnMergeAllAsync(BranchType.Beta,  BranchType.Release))
                ]),
                new MenuBarItem("_Pipelines",
                [
                    new MenuItem("_Configure", "", () => _ = ConfigurePipelineSettingsAsync())
                ])
            ]
        };

        _data = new DataTable();
        _data.Columns.Add("Repository", typeof(string));
        _data.Columns.Add("Alpha",      typeof(string));
        _data.Columns.Add("Beta",       typeof(string));
        _data.Columns.Add("Release",    typeof(string));

        _repositories = new TableView
        {
            X             = 0,
            Y             = 1,
            Width         = Dim.Fill(),
            Height        = Dim.Fill(4),
            Table         = new DataTableSource(_data),
            FullRowSelect = true
        };
        _repositories.Style.AlwaysShowHeaders             = true;
        _repositories.Style.ShowHorizontalHeaderUnderline = true;
        _repositories.Style.ColumnStyles.Add(_data.Columns.IndexOf("Repository"), new ColumnStyle { MinWidth = 24 });
        _repositories.KeyDown += (_, e) =>
        {
            if (e.KeyCode == KeyCode.Enter)
            {
                _         = OnEditAsync();
                e.Handled = true;
            }
        };
        var legend = new Label
        {
            X     = 0,
            Y     = Pos.Bottom(_repositories),
            Width = Dim.Fill(),
            Text  = "Status:  ✔ Succeeded  ✖ Failed  ◑ In progress  ⏸ Canceled  · Not started  ? Unknown"
        };

        var addBtn     = new Button { Text = "_Add", X       = 0, Y                         = Pos.Bottom(legend) };
        var editBtn    = new Button { Text = "_Edit", X      = Pos.Right(addBtn)     + 1, Y = Pos.Top(addBtn) };
        var removeBtn  = new Button { Text = "_Remove", X    = Pos.Right(editBtn)    + 1, Y = Pos.Top(addBtn) };
        var refreshBtn = new Button { Text = "Re_fresh", X   = Pos.Right(removeBtn)  + 1, Y = Pos.Top(addBtn) };
        var abBtn      = new Button { Text = "α→β _all", X   = Pos.Right(refreshBtn) + 3, Y = Pos.Top(addBtn) };
        var brBtn      = new Button { Text = "β→rel a_ll", X = Pos.Right(abBtn)      + 1, Y = Pos.Top(addBtn) };

        addBtn.Accepting     += (_, _) => _ = OnAddAsync();
        editBtn.Accepting    += (_, _) => _ = OnEditAsync();
        removeBtn.Accepting  += (_, _) => _ = OnRemoveAsync();
        refreshBtn.Accepting += (_, _) => _ = RefreshRepositoryStatusAsync();
        abBtn.Accepting      += (_, _) => _ = OnMergeAllAsync(BranchType.Alpha, BranchType.Beta);
        brBtn.Accepting      += (_, _) => _ = OnMergeAllAsync(BranchType.Beta,  BranchType.Release);

        _statusBar = new Label
        {
            X     = 0,
            Y     = Pos.Bottom(addBtn),
            Width = Dim.Fill(),
            Text  = "Ready."
        };

        Root.Add(menu, _repositories, legend,
                 addBtn, editBtn, removeBtn, refreshBtn, abBtn, brBtn,
                 _statusBar);
    }

    private Task ConfigurePipelineSettingsAsync()
    {
        var dialog = _viewControllerFactory.Create<ConfigurePipelineSettingsDialog>();
        dialog.ViewModel.Clone(_appSettingsManager.AppSettings!.PipelineConfiguration);
        _ = _application.Run(dialog.Root);

        if (!dialog.Accepted)
        {
            return Task.CompletedTask;
        }

        _appSettingsManager.AppSettings!.PipelineConfiguration = dialog.ViewModel;
        return _appSettingsManager.SaveAsync();
    }

    public override void SetupBindings()
    {
        // Rebuild rows whenever the collection changes.
        ViewModel.Repositories.CollectionChanged += OnRepositoriesChanged;
        RebuildRows();
    }

    private void OnRepositoriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => _application.Invoke(RebuildRows);

    private void RebuildRows()
    {
        _data.Rows.Clear();
        foreach (var item in ViewModel.Items.ToArray())
        {
            var row = _data.Rows.Add(
                !string.IsNullOrEmpty(item.Repository.DisplayName) ? item.Repository.DisplayName : item.Repository.RepositoryName,
                Glyph(item, BranchType.Alpha),
                Glyph(item, BranchType.Beta),
                Glyph(item, BranchType.Release));

            item.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(GitRepositoryItemModel.PipelineStates))
                {
                    _application.Invoke(() => UpdateRow(item));
                }
            };
        }

        _repositories.SetNeedsDraw();
    }

    private void UpdateRow(GitRepositoryItemModel item)
    {
        var i = ViewModel.Items.IndexOf(item);
        if (i < 0 || i >= _data.Rows.Count)
        {
            return;
        }

        _data.Rows[i]["Alpha"]   = Glyph(item, BranchType.Alpha);
        _data.Rows[i]["Beta"]    = Glyph(item, BranchType.Beta);
        _data.Rows[i]["Release"] = Glyph(item, BranchType.Release);
        _repositories.SetNeedsDraw();
    }

    private static string Glyph(GitRepositoryItemModel item, BranchType branch)
    {
        var state = CollectionExtensions.GetValueOrDefault(item.PipelineStates, branch, PipelineState.Unknown);
        return state switch
        {
            PipelineState.Succeeded  => "✔ Succeeded",
            PipelineState.Failed     => "✖ Failed",
            PipelineState.InProgress => "◑ In progress",
            PipelineState.Canceled   => "⏸ Canceled",
            PipelineState.NotStarted => "· Not started",
            _                        => "? Unknown"
        };
    }

    public async Task RefreshRepositoryStatusAsync()
    {
        SetStatus("Refreshing pipeline statuses…");
        foreach (var item in ViewModel.Items)
        {
            await item.RefreshStates();
        }

        SetStatus("Ready.");
    }

    private async Task OnMergeAllAsync(BranchType from, BranchType to)
    {
        var n = MessageBox.Query(_application, "Confirm merge",
                                 $"Merge {from} → {to} for all {ViewModel.Items.Count} repositories?",
                                 "Cancel", "Merge");
        if (n != 1)
        {
            return;
        }

        SetStatus($"Merging {from} → {to} across all repos…");

        var sprintVersion = Version.TryParse(_appSettingsManager.AppSettings!.SprintVersion, out var result) ? result : new Version(0, 0);

        var major = DateTimeOffset.Now.Year - 2000;
        var minor = sprintVersion.Minor;

        var message = to switch
        {
            BranchType.Beta    => $"Automated merge - sprint {major}.{minor} completed.",
            BranchType.Release => $"Automated merge - quarter {(DateTimeOffset.Now.Month + 2) / 3}",
            BranchType.Alpha   => throw new NotSupportedException("Cannot merge to Alpha"),
            _                  => throw new ArgumentOutOfRangeException(nameof(to), to, null)
        };

        _appSettingsManager.AppSettings.SprintVersion = $"{major}.{minor + 1}";
        await _appSettingsManager.SaveAsync();

        foreach (var repo in _appSettingsManager.AppSettings!.GitRepositories)
        {
            _observableTaskRunnerService.RunTask(
                async (progress, token) =>
                {
                    using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(10));

                    try
                    {
                        await _gitCliService.ExecuteAsync(["checkout", repo.AllBranches[from].Name], repo.Directory, cancellationTokenSource.Token);
                        progress.Report(20);
                        await _gitCliService.ExecuteAsync(["pull"], repo.Directory, cancellationTokenSource.Token);
                        progress.Report(40);
                        await _gitCliService.ExecuteAsync(["checkout", repo.AllBranches[to].Name], repo.Directory, cancellationTokenSource.Token);
                        progress.Report(60);
                        await _gitCliService.ExecuteAsync(["merge", "--no-ff", repo.AllBranches[from].Name, "-m", message], repo.Directory, cancellationTokenSource.Token);
                        progress.Report(80);
                        await _gitCliService.ExecuteAsync(["push", "origin", repo.AllBranches[to].Name], repo.Directory, cancellationTokenSource.Token);
                        progress.Report(100);
                    }
                    catch (Exception e)
                    {
                        MessageBox.ErrorQuery(_application, "There was an error executing git commands",
                                              $"{e.GetType().FullName} {e.Message} {e.StackTrace}",
                                              "Confirm");
                    }
                }, $"Merging repository {repo.DisplayName} branch {from} -> {to}", "");
        }

        SetStatus("Ready.");
    }

    private Task TaskFactory(IProgress<int> arg1, CancellationToken arg2) => throw new NotImplementedException();

    private async Task OnAddAsync()
    {
        var dialog = _viewControllerFactory.Create<AddEditRepositoryDialog>();

        var result = _application.Run(dialog.Root);

        _appSettingsManager.AppSettings!.GitRepositories = [.._appSettingsManager.AppSettings.GitRepositories, dialog.ViewModel];
        await _appSettingsManager.SaveAsync();
    }

    private async Task OnEditAsync()
    {
        if (Selected is null)
        {
            return;
        }

        var newArr = _appSettingsManager.AppSettings!
                                        .GitRepositories
                                        .Where(t => t.RepositoryName != Selected.Repository.RepositoryName)
                                        .ToArray();

        var dialog = _viewControllerFactory.Create<AddEditRepositoryDialog>();
        dialog.ViewModel.Copy(Selected.Repository);

        var result = _application.Run(dialog.Root);

        _appSettingsManager.AppSettings.GitRepositories = [..newArr, dialog.ViewModel];
        await _appSettingsManager.SaveAsync();
    }

    private async Task OnRemoveAsync()
    {
        if (Selected is not { } item)
        {
            return;
        }

        var newArr = _appSettingsManager.AppSettings!
                                        .GitRepositories
                                        .Where(t => t.RepositoryName != Selected.Repository.RepositoryName)
                                        .ToArray();

        if (MessageBox.Query(_application, "Remove", $"Remove {item.Repository.DisplayName}?", "Cancel", "Remove") == 1)
        {
            _appSettingsManager.AppSettings.GitRepositories = [..newArr];
            await _appSettingsManager.SaveAsync();
        }
    }

    private void SetStatus(string text)
    {
        _statusBar.Text = text;
        _statusBar.SetNeedsDraw();
    }
}

public class GitRepositoryItemModel : INotifyPropertyChanged
{
    private readonly IPipelineStatusService _pipelineStatusService;

    public GitRepositoryItemModel(GitRepository repositoryModel, IPipelineStatusService pipelineStatusService)
    {
        Repository             = repositoryModel       ?? throw new ArgumentNullException(nameof(repositoryModel));
        _pipelineStatusService = pipelineStatusService ?? throw new ArgumentNullException(nameof(pipelineStatusService));
    }

    public GitRepository Repository { get; }

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
            imm[branchType] = await _pipelineStatusService.GetPipelineStateForBranchAsync(branches[branchType]);
        }

        PipelineStates = imm.ToImmutableDictionary();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly AppSettingsManager     _appSettingsManager;
    private readonly IPipelineStatusService _pipelineStatusService;

    public DashboardViewModel(AppSettingsManager appSettingsManager,
        IPipelineStatusService                   pipelineStatusService)
    {
        _appSettingsManager                    =  appSettingsManager    ?? throw new ArgumentNullException(nameof(appSettingsManager));
        _pipelineStatusService                 =  pipelineStatusService ?? throw new ArgumentNullException(nameof(pipelineStatusService));
        _                                      =  LoadSettingsAsync();
        _appSettingsManager.AppSettingsChanged += AppSettingsManagerOnAppSettingsChanged;
    }

    public ObservableCollection<GitRepository>          Repositories { get; } = [];
    public ObservableCollection<GitRepositoryItemModel> Items        { get; } = [];

    public AppSettings? AppSettings
    {
        get;
        set => SetField(ref field, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void AppSettingsManagerOnAppSettingsChanged()
    {
        _ = LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        AppSettings = await _appSettingsManager.LoadAsync();
        RefreshRepositories();
    }

    private void RefreshRepositories()
    {
        Repositories.Clear();
        Items.Clear();
        foreach (var repo in AppSettings!.GitRepositories)
        {
            Repositories.Add(repo);
            Items.Add(new GitRepositoryItemModel(repo, _pipelineStatusService));
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? n = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(n);
        return true;
    }
}