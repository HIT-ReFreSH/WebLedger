using System.Text;
using HitRefresh.WebLedger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HitRefresh.WebLedger.Services;

public class DirectConfigManager : IConfigManager
{
    private const int SecretLength = 32;
    private readonly LedgerContext _database;
    private readonly ILogger<DirectLedgerManager> _logger;

    public DirectConfigManager(LedgerContext database, ILogger<DirectLedgerManager> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<string> AddAccess(string name)
    {
        try
        {
            var secret = RandomSecret();
            _database.Access.Add(new LedgerAccess
            {
                Name = name,
                Key = secret
            });
            await _database.SaveChangesAsync();
            return secret;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task RemoveAccess(string name)
    {
        _database.Access.Remove(new LedgerAccess
        {
            Name = name
        });
        await _database.SaveChangesAsync();
    }

    public async Task<LedgerAccess[]> GetAllAccess()
    {
        return await _database.Access.ToArrayAsync();
    }

    private static string RandomSecret()
    {
        const string specialChars = "!@#$^&";
        const int offset = 10 + (26 << 1);
        var randomIntSpace = offset + specialChars.Length;
        var random = new Random();
        var sb = new StringBuilder();
        for (var i = 0; i < SecretLength; i++)
        {
            var rnd = random.Next(randomIntSpace);
            var c = rnd switch
            {
                < 10 => rnd.ToString()[0],
                >= 10 and < 36 => (char)('A' + (rnd - 10)),
                >= 36 and < offset => (char)('a' + (rnd - 36)),
                >= offset => specialChars[rnd - offset]
            };
            sb.Append(c);
        }

        return sb.ToString();
    }
}