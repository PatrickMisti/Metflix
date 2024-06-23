class SeriesUrl {
  final String url;

  SeriesUrl(this.url);

  Map<String, dynamic> toJson() {
    return {"url": url};
  }
}

// todo maybe enum but so much work around
class SeriesProvider {
  static const String Voe = "VOE";
  static const String Doodstream = "Doodstream";
  static const String Vidoza = "Vidoza";
  static const String Streamtape = "Streamtape";

}

class ProviderUrl {
  final String url;
  final String provider;

  ProviderUrl({required this.url, required this.provider});

  Map<String, dynamic> toJson() {
    return {"url": url, "provider": provider};
  }
}
