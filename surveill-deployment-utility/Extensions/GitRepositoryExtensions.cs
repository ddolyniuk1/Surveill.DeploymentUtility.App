using Surveill.DeploymentUtility.App.Settings;

namespace Surveill.DeploymentUtility.App.Extensions;

public static class GitRepositoryExtensions
{
    public static string? GetRepositoryName(this GitRepository repository)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("git", "remote get-url origin")
        {
            RedirectStandardOutput = true,
            UseShellExecute        = false, 
            WorkingDirectory       = repository.Directory
        };
        using var proc   = System.Diagnostics.Process.Start(psi)!;
        var       remote = proc.StandardOutput.ReadToEnd().Trim();
        proc.WaitForExit();

        return ParseRepoName(remote);
    }

    public static string ParseRepoName(string remote)
    {
        remote = remote.TrimEnd('/');
        if (remote.EndsWith(".git")) remote = remote[..^4];
        return remote[(remote.LastIndexOf('/') + 1)..];
    }
}