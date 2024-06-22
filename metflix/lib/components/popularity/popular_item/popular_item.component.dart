import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:metflix/model/popularity.dart';
import 'package:metflix/router.config.dart';

class PopularItem extends StatelessWidget {
  final defaultImage = "lib/assets/no-image.png";
  final PopularitySeries model;
  final Function(String url) goTo;

  const PopularItem({super.key, required this.model, required this.goTo});

  Widget get getImage {
    if (model.image != null) {
      return Image.memory(model.image!);
    }
    return Image.asset(defaultImage);
  }

  void onClick(BuildContext context) {
    debugPrint(model.title);
    goTo(model.url);
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => onClick(context),
      child: Card(
        child: SafeArea(
          child: Column(
            children: [
              Expanded(child: ListTile(title: getImage)),
              ListTile(
                title: Text(
                  model.title,
                  overflow: TextOverflow.fade,
                  maxLines: 3,
                ),
              )
            ],
          ),
        ),
      ),
    );
  }
}
