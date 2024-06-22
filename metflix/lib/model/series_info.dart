import 'dart:convert';

import 'package:flutter/foundation.dart';

class SeriesInfo {
  final String title;
  final String description;
  final Uint8List image;
  final List<Series> series;

  SeriesInfo(
      {required this.title,
      required this.description,
      required this.image,
      required this.series});

  factory SeriesInfo.fromJson(Map<String, dynamic> json) {
    List series = json['series'];
    return SeriesInfo(
      title: json['title'],
      description: json['description'],
      image: base64Decode(json["image"]),
      series: series
          .map((element) => Series.fromJson(element as Map<String, dynamic>))
          .toList(),
    );
  }
}

class Series {
  final String season;
  final String episode;
  final String url;

  Series({required this.season, required this.episode, required this.url});

  factory Series.fromJson(Map<String, dynamic> json) {
    return Series(
      season: json['season'],
      episode: json['episode'],
      url: json['url'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      "season": season,
      "episode": episode,
      "url": url
    };
  }
}
