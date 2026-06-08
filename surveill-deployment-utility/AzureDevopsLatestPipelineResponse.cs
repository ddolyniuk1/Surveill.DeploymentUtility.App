using System.Text.Json.Serialization;

namespace Surveill.DeploymentUtility.App;
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

public class AzureDevopsLatestPipelineResponse
{
    public record Avatar(
        [property: JsonPropertyName("href")] string Href
    );

    public record Badge(
        [property: JsonPropertyName("href")] string Href
    );

    public record Definition(
        [property: JsonPropertyName("drafts")] IReadOnlyList<object> Drafts,
        [property: JsonPropertyName("id")]     int?                  Id,
        [property: JsonPropertyName("name")]   string                Name,
        [property: JsonPropertyName("url")]    string                Url,
        [property: JsonPropertyName("uri")]    string                Uri,
        [property: JsonPropertyName("path")]   string                Path,
        [property: JsonPropertyName("type")]   string                Type,
        [property: JsonPropertyName("queueStatus")]
        string QueueStatus,
        [property: JsonPropertyName("revision")]
        int? Revision,
        [property: JsonPropertyName("project")]
        Project Project
    );

    public record LastChangedBy(
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

    public record Links(
        [property: JsonPropertyName("self")] Self Self,
        [property: JsonPropertyName("web")]  Web  Web,
        [property: JsonPropertyName("timeline")]
        Timeline Timeline,
        [property: JsonPropertyName("badge")]  Badge  Badge,
        [property: JsonPropertyName("avatar")] Avatar Avatar
    );

    public record Logs(
        [property: JsonPropertyName("id")]   int?   Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("url")]  string Url
    );

    public record OrchestrationPlan(
        [property: JsonPropertyName("planId")] string PlanId
    );

    public record Plan(
        [property: JsonPropertyName("planId")] string PlanId
    );

    public record Pool(
        [property: JsonPropertyName("id")]   int?   Id,
        [property: JsonPropertyName("name")] string Name
    );

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

    public record Properties;

    public record Queue(
        [property: JsonPropertyName("id")]   int?   Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("pool")] Pool   Pool
    );

    public record Repository(
        [property: JsonPropertyName("id")]    string Id,
        [property: JsonPropertyName("type")]  string Type,
        [property: JsonPropertyName("name")]  string Name,
        [property: JsonPropertyName("url")]   string Url,
        [property: JsonPropertyName("clean")] object Clean,
        [property: JsonPropertyName("checkoutSubmodules")]
        bool? CheckoutSubmodules
    );

    public record RequestedBy(
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

    public record RequestedFor(
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

    public record Root(
        [property: JsonPropertyName("count")] int?                 Count,
        [property: JsonPropertyName("value")] IReadOnlyList<Value> Value
    );

    public record Self(
        [property: JsonPropertyName("href")] string Href
    );

    public record Timeline(
        [property: JsonPropertyName("href")] string Href
    );

    public record TriggerInfo;

    public record Value(
        [property: JsonPropertyName("_links")] Links Links,
        [property: JsonPropertyName("properties")]
        Properties Properties,
        [property: JsonPropertyName("tags")] IReadOnlyList<object> Tags,
        [property: JsonPropertyName("validationResults")]
        IReadOnlyList<object> ValidationResults,
        [property: JsonPropertyName("plans")] IReadOnlyList<Plan> Plans,
        [property: JsonPropertyName("triggerInfo")]
        TriggerInfo TriggerInfo,
        [property: JsonPropertyName("id")] int? Id,
        [property: JsonPropertyName("buildNumber")]
        string BuildNumber,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("result")] string Result,
        [property: JsonPropertyName("queueTime")]
        DateTime? QueueTime,
        [property: JsonPropertyName("startTime")]
        DateTime? StartTime,
        [property: JsonPropertyName("finishTime")]
        DateTime? FinishTime,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("definition")]
        Definition Definition,
        [property: JsonPropertyName("buildNumberRevision")]
        int? BuildNumberRevision,
        [property: JsonPropertyName("project")]
        Project Project,
        [property: JsonPropertyName("uri")] string Uri,
        [property: JsonPropertyName("sourceBranch")]
        string SourceBranch,
        [property: JsonPropertyName("sourceVersion")]
        string SourceVersion,
        [property: JsonPropertyName("queue")] Queue Queue,
        [property: JsonPropertyName("priority")]
        string Priority,
        [property: JsonPropertyName("reason")] string Reason,
        [property: JsonPropertyName("requestedFor")]
        RequestedFor RequestedFor,
        [property: JsonPropertyName("requestedBy")]
        RequestedBy RequestedBy,
        [property: JsonPropertyName("lastChangedDate")]
        DateTime? LastChangedDate,
        [property: JsonPropertyName("lastChangedBy")]
        LastChangedBy LastChangedBy,
        [property: JsonPropertyName("orchestrationPlan")]
        OrchestrationPlan OrchestrationPlan,
        [property: JsonPropertyName("logs")] Logs Logs,
        [property: JsonPropertyName("repository")]
        Repository Repository,
        [property: JsonPropertyName("retainedByRelease")]
        bool? RetainedByRelease,
        [property: JsonPropertyName("triggeredByBuild")]
        object TriggeredByBuild,
        [property: JsonPropertyName("appendCommitMessageToRunName")]
        bool? AppendCommitMessageToRunName
    );

    public record Web(
        [property: JsonPropertyName("href")] string Href
    );
}