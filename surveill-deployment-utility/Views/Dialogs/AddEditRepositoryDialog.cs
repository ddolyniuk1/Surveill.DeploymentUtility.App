using Surveill.DeploymentUtility.App.Extensions;
using Surveill.DeploymentUtility.App.Services;
using Surveill.DeploymentUtility.App.Settings;
using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Extensions;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Disposable = System.Reactive.Disposables.Disposable;

namespace Surveill.DeploymentUtility.App.Views.Dialogs;
internal class AddEditRepositoryDialog : ViewController<Dialog, AddEditRepositoryViewModel>
{
    private readonly IApplication           _application;
    private readonly IDevopsPipelineService _devopsPipelineService;
    private          Button                 _cancelBtn;
    private          TextField              _repositoryNameTextbox;
    private          Button                 _repositoryPathOpenDialogBtn;
    private          TextField              _repositoryPathTextbox;
    private          Button                 _saveBtn;
    private          Tabs                   _tabs;
 
    private ListView              _branchListView;
    private TextField             _branchNameTextbox;
    private TextField             _branchPipelineIdTextbox;
    private Label                 _branchTypeValueLabel;
    private GitRepositoryBranch[] _branches = [];
    private GitRepositoryBranch?  _selectedBranch;
    private Button                _autoResolvePipelineIdsButton;

    public AddEditRepositoryDialog(IApplication application, IDevopsPipelineService devopsPipelineService)
    {
        _application           = application           ?? throw new ArgumentNullException(nameof(application));
        _devopsPipelineService = devopsPipelineService ?? throw new ArgumentNullException(nameof(devopsPipelineService));
    }
 
    public override void InitializeComponents()
    {
        Root.Title  = "Add / Edit Repository";
        Root.Width  = Dim.Percent(80);
        Root.Height = Dim.Percent(80);
 
        _tabs = new Tabs
        {
            X      = 0,
            Y      = 0,
            Width  = Dim.Fill(),
            Height = Dim.Fill()
        };
 
        BuildGeneralTab();
        BuildBranchesTab();
 
        // ---- Dialog buttons ----
        _saveBtn = new Button
        {
            Text      = "_Save",
            IsDefault = true
        };
        _cancelBtn = new Button { Text = "_Cancel" };
 
        Root.AddButton(_saveBtn);
        Root.AddButton(_cancelBtn);
 
        Root.Add(_tabs);
    }
 
    private void BuildGeneralTab()
    {
        var general = new View
        {
            Width  = Dim.Fill(),
            Height = Dim.Fill(),
            Title   = "General"
        };
 
        // ---- Repository Name ----
        var nameLabel = new Label
        {
            Text = "Repository Name:",
            X    = 1, Y = 1
        };
        _repositoryNameTextbox = new TextField
        {
            X        = Pos.Left(nameLabel),
            Y        = Pos.Bottom(nameLabel),
            Width    = Dim.Fill(2),
            Height   = 1,
            ReadOnly = true
        };
 
        // ---- Repository Path (label + textbox + browse button) ----
        var pathLabel = new Label
        {
            Text = "Repository Path:",
            X    = Pos.Left(nameLabel),
            Y    = Pos.Bottom(_repositoryNameTextbox) + 1
        };
        _repositoryPathTextbox = new TextField
        {
            X      = Pos.Left(pathLabel),
            Y      = Pos.Bottom(pathLabel),
            Width  = Dim.Fill(11),
            Height = 1
        };
 
        _repositoryPathOpenDialogBtn = new Button
        {
            Text        = "...",
            X           = Pos.Right(_repositoryPathTextbox) + 1,
            Y           = Pos.Top(_repositoryPathTextbox),
            Width       = 9,
            Height      = 2,
            ShadowStyle = ShadowStyles.None
        };
 
        _repositoryPathOpenDialogBtn.MouseEvent += RepositoryPathOpenDialogBtnOnMouseEvent;
        Disposable.Create(() =>
        {
            _repositoryPathOpenDialogBtn.MouseEvent -= RepositoryPathOpenDialogBtnOnMouseEvent;
        }).DisposeWith(this);
 
        general.Add(
            nameLabel, _repositoryNameTextbox,
            pathLabel, _repositoryPathTextbox, _repositoryPathOpenDialogBtn
        );
        
        _tabs.InsertTab(0, general);
    }
 
