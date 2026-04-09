using Modeer360.Models;
using System.Collections.Concurrent;

namespace Modeer360.Repositories;

public class InMemoryStore
{
    // Permanently blocked countries
    private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new();

    // Temporarily blocked countries
    private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks = new();

    // Blocked attempt logs
    private readonly ConcurrentBag<BlockAttemptLog> _logs = new();

    // ===== Blocked Countries =====
    public bool AddBlockedCountry(BlockedCountry country) =>
        _blockedCountries.TryAdd(country.CountryCode.ToUpper(), country);

    public bool RemoveBlockedCountry(string countryCode) =>
        _blockedCountries.TryRemove(countryCode.ToUpper(), out _);

    public bool IsBlocked(string countryCode) =>
        _blockedCountries.ContainsKey(countryCode.ToUpper()) ||
        (_temporalBlocks.TryGetValue(countryCode.ToUpper(), out var tb) && !tb.IsExpired);

    public IEnumerable<BlockedCountry> GetAllBlocked() =>
        _blockedCountries.Values;

    // ===== Temporal Blocks =====
    public bool AddTemporalBlock(TemporalBlock block) =>
        _temporalBlocks.TryAdd(block.CountryCode.ToUpper(), block);

    public bool HasTemporalBlock(string countryCode) =>
        _temporalBlocks.ContainsKey(countryCode.ToUpper());

    public void RemoveExpiredTemporalBlocks()
    {
        foreach (var key in _temporalBlocks.Keys)
        {
            if (_temporalBlocks.TryGetValue(key, out var block) && block.IsExpired)
                _temporalBlocks.TryRemove(key, out _);
        }
    }

    // ===== Logs =====
    public void AddLog(BlockAttemptLog log) => _logs.Add(log);

    public IEnumerable<BlockAttemptLog> GetLogs() => _logs;
}