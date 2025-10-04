export type BridgeMouseMove = { type: 'mouse-move'; dx: number; dy: number; ts?: number };
export type BridgeMouseDown = { type: 'mouse-down'; button: 0|1|2; ts?: number };
export type BridgeMouseUp   = { type: 'mouse-up';   button: 0|1|2; ts?: number };
export type BridgeWheel     = { type: 'wheel'; dy: number; ts?: number };
export type BridgeKeyDown   = { type: 'key-down'; code: string; repeat?: boolean; ts?: number };
export type BridgeKeyUp     = { type: 'key-up';   code: string; ts?: number };

export type BridgeMessage =
  | BridgeMouseMove | BridgeMouseDown | BridgeMouseUp
  | BridgeWheel     | BridgeKeyDown   | BridgeKeyUp;
