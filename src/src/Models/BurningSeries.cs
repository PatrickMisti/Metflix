using System.Collections.Immutable;

namespace Metflix.Models;

public record SearchEntry(string Title, string Description, string Link);

public record StreamLink(string FrameLink, string LinkType);

public record StreamInfoLinks(string LanguageTitle, IList<StreamLink>  StreamLinks);

public record Series(string Season, string Episode, string Url);

public record SeriesInfo(string Title, string Description, byte[] Image, IImmutableList<Series> Series);

public record PopularitySeries(string Title, string Category, string Url ,byte[] Image);