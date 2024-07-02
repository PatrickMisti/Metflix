import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/cupertino.dart';
import 'package:metflix/model/popularity.dart';
import 'package:http/http.dart' as http;
import 'package:metflix/model/series_info.dart';
import 'package:metflix/model/stream_links.dart';
import 'package:metflix/util/helpers.dart';

class HttpWrapper {
  final String baseUrl  = "10.0.2.2:5271";
  final String androidEmulatorUrl = 'localhost:5271';
  final String _popularity = 'api/stream/getpopularity';
  final String _series = 'api/stream/series';
  final String _stream = 'api/stream/streamlink';
  final String _link = 'api/stream/link';

  Future<List<PopularitySeries>> getPopularity() async {
    try {
      debugPrint("GET ${Uri.http(baseUrl, _popularity)}");
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
      debugPrint("POST ${Uri.http(baseUrl, _series)}: ${jsonEncode(SeriesUrl(url)..toJson())}");
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

  Future<List<StreamInfoLinks>?> getStreamLinksFromSeries(Series series) async {
    try {
      debugPrint("POST ${Uri.http(baseUrl, _stream)}: ${jsonEncode(series.toJson())}");
      var result = await http
          .post(Uri.http(baseUrl, _stream),
          headers: <String, String>{'Content-Type': 'application/json'},
          body: jsonEncode(series.toJson()))
          .then((response) {
        List element = jsonDecode(response.body);
        return element.map((item) => StreamInfoLinks.fromJson(item as Map<String,dynamic>)).toList();
      }).catchError((onError) {
        debugPrint(onError);
        return <StreamInfoLinks>[];
      });

      return result;
    } on Exception catch (_) {
      return null;
    }
  }

  Future<String?> getLinkForPlayer(ProviderUrl provider) async {
    try {
      //todo need provider to get correct data
      //todo voe is m3u8 file
      //todo doodlestream is normal video player
      debugPrint("POST ${Uri.http(baseUrl, _link)}: ${jsonEncode(provider.toJson())}");
      var result = await http
          .post(Uri.http(baseUrl, _link),
          headers: <String, String>{'Content-Type': 'application/json'},
          body: jsonEncode(provider.toJson()))
          .then((response) {
        var element = response.body;
        return element;
      }).catchError((onError) {
        debugPrint(onError);
        return "";
      });

      return result;
    } on Exception catch (_) {
      return null;
    }
  }
}
