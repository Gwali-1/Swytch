using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swytch.Structures;
using Xunit;

namespace tests;

public class Tests
{

    [Fact]
    public void Addition()
    {

        var mc = TestHelpers.GetRequestContextInstance("");
        var r = mc.Object;
        
        //arrange
        
        //act
        var have = r.IsAuthenticated;

        //assert
        Assert.False(have);
    }
}