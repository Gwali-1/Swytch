using System.Net;
using System.Text.Json;
using JsonApi.Models;
using Swytch.Structures;
using Swytch.utilities;

namespace JsonApi.Actions;

public class Fighters
{
    public List<StreetFighterCharacter>? Fighter { get; set; }

    public Fighters()
    {
        string jsonString = File.ReadAllText("Fighter.Json");
        Fighter = JsonSerializer.Deserialize<List<StreetFighterCharacter>>(jsonString);
    }

    public async Task All(RequestContext context)
    {
        await Fighter.WriteJsonToStream(context, HttpStatusCode.OK);
    }

    public async Task Get(RequestContext context)
    {
        string? name;
        if (context.PathParams.TryGetValue("name", out name))
        {
            StreetFighterCharacter? car = Fighter?.Find(x => x.Name == name);
            if (car is null)
            {
                await car.WriteJsonToStream(context, HttpStatusCode.NotFound);
                return;
            }

            await car.WriteJsonToStream(context, HttpStatusCode.Found);
            return;
        }

        await Utilities.WriteTextToStream(context, "SOMETHING WENT WRONG", HttpStatusCode.ServiceUnavailable);
    }
}