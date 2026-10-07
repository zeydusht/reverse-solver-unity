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

  // Web name gate (index.html #nameVeil) as a real HTML input, so iOS shows
  // its own keyboard. The answer goes to Unity with SendMessage(obj, method, name).
  RS_AskName: function (objPtr, methodPtr) {
    var obj = UTF8ToString(objPtr), method = UTF8ToString(methodPtr);
    if (document.getElementById('rs-name')) return;
    var veil = document.createElement('div');
    veil.id = 'rs-name';
    veil.style.cssText = 'position:fixed;inset:0;z-index:40;display:flex;align-items:center;justify-content:center;' +
      'padding:26px;background:rgba(8,32,34,.9);font-family:ui-sans-serif,-apple-system,"Segoe UI",Roboto,sans-serif;touch-action:auto';
    veil.innerHTML =
      '<div style="max-width:340px;width:100%;background:#14484c;border:1px solid rgba(255,255,255,.16);' +
      'border-radius:18px;padding:24px 22px 18px;text-align:center;color:#eaf3f2">' +
      '<h2 style="font-size:20px;margin:0 0 6px">Reverse Solver</h2>' +
      '<p style="font-size:13px;color:#8fb2b3;line-height:1.55;margin:0 0 18px">Playtest sürümü. Adını yaz, sonuçlar bu isimle kaydedilsin.</p>' +
      '<input id="rs-name-in" placeholder="Adın" autocomplete="off" maxlength="40" style="width:100%;box-sizing:border-box;' +
      'padding:13px;border-radius:11px;border:1px solid rgba(255,255,255,.25);background:rgba(255,255,255,.08);' +
      'color:#eaf3f2;font-size:16px;font-family:inherit;margin-bottom:14px;text-align:center;-webkit-user-select:text;user-select:text">' +
      '<button id="rs-name-go" style="width:100%;border:none;border-radius:11px;padding:13px;font-size:15px;' +
      'font-weight:700;background:#f0a742;color:#33220a;font-family:inherit;cursor:pointer">Başla</button></div>';
    document.body.appendChild(veil);
    var input = document.getElementById('rs-name-in');
    var go = function () {
      var v = input.value.trim();
      if (!v) { input.focus(); return; }
      veil.remove();
      SendMessage(obj, method, v);
    };
    document.getElementById('rs-name-go').onclick = go;
    input.addEventListener('keydown', function (e) { if (e.key === 'Enter') go(); });
    setTimeout(function () { input.focus(); }, 50);
  },

  // Page hidden/shown: "hidden" on visibilitychange (hidden) and pagehide,
  // "visible" when it comes back. Sent to Unity with SendMessage.
  RS_WatchVisibility: function (objPtr, methodPtr) {
    var obj = UTF8ToString(objPtr), method = UTF8ToString(methodPtr);
    var send = function (state) { try { SendMessage(obj, method, state); } catch (e) {} };
    document.addEventListener('visibilitychange', function () { send(document.hidden ? 'hidden' : 'visible'); });
    window.addEventListener('pagehide', function () { send('hidden'); });
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
