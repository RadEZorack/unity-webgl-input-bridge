# Unity WebGL Input Bridge

A tiny, drop‑in library so a _host page_ (e.g., Augmego’s 3D iframe shell) can send **artificial mouse/keyboard events**
into a Unity WebGL build — without relying on browser‑trusted DOM events.

It uses a simple `postMessage` protocol → a lightweight **client** script inside the Unity page → a Unity **MonoBehaviour**
that receives events via `SendMessage` and exposes them to your game (optionally queues them into the new Input System).

## Repo Layout

```
unity-webgl-input-bridge/
├─ host/                     # For the parent page (your Augmego shell)
│  ├─ package.json
│  ├─ tsconfig.json
│  ├─ src/
│  │  ├─ bridge.ts           # Tiny wrapper to normalize + send input via postMessage
│  │  └─ types.ts
│  └─ dist/                  # (built JS)
│
├─ client/                   # For the Unity WebGL page (inside the iframe)
│  ├─ bridge-client.js       # Listens for postMessage and forwards to Unity
│  └─ README.md              # How to include in the WebGL template
│
├─ unity/                    # Unity plugin + C# side
│  ├─ Assets/
│  │  └─ WebGLInputBridge/
│  │     ├─ WebGLInputBridge.cs
│  │     └─ WebGLInputBridge.asmdef (optional)
│
└─ LICENSE
```

## Quick Start

### 1) Unity (client inside iframe)
- Copy `client/bridge-client.js` next to your WebGL `index.html` (or use your WebGL template).
- In `index.html`, include it before the Unity loader and register the instance when ready:

```html
<script src="bridge-client.js"></script>
<script>
  createUnityInstance(canvas, config, (progress) => {/* ... */})
    .then((instance) => {
      window.__UnityInputBridge_Register(instance);
    });
</script>
```

- Add a GameObject named **`WebGLInputBridge`** to your first scene and attach `WebGLInputBridge.cs`.
- (Optional) Define `WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM` in **Project Settings → Player → Scripting Define Symbols**.

### 2) Host (Augmego shell / parent page)
Install and build the lightweight TypeScript helper in `/host`:
```bash
cd host
npm install
npm run build
```

Then use it from your React/TS app (pseudo-example):

```ts
import { createBridge } from "./dist/bridge";

const bridge = createBridge(iframeEl, { targetOrigin: "https://your-unity-domain", maxHz: 120 });

window.addEventListener("mousemove", (e) => bridge.send({ type: "mouse-move", dx: e.movementX, dy: e.movementY }));
window.addEventListener("mousedown", (e) => bridge.send({ type: "mouse-down", button: e.button as 0|1|2 }));
window.addEventListener("mouseup",   (e) => bridge.send({ type: "mouse-up",   button: e.button as 0|1|2 }));
window.addEventListener("wheel",     (e) => bridge.send({ type: "wheel", dy: e.deltaY }), { passive: true });
window.addEventListener("keydown",   (e) => bridge.send({ type: "key-down", code: e.code, repeat: e.repeat }));
window.addEventListener("keyup",     (e) => bridge.send({ type: "key-up",   code: e.code }));
```

> Forward only while your host is in the 3D‑iframe control mode (e.g., pointer-locked).

## Security

- **Origin check:** Host uses `targetOrigin`; client validates `event.origin` and ignores anything else.
- Synthetic DOM keyboard/mouse events are _untrusted_; this approach passes messages into C# where you control the mapping/state.

## License

MIT
