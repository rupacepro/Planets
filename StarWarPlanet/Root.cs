using System.Text.Json.Serialization;

public record Root(
    [property: JsonPropertyName("results")] IReadOnlyList<Result> results
);