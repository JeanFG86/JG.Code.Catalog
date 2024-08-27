using JG.Code.Catalog.EndToEndTests.Extensions.String;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace JG.Code.Catalog.EndToEndTests.Common;

class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();
}

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(String route, object payload)
        where TOutput : class
    {
        var response = await _httpClient.PostAsync(
            route, new StringContent(
                JsonSerializer.Serialize(payload, _serializerOptions), 
                Encoding.UTF8, 
                "application/json")
            );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(String route, object? queryStringParametersObject = null)
       where TOutput : class
    {
        var url = PrepareGetRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(url);            
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(String route)
       where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(route);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(String route, object payload)
        where TOutput : class
    {
        var response = await _httpClient.PutAsync(
            route, new StringContent(
                JsonSerializer.Serialize(payload, _serializerOptions),
                Encoding.UTF8,
                "application/json")
            );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    private async Task<TOutput?> GetOutput<TOutput>(HttpResponseMessage response) where TOutput : class
    {
        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, _serializerOptions);
        }
        return output;
    }

    private string PrepareGetRoute(string route, object? queryStringParametersObject)
    {
        if (queryStringParametersObject is null)
            return route;
        var parametersJson = JsonSerializer.Serialize(queryStringParametersObject);
        var parametersDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }
}
