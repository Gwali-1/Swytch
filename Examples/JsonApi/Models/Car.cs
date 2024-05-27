using System.Text.Json.Serialization;

namespace JsonApi.Models;

public class Car
{
    [JsonPropertyName("make")] public string Make { get; set; }

    [JsonPropertyName("model")] public string Model { get; set; }

    [JsonPropertyName("year")] public int Year { get; set; }

    [JsonPropertyName("isElectric")] public bool IsElectric { get; set; }

    [JsonPropertyName("mileage")] public int Mileage { get; set; }

    public Car(string make, string model, int year, bool isElectric, int mileage)
    {
        Make = make;
        Model = model;
        Year = year;
        IsElectric = isElectric;
        Mileage = mileage;
    }
}