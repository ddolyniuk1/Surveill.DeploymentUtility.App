using Terminal.Gui.Reflect.Base;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Surveill.DeploymentUtility.App.Views.Dialogs;

public class ConfigurePipelineSettingsDialog : ViewController<Dialog, ConfigurePipelineSettingsDialogViewModel>
{
    private Button    _cancelButton      = null!;
    private Button    _okButton          = null!;
    private TextField _organizationField = null!;
    private TextField _patField          = null!;
    private TextField _projectField      = null!;

    public bool Accepted { get; private set; }

    public override void InitializeComponents()
    {
        Root.Title  = "Configure Pipeline Settings";
        Root.Width  = Dim.Percent(60);
        Root.Height = 14;

        var organizationLabel = new Label
        {
            Text = "Organization:",
            X    = 1,
            Y    = 1
        };
        _organizationField = new TextField
        {
            X     = Pos.Right(organizationLabel) + 1,
            Y     = Pos.Top(organizationLabel),
            Width = Dim.Fill(1)
        };

        var projectLabel = new Label
        {
            Text = "Project:",
            X    = 1,
            Y    = Pos.Bottom(organizationLabel) + 1
        };
        _projectField = new TextField
        {
            X     = Pos.Left(_organizationField),
            Y     = Pos.Top(projectLabel),
            Width = Dim.Fill(1)
        };

        var patLabel = new Label
        {
            Text = "PAT:",
            X    = 1,
            Y    = Pos.Bottom(projectLabel) + 1
        };
        _patField = new TextField
        {
            X      = Pos.Left(_organizationField),
            Y      = Pos.Top(patLabel),
            Width  = Dim.Fill(1),
            Secret = true
        };

        _okButton = new Button
        {
            Text      = "OK",
            IsDefault = true
        };
        _okButton.Accepting += (_, e) =>
        {
            Accepted  = true;
            e.Handled = true;
            Root.RequestStop();
        };

        _cancelButton = new Button
        {
            Text = "Cancel"
        };
        _cancelButton.Accepting += (_, e) =>
        {
            Accepted  = false;
            e.Handled = true;
            Root.RequestStop();
        };

        Root.Add(
            organizationLabel, _organizationField,
            projectLabel, _projectField,
            patLabel, _patField);

        Root.AddButton(_okButton);
        Root.AddButton(_cancelButton);
    }

    public override void SetupBindings()
    {
        // Load model -> view
        _organizationField.Text = ViewModel.Organization ?? string.Empty;
        _projectField.Text      = ViewModel.Project      ?? string.Empty;
        _patField.Text          = ViewModel.Pat          ?? string.Empty;

        // View -> model (live binding)
        _organizationField.TextChanged += (_, _) =>
            ViewModel.Organization = _organizationField.Text;
        _projectField.TextChanged += (_, _) =>
            ViewModel.Project = _projectField.Text;
        _patField.TextChanged += (_, _) =>
            ViewModel.Pat = _patField.Text;
    }
}

public class ConfigurePipelineSettingsDialogViewModel : AzureDevopsPipelineConfiguration
{
    public void Clone(AzureDevopsPipelineConfiguration other)
    {
        Organization = other.Organization;
        Project      = other.Project;
        Pat          = other.Pat;
    }
}