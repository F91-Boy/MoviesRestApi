using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;
using System.Text.Json;

//直接使用
//var movieApi = RestService.For<IMoviesApi>("http://localhost:5001");

//通过工厂使用
var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s=> new RefitSettings
    {
       AuthorizationHeaderValueGetter = (request,token)=>s.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(x => x.BaseAddress = new Uri("http://localhost:5001"));

var provider = services.BuildServiceProvider();

var movieApi = provider.GetRequiredService<IMoviesApi>();

//通过Get获取单部电影
//var movie = await movieApi.GetMovieAsync("nixon-1995");

//Console.WriteLine(JsonSerializer.Serialize(movie));

//通过Get获取所有电影
Console.WriteLine("获取所有电影");
var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 5
};
var movies = await movieApi.GetAllMoviesAsync(request);
foreach (var movieResponse in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}


//创建一部电影
//Console.WriteLine("创建一部电影");
//var newMovie = await movieApi.CreateMovieAsync(new CreateMovieRequest
//{
//    Title = "SpiderMan 2",
//    YearOfRelease = 2002,
//    Genres = ["Action"]
//});

//更新一部电影
//Console.WriteLine("更新一部电影");
//await movieApi.UpdateMovieAsync(Guid.Parse("c7512a00-23d6-41a0-8585-c345c33cc3cc"), new UpdateMovieRequest
//{
//    Title = "SpiderMan 2",
//    YearOfRelease = 2006,
//    Genres = ["Action"]
//});



//删除一部电影
Console.WriteLine("删除一部电影");
await movieApi.DeleteMovieAsync(Guid.Parse("c7512a00-23d6-41a0-8585-c345c33cc3cc"));




