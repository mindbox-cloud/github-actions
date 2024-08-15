using System.Text.Json.Serialization;

namespace NotifyBumpPackages;

public class MessageToGrafanaDto
{
    [JsonPropertyName("alert_uid")]
    public Guid AlertUid { get; set; } = Guid.NewGuid();

    [JsonPropertyName("pullRequests")]
    public List<ForgottenPullRequestDto> PullRequests
    {
        get;
        set;
    } = null!;
}