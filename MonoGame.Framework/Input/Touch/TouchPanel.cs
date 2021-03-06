﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Numerics;
using MonoGame.Framework.Utilities;

namespace MonoGame.Framework.Input.Touch
{
    /// <summary>
    /// Allows retrieval of information from a touch panel device.
    /// </summary>
    public class TouchPanel : GameWindowDependency
    {
        /// <summary>
        /// Maximum distance a touch location can wiggle and not be considered to have moved.
        /// </summary>
        internal const float TapJitterTolerance = 35.0f;

        internal static TimeSpan TimeRequiredForHold { get; } = TimeSpan.FromMilliseconds(1024);

        /// <summary>
        /// The reserved touchId for all mouse touch points.
        /// </summary>
        private const int MouseTouchId = 1;

        /// <summary>
        /// The current touch state.
        /// </summary>
        private List<TouchLocation> TouchState { get; } = new List<TouchLocation>();

        /// <summary>
        /// The current gesture state.
        /// </summary>
        private List<TouchLocation> GestureState { get; } = new List<TouchLocation>();

        /// <summary>
        /// The mapping between platform specific touch ids and the touch ids we assign to touch locations.
        /// </summary>
        private Dictionary<int, int> TouchIds { get; } = new Dictionary<int, int>();

        internal Queue<GestureSample> GestureQueue { get; } = new Queue<GestureSample>();

        private List<TouchLocation> _tmpReleased = new List<TouchLocation>();

        /// <summary>
        /// The positional scale to apply to touch input.
        /// </summary>
        private Vector2 _touchScale = Vector2.One;

        /// <summary>
        /// The current size of the display.
        /// </summary>
        private Point _displaySize = Point.Zero;

        /// <summary>
        /// The next touch location identifier.
        /// The value 1 is reserved for the mouse touch point.
        /// </summary>
        private int _nextTouchId = 2;

        /// <summary>
        /// The current timestamp that we use for setting the timestamp of new touch locations.
        /// </summary>
        internal static TimeSpan CurrentTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the display orientation of the touch panel.
        /// </summary>
        public DisplayOrientation DisplayOrientation { get; set; }

        /// <summary>
        /// Gets or sets the display height of the touch panel.
        /// </summary>
        public int DisplayHeight
        {
            get => _displaySize.Y;
            set
            {
                _displaySize.Y = value;
                UpdateTouchScale();
            }
        }

        /// <summary>
        /// Gets or sets the display width of the touch panel.
        /// </summary>
        public int DisplayWidth
        {
            get => _displaySize.X;
            set
            {
                _displaySize.X = value;
                UpdateTouchScale();
            }
        }

        /// <summary>
        /// Gets or sets enabled gestures.
        /// </summary>
        public GestureTypes EnabledGestures { get; set; }

        public bool EnableMouseTouchPoint { get; set; }

        public bool EnableMouseGestures { get; set; }

        /// <summary>
        /// Gets whether a touch gesture is available.
        /// </summary>
        public bool IsGestureAvailable
        {
            get
            {
                // Process the pending gesture events. (May cause hold events)
                UpdateGestures(false);
                return GestureQueue.Count > 0;
            }
        }

        public TouchPanel(GameWindow window) : base(window)
        {
        }

        /// <summary>
        /// Returns capabilities of touch panel device.
        /// </summary>
        public TouchPanelCapabilities GetCapabilities()
        {
            bool isConnected = TouchPanelCapabilities.PlatformGetIsConnected(Window);
            int maxTouchCount = TouchPanelCapabilities.PlatformGetMaxTouchCount(Window);

            // There does not appear to be a way of finding out if a touch device supports pressure.
            // XNA does not expose a pressure value, so let's assume it doesn't support it.

            return new TouchPanelCapabilities(isConnected, maxTouchCount, hasPressure: false);
        }

        /// <summary>
        /// Age all the touches, so any that were Pressed become Moved, and any that were Released are removed
        /// </summary>
        private static void AgeTouches(List<TouchLocation> state)
        {
            for (int i = state.Count - 1; i >= 0; i--)
            {
                var touch = state[i];
                switch (touch.State)
                {
                    case TouchLocationState.Released:
                        state.RemoveAt(i);
                        break;

                    case TouchLocationState.Pressed:
                    case TouchLocationState.Moved:
                        touch.AgeState();
                        state[i] = touch;
                        break;
                }
            }
        }

