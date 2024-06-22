class StreamLink {
  final String frameLink;
  final String linkType;

  StreamLink({required this.frameLink, required this.linkType});

  factory StreamLink.fromJson(Map<String, dynamic> json) {
    return StreamLink(
      frameLink: json['frameLink'],
      linkType: json['linkType'],
    );
  }
}

class StreamInfoLinks {
  final String languageTitle;
  final List<StreamLink> streamLinks;

  StreamInfoLinks({required this.languageTitle, required this.streamLinks});

  factory StreamInfoLinks.fromJson(Map<String, dynamic> json) {
    List list = json['streamLinks'];
    return StreamInfoLinks(
      languageTitle: json['languageTitle'],
      streamLinks: list
          .map(
              (element) => StreamLink.fromJson(element as Map<String, dynamic>))
          .toList(),
    );
  }
}
