using System.Text.Json.Serialization;

namespace JsonApi.Models;

public class StreetFighterCharacter
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("fighting_style")] public string FightingStyle { get; set; }

    [JsonPropertyName("origin_country")] public string OriginCountry { get; set; }

    [JsonPropertyName("health_points")] public int HealthPoints { get; set; }

    [JsonPropertyName("special_moves")] public List<string> SpecialMoves { get; set; }

    public StreetFighterCharacter(string name, string fightingStyle, string originCountry, int healthPoints,
        List<string> specialMoves)
    {
        Name = name;
        FightingStyle = fightingStyle;
        OriginCountry = originCountry;
        HealthPoints = healthPoints;
        SpecialMoves = specialMoves;
    }
}