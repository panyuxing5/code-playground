using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace VoxelCraft.Core
{
    public class InputManager
    {
        private readonly GameWindow window;
        private KeyboardState previousKeyboard;
        private MouseState previousMouse;
        private KeyboardState currentKeyboard;
        private MouseState currentMouse;

        // 鼠标状态
        public Vector2 MousePosition { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        public float MouseWheelDelta { get; private set; }
        public bool IsMouseLeftDown { get; private set; }
        public bool IsMouseRightDown { get; private set; }
        public bool IsMouseMiddleDown { get; private set; }
        public bool IsMouseLeftPressed { get; private set; }
        public bool IsMouseRightPressed { get; private set; }
        public bool IsMouseMiddlePressed { get; private set; }
        public bool IsMouseLeftReleased { get; private set; }
        public bool IsMouseRightReleased { get; private set; }
        public bool IsMouseMiddleReleased { get; private set; }

        // 鼠标长按
        public float MouseLeftHoldTime { get; private set; }
        public float MouseRightHoldTime { get; private set; }
        public bool IsMouseLeftHolding => MouseLeftHoldTime > 0.3f;
        public bool IsMouseRightHolding => MouseRightHoldTime > 0.3f;

        // 键盘状态缓存
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private readonly HashSet<Keys> releasedKeys = new HashSet<Keys>();
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();

        // 输入绑定
        private readonly Dictionary<string, List<Keys>> keyBindings = new Dictionary<string, List<Keys>>();
        private readonly Dictionary<string, List<MouseButton>> mouseBindings = new Dictionary<string, List<MouseButton>>();

        // 文本输入
        public string TextInput { get; private set; } = "";
        public bool IsTextInputMode { get; private set; } = false;

        // 事件
        public event Action<Keys> OnKeyPressed;
        public event Action<Keys> OnKeyReleased;
        public event Action<MouseButton> OnMousePressed;
        public event Action<MouseButton> OnMouseReleased;
        public event Action<float> OnMouseWheel;
        public event Action<char> OnTextInput;

        public InputManager(GameWindow window)
        {
            this.window = window;
            InitializeBindings();
        }

        public Vector2 GetMouseDelta()
        {
            return MouseDelta;
        }

        public float GetMouseScroll()
        {
            return MouseWheelDelta;
        }

        private void InitializeBindings()
        {
            // 移动
            keyBindings["forward"] = new List<Keys> { Keys.W };
            keyBindings["backward"] = new List<Keys> { Keys.S };
            keyBindings["left"] = new List<Keys> { Keys.A };
            keyBindings["right"] = new List<Keys> { Keys.D };
            keyBindings["jump"] = new List<Keys> { Keys.Space };
            keyBindings["sneak"] = new List<Keys> { Keys.LeftShift, Keys.RightShift };
            keyBindings["sprint"] = new List<Keys> { Keys.LeftControl, Keys.RightControl };
            keyBindings["fly"] = new List<Keys> { Keys.F };
            keyBindings["inventory"] = new List<Keys> { Keys.E };
            keyBindings["pause"] = new List<Keys> { Keys.Escape };
            keyBindings["chat"] = new List<Keys> { Keys.T };
            keyBindings["command"] = new List<Keys> { Keys.Slash };
            keyBindings["drop"] = new List<Keys> { Keys.Q };
            keyBindings["screenshot"] = new List<Keys> { Keys.F2 };
            keyBindings["debug"] = new List<Keys> { Keys.F3 };
            keyBindings["perspective"] = new List<Keys> { Keys.F5 };
            keyBindings["fullscreen"] = new List<Keys> { Keys.F11 };
            keyBindings["toggleclouds"] = new List<Keys> { Keys.F6 };
            keyBindings["toggleparticles"] = new List<Keys> { Keys.F7 };
            keyBindings["increasedistance"] = new List<Keys> { Keys.Equal };
            keyBindings["decreasedistance"] = new List<Keys> { Keys.Minus };

            // 快捷栏
            keyBindings["hotbar1"] = new List<Keys> { Keys.D1 };
            keyBindings["hotbar2"] = new List<Keys> { Keys.D2 };
            keyBindings["hotbar3"] = new List<Keys> { Keys.D3 };
            keyBindings["hotbar4"] = new List<Keys> { Keys.D4 };
            keyBindings["hotbar5"] = new List<Keys> { Keys.D5 };
            keyBindings["hotbar6"] = new List<Keys> { Keys.D6 };
            keyBindings["hotbar7"] = new List<Keys> { Keys.D7 };
            keyBindings["hotbar8"] = new List<Keys> { Keys.D8 };
            keyBindings["hotbar9"] = new List<Keys> { Keys.D9 };

            // 鼠标
            mouseBindings["attack"] = new List<MouseButton> { MouseButton.Left };
            mouseBindings["use"] = new List<MouseButton> { MouseButton.Right };
            mouseBindings["pick"] = new List<MouseButton> { MouseButton.Middle };
        }

        public void Update(float deltaTime)
        {
            previousKeyboard = currentKeyboard;
            previousMouse = currentMouse;
            currentKeyboard = window.KeyboardState;
            currentMouse = window.MouseState;

            // 更新鼠标位置
            Vector2 newMousePos = new Vector2(currentMouse.X, currentMouse.Y);
            MouseDelta = newMousePos - MousePosition;
            MousePosition = newMousePos;

            // 更新鼠标按键
            IsMouseLeftDown = currentMouse.IsButtonDown(MouseButton.Left);
            IsMouseRightDown = currentMouse.IsButtonDown(MouseButton.Right);
            IsMouseMiddleDown = currentMouse.IsButtonDown(MouseButton.Button3);

            IsMouseLeftPressed = IsMouseLeftDown && !previousMouse.IsButtonDown(MouseButton.Left);
            IsMouseRightPressed = IsMouseRightDown && !previousMouse.IsButtonDown(MouseButton.Right);
            IsMouseMiddlePressed = IsMouseMiddleDown && !previousMouse.IsButtonDown(MouseButton.Button3);

            IsMouseLeftReleased = !IsMouseLeftDown && previousMouse.IsButtonDown(MouseButton.Left);
            IsMouseRightReleased = !IsMouseRightDown && previousMouse.IsButtonDown(MouseButton.Right);
            IsMouseMiddleReleased = !IsMouseMiddleDown && previousMouse.IsButtonDown(MouseButton.Button3);

            // 鼠标长按计时
            if (IsMouseLeftDown)
            {
                MouseLeftHoldTime += deltaTime;
            }
            else
            {
                MouseLeftHoldTime = 0f;
            }

            if (IsMouseRightDown)
            {
                MouseRightHoldTime += deltaTime;
            }
            else
            {
                MouseRightHoldTime = 0f;
            }

            // 鼠标滚轮
            MouseWheelDelta = currentMouse.ScrollDelta.Y - previousMouse.ScrollDelta.Y;
            if (Math.Abs(MouseWheelDelta) > 0.01f)
            {
                OnMouseWheel?.Invoke(MouseWheelDelta);
            }

            // 更新键盘状态
            pressedKeys.Clear();
            releasedKeys.Clear();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown) continue;

                bool isDown = currentKeyboard.IsKeyDown(key);
                bool wasDown = previousKeyboard.IsKeyDown(key);

                if (isDown && !wasDown)
                {
                    pressedKeys.Add(key);
                    heldKeys.Add(key);
                    OnKeyPressed?.Invoke(key);
                }
                else if (!isDown && wasDown)
                {
                    releasedKeys.Add(key);
                    heldKeys.Remove(key);
                    OnKeyReleased?.Invoke(key);
                }
            }

            // 触发鼠标事件
            if (IsMouseLeftPressed) OnMousePressed?.Invoke(MouseButton.Left);
            if (IsMouseRightPressed) OnMousePressed?.Invoke(MouseButton.Right);
            if (IsMouseMiddlePressed) OnMousePressed?.Invoke(MouseButton.Middle);
            if (IsMouseLeftReleased) OnMouseReleased?.Invoke(MouseButton.Left);
            if (IsMouseRightReleased) OnMouseReleased?.Invoke(MouseButton.Right);
            if (IsMouseMiddleReleased) OnMouseReleased?.Invoke(MouseButton.Middle);
        }

        // ========================================
        // 键盘查询
        // ========================================
        public bool IsKeyDown(Keys key)
        {
            return currentKeyboard.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return releasedKeys.Contains(key);
        }

        public bool IsAnyKeyDown(params Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (IsKeyDown(key)) return true;
            }
            return false;
        }

        public bool IsAnyKeyPressed(params Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (IsKeyPressed(key)) return true;
            }
            return false;
        }

        // ========================================
        // 输入绑定查询
        // ========================================
        public bool IsActionDown(string action)
        {
            if (keyBindings.TryGetValue(action, out var keys))
            {
                foreach (var key in keys)
                {
                    if (IsKeyDown(key)) return true;
                }
            }
            if (mouseBindings.TryGetValue(action, out var buttons))
            {
                foreach (var button in buttons)
                {
                    if (IsMouseButtonDown(button)) return true;
                }
            }
            return false;
        }

        public bool IsActionPressed(string action)
        {
            if (keyBindings.TryGetValue(action, out var keys))
            {
                foreach (var key in keys)
                {
                    if (IsKeyPressed(key)) return true;
                }
            }
            if (mouseBindings.TryGetValue(action, out var buttons))
            {
                foreach (var button in buttons)
                {
                    if (IsMouseButtonPressed(button)) return true;
                }
            }
            return false;
        }

        public bool IsActionReleased(string action)
        {
            if (keyBindings.TryGetValue(action, out var keys))
            {
                foreach (var key in keys)
                {
                    if (IsKeyReleased(key)) return true;
                }
            }
            if (mouseBindings.TryGetValue(action, out var buttons))
            {
                foreach (var button in buttons)
                {
                    if (IsMouseButtonReleased(button)) return true;
                }
            }
            return false;
        }

        // ========================================
        // 鼠标查询
        // ========================================
        public bool IsMouseButtonDown(MouseButton button)
        {
            return currentMouse.IsButtonDown(button);
        }

        public bool IsMouseButtonPressed(MouseButton button)
        {
            return currentMouse.IsButtonDown(button) && !previousMouse.IsButtonDown(button);
        }

        public bool IsMouseButtonReleased(MouseButton button)
        {
            return !currentMouse.IsButtonDown(button) && previousMouse.IsButtonDown(button);
        }

        // ========================================
        // 移动输入
        // ========================================
        public Vector3 GetMovementInput()
        {
            Vector3 input = Vector3.Zero;

            if (IsActionDown("forward")) input.Z -= 1;
            if (IsActionDown("backward")) input.Z += 1;
            if (IsActionDown("left")) input.X -= 1;
            if (IsActionDown("right")) input.X += 1;

            if (input.LengthSquared > 0)
            {
                input.Normalize();
            }

            return input;
        }

        public bool IsJumping => IsActionDown("jump");
        public bool IsSneaking => IsActionDown("sneak");
        public bool IsSprinting => IsActionDown("sprint");
        public bool IsFlying => IsActionDown("fly");

        // ========================================
        // 快捷栏
        // ========================================
        public int GetHotbarSelection()
        {
            for (int i = 1; i <= 9; i++)
            {
                if (IsKeyPressed(Keys.D0 + i))
                {
                    return i - 1;
                }
            }
            return -1;
        }

        public int GetHotbarScroll()
        {
            if (MouseWheelDelta > 0) return 1;
            if (MouseWheelDelta < 0) return -1;
            return 0;
        }

        // ========================================
        // 文本输入模式
        // ========================================
        public void BeginTextInput()
        {
            IsTextInputMode = true;
            TextInput = "";
        }

        public void EndTextInput()
        {
            IsTextInputMode = false;
        }

        public void AddTextInput(char c)
        {
            if (!IsTextInputMode) return;

            if (c == '\b')
            {
                if (TextInput.Length > 0)
                {
                    TextInput = TextInput.Substring(0, TextInput.Length - 1);
                }
            }
            else if (c == '\r' || c == '\n')
            {
                // 回车确认
            }
            else if (!char.IsControl(c))
            {
                TextInput += c;
            }

            OnTextInput?.Invoke(c);
        }

        // ========================================
        // 输入绑定管理
        // ========================================
        public void SetKeyBinding(string action, Keys key)
        {
            if (!keyBindings.ContainsKey(action))
            {
                keyBindings[action] = new List<Keys>();
            }
            keyBindings[action].Clear();
            keyBindings[action].Add(key);
        }

        public void AddKeyBinding(string action, Keys key)
        {
            if (!keyBindings.ContainsKey(action))
            {
                keyBindings[action] = new List<Keys>();
            }
            keyBindings[action].Add(key);
        }

        public void SetMouseBinding(string action, MouseButton button)
        {
            if (!mouseBindings.ContainsKey(action))
            {
                mouseBindings[action] = new List<MouseButton>();
            }
            mouseBindings[action].Clear();
            mouseBindings[action].Add(button);
        }

        public List<Keys> GetKeyBindings(string action)
        {
            return keyBindings.ContainsKey(action) ? keyBindings[action] : new List<Keys>();
        }

        public List<MouseButton> GetMouseBindings(string action)
        {
            return mouseBindings.ContainsKey(action) ? mouseBindings[action] : new List<MouseButton>();
        }

        public void ResetToDefaultBindings()
        {
            keyBindings.Clear();
            mouseBindings.Clear();
            InitializeBindings();
        }

        // ========================================
        // 鼠标锁定
        // ========================================
        public void LockMouse()
        {
            // window.CursorVisible = false;
        }

        public void UnlockMouse()
        {
            // window.CursorVisible = true;
        }

        public void SetMousePosition(float x, float y)
        {
            window.MousePosition = new Vector2(x, y);
        }

        public void CenterMouse()
        {
            SetMousePosition(window.Size.X / 2f, window.Size.Y / 2f);
        }
    }
}
