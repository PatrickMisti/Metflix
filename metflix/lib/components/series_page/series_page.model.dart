import 'dart:async';

import 'package:chewie/chewie.dart';
import 'package:flutter/material.dart';
import 'package:video_player/video_player.dart';
import 'package:metflix/model/series_info.dart';
import 'package:metflix/model/stream_links.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/helpers.dart';
import 'package:metflix/util/view-model-builder.dart';
import 'package:collection/collection.dart';
import 'package:better_player/better_player.dart';

class SeriesPageModel extends BaseModel {
  final BuildContext _context;
  final HttpWrapper _httpWrapper;
  final String searchUrl;
  SeriesInfo? series;
  List<StreamInfoLinks>? _links;

  String? _currentLinkLanguage;
  StreamLink? _currentStreamLink;
  String? _currentSeason;
  String? _currentEpisode;
  String? _currentStream;
  Map<String, List<Series>>? _currentSeries;

  bool isEpisodeLoading = false;
  bool isStreamLoading = false;

  final defaultImage = "lib/assets/no-image.png";

  BetterPlayerController? controller;

  SeriesPageModel(this._context, this._httpWrapper, {required this.searchUrl});

  @override
  void init() async {
    series = await _httpWrapper.getSeriesFromUrl(searchUrl);
    _currentSeries = getSeriesBySeason();
    _currentSeason = _currentSeries!.entries.first.key;
    _currentEpisode = _currentSeries!.entries.first.value.first.episode;
    await loadLinksFromEpisode();
    await loadStreamFromPlayer();
    await setUpPlayerController();
    super.init();
  }

  String url ="";
  setUpPlayerController() async {
    url = "https://delivery-node-bfpibfrrprmpeojr.voe-network.net/engine/hls2-c/01/11112/munelcc6cdw1_,n,l,.urlset/master.m3u8?t=yvRjsiUgzBxH0OFZcP1sUXDpSuZ2iGxHX6OdURd2Wug&s=1719658191&e=14400&f=55815614&node=delivery-node-o92n1eqci9twxhqi.voe-network.net&i=190.2&sp=2500&asn=49981";
    debugPrint("setup");

    controller = BetterPlayerController(
      BetterPlayerConfiguration(

      ),
      betterPlayerDataSource: BetterPlayerDataSource.network(url),
    );
    /*controller = VideoPlayerController.networkUrl(Uri.parse(url));
    await controller?.initialize();
    //debugPrint("middle:"+controller!.dataSource.toString());
    controllerChewie = ChewieController(videoPlayerController: controller!,looping: true, autoPlay: true);*/
    //debugPrint(controllerChewie!.toString());
  }

  Future<void> loadLinksFromEpisode() async {
    _links = await _httpWrapper.getStreamLinksFromSeries(getSeries());
    _currentLinkLanguage = _links!.first.languageTitle;
    _currentStreamLink = _links!.first.streamLinks.first;
  }

  Future<void> loadStreamFromPlayer() async {
    _currentStream = await _httpWrapper.getLinkForPlayer(ProviderUrl(url: _currentStreamLink!.frameLink, provider: _currentStreamLink!.linkType));
  }

  Image get getSeriesImage => series?.image != null ? Image.memory(series!.image) : Image.asset(defaultImage);

  String get getSeriesName => series?.title ?? 'No Title';

  String get currentSeason => _currentSeason ?? _currentSeries!.entries.first.key;

  String get currentEpisode => _currentEpisode ?? _currentSeries![currentSeason]!.first.episode;

  String get currentLinkLanguage => _currentLinkLanguage ?? "No Service";

  StreamLink get currentStreamLink => _currentStreamLink ?? StreamLink(frameLink: "", linkType: "No Service");

  Map<String, List<Series>> getSeriesBySeason() {
    return groupBy(series!.series, (item) => item.season);
  }

  Series getSeries() => _currentSeries![_currentSeason!]!.firstWhere((element) => element.episode.contains(_currentEpisode!));

  List<Series> get getSeriesList => getEpisodeFromId(_currentSeason!);

  List<StreamInfoLinks> get getStreamInfoLinks => _links!.toList();


  List<StreamLink> getStreamLinkFromId(String id) =>_links!.firstWhere((element) => element.languageTitle.contains(_currentLinkLanguage!)).streamLinks;

  List<Series> getEpisodeFromId(String id) => _currentSeries![id]!.toList();


  dropDownSeasonChanged(String? newCurrentSeason) {
    _currentSeason = newCurrentSeason;
    _currentEpisode = _currentSeries![_currentSeason]!.first.episode;
    notifyListeners();
  }

  dropDownEpisodeChanged(String? newCurrentEpisode) {
    if (isEpisodeLoading) {
      notifyListeners();
      return;
    }
    _currentEpisode = newCurrentEpisode;
    isEpisodeLoading = true;
    onEpisodeChangedUpdateLinks();
  }

  dropDownLanguageLinkChanged(String? newCurrentLanguage) {
    _currentLinkLanguage = newCurrentLanguage;
    _currentStreamLink = _links!.firstWhere((item) => item.languageTitle.contains(_currentLinkLanguage!)).streamLinks.first;
    notifyListeners();
  }

  dropDownStreamLinkChanged(String? newCurrentStreamLink) {
    _currentStreamLink = getStreamLinkFromId(_currentLinkLanguage!).firstWhere((element) => element.frameLink.contains(newCurrentStreamLink!));
    onStreamLinkChosen();
  }

  onEpisodeChangedUpdateLinks() async {
    await loadLinksFromEpisode();
    isEpisodeLoading = false;
    notifyListeners();
  }

  onStreamLinkChosen() async {
    await loadStreamFromPlayer();
    isStreamLoading = false;
    notifyListeners();
  }

  @override
  void dispose() {
    controller?.dispose();
    super.dispose();
  }
}
