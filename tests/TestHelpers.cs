using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Swytch.App;
using Swytch.Structures;

namespace tests;

public static class TestHelpers
{
    public async static Task StartTestServer(string uri, SwytchApp testServer)
    {
        Task.Run(async () => { await testServer.Listen("http://localhost:6081/"); });
    }
}