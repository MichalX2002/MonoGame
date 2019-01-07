using System;

namespace Microsoft.Xna.Framework.Net
{
	internal class CommandSessionStateChange : ICommand
	{
        NetworkSessionState oldState;
		
		public CommandSessionStateChange (NetworkSessionState newState, NetworkSessionState oldState)
		{
			this.NewState = newState;
			this.oldState = oldState;
		}

        public NetworkSessionState NewState { get; }

        public NetworkSessionState OldState => oldState;

        public CommandEventType Command => CommandEventType.SessionStateChange;
    }
}

