import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:metflix/components/navbar/navbar.component.dart';
import 'package:metflix/components/popularity/popularity.component.dart';
import 'package:metflix/components/series_page/series_page.component.dart';

class StaticRouterConfig {
  static String routerInitLink = RouterEnumConfig.home;
  static final GlobalKey<NavigatorState> _rootNavigatorKey =
      GlobalKey<NavigatorState>();
  static final GlobalKey<NavigatorState> _shellNavigatorKey =
      GlobalKey<NavigatorState>();

  static GoRouter routerConfig = GoRouter(
      navigatorKey: _rootNavigatorKey,
      initialLocation: routerInitLink,
      routes: [
        ShellRoute(
          navigatorKey: _shellNavigatorKey,
          builder: (context, state, child) => NavbarComponent(child: child),
          routes: [
            GoRoute(
              path: routerInitLink,
              builder: (context, state) => const PopularityComponent(),
              routes: <RouteBase> [
                GoRoute(
                  parentNavigatorKey: _shellNavigatorKey,
                  path: RouterEnumConfig.homeId,
                  builder: (context, state) {
                    final extra = state.extra as Map<String,String>;
                    return SeriesPageComponent(searchUrl: extra['id']!);
                  },
                )
              ],
            ),
            GoRoute(
              path: RouterEnumConfig.news,
              builder: (context, state) => const Center(
                child: Text("news"),
              ),
            ),
            GoRoute(
              path: RouterEnumConfig.setting,
              builder: (context, state) => const Center(
                child: Text("setting"),
              ),
            )
          ],
        )
      ]);
}

abstract class RouterEnumConfig {
  static String home = "/home";
  static String homeId = "series";
  static String news = "/news";
  static String setting = "/setting";
}
