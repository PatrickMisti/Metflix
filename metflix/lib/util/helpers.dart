
class SeriesUrl {
  final String url;

  SeriesUrl(this.url);

  Map<String, dynamic> toJson() {
    return {"url": url};
  }
}