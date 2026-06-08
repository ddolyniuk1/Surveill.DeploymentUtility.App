using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Terminal.Gui.App;
using Terminal.Gui.Reflect.Base;
using Terminal.Gui.Reflect.Interfaces;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Surveill.DeploymentUtility.App.Views.Dialogs;

public class TaskLoadingDialog : ViewController<Dialog, TaskLoadingDialogViewModel>
{
    private readonly IApplication  _application;
    private readonly List<TaskRow> _rows = new();

    public TaskLoadingDialog(IApplication application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
    }

    public override void InitializeComponents()
    {
        Root.Title = "Loading";
        Root.Width = Dim.Percent(60);
        Root.Height = Dim.Auto();
    }

    public override void SetupBindings()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        BuildRows();

        SyncModalState();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskLoadingDialogViewModel.Tasks))
        {
            _application.Invoke(BuildRows);
        }

        if (e.PropertyName == nameof(TaskLoadingDialogViewModel.ShouldBeVisible))
        {
            _application.Invoke(SyncModalState);
        }
    }
    
    private SessionToken? _runState;

    private void SyncModalState()
    {
        var showing = _runState is not null;

        switch (ViewModel.ShouldBeVisible)
        {
            case true when !showing:
                _runState = _application.Begin(Root);
                break;
            case false when showing:
                _application.End(_runState!);
                _runState = null;
                break;
        }
    }
    private void BuildRows()
    {
        foreach (var row in _rows)
        {
            row.Detach();
            Root.Remove(row.Container);
            row.Container.Dispose();
        }
        _rows.Clear();

        var tasks = ViewModel.Tasks ?? Array.Empty<IObservableTask>();
        View? previous = null;

        foreach (var task in tasks)
        {
            var row = new TaskRow(_application, task)
            {
                Container =
                {
                    Y     = previous is null ? 0 : Pos.Bottom(previous),
                    X     = 0,
                    Width = Dim.Fill()
                }
            };

            Root.Add(row.Container);
            row.Attach();
            _rows.Add(row);
            previous = row.Container;
        }

        UpdateCloseState();
    }

    private void UpdateCloseState()
    {
        var tasks = ViewModel.Tasks ?? Array.Empty<IObservableTask>();
        var allDone = tasks.Length > 0 && tasks.All(t =>
                                                        t.Status is EObservableTaskStatus.Completed
                                                            or EObservableTaskStatus.Failed
                                                            or EObservableTaskStatus.Cancelled);

        if (allDone)
        {
            _application.Invoke(() => _application.RequestStop(Root));
        }
    }

    private sealed class TaskRow
    {
        private readonly IApplication    _application;
        private readonly IObservableTask _task;
        private readonly Label           _label;
        private readonly Label           _statusLabel;
        private readonly ProgressBar?    _progress;
        private readonly SpinnerView?    _spinner;

        public View Container { get; }

        public TaskRow(IApplication application, IObservableTask task)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));
            _task        = task        ?? throw new ArgumentNullException(nameof(task));

            Container = new View
            {
                Height = 2,
            };

            _label = new Label
            {
                X = 0,
                Y = 0,
                Text = task.Label,
            };

            _statusLabel = new Label
            {
                X = Pos.AnchorEnd(),
                Y = 0,
                Text = string.Empty,
            };

            Container.Add(_label, _statusLabel);

            if (task.IsIndeterminate)
            {
                _spinner = new SpinnerView
                {
                    X = 0,
                    Y = 1,
                    AutoSpin = true,
                    Visible = task.IsRunning,
                    Style = new SpinnerStyle.Dots(),
                };
                Container.Add(_spinner);
            }
            else
            {
                _progress = new ProgressBar
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = 1,
                    ProgressBarStyle = ProgressBarStyle.Continuous,
                    Fraction = task.Progress / 100f,
                };
                Container.Add(_progress);
            }
        }

        public void Attach()
        {
            _task.PropertyChanged += OnTaskPropertyChanged;
            Refresh();
        }

        public void Detach()
        {
            _task.PropertyChanged -= OnTaskPropertyChanged;
            if (_spinner is not null) _spinner.AutoSpin = false;
        }

        private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _application.Invoke(Refresh);
        }

        private void Refresh()
        {
            _label.Text = _task.Label;

            _progress?.Fraction = Math.Clamp(_task.Progress / 100f, 0f, 1f);

            if (_spinner is not null)
            {
                _spinner.AutoSpin = _task.IsRunning;
                _spinner.Visible = _task.IsRunning;
            }

            _statusLabel.Text = _task.Status switch
            {
                EObservableTaskStatus.Completed => "✓ Done",
                EObservableTaskStatus.Failed => $"✗ {_task.ErrorMessage}",
                EObservableTaskStatus.Cancelled => "Canceled",
                _ when _task.IsRunning && !_task.IsIndeterminate => $"{_task.Progress}%",
                _ when _task.IsRunning => "Running…",
                _ => "Pending",
            };
        }
    }
}

public class TaskLoadingDialogViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<IObservableTask> _source;

    public TaskLoadingDialogViewModel(IObservableTaskRunnerService runner)
    {
        _source                   =  runner.Tasks;
        _source.CollectionChanged += OnSourceChanged;
        Tasks                     =  _source.ToArray();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IObservableTask[] Tasks
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool ShouldBeVisible => Tasks.Length > 0 && Tasks.Any(t => t.IsRunning);

    private void OnSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Tasks = _source.ToArray();
        OnPropertyChanged(nameof(ShouldBeVisible));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? p = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(p);
        return true;
    }
}