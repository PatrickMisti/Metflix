import 'package:flutter/widgets.dart';
import 'package:metflix/util/view-model-builder.dart';
import 'package:provider/provider.dart';

class ViewModel<T extends BaseModel> extends StatefulWidget {
  final Widget? child;

  final T Function() viewModelBuilder;

  final Widget Function(BuildContext context, T viewModel, Widget? child) builder;

  final Function(T viewModel)? onDispose;

  const ViewModel({
    super.key,
    this.child,
    required this.viewModelBuilder,
    required this.builder,
    this.onDispose,
  });

  @override
  State<StatefulWidget> createState() => _ViewModel<T>();
}

class _ViewModel<T extends BaseModel> extends State<ViewModel<T>> {
  T? _viewModel;

  @override
  void initState() {
    super.initState();
    _viewModel ??= widget.viewModelBuilder();
    _viewModel?.isBusy = true;
    _viewModel?.init();
    // todo if need: second fired if needed
  }

  @override
  void dispose() {
    super.dispose();
    widget.onDispose?.call(_viewModel!);
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<T>(
      create: (context) => _viewModel!,
      child: Consumer<T>(
        builder: widget.builder,
        child: widget.child,
      ),
    );
  }
}
