# Client (Unity WebGL Page)

1. Copy `bridge-client.js` next to your Unity WebGL `index.html` (or add to your WebGL template).
2. Include it **before** the Unity loader and register the instance when ready:

```html
<script src="bridge-client.js"></script>
<script>
  createUnityInstance(canvas, config, (progress) => {/* ... */})
    .then((instance) => {
      window.__UnityInputBridge_Register(instance);
    });
</script>
```

3. In Unity, add a GameObject named **`WebGLInputBridge`** and attach `WebGLInputBridge.cs` from `unity/Assets/WebGLInputBridge/`.

4. (Optional) Define `WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM` to forward into the new Input System.
