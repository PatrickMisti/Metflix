import 'package:flutter/material.dart';
import 'package:metflix/router.config.dart';
import 'package:metflix/util/service-config.dart';
import 'package:responsive_framework/responsive_framework.dart';

void main() {
  ServiceConfig.registerStartup();
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      debugShowCheckedModeBanner: false,
      routerConfig: StaticRouterConfig.routerConfig,
      builder: (context, child) => ResponsiveBreakpoints(
        breakpoints: const [
          Breakpoint(start: 0, end: 450, name: MOBILE),
          Breakpoint(start: 451, end: 800, name: TABLET),
          Breakpoint(start: 801, end: 1920, name: DESKTOP),
          Breakpoint(start: 1921, end: double.infinity, name: '4K'),
        ],
        child: child!,
      ),
    );
  }
}
