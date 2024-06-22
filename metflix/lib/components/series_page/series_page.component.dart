
import 'package:flutter/material.dart';
import 'package:metflix/components/series_page/series_page.model.dart';
import 'package:metflix/util/view-model-builder.dart';

class SeriesPageComponent extends ViewModelBuilder<SeriesPageModel> {
  final String searchUrl;

  const SeriesPageComponent({super.key, required this.searchUrl});

  @override
  Widget builder(BuildContext context, SeriesPageModel viewModel, _) {
    return Center(
      child: Column(
        children: [
          Text(searchUrl),
          OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: Text("Back"),
          )
        ],
      ),
    );
  }

  @override
  SeriesPageModel viewModelBuilder(BuildContext context) {
    return SeriesPageModel(context);
  }
}