        /// <summary>
        /// Apply the given new touch to the state.
        /// If it is a Pressed it will be added as a new touch, otherwise we update the existing touch it matches
        /// </summary>
        private static void ApplyTouch(List<TouchLocation> state, TouchLocation touch)
        {
            if (touch.State == TouchLocationState.Pressed)
            {
                state.Add(touch);
                return;
            }

            //Find the matching touch
            for (var i = 0; i < state.Count; i++)
            {
                var existingTouch = state[i];

                if (existingTouch.Id == touch.Id)
                {
                    //If we are moving straight from Pressed to Released and we've existed for multiple frames,
                    // that means we've never been seen, so just get rid of us
                    if (existingTouch.State == TouchLocationState.Pressed &&
                        touch.State == TouchLocationState.Released &&
                        existingTouch.PressTimestamp != touch.Timestamp)
                    {
                        state.RemoveAt(i);
                    }
                    else
                    {
                        //Otherwise update the touch based on the new one
                        existingTouch.UpdateState(touch);
                        state[i] = existingTouch;
                    }

                    break;
                }
            }
        }

        public TouchCollection GetState()
        {
            // Clear out touches from previous frames that were released on
            // the same frame they were touched that haven't been seen
            for (var i = TouchState.Count; i-- > 0;)
            {
                var touch = TouchState[i];

                //If a touch was pressed and released in a previous frame and 
                // the user didn't ask about it then trash it.
                if (touch.SameFrameReleased &&
                    touch.Timestamp < CurrentTimestamp &&
                    touch.State == TouchLocationState.Pressed)
                {
                    TouchState.RemoveAt(i);
                }
            }

            bool connected = TouchPanelCapabilities.PlatformGetIsConnected(Window);
            var result = new TouchCollection(TouchState.ToArray(), connected);

            AgeTouches(TouchState);
            return result;
        }

        internal void AddEvent(int id, TouchLocationState state, Vector2 position)
        {
            AddEvent(id, state, position, false);
        }

        internal void AddEvent(int id, TouchLocationState state, Vector2 position, bool isMouse)
        {
            // Different platforms return different touch identifiers
            // based on the specifics of their implementation and the
            // system drivers.
            //
            // Sometimes these ids are suitable for our use, but other
            // times it can recycle ids or do cute things like return
            // the same id for double tap events.
            //
            // We instead provide consistent ids by generating them
            // ourselves on the press and looking them up on move 
            // and release events.

            if (state == TouchLocationState.Pressed)
            {
                if (isMouse)
                {
                    // Mouse pointing devices always use a reserved Id
                    TouchIds[id] = MouseTouchId;
                }
                else
                {
                    TouchIds[id] = _nextTouchId++;
                }
            }

            // Try to find the touch id.
            if (!TouchIds.TryGetValue(id, out int touchId))
            {
                // If we got here that means either the device is sending
                // us bad, out of order, or old touch events.  In any case
                // just ignore them.
                return;
            }

            if (!isMouse || EnableMouseTouchPoint || EnableMouseGestures)
            {
                // Add the new touch event keeping the list from getting
                // too large if no one happens to be requesting the state.
                var location = new TouchLocation(
                    touchId, state, position * _touchScale, CurrentTimestamp);

                if (!isMouse || EnableMouseTouchPoint)
                    ApplyTouch(TouchState, location);

                //If we have gestures enabled then collect events for those too.
                //We also have to keep tracking any touches while we know about touches so 
                // we don't miss releases even if gesture recognition is disabled
                if ((EnabledGestures != GestureTypes.None || GestureState.Count > 0) &&
                    (!isMouse || EnableMouseGestures))
                {
                    ApplyTouch(GestureState, location);

                    if (EnabledGestures != GestureTypes.None)
                        UpdateGestures(true);

                    AgeTouches(GestureState);
                }
            }

            // If this is a release unmap the hardware id.
            if (state == TouchLocationState.Released)
                TouchIds.Remove(id);
        }

        private void UpdateTouchScale()
        {
            // Recalculate the touch scale.
            _touchScale = _displaySize.ToVector2() / Window.Bounds.Size;
        }

        /// <summary>
        /// This will release all touch locations.
        /// It should only be called on platforms where touch state is reset all at once.
        /// </summary>
        internal void ReleaseAllTouches()
        {
            try
            {
                // Submit a new event for each non-released location.
                if (TouchState.Count > 0)
                {
                    _tmpReleased.AddRange(TouchState);
                    foreach (var touch in _tmpReleased)
                    {
                        if (touch.State != TouchLocationState.Released)
                            ApplyTouch(TouchState, new TouchLocation(
                                touch.Id, TouchLocationState.Released, touch.Position, CurrentTimestamp));
                    }
                    _tmpReleased.Clear();
                }

                if (GestureState.Count > 0)
                {
                    _tmpReleased.AddRange(GestureState);
                    foreach (var touch in _tmpReleased)
                    {
                        if (touch.State != TouchLocationState.Released)
                            ApplyTouch(GestureState, new TouchLocation(
                                touch.Id, TouchLocationState.Released, touch.Position, CurrentTimestamp));
                    }
                    _tmpReleased.Clear();
                }
            }
            catch
            {
                _tmpReleased.Clear();
                throw;
            }
            finally
            {
                // Release all the touch id mappings.
                TouchIds.Clear();
            }
        }

