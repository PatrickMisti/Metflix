
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:metflix/components/popularity/popular_item/popular_item.component.dart';
import 'package:metflix/components/popularity/popularity.model.dart';
import 'package:metflix/services/http-wrapper.dart';
import 'package:metflix/util/view-model-builder.dart';

class PopularityComponent extends ViewModelBuilder<PopularityModel> {
  const PopularityComponent({super.key});

  setListViewBody(PopularityModel model) {
    if (model.isBusy) return const Center(child: CircularProgressIndicator());

    return CustomScrollView(
      slivers: [
        SliverGrid.builder(
          gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: 250),
          itemCount: model.series.length,
          itemBuilder: (context, index) => PopularItem(
            model: model.series[index],
            goTo: model.goToPage,
          ),
        )
      ],
    );
  }

  @override
  Widget builder(
      BuildContext context, PopularityModel viewModel, Widget? child) {
    return SafeArea(
      child: setListViewBody(viewModel),
    );
  }

  @override
  PopularityModel viewModelBuilder(BuildContext context) {
    final getIt = GetIt.instance;
    var http = getIt.get<HttpWrapper>();
    return PopularityModel(context, http);
  }
}
