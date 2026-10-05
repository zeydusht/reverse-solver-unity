// Calls from C# into the page (Assets/WebGLTemplates/ReverseSolver/index.html).
mergeInto(LibraryManager.library, {

  // TEMPORARY (M1): startup check line for the load-time panel.
  RS_BootReport: function (msgPtr) {
    var msg = UTF8ToString(msgPtr);
    if (typeof window.ltBoot === 'function') window.ltBoot(msg);
    else console.log('[boot] ' + msg);
  }
});
