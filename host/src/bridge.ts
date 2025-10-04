import { BridgeMessage } from './types';

export type Bridge = {
  send: (msg: BridgeMessage) => void;
  destroy: () => void;
};

export function createBridge(
  iframe: HTMLIFrameElement,
  opts: { targetOrigin: string; maxHz?: number }
): Bridge {
  const { targetOrigin, maxHz = 120 } = opts;
  let lastSent = 0;
  const minDt = 1000 / Math.max(1, maxHz);

  function send(msg: BridgeMessage) {
    const now = performance.now();
    if (msg.type === 'mouse-move' && now - lastSent < minDt) return; // throttle high‑freq events
    lastSent = now;
    msg.ts = now;
    iframe.contentWindow?.postMessage({ __unityInputBridge: true, ...msg }, targetOrigin);
  }

  return { send, destroy: () => {} };
}
