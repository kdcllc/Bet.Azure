using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Bet.Azure.Messaging.Options;

using Microsoft.Extensions.Options;

namespace Bet.Azure.Messaging.Internal;

internal class SasTokenGenerator : ISasTokenGenerator
{
    private SasTokenGeneratorOptions _options;

    public SasTokenGenerator(IOptionsMonitor<SasTokenGeneratorOptions> optionsMonitor)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(o => _options = o);
    }

    public string Create(string topicName, int days, CancellationToken cancellationToken)
    {
        var resourceUri = $"https://{_options.Namespace}.servicebus.windows.net/{topicName}/messages";

        return GetSasToken(resourceUri, _options?.SasKeyName ?? string.Empty, _options?.SasKey ?? string.Empty, TimeSpan.FromDays(days));
    }

    private string GetSasToken(string resourceUri, string keyName, string key, TimeSpan ttl)
    {
        var expiry = GetExpiry(ttl);
        var stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        return string.Format(
            CultureInfo.InvariantCulture,
            "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
            HttpUtility.UrlEncode(resourceUri),
            HttpUtility.UrlEncode(signature),
            expiry,
            keyName);
    }

    private string GetExpiry(TimeSpan ttl)
    {
        var expirySinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1) + ttl;
        return Convert.ToString((int)expirySinceEpoch.TotalSeconds);
    }
}