        /// <summary>
        /// Tries to read the next available gesture on the touch panel.
        /// </summary>
        public bool TryReadGesture(out GestureSample gesture)
        {
            return GestureQueue.TryDequeue(out gesture);
        }

        #region Gesture Recognition

        /// <summary>
        /// The pinch touch locations.
        /// </summary>
        private TouchLocation[] PinchTouch { get; } = new TouchLocation[2];

        /// <summary>
        /// If true the pinch touch locations are valid and a pinch gesture has begun.
        /// </summary>
        private bool _pinchGestureStarted;

        private GestureTypes _dragGestureStarted = GestureTypes.None;

        /// <summary>
        /// Used to disable emitting of tap gestures.
        /// </summary>
        private bool _tapDisabled;

        /// <summary>
        /// Used to disable emitting of hold gestures.
        /// </summary>
        private bool _holdDisabled;

        private bool GestureIsEnabled(GestureTypes gestureType)
        {
            return (EnabledGestures & gestureType) != 0;
        }

        private void UpdateGestures(bool stateChanged)
        {
            // These are observed XNA gesture rules which we follow below.  Please
            // add to them if a new case is found.
            //
            //  - Tap occurs on release.
            //  - DoubleTap occurs on the first press after a Tap.
            //  - Tap, Double Tap, and Hold are disabled if a drag begins or more than one finger is pressed.
            //  - Drag occurs when one finger is down and actively moving.
            //  - Pinch occurs if 2 or more fingers are down and at least one is moving.
            //  - If you enter a Pinch during a drag a DragComplete is fired.
            //  - Drags are classified as horizontal, vertical, free, or none and stay that way.
            //

            // First get a count of touch locations which 
            // are not in the released state.
            var heldLocations = 0;
            foreach (var touch in GestureState)
                heldLocations += touch.State != TouchLocationState.Released ? 1 : 0;

            // As soon as we have more than one held point then 
            // tap and hold gestures are disabled until all the 
            // points are released.
            if (heldLocations > 1)
            {
                _tapDisabled = true;
                _holdDisabled = true;
            }

            // Process the touch locations for gestures.
            foreach (var touch in GestureState)
            {
                switch (touch.State)
                {
                    case TouchLocationState.Pressed:
                    case TouchLocationState.Moved:
                    {
                        // The DoubleTap event is emitted on first press as
                        // opposed to Tap which happens on release.
                        if (touch.State == TouchLocationState.Pressed &&
                            ProcessDoubleTap(touch))
                            break;

                        // Any time more than one finger is down and pinch is
                        // enabled then we exclusively do pinch processing.
                        if (GestureIsEnabled(GestureTypes.Pinch) && heldLocations > 1)
                        {
                            // Save or update the first pinch point.
                            if (PinchTouch[0].State == TouchLocationState.Invalid || PinchTouch[0].Id == touch.Id)
                            {
                                PinchTouch[0] = touch;
                            }
                            // Save or update the second pinch point.
                            else if (PinchTouch[1].State == TouchLocationState.Invalid || PinchTouch[1].Id == touch.Id)
                            {
                                PinchTouch[1] = touch;
                            }

                            // NOTE: Actual pinch processing happens outside and
                            // below this loop to ensure both points are updated
                            // before gestures are emitted.
                            break;
                        }

                        // If we're not dragging try to process a hold event.
                        var dist = Vector2.Distance(touch.Position, touch.PressPosition);
                        if (_dragGestureStarted == GestureTypes.None && dist < TapJitterTolerance)
                        {
                            ProcessHold(touch);
                            break;
                        }

                        // If the touch state has changed then do a drag gesture.
                        if (stateChanged)
                            ProcessDrag(touch);
                        break;
                    }

                    case TouchLocationState.Released:
                    {
                        // If the touch state hasn't changed then this
                        // is an old release event... skip it.
                        if (!stateChanged)
                            break;

                        // If this is one of the pinch locations then we
                        // need to fire off the complete event and stop
                        // the pinch gesture operation.
                        if (_pinchGestureStarted && (touch.Id == PinchTouch[0].Id || touch.Id == PinchTouch[1].Id))
                        {
                            if (GestureIsEnabled(GestureTypes.PinchComplete))
                                GestureQueue.Enqueue(new GestureSample(
                                    GestureTypes.PinchComplete,
                                    touch.Timestamp,
                                    Vector2.Zero, Vector2.Zero,
                                    Vector2.Zero, Vector2.Zero));

                            _pinchGestureStarted = false;
                            PinchTouch[0] = TouchLocation.Invalid;
                            PinchTouch[1] = TouchLocation.Invalid;
                            break;
                        }

                        // If there are still other pressed locations then there
                        // is nothing more we can do with this release.
                        if (heldLocations != 0)
                            break;

                        // From testing XNA it seems we need a velocity 
                        // of about 100 to classify this as a flick.
                        var dist = Vector2.Distance(touch.Position, touch.PressPosition);
                        if (GestureIsEnabled(GestureTypes.Flick) &&
                            dist > TapJitterTolerance && touch.Velocity.Length() > 100f)
                        {
                            GestureQueue.Enqueue(new GestureSample(
                                GestureTypes.Flick,
                                touch.Timestamp,
                                Vector2.Zero, Vector2.Zero,
                                touch.Velocity, Vector2.Zero));

                            //fall through, a drag should still happen even if a flick does
                        }

                        // If a drag is active then we need to finalize it.
                        if (_dragGestureStarted != GestureTypes.None)
                        {
                            if (GestureIsEnabled(GestureTypes.DragComplete))
                                GestureQueue.Enqueue(new GestureSample(
                                    GestureTypes.DragComplete,
                                    touch.Timestamp,
                                    Vector2.Zero, Vector2.Zero,
                                    Vector2.Zero, Vector2.Zero));

                            _dragGestureStarted = GestureTypes.None;
                            break;
                        }

                        // If all else fails try to process it as a tap.
                        ProcessTap(touch);
                        break;
                    }
                }
            }

            // If the touch state hasn't changed then there is no 
            // cleanup to do and no pinch to process.
            if (!stateChanged)
                return;

            // If we have two pinch points then update the pinch state.
            if (GestureIsEnabled(GestureTypes.Pinch) &&
                PinchTouch[0].State != TouchLocationState.Invalid &&
                PinchTouch[1].State != TouchLocationState.Invalid)
            {
                ProcessPinch(PinchTouch);
            }
            else
            {
                // Make sure a partial pinch state 
                // is not left hanging around.
                _pinchGestureStarted = false;
                PinchTouch[0] = TouchLocation.Invalid;
                PinchTouch[1] = TouchLocation.Invalid;
            }

            // If all points are released then clear some states.
            if (heldLocations == 0)
            {
                _tapDisabled = false;
                _holdDisabled = false;
                _dragGestureStarted = GestureTypes.None;
            }
        }

