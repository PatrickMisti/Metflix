import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/cupertino.dart';
import 'package:metflix/model/popularity.dart';
import 'package:http/http.dart' as http;
import 'package:metflix/model/series_info.dart';

class HttpWrapper {
  final String baseUrl = 'localhost:5271';
  final String _popularity = 'api/stream/getpopularity';
  final String _series = 'api/stream/series';

  Future<List<PopularitySeries>> getPopularity() async {
    try {
      var response = await http.get(Uri.http(baseUrl, _popularity));
      List list = jsonDecode(response.body);
      return list
          .map((element) =>
              PopularitySeries.fromJson(element as Map<String, dynamic>))
          .toList();
    } on Exception catch (_) {
      return [];
    }
  }

  Future<SeriesInfo?> getSeriesFromUrl(String url) async {
    try {
      debugPrint(jsonEncode(SeriesUrl(url)..toJson()));
      debugPrint(Uri.http(baseUrl, _series).toString());
      var result = await http
          .post(Uri.http(baseUrl, _series),
              headers: <String, String>{'Content-Type': 'application/json'},
              body: jsonEncode(SeriesUrl(url)..toJson()))
          .then((response) {
        var element = jsonDecode(response.body);
        return SeriesInfo.fromJson(element as Map<String, dynamic>);
      }).catchError((onError) {
        debugPrint(onError);
        return SeriesInfo(title: "", description: "", image: Uint8List(0), series: []);
      });

      return result;
    } on Exception catch (_) {
      return null;
    }
  }
}

class SeriesUrl {
  final String url;

  SeriesUrl(this.url);

  Map<String, dynamic> toJson() {
    return {"url": url};
  }
}
