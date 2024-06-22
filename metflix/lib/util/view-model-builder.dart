
import 'package:flutter/cupertino.dart';
import 'package:metflix/util/view-model.dart';

abstract class ViewModelBuilder<T extends BaseModel> extends StatelessWidget {
  const ViewModelBuilder({super.key});

  T viewModelBuilder(BuildContext context);

  Widget builder(BuildContext context, T viewModel, Widget? child);

  void onViewModelReady(T viewModel) {}

  void onDispose(T viewModel) {}

  Widget? staticChildBuilder(BuildContext context) => null;

  @override
  Widget build(BuildContext context) {
    return ViewModel<T>(
      builder: builder,
      viewModelBuilder: () => viewModelBuilder(context),
      onDispose: onDispose,
      child: staticChildBuilder(context),
    );
  }
}

class BaseModel extends ChangeNotifier {
  bool _isBusy = false;

  bool get isBusy => _isBusy;

  set isBusy(value){
    _isBusy = value;
    notifyListeners();
  }

  void init(){
    isBusy = false;
  }
}