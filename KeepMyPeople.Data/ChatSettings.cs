namespace KeepMyPeople.Data;

public record ChatSettings
{
    public string ConnectionId { get; set; }
    public required AirUser Me { get; set; }
    public required AirUser Other { get; set; }
    public required string MeKey { get; set; }
    public required string OtherKey { get; set; }
}