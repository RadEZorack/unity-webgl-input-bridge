using System;
using System.Collections.Generic;
using UnityEngine;

#if WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WebGLInputBridge : MonoBehaviour {
    // Public state to poll
    public Vector2 MouseDelta { get; private set; }
    public float   WheelDelta { get; private set; }
    public bool    LeftDown   { get; private set; }
    public bool    RightDown  { get; private set; }
    public bool    MiddleDown { get; private set; }

    private readonly HashSet<string> _keys = new HashSet<string>();
    public bool GetKey(string code) => _keys.Contains(code);

    private Vector2 _accumMouse;
    private float   _accumWheel;

    // Optional events
    public event Action<Vector2> OnMouseMoved; // per message
    public event Action<int>     OnMouseButtonDown;
    public event Action<int>     OnMouseButtonUp;
    public event Action<string>  OnKeyDownEvt;
    public event Action<string>  OnKeyUpEvt;

    void LateUpdate(){
        MouseDelta = _accumMouse; _accumMouse = Vector2.zero;
        WheelDelta = _accumWheel; _accumWheel = 0f;
    }

    // --- Called from JS via SendMessage ---
    [System.Serializable]
    private struct MouseMovePayload { public float dx; public float dy; public double ts; }

    public void OnMouseMove(string json){
        try {
            var payload = JsonUtility.FromJson<MouseMovePayload>(json);
            var delta = new Vector2(payload.dx, payload.dy);
            _accumMouse += delta;
            OnMouseMoved?.Invoke(delta);
#if WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM
            if (Mouse.current != null) {
                InputSystem.QueueDeltaStateEvent(Mouse.current.position, delta);
            }
#endif
        } catch { /* ignore */ }
    }

    public void OnMouseDown(string btnStr){
        int b = ParseInt(btnStr);
        switch (b){ case 0: LeftDown=true; break; case 1: MiddleDown=true; break; case 2: RightDown=true; break; }
        OnMouseButtonDown?.Invoke(b);
    }
    public void OnMouseUp(string btnStr){
        int b = ParseInt(btnStr);
        switch (b){ case 0: LeftDown=false; break; case 1: MiddleDown=false; break; case 2: RightDown=false; break; }
        OnMouseButtonUp?.Invoke(b);
    }
    public void OnWheel(string dyStr){
        float dy; if (float.TryParse(dyStr, out dy)) { _accumWheel += dy; }
    }
    public void OnKeyDown(string code){
        if (!_keys.Contains(code)) _keys.Add(code);
        OnKeyDownEvt?.Invoke(code);
#if WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM
        if (Keyboard.current != null) {
            var key = TryMapKey(code);
            if (key != Key.None) InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(key, true));
        }
#endif
    }
    public void OnKeyUp(string code){
        if (_keys.Contains(code)) _keys.Remove(code);
        OnKeyUpEvt?.Invoke(code);
#if WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM
        if (Keyboard.current != null) {
            var key = TryMapKey(code);
            if (key != Key.None) InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(key, false));
        }
#endif
    }

    private static int ParseInt(string s){ int v; return int.TryParse(s, out v) ? v : 0; }

#if WEBGL_INPUT_BRIDGE_USE_INPUT_SYSTEM
    private static Key TryMapKey(string code){
        switch(code){
            case "KeyW": return Key.W; case "KeyA": return Key.A; case "KeyS": return Key.S; case "KeyD": return Key.D;
            case "Space": return Key.Space; case "ShiftLeft": return Key.LeftShift; case "ShiftRight": return Key.RightShift;
            case "Escape": return Key.Escape; case "Enter": return Key.Enter;
            case "ArrowUp": return Key.UpArrow; case "ArrowDown": return Key.DownArrow;
            case "ArrowLeft": return Key.LeftArrow; case "ArrowRight": return Key.RightArrow;
            default: return Key.None;
        }
    }
#endif
}
