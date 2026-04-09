using Modeer360.Models;
using Modeer360.Repositories;
using System.Net;
using System.Text.Json;

namespace Modeer360.Services;

public class IpService
{
    private readonly HttpClient _httpClient;
    private readonly InMemoryStore _store;
    private readonly IConfiguration _config;

    public IpService(HttpClient httpClient, InMemoryStore store, IConfiguration config)
    {
        _httpClient = httpClient;
        _store = store;
        _config = config;
    }

    public bool IsValidIp(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }

    public async Task<IpLookupResult?> LookupIpAsync(string ipAddress)
    {
        var apiKey = _config["IpApi:ApiKey"];
        var url = $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ipAddress}";

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            return new IpLookupResult
            {
                IpAddress = ipAddress,
                CountryCode = root.GetProperty("country_code2").GetString() ?? "",
                CountryName = root.GetProperty("country_name").GetString() ?? "",
                Isp = root.GetProperty("isp").GetString() ?? ""
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<(IpLookupResult? Result, bool IsBlocked)> CheckBlockAsync(string ipAddress)
    {
        var result = await LookupIpAsync(ipAddress);
        if (result == null) return (null, false);

        bool isBlocked = _store.IsBlocked(result.CountryCode);
        return (result, isBlocked);
    }

    public void AddLog(BlockAttemptLog log) => _store.AddLog(log);

    public IEnumerable<BlockAttemptLog> GetLogs(int page, int pageSize)
    {
        return _store.GetLogs()
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}