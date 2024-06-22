

import 'package:flutter/cupertino.dart';
import 'package:metflix/model/series_info.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/view-model-builder.dart';

class SeriesPageModel extends BaseModel {
  final BuildContext _context;
  final HttpWrapper _httpWrapper;
  final String searchUrl;
  SeriesInfo? info;

  SeriesPageModel(this._context, this._httpWrapper,{required this.searchUrl});

  @override
  void init() async {
    info = await _httpWrapper.getSeriesFromUrl(searchUrl);
    super.init();
  }
}