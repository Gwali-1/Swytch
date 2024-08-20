using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;
using Xunit;

namespace tests;

public class Tests
{

    private static SwytchApp s = new SwytchApp() ;
    public Tests()
    {
        //create server and start it herw
    }

    [Fact]
    public void TestUnit()
    {
        var mc = TestHelpers.GetRequestContextInstance("");
        var r = mc.Object;

        //arrange

        //act
        var have = r.IsAuthenticated;

        //assert
        Assert.False(have);
    }


    // [Theory]
    // [InlineData("/",HttpStatusCode.OK,"home")]
    // [InlineData("/profile/page",HttpStatusCode.OK,"profile")]
    // [InlineData("/authentication",HttpStatusCode.OK,"auth")]
    [Fact]
    [InlineData("/kingship", HttpStatusCode.OK, "king")]
    public async Task TestIntegration()
    {
        using (HttpListener listener = new HttpListener())
        {
            //Arrange

            var s = new SwytchApp();
            s.AddAction("GET", "/kingship",
                async c => { Utilities.WriteTextToStream(c, "king", HttpStatusCode.Accepted); });

            //put the server on a different thread 
            Task.Run(async () => { await s.Listen("http://localhost:8081/"); });

            using (var client = new HttpClient())
            {
                //Act 
                var gs = "http://localhost:8081/kingship";
                var response = await client.GetAsync(gs);
                var respo = await response.Content.ReadAsStringAsync();

                //Assert
                Assert.Equal("king", respo);
                Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            }
        }
    }
    
    
    
    
    [Theory]
    [InlineData("/",HttpStatusCode.OK,"home")]
    [InlineData("/profile/page",HttpStatusCode.OK,"profile")]
    [InlineData("/authentication",HttpStatusCode.OK,"auth")]
    [InlineData("/kingship",HttpStatusCode.OK,"king")]
    public async Task TestIntegration2(string p, HttpStatusCode co, string r)
    {
        using (HttpListener listener = new HttpListener())
        {
            
            //Arrange

            s.AddAction("GET",p, async c =>
            {
                Utilities.WriteTextToStream(c, p, co);
            });

            //put the server on a different thread 
            Task.Run(async () =>
            {
                await s.Listen("http://localhost:8081/");
            });

            using (var client = new HttpClient())
            {
                
                //Act 
                var gs = "http://localhost:8081"+p;
                var response = await client.GetAsync(gs);
                var respo = await response.Content.ReadAsStringAsync();
                
                //Assert
                Assert.Equal(p,respo);
                Assert.Equal(co, response.StatusCode);
            }
        }
    }
}