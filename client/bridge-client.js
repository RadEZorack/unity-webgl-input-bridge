(function(){
  const allowedOrigin = window.__BRIDGE_ALLOWED_ORIGIN || window.location.origin;
  let unityInstance = null;
  const queue = [];

  // Host page should call this once Unity is ready.
  window.__UnityInputBridge_Register = function(instance){
    unityInstance = instance;
    while (queue.length) {
      const [m, a] = queue.shift();
      unityInstance.SendMessage('WebGLInputBridge', m, a);
    }
  };

  function callUnity(method, arg) {
    if (!unityInstance) { queue.push([method, arg]); return; }
    unityInstance.SendMessage('WebGLInputBridge', method, arg);
  }

  window.addEventListener('message', (event) => {
    if (event.origin !== allowedOrigin) return;
    const data = event.data || {};
    if (!data.__unityInputBridge) return;

    switch (data.type) {
      case 'mouse-move': callUnity('OnMouseMove', JSON.stringify({dx:data.dx||0, dy:data.dy||0, ts:data.ts||0})); break;
      case 'mouse-down': callUnity('OnMouseDown', String(data.button|0)); break;
      case 'mouse-up':   callUnity('OnMouseUp',   String(data.button|0)); break;
      case 'wheel':      callUnity('OnWheel',     String(data.dy||0)); break;
      case 'key-down':   callUnity('OnKeyDown',   data.code||''); break;
      case 'key-up':     callUnity('OnKeyUp',     data.code||''); break;
    }
  });
})();