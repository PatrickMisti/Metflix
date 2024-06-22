import 'dart:convert';

import 'package:metflix/model/popularity.dart';
import 'package:http/http.dart' as http;

class HttpWrapper {
  final String baseUrl = 'http://localhost:5271';
  final String popularity = 'api/stream/getpopularity';

  Future<List<PopularitySeries>> getPopularity() async {
    var response = await http.get(Uri.http(baseUrl, popularity));
    return jsonDecode(response.body)
        .map((element) =>
            PopularitySeries.fromJson(element as Map<String, dynamic>))
        .toList();
  }
}
