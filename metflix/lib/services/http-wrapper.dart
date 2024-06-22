import 'dart:async';
import 'dart:convert';

import 'package:metflix/model/popularity.dart';
import 'package:http/http.dart' as http;

class HttpWrapper {
  final String baseUrl = 'localhost:5271';
  final String popularity = 'api/stream/getpopularity';

  Future<List<PopularitySeries>> getPopularity() async {
    try {
      var response = await http.get(Uri.http(baseUrl, popularity));
      List list = jsonDecode(response.body);
      return list
          .map((element) => PopularitySeries.fromJson(element as Map<String, dynamic>))
          .toList();
    } on Exception catch(_) {
      return [];
    }
  }
}
