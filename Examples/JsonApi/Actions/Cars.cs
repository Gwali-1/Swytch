using System.Net;
using System.Text.Json;
using JsonApi.Models;
using Swytch.Structures;
using Swytch.utilities;

namespace JsonApi.Actions;

public class Cars
{
    public List<Car> Car;

    public Cars()
    {
        string jsonString = File.ReadAllText("Cars.Json");
        Car = JsonSerializer.Deserialize<List<Car>>(jsonString);
    }

    public async Task All(RequestContext context)
    {
        await Car.WriteJsonToStream(context, HttpStatusCode.OK);
    }

    public async Task Get(RequestContext context)
    {
        string carName;
        if (context.PathParams.TryGetValue("make", out carName))
        {
            var car = Car.Find(x => x.Make == carName);
            if (car is null)
            {
                await car.WriteJsonToStream(context, HttpStatusCode.NotFound);
                return;
            }

            await car.WriteJsonToStream(context, HttpStatusCode.Found);
            return;
        }

        await ResponseUtility.WriteTextToStream(context, "SOMETHING WENT WRONG", HttpStatusCode.ServiceUnavailable);
    }
}