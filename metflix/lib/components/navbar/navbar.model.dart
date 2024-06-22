
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:metflix/model/utilities.dart';
import 'package:metflix/router.config.dart';
import 'package:metflix/util/service-config.dart';
import 'package:metflix/util/view-model-builder.dart';
import 'package:responsive_framework/responsive_framework.dart';

class NavbarModel extends BaseModel {
  final BuildContext _context;
  int _currentIndex = 0;
  String get appBarTitle => "Metflix";
  List<NavElement> navi = [
    NavElement(0,"Home",Icons.newspaper),
    NavElement(1, "MyList", Icons.home),
    NavElement(2, "Settings", Icons.settings)
  ];


  NavbarModel(this._context);

  int get currentIndex => _currentIndex;

  BuildContext get context => _context;

  void setPage(int value) {
    switch (value) {
      case 0:
        _context.go(RouterEnumConfig.home);
        break;
      case 1:
        _context.go(RouterEnumConfig.news);
        break;
      default:
        _context.go(RouterEnumConfig.setting);
        break;
    }
    _currentIndex = value;
  }

  bool get isMobile => ResponsiveBreakpoints.of(_context).isMobile;

  bool get isTable => ResponsiveBreakpoints.of(_context).isTablet;

  bool get isDesktop => ResponsiveBreakpoints.of(_context).isDesktop;

  void popNavigation() => Navigator.pop(_context);

  void openDrawer() => Scaffold.of(_context).openDrawer();

  @override
  void dispose() {
    ServiceConfig.unregisterAll();
    super.dispose();
  }

}