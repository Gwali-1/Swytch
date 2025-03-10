// See https://aka.ms/new-console-template for more information


using System.Diagnostics;

var client = new HttpClient();
var apCall = async () =>
{
    var r =await client.GetAsync("http://127.0.0.1:8080/");
};

var w = Enumerable.Range(0, 100000).Select(x => apCall).ToList();


var s = new Stopwatch();

s.Start();
await Parallel.ForEachAsync(w, async (i,c) =>
{
    await i();
});

s.Stop();

Console.WriteLine($"Elapsed time:`{s.Elapsed.Seconds}");