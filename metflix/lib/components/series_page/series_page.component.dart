import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:metflix/components/series_page/series_page.model.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/view-model-builder.dart';

class SeriesPageComponent extends ViewModelBuilder<SeriesPageModel> {
  final String searchUrl;

  const SeriesPageComponent({super.key, required this.searchUrl});

  Widget getVideoPlayer(SeriesPageModel model) {
    if (model.isBusy) return const Center(child: CircularProgressIndicator());


    //return model.iframe;
    return Text("data");
  }

  @override
  Widget builder(BuildContext context, SeriesPageModel viewModel, _) {
    return Center(
      child: Column(
        children: [
          Text(viewModel.isBusy
              ? searchUrl
              : viewModel.links?.first.languageTitle ?? "Null"),
          getVideoPlayer(viewModel),
          OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: Text("Back"),
          ),
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
