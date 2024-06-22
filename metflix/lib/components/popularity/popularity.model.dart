
import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import 'package:metflix/model/popularity.dart';
import 'package:metflix/router.config.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/view-model-builder.dart';

class PopularityModel extends BaseModel {
  final BuildContext _context;
  final HttpWrapper _httpWrapper;

  List<PopularitySeries> series = [];

  PopularityModel(this._context, this._httpWrapper);

  @override
  init() async {
    series = await getAllPopularity();
    super.init();
  }

  Future<List<PopularitySeries>> getAllPopularity() async {
    return await _httpWrapper.getPopularity();
  }

  void goToPage(String url) {
    _context.go("${RouterEnumConfig.home}/${RouterEnumConfig.homeId}", extra: {"id": url});
  }
}