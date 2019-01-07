using System;

namespace Microsoft.Xna.Framework.Net
{
	internal class CommandGamerLeft : ICommand
	{
        internal long remoteUniqueIdentifier = -1;
		
		public CommandGamerLeft (int internalIndex)
		{
			InternalIndex = internalIndex;
			
		}
		
		public CommandGamerLeft (long uniqueIndentifier)
		{
			this.remoteUniqueIdentifier = uniqueIndentifier;
			
		}

        public int InternalIndex { get; } = -1;

        public CommandEventType Command => CommandEventType.GamerLeft;
    }
}

