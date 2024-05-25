using System.Text.Json;

namespace Metflix.HttpWrappers
{
    public class HttpWrapper(string url) : IHttpWrapper
    {
        private readonly JsonSerializerOptions _defaultOptions = new() { PropertyNameCaseInsensitive = true };

        protected readonly HttpClient Client = new()
        {
            BaseAddress = new Uri(url),
            Timeout = TimeSpan.FromSeconds(5) // not sure if too much
        };
        

        /// <summary>
        /// Get all from uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Stream</returns>
        public async Task<Stream> GetAllAsync(string uri)
        {
            HttpResponseMessage response = await Client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Search for list of series by form data call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IList<T>> SearchWithFormDataAsync<T>(string path, string key, string search)
        {
            MultipartFormDataContent ss = new()
            {
                {new StringContent(search), key}
            };

            using var result = await Client.PostAsync(path, ss);
            result.EnsureSuccessStatusCode();

            var streamResult = await result.Content.ReadAsStreamAsync();
            var res = await JsonSerializer.DeserializeAsync<IList<T>>(streamResult, _defaultOptions);
            return res ?? [];
        }
    }
}
