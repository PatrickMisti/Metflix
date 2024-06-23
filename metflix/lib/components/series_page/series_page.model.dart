
import 'package:flutter/cupertino.dart';
import 'package:metflix/model/series_info.dart';
import 'package:metflix/model/stream_links.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/helpers.dart';
import 'package:metflix/util/view-model-builder.dart';

class SeriesPageModel extends BaseModel {
  final BuildContext _context;
  final HttpWrapper _httpWrapper;
  final String searchUrl;
  SeriesInfo? info;
  List<StreamInfoLinks>? links;
  late final iframe;

  SeriesPageModel(this._context, this._httpWrapper, {required this.searchUrl});

  @override
  void init() async {
    info = await _httpWrapper.getSeriesFromUrl(searchUrl);
    links = await _httpWrapper.getStreamLinksFromSeries(info!.series.first);

    var iFrameElement =
        "https://aniworld.to${links!.first.streamLinks.first.frameLink}";
    // VOE // Doodstream // Vidoza // Streamtape
    debugPrint(links!.first.streamLinks.first.frameLink);
    var s = await _httpWrapper.getLinkForPlayer(ProviderUrl(url: links!.first.streamLinks[1].frameLink, provider: links!.first.streamLinks[1].linkType));
    debugPrint(iFrameElement);
    debugPrint(s);
    //iframe = <iframe src='$iFrameElement'/>;
    super.init();
  }
}
