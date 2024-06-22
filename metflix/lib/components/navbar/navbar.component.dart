
import 'package:flutter/material.dart';
import 'package:metflix/components/navbar/navbar.model.dart';
import 'package:metflix/util/view-model-builder.dart';

class NavbarComponent extends ViewModelBuilder<NavbarModel> {
  final Widget child;

  const NavbarComponent({super.key, required this.child});

  AppBar? getAppbar(NavbarModel model) {
    if (!model.isTable && !model.isDesktop) return null;
    return AppBar(
      title: Text(model.appBarTitle),
      leading: Builder(
        builder: (context) => IconButton(
          onPressed: () => Scaffold.of(context).openDrawer(),
          icon: const Icon(Icons.menu),
        ),
      ),
    );
  }

  BottomNavigationBar? getBottomNavBar(NavbarModel model) {
    if (!model.isMobile) return null;

    return BottomNavigationBar(
      onTap: (value) => model.setPage(value),
      backgroundColor: const Color(0xffe0b9f6),
      currentIndex: model.currentIndex,
      items: model.navi
          .map((element) => BottomNavigationBarItem(
              icon: Icon(element.icon), label: element.name))
          .toList(),
    );
  }

  Drawer getDrawer(NavbarModel model) => Drawer(
        child: ListView(
            padding: EdgeInsets.zero,
            children: model.navi
                .map((element) => ListTile(
                      title: Text(element.name),
                      leading: Icon(element.icon),
                      selected: model.currentIndex == element.id,
                      onTap: () {
                        model.setPage(element.id);
                        model.popNavigation();
                      },
                    ))
                .toList()),
      );

  @override
  NavbarModel viewModelBuilder(BuildContext context) {
    return NavbarModel(context);
  }

  @override
  Widget builder(BuildContext context, NavbarModel viewModel, Widget? child) {
    return Scaffold(
      body: this.child,
      bottomNavigationBar: getBottomNavBar(viewModel),
      drawer: getDrawer(viewModel),
      appBar: getAppbar(viewModel),
    );
  }
}
