using System;

namespace Microsoft.Xna.Framework.Net
{
	internal class CommandGamerStateChange : ICommand
	{
        GamerStates oldState;
		NetworkGamer gamer;
		
		public CommandGamerStateChange (NetworkGamer gamer)
		{
			this.gamer = gamer;
			this.NewState = gamer.State;
			this.oldState = gamer.OldState;
		}

        public NetworkGamer Gamer => gamer;
        public GamerStates NewState { get; }

        public GamerStates OldState => oldState;

        public CommandEventType Command => CommandEventType.GamerStateChange;
    }
}