    private void BuildBranchesTab()
    {
        var branches = new View
        {
            Width  = Dim.Fill(),
            Height = Dim.Fill(),
            Title   = "Branches"
        };
 
        // ---- Left: list of branches ----
        var listFrame = new FrameView
        {
            Title  = "Branches",
            X      = 0,
            Y      = 0,
            Width  = Dim.Percent(35),
            Height = Dim.Fill()
        };
        _branchListView = new ListView
        {
            X      = 0,
            Y      = 0,
            Width  = Dim.Fill(),
            Height = Dim.Fill()
        };
        listFrame.Add(_branchListView);
 
        // ---- Right: editor for selected branch ----
        var editorFrame = new FrameView
        {
            Title  = "Branch Details",
            X      = Pos.Right(listFrame),
            Y      = 0,
            Width  = Dim.Fill(),
            Height = Dim.Fill()
        };
 
        var typeLabel = new Label
        {
            Text = "Type:",
            X    = 1, Y = 1
        };
        _branchTypeValueLabel = new Label
        {
            Text = string.Empty,
            X    = Pos.Right(typeLabel) + 1,
            Y    = Pos.Top(typeLabel)
        };
 
        var nameLabel = new Label
        {
            Text = "Name:",
            X    = Pos.Left(typeLabel),
            Y    = Pos.Bottom(typeLabel) + 1
        };
        _branchNameTextbox = new TextField
        {
            X      = Pos.Left(nameLabel),
            Y      = Pos.Bottom(nameLabel),
            Width  = Dim.Fill(2),
            Height = 1
        };
 
        var pipelineLabel = new Label
        {
            Text = "Pipeline ID:",
            X    = Pos.Left(nameLabel),
            Y    = Pos.Bottom(_branchNameTextbox) + 1
        };
        _branchPipelineIdTextbox = new TextField
        {
            X      = Pos.Left(pipelineLabel),
            Y      = Pos.Bottom(pipelineLabel),
            Width  = Dim.Fill(2),
            Height = 1
        };

        _autoResolvePipelineIdsButton = new Button()
        {
            X = 0,
            Y = Pos.Bottom(_branchPipelineIdTextbox) + 2,
            Width = 10,
            Height = 2,
            Text = "Resolve"
        };
 
        editorFrame.Add(
            typeLabel, _branchTypeValueLabel,
            nameLabel, _branchNameTextbox,
            pipelineLabel, _branchPipelineIdTextbox, _autoResolvePipelineIdsButton
        );
 
        branches.Add(listFrame, editorFrame);
 
        _branchListView.ValueChanged += BranchListViewOnSelectedItemChanged;
        _branchNameTextbox.TextChanged       += BranchNameTextboxOnTextChanged;
        _branchPipelineIdTextbox.TextChanged += BranchPipelineIdTextboxOnTextChanged;
 
        Disposable.Create(() =>
        {
            _branchListView.ValueChanged         -= BranchListViewOnSelectedItemChanged;
            _branchNameTextbox.TextChanged       -= BranchNameTextboxOnTextChanged;
            _branchPipelineIdTextbox.TextChanged -= BranchPipelineIdTextboxOnTextChanged;
        }).DisposeWith(this);
 
        _tabs.InsertTab(1, branches);
    }

    private void AutoResolvePipelineIdsButtonOnMouseEvent(object? sender, Mouse e)
    {
        if (e.IsPressed)
        {
            _ = ResolvePipelineIdsAsync();
        }
    }

    private Task ResolvePipelineIdsAsync()
    {
        foreach (var branch in ViewModel.AllBranches.Values)
        {
            _application.Invoke(async void () =>
            {
                try
                {
                    branch.PipelineId = await _devopsPipelineService.FindPipelineIdByRepoAsync(ViewModel.RepositoryName, branch.Name);
                }
                catch (Exception e)
                {
                    //
                }
            });
        }

        return Task.CompletedTask;
    }

