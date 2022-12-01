mergeInto(LibraryManager.library, {
  SendScreenshot: function (imgData) {
    window.dispatchReactUnityEvent(
      "SendScreenshot",
      Pointer_stringify(imgData)
    );
  },
});