using Application.Contracts.Commons;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infraestructure.RestServices.Implement;

public class RestBuilder : IRestBuilder
{

    private HttpMethod? _method { get; set; }
    private Dictionary<string, string>? _headers { get; set; }
    private string? _body { get; set; }
    private string? _baseUl { get; set; }
    private string? _apiUrl { get; set; }

    private Dictionary<string, object>? _DicToBody { get; set; }


    private readonly HttpClient _httpclient;

    public RestBuilder(HttpClient httpclient)
    {
        _httpclient = httpclient;
    }

    public IRestBuilder Method(HttpMethod method)
    {
        _method = method;
        return this;
    }

    public IRestBuilder Body(object body)
    {
        _body = body != null ? JsonSerializer.Serialize(body) : "";
        _DicToBody = JsonSerializer.Deserialize<Dictionary<string, object>>(_body)!;
        return this;
    }

    public IRestBuilder Header(Dictionary<string, string> headers)
    {
        _headers = headers;
        return this;
    }

    public IRestBuilder Urlbuilder(string baseUrl, string apiUrl)
    {
        _baseUl = baseUrl;
        _apiUrl = apiUrl;
        return this;
    }

    public async Task<T> Request<T>(string api) where T : class
    {
        T? vResp;
        try
        {
            var request = new HttpRequestMessage
            {
                Method = _method!,
                RequestUri = new Uri($"{_baseUl}/{_apiUrl}/{api}")
            };

            request.Headers.Add("Accept", "*/*");

            if (_headers != null)
            {
                foreach (var item in _headers!)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (!string.IsNullOrEmpty(_body))
            {
                _body = JsonSerializer.Serialize(_DicToBody);
                request.Content = new StringContent(_body!, Encoding.UTF8, "application/json");
            }

            using (var response = await _httpclient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                vResp = await response.Content.ReadFromJsonAsync<T>();
            }

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return vResp!;
    }

}
