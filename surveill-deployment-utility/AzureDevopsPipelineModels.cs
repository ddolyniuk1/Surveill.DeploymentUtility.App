using System.Text.Json.Serialization;

namespace Surveill.DeploymentUtility.App;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AzureDevopsPipelineDefinitionsResponse
{
    public record AgentSpecification(
        [property: JsonPropertyName("identifier")]
        string Identifier
    );

    public record AuthoredBy(
        [property: JsonPropertyName("displayName")]
        string DisplayName,
        [property: JsonPropertyName("url")]    string Url,
        [property: JsonPropertyName("_links")] Links  Links,
        [property: JsonPropertyName("id")]     string Id,
        [property: JsonPropertyName("uniqueName")]
        string UniqueName,
        [property: JsonPropertyName("imageUrl")]
        string ImageUrl,
        [property: JsonPropertyName("descriptor")]
        string Descriptor
    );

    public record Avatar(
        [property: JsonPropertyName("href")] string Href
    );

    public record Badge(
        [property: JsonPropertyName("href")] string Href
    );

    public record Definition(
        [property: JsonPropertyName("id")]   string Id,
        [property: JsonPropertyName("url")]  string Url,
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("queueStatus")]
        string QueueStatus,
        [property: JsonPropertyName("project")]
        Project Project
    );

    public record Draft(
        [property: JsonPropertyName("id")]   int?   Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")]  string Url,
        [property: JsonPropertyName("uri")]  string Uri,
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("queueStatus")]
        string QueueStatus,
        [property: JsonPropertyName("revision")]
        int? Revision,
        [property: JsonPropertyName("createdDate")]
        DateTime? CreatedDate,
        [property: JsonPropertyName("project")]
        Project Project
    );

    public record Editor(
        [property: JsonPropertyName("href")] string Href
    );

    public record Environment;

    public record ExecutionOptions(
        [property: JsonPropertyName("type")] int? Type
    );

    public record Inputs(
        [property: JsonPropertyName("branchFilters")]
        string BranchFilters,
        [property: JsonPropertyName("additionalFields")]
        string AdditionalFields,
        [property: JsonPropertyName("workItemType")]
        string WorkItemType,
        [property: JsonPropertyName("assignToRequestor")]
        string AssignToRequestor,
        [property: JsonPropertyName("ConnectedServiceName")]
        string ConnectedServiceName,
        [property: JsonPropertyName("command")]
        string Command,
        [property: JsonPropertyName("publishWebProjects")]
        string PublishWebProjects,
        [property: JsonPropertyName("projects")]
        string Projects,
        [property: JsonPropertyName("custom")] string Custom,
        [property: JsonPropertyName("arguments")]
        string Arguments,
        [property: JsonPropertyName("restoreArguments")]
        string RestoreArguments,
        [property: JsonPropertyName("publishTestResults")]
        string PublishTestResults,
        [property: JsonPropertyName("testRunTitle")]
        string TestRunTitle,
        [property: JsonPropertyName("zipAfterPublish")]
        string ZipAfterPublish,
        [property: JsonPropertyName("modifyOutputPath")]
        string ModifyOutputPath,
        [property: JsonPropertyName("selectOrConfig")]
        string SelectOrConfig,
        [property: JsonPropertyName("feedRestore")]
        string FeedRestore,
        [property: JsonPropertyName("includeNuGetOrg")]
        string IncludeNuGetOrg,
        [property: JsonPropertyName("nugetConfigPath")]
        string NugetConfigPath,
        [property: JsonPropertyName("externalEndpoints")]
        string ExternalEndpoints,
        [property: JsonPropertyName("noCache")]
        string NoCache,
        [property: JsonPropertyName("packagesDirectory")]
        string PackagesDirectory,
        [property: JsonPropertyName("verbosityRestore")]
        string VerbosityRestore,
        [property: JsonPropertyName("searchPatternPush")]
        string SearchPatternPush,
        [property: JsonPropertyName("nuGetFeedType")]
        string NuGetFeedType,
        [property: JsonPropertyName("feedPublish")]
        string FeedPublish,
        [property: JsonPropertyName("publishPackageMetadata")]
        string PublishPackageMetadata,
        [property: JsonPropertyName("externalEndpoint")]
        string ExternalEndpoint,
        [property: JsonPropertyName("searchPatternPack")]
        string SearchPatternPack,
        [property: JsonPropertyName("configurationToPack")]
        string ConfigurationToPack,
        [property: JsonPropertyName("outputDir")]
        string OutputDir,
        [property: JsonPropertyName("nobuild")]
        string Nobuild,
        [property: JsonPropertyName("versioningScheme")]
        string VersioningScheme,
        [property: JsonPropertyName("versionEnvVar")]
        string VersionEnvVar,
        [property: JsonPropertyName("requestedMajorVersion")]
        string RequestedMajorVersion,
        [property: JsonPropertyName("requestedMinorVersion")]
        string RequestedMinorVersion,
        [property: JsonPropertyName("requestedPatchVersion")]
        string RequestedPatchVersion,
        [property: JsonPropertyName("buildProperties")]
        string BuildProperties,
        [property: JsonPropertyName("verbosityPack")]
        string VerbosityPack,
        [property: JsonPropertyName("workingDirectory")]
        string WorkingDirectory,
        [property: JsonPropertyName("requestTimeout")]
        string RequestTimeout,
        [property: JsonPropertyName("BuildConfiguration")]
        string BuildConfiguration,
        [property: JsonPropertyName("DOCKER_TAG")]
        string DOCKERTAG,
        [property: JsonPropertyName("GitVersion.SemVer")]
        string GitVersionSemVer,
        [property: JsonPropertyName("RepositoryName")]
        string RepositoryName,
        [property: JsonPropertyName("script")] string Script,
        [property: JsonPropertyName("failOnStderr")]
        string FailOnStderr,
        [property: JsonPropertyName("BuildSolution")]
        string BuildSolution,
        [property: JsonPropertyName("FileNamePrefix")]
        string FileNamePrefix,
        [property: JsonPropertyName("GitVersion.AssemblySemVer")]
        string GitVersionAssemblySemVer,
        [property: JsonPropertyName("GitVersion.ShortSha")]
        string GitVersionShortSha,
        [property: JsonPropertyName("PluginFileName")]
        string PluginFileName,
        [property: JsonPropertyName("SubmoduleDir")]
        string SubmoduleDir,
        [property: JsonPropertyName("runtime")]
        string Runtime,
        [property: JsonPropertyName("preferBundledVersion")]
        string PreferBundledVersion,
        [property: JsonPropertyName("configFilePath")]
        string ConfigFilePath,
        [property: JsonPropertyName("updateAssemblyInfo")]
        string UpdateAssemblyInfo,
        [property: JsonPropertyName("updateAssemblyInfoFilename")]
        string UpdateAssemblyInfoFilename,
        [property: JsonPropertyName("gitVersionPath")]
        string GitVersionPath,
        [property: JsonPropertyName("targetPath")]
        string TargetPath,
        [property: JsonPropertyName("additionalArguments")]
        string AdditionalArguments,
        [property: JsonPropertyName("versionSpec")]
        string VersionSpec,
        [property: JsonPropertyName("includePrerelease")]
        string IncludePrerelease,
        [property: JsonPropertyName("ignoreFailedSources")]
        string IgnoreFailedSources,
        [property: JsonPropertyName("useConfigFile")]
        string UseConfigFile,
        [property: JsonPropertyName("solution")]
        string Solution,
        [property: JsonPropertyName("disableParallelProcessing")]
        string DisableParallelProcessing,
        [property: JsonPropertyName("allowPackageConflicts")]
        string AllowPackageConflicts,
        [property: JsonPropertyName("verbosityPush")]
        string VerbosityPush,
        [property: JsonPropertyName("includeReferencedProjects")]
        string IncludeReferencedProjects,
        [property: JsonPropertyName("packTimezone")]
        string PackTimezone,
        [property: JsonPropertyName("includeSymbols")]
        string IncludeSymbols,
        [property: JsonPropertyName("toolPackage")]
        string ToolPackage,
        [property: JsonPropertyName("basePath")]
        string BasePath,
        [property: JsonPropertyName("PublishProject")]
        string PublishProject,
        [property: JsonPropertyName("containerRegistry")]
        string ContainerRegistry,
        [property: JsonPropertyName("repository")]
        string Repository,
        [property: JsonPropertyName("Dockerfile")]
        string Dockerfile,
        [property: JsonPropertyName("buildContext")]
        string BuildContext,
        [property: JsonPropertyName("tags")] string Tags,
        [property: JsonPropertyName("addPipelineData")]
        string AddPipelineData,
        [property: JsonPropertyName("addBaseImageData")]
        string AddBaseImageData,
        [property: JsonPropertyName("container")]
        string Container,
        [property: JsonPropertyName("preferLatestVersion")]
        string PreferLatestVersion,
        [property: JsonPropertyName("disableCache")]
        string DisableCache,
        [property: JsonPropertyName("disableNormalization")]
        string DisableNormalization,
        [property: JsonPropertyName("overrideConfig")]
        string OverrideConfig,
        [property: JsonPropertyName("VelopackChannel")]
        string VelopackChannel,
        [property: JsonPropertyName("rootFolderOrFile")]
        string RootFolderOrFile,
        [property: JsonPropertyName("includeRootFolder")]
        string IncludeRootFolder,
        [property: JsonPropertyName("archiveType")]
        string ArchiveType,
        [property: JsonPropertyName("sevenZipCompression")]
        string SevenZipCompression,
        [property: JsonPropertyName("tarCompression")]
        string TarCompression,
        [property: JsonPropertyName("archiveFile")]
        string ArchiveFile,
        [property: JsonPropertyName("replaceExistingArchive")]
        string ReplaceExistingArchive,
        [property: JsonPropertyName("verbose")]
        string Verbose,
        [property: JsonPropertyName("quiet")] string Quiet,
        [property: JsonPropertyName("targetType")]
        string TargetType,
        [property: JsonPropertyName("filePath")]
        string FilePath,
        [property: JsonPropertyName("errorActionPreference")]
        string ErrorActionPreference,
        [property: JsonPropertyName("warningPreference")]
        string WarningPreference,
        [property: JsonPropertyName("informationPreference")]
        string InformationPreference,
        [property: JsonPropertyName("verbosePreference")]
        string VerbosePreference,
        [property: JsonPropertyName("debugPreference")]
        string DebugPreference,
        [property: JsonPropertyName("progressPreference")]
        string ProgressPreference,
        [property: JsonPropertyName("showWarnings")]
        string ShowWarnings,
        [property: JsonPropertyName("ignoreLASTEXITCODE")]
        string IgnoreLASTEXITCODE,
        [property: JsonPropertyName("pwsh")] string Pwsh,
        [property: JsonPropertyName("runScriptInSeparateScope")]
        string RunScriptInSeparateScope,
        [property: JsonPropertyName("awsCredentials")]
        string AwsCredentials,
        [property: JsonPropertyName("regionName")]
        string RegionName,
        [property: JsonPropertyName("bucketName")]
        string BucketName,
        [property: JsonPropertyName("sourceFolder")]
        string SourceFolder,
        [property: JsonPropertyName("globExpressions")]
        string GlobExpressions,
        [property: JsonPropertyName("targetFolder")]
        string TargetFolder,
        [property: JsonPropertyName("filesAcl")]
        string FilesAcl,
        [property: JsonPropertyName("createBucket")]
        string CreateBucket,
        [property: JsonPropertyName("keyManagement")]
        string KeyManagement,
        [property: JsonPropertyName("encryptionAlgorithm")]
        string EncryptionAlgorithm,
        [property: JsonPropertyName("kmsMasterKeyId")]
        string KmsMasterKeyId,
        [property: JsonPropertyName("customerKey")]
        string CustomerKey,
        [property: JsonPropertyName("flattenFolders")]
        string FlattenFolders,
        [property: JsonPropertyName("contentType")]
        string ContentType,
        [property: JsonPropertyName("contentEncoding")]
        string ContentEncoding,
        [property: JsonPropertyName("storageClass")]
        string StorageClass,
        [property: JsonPropertyName("forcePathStyleAddressing")]
        string ForcePathStyleAddressing,
        [property: JsonPropertyName("cacheControl")]
        string CacheControl,
        [property: JsonPropertyName("logRequest")]
        string LogRequest,
        [property: JsonPropertyName("logResponse")]
        string LogResponse,
        [property: JsonPropertyName("nuGetServiceConnections")]
        string NuGetServiceConnections,
        [property: JsonPropertyName("forceReinstallCredentialProvider")]
        string ForceReinstallCredentialProvider,
        [property: JsonPropertyName("msbuildLocationMethod")]
        string MsbuildLocationMethod,
        [property: JsonPropertyName("msbuildVersion")]
        string MsbuildVersion,
        [property: JsonPropertyName("msbuildArchitecture")]
        string MsbuildArchitecture,
        [property: JsonPropertyName("msbuildLocation")]
        string MsbuildLocation,
        [property: JsonPropertyName("platform")]
        string Platform,
        [property: JsonPropertyName("configuration")]
        string Configuration,
        [property: JsonPropertyName("msbuildArguments")]
        string MsbuildArguments,
        [property: JsonPropertyName("clean")] string Clean,
        [property: JsonPropertyName("maximumCpuCount")]
        string MaximumCpuCount,
        [property: JsonPropertyName("restoreNugetPackages")]
        string RestoreNugetPackages,
        [property: JsonPropertyName("logProjectEvents")]
        string LogProjectEvents,
        [property: JsonPropertyName("createLogFile")]
        string CreateLogFile,
        [property: JsonPropertyName("logFileVerbosity")]
        string LogFileVerbosity,
        [property: JsonPropertyName("filename")]
        string Filename,
        [property: JsonPropertyName("modifyEnvironment")]
        string ModifyEnvironment,
        [property: JsonPropertyName("workingFolder")]
        string WorkingFolder,
        [property: JsonPropertyName("failOnStandardError")]
        string FailOnStandardError,
        [property: JsonPropertyName("DockerImage.ContainerName")]
        string DockerImageContainerName,
        [property: JsonPropertyName("DockerImage.ContainerTag")]
        string DockerImageContainerTag,
        [property: JsonPropertyName("DockerImage.Name")]
        string DockerImageName
    );

    public record Links(
        [property: JsonPropertyName("self")]   Self   Self,
        [property: JsonPropertyName("web")]    Web    Web,
        [property: JsonPropertyName("editor")] Editor Editor,
        [property: JsonPropertyName("badge")]  Badge  Badge,
        [property: JsonPropertyName("avatar")] Avatar Avatar
    );

    public record Option(
        [property: JsonPropertyName("enabled")]
        bool? Enabled,
        [property: JsonPropertyName("definition")]
        Definition Definition,
        [property: JsonPropertyName("inputs")] Inputs Inputs
    );

    public record Phase(
        [property: JsonPropertyName("steps")] IReadOnlyList<Step> Steps,
        [property: JsonPropertyName("name")]  string              Name,
        [property: JsonPropertyName("refName")]
        string RefName,
        [property: JsonPropertyName("condition")]
        string Condition,
        [property: JsonPropertyName("target")] Target Target,
        [property: JsonPropertyName("jobAuthorizationScope")]
        string JobAuthorizationScope
    );

    public record Pool(
        [property: JsonPropertyName("id")]   int?   Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("isHosted")]
        bool? IsHosted
    );

    public record Process(
        [property: JsonPropertyName("phases")] IReadOnlyList<Phase> Phases,
        [property: JsonPropertyName("type")]   int?                 Type,
        [property: JsonPropertyName("target")] Target               Target
    );

    public record ProcessParameters;

    public record Project(
        [property: JsonPropertyName("id")]    string Id,
        [property: JsonPropertyName("name")]  string Name,
        [property: JsonPropertyName("url")]   string Url,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("revision")]
        int? Revision,
        [property: JsonPropertyName("visibility")]
        string Visibility,
        [property: JsonPropertyName("lastUpdateTime")]
        DateTime? LastUpdateTime
    );

    public record Properties(
        [property: JsonPropertyName("fullName")]
        string FullName,
        [property: JsonPropertyName("cloneUrl")]
        string CloneUrl,
        [property: JsonPropertyName("isPrivate")]
        string IsPrivate,
        [property: JsonPropertyName("checkoutNestedSubmodules")]
        string CheckoutNestedSubmodules,
        [property: JsonPropertyName("cleanOptions")]
        string CleanOptions,
        [property: JsonPropertyName("fetchDepth")]
        string FetchDepth,
        [property: JsonPropertyName("gitLfsSupport")]
        string GitLfsSupport,
        [property: JsonPropertyName("reportBuildStatus")]
        string ReportBuildStatus,
        [property: JsonPropertyName("connectedServiceId")]
        string ConnectedServiceId,
        [property: JsonPropertyName("skipSyncSource")]
        string SkipSyncSource,
        [property: JsonPropertyName("fetchTags")]
        string FetchTags,
        [property: JsonPropertyName("labelSourcesFormat")]
        string LabelSourcesFormat,
        [property: JsonPropertyName("labelSources")]
        string LabelSources
    );

    public record Queue(
        [property: JsonPropertyName("_links")] Links  Links,
        [property: JsonPropertyName("id")]     int?   Id,
        [property: JsonPropertyName("name")]   string Name,
        [property: JsonPropertyName("url")]    string Url,
        [property: JsonPropertyName("pool")]   Pool   Pool
    );

    public record Repository(
        [property: JsonPropertyName("properties")]
        Properties Properties,
        [property: JsonPropertyName("id")]   string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")]  string Url,
        [property: JsonPropertyName("defaultBranch")]
        string DefaultBranch,
        [property: JsonPropertyName("clean")] string Clean,
        [property: JsonPropertyName("checkoutSubmodules")]
        bool? CheckoutSubmodules
    );

    public record Root(
        [property: JsonPropertyName("count")] int?                 Count,
        [property: JsonPropertyName("value")] IReadOnlyList<Value> Value
    );

    public record Self(
        [property: JsonPropertyName("href")] string Href
    );

    public record Step(
        [property: JsonPropertyName("environment")]
        Environment Environment,
        [property: JsonPropertyName("enabled")]
        bool? Enabled,
        [property: JsonPropertyName("continueOnError")]
        bool? ContinueOnError,
        [property: JsonPropertyName("alwaysRun")]
        bool? AlwaysRun,
        [property: JsonPropertyName("displayName")]
        string DisplayName,
        [property: JsonPropertyName("timeoutInMinutes")]
        int? TimeoutInMinutes,
        [property: JsonPropertyName("retryCountOnTaskFailure")]
        int? RetryCountOnTaskFailure,
        [property: JsonPropertyName("condition")]
        string Condition,
        [property: JsonPropertyName("task")]   Task   Task,
        [property: JsonPropertyName("inputs")] Inputs Inputs
    );

    public record SystemDebug(
        [property: JsonPropertyName("value")] string Value,
        [property: JsonPropertyName("allowOverride")]
        bool? AllowOverride
    );

    public record SystemPAT(
        [property: JsonPropertyName("value")] object Value,
        [property: JsonPropertyName("isSecret")]
        bool? IsSecret
    );

    public record Target(
        [property: JsonPropertyName("executionOptions")]
        ExecutionOptions ExecutionOptions,
        [property: JsonPropertyName("allowScriptsAuthAccessOption")]
        bool? AllowScriptsAuthAccessOption,
        [property: JsonPropertyName("type")] int? Type,
        [property: JsonPropertyName("agentSpecification")]
        AgentSpecification AgentSpecification
    );

    public record Task(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("versionSpec")]
        string VersionSpec,
        [property: JsonPropertyName("definitionType")]
        string DefinitionType
    );

    public record Trigger(
        [property: JsonPropertyName("branchFilters")]
        IReadOnlyList<string> BranchFilters,
        [property: JsonPropertyName("pathFilters")]
        IReadOnlyList<object> PathFilters,
        [property: JsonPropertyName("batchChanges")]
        bool? BatchChanges,
        [property: JsonPropertyName("maxConcurrentBuildsPerBranch")]
        int? MaxConcurrentBuildsPerBranch,
        [property: JsonPropertyName("pollingInterval")]
        int? PollingInterval,
        [property: JsonPropertyName("pollingJobId")]
        string PollingJobId,
        [property: JsonPropertyName("triggerType")]
        string TriggerType,
        [property: JsonPropertyName("definition")]
        Definition Definition,
        [property: JsonPropertyName("requiresSuccessfulBuild")]
        bool? RequiresSuccessfulBuild
    );

    public record Value(
        [property: JsonPropertyName("options")]
        IReadOnlyList<Option> Options,
        [property: JsonPropertyName("triggers")]
        IReadOnlyList<Trigger> Triggers,
        [property: JsonPropertyName("variables")]
        Variables Variables,
        [property: JsonPropertyName("properties")]
        Properties Properties,
        [property: JsonPropertyName("tags")]   IReadOnlyList<object> Tags,
        [property: JsonPropertyName("_links")] Links                 Links,
        [property: JsonPropertyName("jobAuthorizationScope")]
        string JobAuthorizationScope,
        [property: JsonPropertyName("jobTimeoutInMinutes")]
        int? JobTimeoutInMinutes,
        [property: JsonPropertyName("jobCancelTimeoutInMinutes")]
        int? JobCancelTimeoutInMinutes,
        [property: JsonPropertyName("process")]
        Process Process,
        [property: JsonPropertyName("repository")]
        Repository Repository,
        [property: JsonPropertyName("processParameters")]
        ProcessParameters ProcessParameters,
        [property: JsonPropertyName("quality")]
        string Quality,
        [property: JsonPropertyName("authoredBy")]
        AuthoredBy AuthoredBy,
        [property: JsonPropertyName("drafts")] IReadOnlyList<Draft> Drafts,
        [property: JsonPropertyName("queue")]  Queue                Queue,
        [property: JsonPropertyName("id")]     int?                 Id,
        [property: JsonPropertyName("name")]   string               Name,
        [property: JsonPropertyName("url")]    string               Url,
        [property: JsonPropertyName("uri")]    string               Uri,
        [property: JsonPropertyName("path")]   string               Path,
        [property: JsonPropertyName("type")]   string               Type,
        [property: JsonPropertyName("queueStatus")]
        string QueueStatus,
        [property: JsonPropertyName("revision")]
        int? Revision,
        [property: JsonPropertyName("createdDate")]
        DateTime? CreatedDate,
        [property: JsonPropertyName("project")]
        Project Project,
        [property: JsonPropertyName("buildNumberFormat")]
        string BuildNumberFormat,
        [property: JsonPropertyName("comment")]
        string Comment,
        [property: JsonPropertyName("description")]
        string Description
    );

    public record Variables(
        [property: JsonPropertyName("system.debug")]
        SystemDebug SystemDebug,
        [property: JsonPropertyName("system.PAT")]
        SystemPAT SystemPAT
    );

    public record Web(
        [property: JsonPropertyName("href")] string Href
    );
}