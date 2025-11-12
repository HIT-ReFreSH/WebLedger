using System.Text;
using HitRefresh.WebLedger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HitRefresh.WebLedger.Services;

public class DirectConfigManager(LedgerContext database, ILogger<DirectLedgerManager> logger)
    : IConfigManager
{
    private const int kSecretLength = 32;

    public async Task<string> AddAccess(string name)
    {
        try
        {
            var secret = RandomSecret();
            database.Access.Add(new LedgerAccess
            {
                Name = name,
                Key = secret
            });
            await database.SaveChangesAsync();
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
        database.Access.Remove(new LedgerAccess
        {
            Name = name
        });
        await database.SaveChangesAsync();
    }

    public async Task<LedgerAccess[]> GetAllAccess()
    {
        return await database.Access.ToArrayAsync();
    }

    private static string RandomSecret()
    {
        const string specialChars = "!@#$^&";
        const int offset = 10 + (26 << 1);
        var randomIntSpace = offset + specialChars.Length;
        var random = new Random();
        var sb = new StringBuilder();
        for (var i = 0; i < kSecretLength; i++)
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
    public async Task<bool> CheckAccess(string name, string key)
    {
        var access = await database.Access.FirstOrDefaultAsync(a => a.Name == name && a.Key == key);
        return access != null;
    }
    public async Task<bool> CheckDuplicate(string name)
    {
        var access = await database.Access.FirstOrDefaultAsync(a => a.Name == name);
        return access != null;
    }
}