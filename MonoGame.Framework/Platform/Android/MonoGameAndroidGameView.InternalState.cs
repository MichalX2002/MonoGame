// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework
{
    public partial class MonoGameAndroidGameView
    {
        /// <summary>
        /// What is the state of the app, for tracking surface recreation inside this class.
        /// This acts as a replacement for the all-out monitor wait approach 
        /// which caused code to be quite fragile.
        /// </summary>
        enum InternalState
        {
            /// <summary>
            /// Set by android UI thread and the game thread 
            /// process it and transitions into 'Paused' state.
            /// </summary>
            Pausing_UIThread,

            /// <summary>
            /// Set by android UI thread and the game thread 
            /// process it and transitions into 'Running' state.
            /// </summary>
            Resuming_UIThread,

            /// <summary>
            /// Set either by game or android UI thread and the game thread
            /// process it and transitions into 'Exited' state.
            /// </summary>
            Exiting,

            /// <summary>
            /// Set by game thread after processing 'Pausing' state.
            /// </summary>
            Paused_GameThread,

            /// <summary>
            /// Set by game thread after processing 'Resuming' state.
            /// </summary>
            Running_GameThread,

            /// <summary>
            /// Set by game thread after processing 'Exiting' state.
            /// </summary>
            Exited_GameThread,

            /// <summary>
            /// Used to create the surface the 1st time or when screen orientation changes
            /// </summary>
            ForceRecreateSurface,
        }
    }
}