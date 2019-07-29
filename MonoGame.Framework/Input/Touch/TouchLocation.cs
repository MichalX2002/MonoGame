#region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2009-2010 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
#endregion License

#region Using clause
using System;
using System.Diagnostics;
#endregion Using clause


namespace MonoGame.Framework.Input.Touch
{
    public struct TouchLocation : IEquatable<TouchLocation>
    {
        private Vector2 _position;
		private Vector2 _previousPosition;
        private TouchLocationState _previousState;
        private float _previousPressure;
        private TimeSpan _timestamp;

        /// <summary>
        /// True if this touch was pressed and released on the same frame.
        /// In this case we will keep it around for the user to get by GetState that frame.
        /// However if they do not call GetState that frame, this touch will be forgotten.
        /// </summary>
        internal bool SameFrameReleased;

        /// <summary>
        /// Helper for assigning an invalid touch location.
        /// </summary>
        internal static readonly TouchLocation Invalid = new TouchLocation();

        #region Properties

        internal Vector2 PressPosition { get; private set; }

        internal TimeSpan PressTimestamp { get; private set; }

        internal TimeSpan Timestamp => _timestamp;

        internal Vector2 Velocity { get; private set; }

        public int Id { get; private set; }

        public Vector2 Position => _position;

        public float Pressure { get; private set; }

        public TouchLocationState State { get; private set; }

        #endregion

        #region Constructors

        public TouchLocation(int id, TouchLocationState state, Vector2 position)
            : this(id, state, position, TouchLocationState.Invalid, Vector2.Zero)
        {
        }

        public TouchLocation(   int id, TouchLocationState state, Vector2 position, 
                                TouchLocationState previousState, Vector2 previousPosition)
            : this(id, state, position, previousState, previousPosition, TimeSpan.Zero)
        {
        }

        internal TouchLocation(int id, TouchLocationState state, Vector2 position, TimeSpan timestamp)
            : this(id, state, position, TouchLocationState.Invalid, Vector2.Zero, timestamp)
        {
        }

        internal TouchLocation(int id, TouchLocationState state, Vector2 position,
            TouchLocationState previousState, Vector2 previousPosition, TimeSpan timestamp)
        {
            Id = id;
            State = state;
            _position = position;
            Pressure = 0.0f;

            _previousState = previousState;
            _previousPosition = previousPosition;
            _previousPressure = 0.0f;

            _timestamp = timestamp;
            Velocity = Vector2.Zero;

            // If this is a pressed location then store the 
            // current position and timestamp as pressed.
            if (state == TouchLocationState.Pressed)
            {
                PressPosition = _position;
                PressTimestamp = _timestamp;
            }
            else
            {
                PressPosition = Vector2.Zero;
                PressTimestamp = TimeSpan.Zero;
            }

            SameFrameReleased = false;
        }

		#endregion

        /// <summary>
        /// Returns a copy of the touch with the state changed to moved.
        /// </summary>
        /// <returns>The new touch location.</returns>
        internal TouchLocation AsMovedState()
        {
            var touch = this;

            // Store the current state as the previous.
            touch._previousState = touch.State;
            touch._previousPosition = touch._position;
            touch._previousPressure = touch.Pressure;

            // Set the new state.
            touch.State = TouchLocationState.Moved;
            
            return touch;
        }

        /// <summary>
        /// Updates the touch location using the new event.
        /// </summary>
        /// <param name="touchEvent">The next event for this touch location.</param>
        internal bool UpdateState(TouchLocation touchEvent)
        {
            Debug.Assert(Id == touchEvent.Id, "The touch event must have the same Id!");
            Debug.Assert(State != TouchLocationState.Released, "We shouldn't be changing state on a released location!");
            Debug.Assert(   touchEvent.State == TouchLocationState.Moved ||
                            touchEvent.State == TouchLocationState.Released, "The new touch event should be a move or a release!");
            Debug.Assert(touchEvent.Timestamp >= _timestamp, "The touch event is older than our timestamp!");

            // Store the current state as the previous one.
            _previousPosition = _position;
            _previousState = State;
            _previousPressure = Pressure;

            // Set the new state.
            _position = touchEvent._position;
            if (touchEvent.State == TouchLocationState.Released)
                State = touchEvent.State;
            Pressure = touchEvent.Pressure;

            // If time has elapsed then update the velocity.
            var delta = _position - _previousPosition;
            var elapsed = touchEvent.Timestamp - _timestamp;
            if (elapsed > TimeSpan.Zero)
            {
                // Use a simple low pass filter to accumulate velocity.
                var velocity = delta / (float)elapsed.TotalSeconds;
                Velocity += (velocity - Velocity) * 0.45f;
            }

            //Going straight from pressed to released on the same frame
            if (_previousState == TouchLocationState.Pressed && State == TouchLocationState.Released && elapsed == TimeSpan.Zero)
            {
                //Lie that we are pressed for now
                SameFrameReleased = true;
                State = TouchLocationState.Pressed;
            }

            // Set the new timestamp.
            _timestamp = touchEvent.Timestamp;

            // Return true if the state actually changed.
            return State != _previousState || delta.LengthSquared() > 0.001f;
        }

        public override bool Equals(object obj)
        {
			if (obj is TouchLocation)
				return Equals((TouchLocation)obj);

			return false;
		}

        public bool Equals(TouchLocation other)
        {
            return  Id.Equals(other.Id) &&
                    _position.Equals(other._position) &&
                    _previousPosition.Equals(other._previousPosition);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return "Touch id:"+Id+" state:"+State + " position:" + _position + " pressure:" + Pressure +" prevState:"+_previousState+" prevPosition:"+ _previousPosition + " previousPressure:" + _previousPressure;
        }

        public bool TryGetPreviousLocation(out TouchLocation aPreviousLocation)
        {
			if (_previousState == TouchLocationState.Invalid)
			{
                aPreviousLocation = new TouchLocation();
                return false;
			}

            aPreviousLocation = new TouchLocation
            {
                Id = Id,
                State = _previousState,
                _position = _previousPosition,
                _previousState = TouchLocationState.Invalid,
                _previousPosition = Vector2.Zero,
                Pressure = _previousPressure,
                _previousPressure = 0.0f,
                _timestamp = _timestamp,
                PressPosition = PressPosition,
                PressTimestamp = PressTimestamp,
                Velocity = Velocity,
                SameFrameReleased = SameFrameReleased
            };
            return true;
        }

        public static bool operator !=(TouchLocation value1, TouchLocation value2)
        {
			return  value1.Id != value2.Id || 
			        value1.State != value2.State ||
			        value1._position != value2._position ||
			        value1._previousState != value2._previousState ||
			        value1._previousPosition != value2._previousPosition;
        }

        public static bool operator ==(TouchLocation value1, TouchLocation value2)
        {
            return  value1.Id == value2.Id && 
			        value1.State == value2.State &&
			        value1._position == value2._position &&
			        value1._previousState == value2._previousState &&
			        value1._previousPosition == value2._previousPosition;
        }


        internal void AgeState()
        {
            if (State == TouchLocationState.Moved)
            {
                _previousState = State;
                _previousPosition = _position;
                _previousPressure = Pressure;
            }
            else
            {
                Debug.Assert(State == TouchLocationState.Pressed,
                    "Can only age the state of touches that are in the Pressed State");

                _previousState = State;
                _previousPosition = _position;
                _previousPressure = Pressure;

                if (SameFrameReleased)
                    State = TouchLocationState.Released;
                else
                    State = TouchLocationState.Moved;
            }
        }
    }
}
