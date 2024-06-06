using System.Text.Json;

try
{
    new App(new JsonReader()).Run();
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}


Console.ReadKey();

class App
{
    private readonly IJsonReader _reader;

    public App(IJsonReader reader)
    {
        _reader = reader;
    }

    public async void Run()
    {
        string baseAddress = "https://swapi.dev/api/";
        string requestUri = "planets";

        var json = await _reader.Read(baseAddress, requestUri);

        var root = JsonSerializer.Deserialize<Root>(json);

        var planets = ToPlanet(root);

        var properties = new Dictionary<string, Func<Planet, long?>>()
        {
            ["population"] = planet => planet.Population,
            ["surface water"] = planet => planet.SurfaceWater,
            ["diameter"] = planet => planet.Diameter
        };

        TablePrinter.Print(planets);
        Console.WriteLine("The statistics of which property would you like to see?");
        Console.WriteLine(
            string.Join(Environment.NewLine, properties.Keys));

        var userChoice = Console.ReadLine();

        if (userChoice is null || !properties.ContainsKey(userChoice))
        {
            Console.WriteLine("Invalid choice.");
        }
        else
        {
            ShowStatistics(planets, userChoice, properties[userChoice]);
        }

    }

    private void ShowStatistics(IEnumerable<Planet> planets, string userChoice, Func<Planet, long?> value)
    {
        Planet maxValue = planets.MaxBy(value);
        Planet minValue = planets.MinBy(value);
        Console.WriteLine($"max {userChoice} is {value(maxValue)} (planet: {maxValue.Name})");
        Console.WriteLine($"min {userChoice} is {value(minValue)} (planet: {minValue.Name})");
    }

    private IEnumerable<Planet> ToPlanet(Root? root)
    {
        if (root == null)
        {
            throw new ArgumentNullException(nameof(root));
        }
        var planets = new List<Planet>();
        foreach (var planetDto in root.results)
        {
            Planet planet = (Planet)planetDto;
            planets.Add(planet);
        }
        return planets;
    }
}


public readonly record struct Planet
{
    public Planet(string name, int diameter, int? surfaceWater, long? population)
    {
        if(name is null)
        {
            throw new ArgumentException(nameof(name));  
        }
        Name = name;
        Diameter = diameter;
        SurfaceWater = surfaceWater;
        Population = population;
    }

    public string Name { get; }
    public int Diameter { get; }
    public int? SurfaceWater { get; }
    public long? Population { get; }

    public static explicit operator Planet(Result planetDto)
    {
        var name = planetDto.name;
        var diameter = int.Parse(planetDto.diameter);
        long? population = ToLongOrNull(planetDto.population);
        int? surfaceWater = ToIntOrNull(planetDto.surface_water);
        return new Planet(name, diameter, surfaceWater, population);
    }

    public static int? ToIntOrNull(string input)
    {
        int? result = null;
        if(int.TryParse(input, out int resultParsed)){
            result = resultParsed;
        }
        return result;
    }
    public static long? ToLongOrNull(string input)
    {
        long? result = null;
        if (long.TryParse(input, out long resultParsed))
        {
            result = resultParsed;
        }
        return result;
    }
}

public static class TablePrinter
{
    public static void Print<T>(IEnumerable<T> planets)
    {
        var columnWidth = 15;
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            Console.Write($"{{0, -{columnWidth}}}|", property.Name);
        }
        Console.WriteLine();
        Console.WriteLine(
            new string('-', properties.Length * (columnWidth + 1)));
        foreach(var item in planets)
        {
            foreach(var property in properties)
            {
                Console.Write($"{{0, -{columnWidth}}}|", property.GetValue(item));
            }
            Console.WriteLine();
        }
    }
}