        private void ProcessHold(TouchLocation touch)
        {
            if (!GestureIsEnabled(GestureTypes.Hold) || _holdDisabled)
                return;

            var elapsed = CurrentTimestamp - touch.PressTimestamp;
            if (elapsed < TimeRequiredForHold)
                return;

            _holdDisabled = true;

            GestureQueue.Enqueue(new GestureSample(
                GestureTypes.Hold,
                touch.Timestamp,
                touch.Position, Vector2.Zero,
                Vector2.Zero, Vector2.Zero));
        }

        private bool ProcessDoubleTap(TouchLocation touch)
        {
            if (!GestureIsEnabled(GestureTypes.DoubleTap) || _tapDisabled || _lastTap.State == TouchLocationState.Invalid)
                return false;

            // If the new tap is too far away from the last then
            // this cannot be a double tap event.
            var dist = Vector2.Distance(touch.Position, _lastTap.Position);
            if (dist > TapJitterTolerance)
                return false;

            // Check that this tap happened within the standard 
            // double tap time threshold of 300 milliseconds.
            var elapsed = touch.Timestamp - _lastTap.Timestamp;
            if (elapsed.TotalMilliseconds > 300)
                return false;

            GestureQueue.Enqueue(new GestureSample(
                GestureTypes.DoubleTap, touch.Timestamp,
                touch.Position, Vector2.Zero,
                Vector2.Zero, Vector2.Zero));

            // Disable taps until after the next release.
            _tapDisabled = true;

            return true;
        }

        private TouchLocation _lastTap;

