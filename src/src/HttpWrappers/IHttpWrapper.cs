namespace Metflix.HttpWrappers;

public interface IHttpWrapper
{
    Task<Stream> GetAllStreamAsync(string uri);
    Task<IList<T>> SearchWithFormDataAsync<T>(string path, string key, string search);
}