using System.Text.Json.Serialization;

public record Result(
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("diameter")] string diameter,
        [property: JsonPropertyName("surface_water")] string surface_water,
        [property: JsonPropertyName("population")] string population
    );
