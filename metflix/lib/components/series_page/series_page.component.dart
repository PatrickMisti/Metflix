import 'package:better_player/better_player.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:metflix/components/series_page/series_page.model.dart';
import 'package:metflix/model/series_info.dart';
import 'package:metflix/model/stream_links.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/view-model-builder.dart';
import 'package:chewie/chewie.dart';
import 'package:video_player/video_player.dart';

class SeriesPageComponent extends ViewModelBuilder<SeriesPageModel> {
  final String searchUrl;

  const SeriesPageComponent({super.key, required this.searchUrl});

  List<DropdownMenuEntry<String>> getDropdownMenuSeason(
          Map<String, List<Series>> list) =>
      list.entries.map((element) {
        var title =
            element.key.contains("0") ? "Filme / OVA" : "Season ${element.key}";
        return DropdownMenuEntry(
          value: element.key,
          label: title,
        );
      }).toList();

  List<DropdownMenuEntry<String>> getDropdownMenuEpisode(List<Series> list) =>
      list.map((element) {
        return DropdownMenuEntry(
          value: element.episode,
          label: "Episode ${element.episode}",
        );
      }).toList();

  List<DropdownMenuEntry<String>> getDropdownMenuLinks(
          List<StreamInfoLinks> list) =>
      list.map((element) {
        return DropdownMenuEntry(
          value: element.languageTitle,
          label: element.languageTitle,
        );
      }).toList();

  List<DropdownMenuEntry<String>> getDropdownMenuStreamLink(
          List<StreamLink> list) =>
      list.map((element) {
        return DropdownMenuEntry(
          value: element.frameLink,
          label: element.linkType,
        );
      }).toList();

  @override
  Widget builder(BuildContext context, SeriesPageModel viewModel, _) {
    if (viewModel.isBusy) {
      return const Center(child: CircularProgressIndicator());
    }

    Size size = MediaQuery.of(context).size;

    return Card(
      child: ListView(
        children: [
          SizedBox(
            width: double.infinity,
            child: viewModel.getSeriesImage,
          ),
          ListTile(
            title: Text(viewModel.getSeriesName),
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              DropdownMenu<String>(
                label: const Text("Season/Filme"),
                dropdownMenuEntries:
                    getDropdownMenuSeason(viewModel.getSeriesBySeason()),
                initialSelection: viewModel.currentSeason,
                onSelected: viewModel.dropDownSeasonChanged,
              ),
              DropdownMenu<String>(
                dropdownMenuEntries:
                    getDropdownMenuEpisode(viewModel.getSeriesList),
                label: const Text("Episode"),
                initialSelection: viewModel.currentEpisode,
                onSelected: viewModel.dropDownEpisodeChanged,
              )
            ],
          ),
          Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              DropdownMenu(
                enabled: !viewModel.isEpisodeLoading,
                dropdownMenuEntries:
                    getDropdownMenuLinks(viewModel.getStreamInfoLinks),
                label: const Text("Sprache"),
                initialSelection: viewModel.currentLinkLanguage,
                onSelected: viewModel.dropDownLanguageLinkChanged,
              ),
              DropdownMenu(
                enabled: !viewModel.isEpisodeLoading,
                dropdownMenuEntries: getDropdownMenuStreamLink(viewModel
                    .getStreamLinkFromId(viewModel.currentLinkLanguage)),
                label: const Text("Provider"),
                initialSelection: viewModel.currentStreamLink.frameLink,
                onSelected: viewModel.dropDownStreamLinkChanged,
              )
            ],
          ),
          /*Chewie(
            controller: viewModel.controllerChewie!,
          ),*/
          /*viewModel.controller!.value.isInitialized
              ? Chewie(
            controller: viewModel.controllerChewie!,
          )
              : Container()*/
          /*VideoPlayer(viewModel.controller!)*/
          BetterPlayer(controller: viewModel.controller!)
        ],
      ),
    );
  }

  @override
  SeriesPageModel viewModelBuilder(BuildContext context) {
    final getIt = GetIt.instance;
    var http = getIt.get<HttpWrapper>();
    return SeriesPageModel(
      context,
      http,
      searchUrl: searchUrl,
    );
  }
}
