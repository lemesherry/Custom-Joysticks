using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sherry.CustomJoysticks.Scripts {

    public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

        [System.Serializable]
        public enum JoystickDirection {

            Horizontal,
            Vertical,
            Both

        }
    
        [Serializable]
        public class Settings {

            [Tooltip( "Range of handle movement from center. \nNote: value 1 means it will only move within Joystick background Image/GameObject" )]
            [Range( 0, 2f )] public float handleMovementRange = 1f;
            [Tooltip( "To allow joystick movement only in certain axis select either vertical or horizontal" )]
            public JoystickDirection joystickDirection = JoystickDirection.Both;
            
        }

        [Space( 10 )]
        [Tooltip( "Drag and drop Joystick background Image/GameObject here" )]
        public RectTransform joystickBackground;

        [Tooltip( "Drag and drop Joystick handle Image/GameObject here" )]
        public RectTransform joystickHandle;
        // ★彡[ Setting serializable reference to show it in inspector ]彡★
        public Settings advancedSettings;

        // ★彡[ Input values according to the Joystick movement (x: -1 to 1, y: -1 to 1) ]彡★
        public Vector2 Input { get; private set; } = Vector2.zero;

        // ★彡[ Boolean to check if joystick is moving or not ]彡★
        public bool IsMoving { get; private set; }

        // ★彡[ Getters for specific axis values ]彡★
        public float Vertical => Input.y;
        public float Horizontal => Input.x;

        private Vector2 joystickPosition = Vector2.zero;

        public void OnPointerDown( PointerEventData eventData ) {

            // ★彡[ Activating Joystick game object when touch/mouse input is detected ]彡★
            ActiveJoystickBackGround( true );

            // ★彡[ Calling OnDrag function after input is detected to perform drag functionality accordingly ]彡★
            OnDrag( eventData );

            // ★彡[ Setting moving boolean to true after input is detected ]彡★
            SetIsMoving( true );

            // ★彡[ Setting Joystick initial position according to touch/mouse input position ]彡★
            joystickPosition = eventData.position;

            // ★彡[ Setting Joystick background initial position according to touch/mouse input position ]彡★
            joystickBackground.position = eventData.position;

            // ★彡[ Setting Joystick handle initial anchor position to zero to make it work from center of Joystick ]彡★
            SetJoystickHandlePosition( Vector2.zero );
        }

        public void OnDrag( PointerEventData eventData ) {

            // ★彡[ Setting direction by subtracting current input position and Joystick initial position ]彡★
            var _direction = eventData.position - joystickPosition;

            // ★彡[ Getting input values by checking if the direction vector's magnitude is greater than half of the Joystick background height(because handle is in center of joystick background) to either normalize its vector or to divide direction vector with than half of the Joystick background height to get the actual input in both axis (x: -1 to 1, y: -1 to 1 ]彡★
            Input = ( _direction.magnitude > joystickBackground.sizeDelta.x / 2f ) ? _direction.normalized : _direction / ( joystickBackground.sizeDelta.x / 2f );

            // ★彡[ Switch arms cases to check the enum values and only allow certain axis movement of joystick ]彡★
            Input = advancedSettings.joystickDirection switch {
                JoystickDirection.Horizontal => new Vector2( Input.x, 0f ),
                JoystickDirection.Vertical => new Vector2( 0f, Input.y ),
                _ => ( _direction.magnitude > joystickBackground.sizeDelta.x / 2f ) ? _direction.normalized : _direction / ( joystickBackground.sizeDelta.x / 2f )
            };

            // ★彡[ Setting Joystick handle position according to input multiplied by half of the Joystick height and multiplying it to handle movement range to restrict handle's movement according to the variable ]彡★
            SetJoystickHandlePosition( ( Input * joystickBackground.sizeDelta.x / 2f ) * advancedSettings.handleMovementRange );

        }

        public void OnPointerUp( PointerEventData eventData ) {

            // ★彡[ Resetting input to zero when there is no touch/mouse input ]彡★
            Input = Vector2.zero;

            // ★彡[ Setting moving boolean to false after no input ]彡★
            SetIsMoving( false );

            // ★彡[ Setting Joystick handle initial anchor position to zero to make it work from center of Joystick ]彡★
            SetJoystickHandlePosition( Vector2.zero );

            // ★彡[ Deactivating Joystick game object after no input ]彡★
            ActiveJoystickBackGround( false );
        }

        private void SetJoystickHandlePosition( Vector2 value ) => joystickHandle.anchoredPosition = value;

        private void SetIsMoving( bool value ) => IsMoving = value;

        private void ActiveJoystickBackGround( bool value ) => joystickBackground.gameObject.SetActive( value );

    }

}
