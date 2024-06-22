
import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/foundation.dart';

class PopularitySeries {
  String title;
  String category;
  String url;
  Uint8List? image;

  PopularitySeries({required this.title, required this.category, required this.url, this.image});

  factory PopularitySeries.fromJson(Map<String, dynamic> json) {
    return PopularitySeries(
      title: json['title'],
      category: json['category'],
      url: json['url'],
      image: base64Decode(json['image'])
    );
  }
}