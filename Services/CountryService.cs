using Modeer360.Models;
using Modeer360.Repositories;

namespace Modeer360.Services;

public class CountryService
{
    private readonly InMemoryStore _store;

    public CountryService(InMemoryStore store)
    {
        _store = store;
    }

    public (bool Success, string Message) AddBlockedCountry(string countryCode, string countryName)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            return (false, "Invalid country code.");

        var country = new BlockedCountry
        {
            CountryCode = countryCode.ToUpper(),
            CountryName = countryName
        };

        bool added = _store.AddBlockedCountry(country);
        return added
            ? (true, "Country blocked successfully.")
            : (false, "Country is already blocked.");
    }

    public (bool Success, string Message) RemoveBlockedCountry(string countryCode)
    {
        bool removed = _store.RemoveBlockedCountry(countryCode);
        return removed
            ? (true, "Country unblocked successfully.")
            : (false, "Country not found in blocked list.");
    }

    public IEnumerable<BlockedCountry> GetBlockedCountries(int page, int pageSize, string? search)
    {
        var all = _store.GetAllBlocked();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToUpper();
            all = all.Where(c =>
                c.CountryCode.Contains(search) ||
                c.CountryName.ToUpper().Contains(search));
        }

        return all
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public (bool Success, string Message) AddTemporalBlock(string countryCode, string countryName, int durationMinutes)
    {
        if (durationMinutes < 1 || durationMinutes > 1440)
            return (false, "Duration must be between 1 and 1440 minutes.");

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            return (false, "Invalid country code.");

        if (_store.HasTemporalBlock(countryCode))
            return (false, "Country is already temporarily blocked.");

        var block = new TemporalBlock
        {
            CountryCode = countryCode.ToUpper(),
            CountryName = countryName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes)
        };

        _store.AddTemporalBlock(block);
        return (true, $"Country blocked temporarily for {durationMinutes} minutes.");
    }
}