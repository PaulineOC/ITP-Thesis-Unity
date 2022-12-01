mergeInto(LibraryManager.library, {
  UnityHasAwoken: function (hasAwoken) {
    window.dispatchReactUnityEvent(
      "UnityHasAwoken",
      Pointer_stringify(hasAwoken)
    );
  },
});