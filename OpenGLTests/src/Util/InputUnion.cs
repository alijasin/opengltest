using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OpenGLTests.src.Util
{
    class InputUnion
    {
        public enum Directions { Undefined, Up, Down, Repeat, Moved };
        public Directions Direction { get; }

        public KeyboardKeyEventArgs KeyboardArgs { get; }
        public bool IsKeyboardInput => KeyboardArgs != null;

        public MouseButtonEventArgs MouseButtonArgs { get; }
        public bool IsMouseInput => MouseButtonArgs != null;

        public Buttons? GamePadButtonArgs { get; }
        public bool IsGamePadInput => GamePadButtonArgs != null;

        public InputUnion(Directions direction, MouseButtonEventArgs buttonArgs)
        {
            Direction = direction;
            MouseButtonArgs = buttonArgs;
        }

        public InputUnion(Directions direction, KeyboardKeyEventArgs keyboardArgs)
        {
            Direction = direction;
            KeyboardArgs = keyboardArgs;
        }

        public InputUnion(Directions direction, Buttons gamePadButtonArgs)
        {
            Direction = direction;
            GamePadButtonArgs = gamePadButtonArgs;
        }

        public InputUnion(Directions direction, MouseMoveEventArgs gamePadButtonArgs)
        {
            Direction = direction;
            MouseButtonArgs = null;
        }

        public override string ToString()
        {
            if (IsKeyboardInput)
            {
                return KeyboardArgs.Key.ToString();
            }
            if (IsGamePadInput)
            {
                return GamePadButtonArgs.ToString();
            }
            if (IsMouseInput)
            {
                return MouseButtonArgs.ToString();
            }
            return "Failed to stringify InputUnion";
        }
    }

    /// <summary>
    /// A class which maps an InputUnion to a callback
    /// </summary>
    class HotkeyMapping
    {
        public delegate void HotkeyCallback(InputUnion input);
        public delegate bool HotkeyFilter(InputUnion input);

        private HotkeyFilter Filter { get; }
        private HotkeyCallback Callback { get; }

        public HotkeyMapping(HotkeyFilter filter, HotkeyCallback callback)
        {
            this.Filter = filter;
            this.Callback = callback;
        }

        /// <summary>
        /// Filters the input and if it makes it through the filter
        /// a callback ensues
        /// </summary>
        /// <param name="input">The input to be filtered and subsequently provided to the callback</param>
        public void Tickle(InputUnion input)
        {
            if (Filter(input))
            {
                Callback?.Invoke(input);
            }
        }

    }

    class Hotkey
    {
        public HotkeyMapping Activate { get; }
        public HotkeyMapping Deactivate { get; }

        public Hotkey(
            HotkeyMapping.HotkeyFilter filter,
            HotkeyMapping.HotkeyCallback activate,
            HotkeyMapping.HotkeyCallback deactivate
            )
        {
            Activate = new HotkeyMapping(input =>
                filter(input) && input.Direction == InputUnion.Directions.Down, activate);
            Deactivate = new HotkeyMapping(input =>
                filter(input) && input.Direction == InputUnion.Directions.Up, deactivate);
        }
    }
}