    private void BranchListViewOnSelectedItemChanged(object? sender, ValueChangedEventArgs<int?> e)
    {
        if (e.NewValue == null || e.NewValue < 0 || e.NewValue >= _branches.Length)
        {
            _selectedBranch = null;
            return;
        }
 
        _selectedBranch = _branches[e.NewValue.Value];
 
        _branchTypeValueLabel.Text    = _selectedBranch.BranchType.ToString();
        _branchNameTextbox.Text       = _selectedBranch.Name ?? string.Empty;
        _branchPipelineIdTextbox.Text = _selectedBranch.PipelineId?.ToString() ?? string.Empty;
    }
 
    private void BranchNameTextboxOnTextChanged(object? sender, EventArgs e)
    {
        if (_selectedBranch is null)
        {
            return;
        }
 
        _selectedBranch.Name = _branchNameTextbox.Text;
        RefreshBranchList();
    }
 
    private void BranchPipelineIdTextboxOnTextChanged(object? sender, EventArgs e)
    {
        if (_selectedBranch is null)
        {
            return;
        }
 
        var text = _branchPipelineIdTextbox.Text;
        _selectedBranch.PipelineId = int.TryParse(text, out var id) ? id : null;
    }
 
    private void RefreshBranchList()
    {
        var selected = _branchListView.SelectedItem;
        _branchListView.SetSource(new ObservableCollection<string>(_branches.Select(BranchDisplay)));
        if (selected >= 0 && selected < _branches.Length)
        {
            _branchListView.SelectedItem = selected;
        }
    }
 
    private static string BranchDisplay(GitRepositoryBranch branch)
    {
        var name = string.IsNullOrWhiteSpace(branch.Name) ? "(unset)" : branch.Name;
        return $"{branch.BranchType}: {name}";
    }
 
    private void RepositoryPathOpenDialogBtnOnMouseEvent(object? sender, Mouse e)
    {
        using var od = new OpenDialog();
        od.Title                   = "Select Git Repository Folder";
        od.OpenMode                = OpenMode.Directory;
        od.AllowsMultipleSelection = false;
 
        _application.Run(od);
 
        if (od is not { Canceled: false, Path: { Length: > 0 } selectedPath })
        {
            return;
        }
 
        if (!Directory.Exists(Path.Combine(selectedPath, ".git")))
        {
            MessageBox.ErrorQuery(_application, "Not a Git Repository",
                                  "The selected folder does not contain a .git directory.", "OK");
            return;
        }
 
        ViewModel.Directory      = selectedPath;
        ViewModel.RepositoryName = ViewModel.GetRepositoryName() ?? "Unknown";
    }
 
    public override void SetupBindings()
    {
        Bind(model => model.RepositoryName, _repositoryNameTextbox, field => field.Text);
        Bind(model => model.Directory, _repositoryPathTextbox, field => field.Text);
 
        // The list/editor work directly against the model's branch instances.
        _branches =
        [
            ViewModel.AlphaBranch,
            ViewModel.BetaBranch,
            ViewModel.ReleaseBranch
        ];
 
        _branchListView.SetSource(new ObservableCollection<string>(_branches.Select(BranchDisplay)));
        if (_branches.Length <= 0)
        {
            return;
        }

        _branchListView.SelectedItem = 0;
        BranchListViewOnSelectedItemChanged(this, new ValueChangedEventArgs<int?>(0, 0));

        _autoResolvePipelineIdsButton.MouseEvent += AutoResolvePipelineIdsButtonOnMouseEvent;
        AddCleanupOperation(() =>
        {
            _autoResolvePipelineIdsButton.MouseEvent -= AutoResolvePipelineIdsButtonOnMouseEvent;
        });
    }
}
 
public class AddEditRepositoryViewModel : GitRepository
{
    public void Copy(GitRepository original)
    {
        Directory = original.Directory;
        AlphaBranch.Copy(original.AlphaBranch);
        BetaBranch.Copy(original.BetaBranch);
        ReleaseBranch.Copy(original.ReleaseBranch);
        DisplayName    = original.DisplayName;
        RepositoryName = original.RepositoryName;
    }
}
