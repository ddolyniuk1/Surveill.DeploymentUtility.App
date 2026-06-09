using CliWrap;
using CliWrap.Buffered;

namespace Surveill.DeploymentUtility.App.Services;

public class GitCliService : IGitCliService
{ 
    public CommandTask<BufferedCommandResult> ExecuteAsync(string[] args, string workingDirectory, CancellationToken ct) =>
        Cli.Wrap("cmd.exe")
           .WithArguments(new[] { "/c", "git" }.Concat(args).ToArray())
           .WithWorkingDirectory(workingDirectory)
           .ExecuteBufferedAsync(ct);
}

public interface IGitCliService
{
    CommandTask<BufferedCommandResult> ExecuteAsync(string[] args, string workingDirectory, CancellationToken ct);
}