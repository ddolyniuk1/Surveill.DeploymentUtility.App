namespace Surveill.DeploymentUtility.App.Enums;

[Flags]
public enum ActivePipelineState
{
    RunningAlpha   = 1,
    RunningBeta    = 2,
    RunningRelease = 4,
}