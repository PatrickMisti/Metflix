using Metflix.Models;

namespace Metflix.Controllers.helpers
{
    public record SeriesUrl(string url);

    public record ProviderUrl(string url, SeriesProvider provider)
    {
        public static ProviderUrl ToMap(ProviderUrlDao providerDao)
        {
            Enum.TryParse<SeriesProvider>(providerDao.provider, out var provider);
            return new ProviderUrl(providerDao.url, provider);
        }
    }

    public record ProviderUrlDao(string url, string provider);
}
