using System;
using R3;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InputScheme : IDisposable
    {
        public enum InputSchemeType
        {
            KeyboardAndMouse,
            GamePad
        }

        private readonly ReactiveProperty<InputSchemeType> inputSchemeTypeReactiveProperty = new();
        public ReadOnlyReactiveProperty<InputSchemeType> InputSchemeTypeReactiveProperty => inputSchemeTypeReactiveProperty;
        public InputSchemeType CurrentInputSchemeType => inputSchemeTypeReactiveProperty.Value;

        private readonly ReactiveProperty<Gamepad> gamepadReactiveProperty = new();
        public ReadOnlyReactiveProperty<Gamepad> GamepadReactiveProperty => gamepadReactiveProperty;
        public Gamepad CurrentGamepad => gamepadReactiveProperty.Value;

        public Observable<Unit> AnyChangedAsObservable()
        {
            return Observable.Merge(
                inputSchemeTypeReactiveProperty.AsUnitObservable(),
                gamepadReactiveProperty.AsUnitObservable()
                );
        }

        public InputScheme()
        {
            InputSystem.onEvent += OnEvent;
        }


        public void Dispose()
        {
            InputSystem.onEvent -= OnEvent;
        }

        public string GetCurrentSchemeName()
        {
            return GetSchemeName(CurrentInputSchemeType);
        }

        public static string GetSchemeName(InputSchemeType inputSchemeType)
        {
            return inputSchemeType switch
            {
                InputSchemeType.KeyboardAndMouse => "Keyboard&Mouse",
                InputSchemeType.GamePad => "Gamepad",
                _ => "Unknown"
            };
        }

        private void OnEvent(InputEventPtr inputEventPtr, InputDevice inputDevice)
        {
            var eventType = inputEventPtr.type;
            if (eventType != StateEvent.Type && eventType != DeltaStateEvent.Type)
            {
                return;
            }

            var controls = inputEventPtr.EnumerateControls(
                InputControlExtensions.Enumerate.IncludeNonLeafControls |
                InputControlExtensions.Enumerate.IncludeSyntheticControls |
                InputControlExtensions.Enumerate.IgnoreControlsInCurrentState |
                InputControlExtensions.Enumerate.IgnoreControlsInDefaultState
                );
            var anyControl = controls.GetEnumerator().MoveNext();
            if (!anyControl)
            {
                return;
            }

            if (inputDevice is Gamepad gamepad && gamepadReactiveProperty.Value != gamepad)
            {
                gamepadReactiveProperty.Value = gamepad;
            }

            var newSchemeType = inputDevice switch
            {
                Keyboard or Mouse => InputSchemeType.KeyboardAndMouse,
                Gamepad => InputSchemeType.GamePad,
                _ => CurrentInputSchemeType
            };

            if (newSchemeType != CurrentInputSchemeType)
            {
                inputSchemeTypeReactiveProperty.Value = newSchemeType;
            }
        }
    }
}