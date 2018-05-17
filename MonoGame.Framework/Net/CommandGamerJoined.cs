using System;

namespace Microsoft.Xna.Framework.Net
{
	internal class CommandGamerJoined : ICommand
	{
        internal long remoteUniqueIdentifier = -1;
		GamerStates states;
        string displayName = string.Empty;
		
		public CommandGamerJoined (int internalIndex, bool isHost, bool isLocal)
		{
			InternalIndex = internalIndex;
			
			if (isHost)
				states = states | GamerStates.Host;
			if (isLocal)
				states = states | GamerStates.Local;
			
		}
		
		public CommandGamerJoined (long uniqueIndentifier)
		{
			this.remoteUniqueIdentifier = uniqueIndentifier;
			
		}
		
		public string DisplayName {
			get {
				return displayName;
			}
			set {
				displayName = value;
			}
		}

        public string GamerTag { get; set; } = string.Empty;

        public GamerStates State
		{
			get { return states; }
			set { states = value; }
		}

        public int InternalIndex { get; } = -1;

        public CommandEventType Command {
			get { return CommandEventType.GamerJoined; }
		}
	}
}

