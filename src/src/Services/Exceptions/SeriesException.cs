namespace Metflix.Services.Exceptions
{
    [Serializable]
    public class SeriesListNotFoundException: Exception
    {
        public SeriesListNotFoundException() { }

        public SeriesListNotFoundException(string message) : base(message) { }

        public SeriesListNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class SeriesInfoNotFoundException : Exception
    {
        public SeriesInfoNotFoundException() { }

        public SeriesInfoNotFoundException(string message) : base(message) { }

        public SeriesInfoNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public class SeriesLanguageNotFoundException : Exception
    {
        public SeriesLanguageNotFoundException() { }

        public SeriesLanguageNotFoundException(string message) : base(message) { }

        public SeriesLanguageNotFoundException(string message, Exception e) : base(message, e) { }
    }

    [Serializable]
    public class StreamLinkNotFoundException : Exception
    {
        public StreamLinkNotFoundException() { }

        public StreamLinkNotFoundException(string message) : base(message) { }

        public StreamLinkNotFoundException(string message, Exception e) : base(message, e) { }
    }

    [Serializable]
    public class SeasonLinkNotFoundException : Exception
    {
        public SeasonLinkNotFoundException() { }

        public SeasonLinkNotFoundException(string message) : base(message) { }

        public SeasonLinkNotFoundException(string message, Exception e) : base(message, e) { }
    }
}