        private void ProcessTap(in TouchLocation touch)
        {
            if (_tapDisabled)
                return;

            // If the release is too far away from the press 
            // position then this cannot be a tap event.
            var dist = Vector2.Distance(touch.PressPosition, touch.Position);
            if (dist > TapJitterTolerance)
                return;

            // If we pressed and held too long then don't 
            // generate a tap event for it.
            var elapsed = CurrentTimestamp - touch.PressTimestamp;
            if (elapsed > TimeRequiredForHold)
                return;

            // Store the last tap for 
            // double tap processing.
            _lastTap = touch;

            // Fire off the tap event immediately.
            if (GestureIsEnabled(GestureTypes.Tap))
            {
                var tap = new GestureSample(
                    GestureTypes.Tap,
                    touch.Timestamp,
                    touch.Position, Vector2.Zero,
                    Vector2.Zero, Vector2.Zero);
                GestureQueue.Enqueue(tap);
            }
        }

        private void ProcessDrag(in TouchLocation touch)
        {
            var dragH = GestureIsEnabled(GestureTypes.HorizontalDrag);
            var dragV = GestureIsEnabled(GestureTypes.VerticalDrag);
            var dragF = GestureIsEnabled(GestureTypes.FreeDrag);

            if (!dragH && !dragV && !dragF)
                return;

            // Make sure this is a move event and that we have
            // a previous touch location.
            if (touch.State != TouchLocationState.Moved ||
                !touch.TryGetPreviousLocation(out TouchLocation prevTouch))
                return;

            var delta = touch.Position - prevTouch.Position;

            // If we're free dragging then stick to it.
            if (_dragGestureStarted != GestureTypes.FreeDrag)
            {
                var isHorizontalDelta = Math.Abs(delta.X) > Math.Abs(delta.Y * 2f);
                var isVerticalDelta = Math.Abs(delta.Y) > Math.Abs(delta.X * 2f);
                var classify = _dragGestureStarted == GestureTypes.None;

                // Once we enter either vertical or horizontal drags
                // we stick to it... regardless of the delta.
                if (dragH && ((classify && isHorizontalDelta) || _dragGestureStarted == GestureTypes.HorizontalDrag))
                {
                    delta.Y = 0;
                    _dragGestureStarted = GestureTypes.HorizontalDrag;
                }
                else if (dragV && ((classify && isVerticalDelta) || _dragGestureStarted == GestureTypes.VerticalDrag))
                {
                    delta.X = 0;
                    _dragGestureStarted = GestureTypes.VerticalDrag;
                }

                // If the delta isn't either horizontal or vertical
                //then it could be a free drag if not classified.
                else if (dragF && classify)
                {
                    _dragGestureStarted = GestureTypes.FreeDrag;
                }
                else
                {
                    // If we couldn't classify the drag then
                    // it is nothing... set it to complete.
                    _dragGestureStarted = GestureTypes.DragComplete;
                }
            }

            // If the drag could not be classified then no gesture.
            if (_dragGestureStarted == GestureTypes.None || _dragGestureStarted == GestureTypes.DragComplete)
                return;

            _tapDisabled = true;
            _holdDisabled = true;

            GestureQueue.Enqueue(new GestureSample(
                _dragGestureStarted, touch.Timestamp,
                touch.Position, Vector2.Zero,
                delta, Vector2.Zero));
        }

        private void ProcessPinch(TouchLocation[] touches)
        {
            if (!touches[0].TryGetPreviousLocation(out TouchLocation prevPos0))
                prevPos0 = touches[0];

            if (!touches[1].TryGetPreviousLocation(out TouchLocation prevPos1))
                prevPos1 = touches[1];

            var delta0 = touches[0].Position - prevPos0.Position;
            var delta1 = touches[1].Position - prevPos1.Position;

            // Get the newest timestamp.
            var timestamp = touches[0].Timestamp > touches[1].Timestamp
                ? touches[0].Timestamp
                : touches[1].Timestamp;

            // If we were already in a drag state then fire
            // off the drag completion event.
            if (_dragGestureStarted != GestureTypes.None)
            {
                if (GestureIsEnabled(GestureTypes.DragComplete))
                    GestureQueue.Enqueue(new GestureSample(
                        GestureTypes.DragComplete,
                        timestamp,
                        Vector2.Zero, Vector2.Zero,
                        Vector2.Zero, Vector2.Zero));

                _dragGestureStarted = GestureTypes.None;
            }

            GestureQueue.Enqueue(new GestureSample(
                GestureTypes.Pinch,
                timestamp,
                touches[0].Position, touches[1].Position,
                delta0, delta1));

            _pinchGestureStarted = true;
            _tapDisabled = true;
            _holdDisabled = true;
        }

        #endregion
    }
}
