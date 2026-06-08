using CliWrap;
using CliWrap.Buffered;

namespace Surveill.DeploymentUtility.App;

public class GitCliService : IGitCliService
{ 
    public CommandTask<BufferedCommandResult> ExecuteAsync(string[] args, string workingDirectory, CancellationToken ct) => Cli.Wrap("git").WithArguments(args).WithWorkingDirectory(workingDirectory).ExecuteBufferedAsync();
}

public interface IGitCliService
{
    CommandTask<BufferedCommandResult> ExecuteAsync(string[] args, string workingDirectory, CancellationToken ct);
}