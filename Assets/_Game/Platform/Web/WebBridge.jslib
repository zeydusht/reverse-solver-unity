// Calls from C# into the page (Assets/WebGLTemplates/ReverseSolver/index.html).
mergeInto(LibraryManager.library, {

  // Load-time panel line (shown with ?debug=1).
  RS_BootReport: function (msgPtr) {
    var msg = UTF8ToString(msgPtr);
    if (typeof window.ltBoot === 'function') window.ltBoot(msg);
    else console.log('[boot] ' + msg);
  },

  // URL query parameter; returns a heap string the caller does not free, or null.
  RS_Query: function (namePtr) {
    var name = UTF8ToString(namePtr);
    var value = new URLSearchParams(window.location.search).get(name);
    if (value === null) return 0;
    var size = lengthBytesUTF8(value) + 1;
    var buf = _malloc(size);
    stringToUTF8(value, buf, size);
    return buf;
  },

  // Device pixels per CSS px the canvas renders at; the template caps it at 2.
  RS_PixelRatio: function () {
    return Math.min(window.devicePixelRatio || 1, 2);
  },

  // Safe-area inset in CSS px: 0 left, 1 top, 2 right, 3 bottom (template --sal/--sat/--sar/--sab).
  RS_SafeInset: function (side) {
    var name = ['--sal', '--sat', '--sar', '--sab'][side];
    var probe = document.getElementById('rs-safe-probe');
    if (!probe) {
      probe = document.createElement('div');
      probe.id = 'rs-safe-probe';
      probe.style.cssText = 'position:fixed;visibility:hidden;pointer-events:none;' +
        'padding-left:var(--sal);padding-top:var(--sat);padding-right:var(--sar);padding-bottom:var(--sab)';
      document.body.appendChild(probe);
    }
    var cs = getComputedStyle(probe);
    var v = [cs.paddingLeft, cs.paddingTop, cs.paddingRight, cs.paddingBottom][side];
    return parseFloat(v) || 0;
  }
});
