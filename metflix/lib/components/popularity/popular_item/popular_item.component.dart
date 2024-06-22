import 'package:flutter/material.dart';
import 'package:metflix/model/popularity.dart';

class PopularItem extends StatelessWidget {
  final defaultImage = "lib/assets/no-image.png";
  final PopularitySeries model;

  const PopularItem({super.key, required this.model});

  Widget get getImage {
    if (model.image != null) {
      return Image.memory(model.image!);
    }
    return Image.asset(defaultImage);
  }

  void onClick() {
    debugPrint(model.title);
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onClick,
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
