
import 'package:get_it/get_it.dart';
import 'package:metflix/services/http-wrapper.dart';

class ServiceConfig {
  static final GetIt _getIt = GetIt.instance;
  static void registerStartup() {
    _getIt.registerSingleton<HttpWrapper>(HttpWrapper());
  }

  static void unregisterAll() {
    _getIt.unregister<HttpWrapper>();
  }
}