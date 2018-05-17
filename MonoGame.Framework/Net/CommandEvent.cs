using System;

namespace Microsoft.Xna.Framework.Net
{
	internal class CommandEvent
	{
        object commandObject;
		
		public CommandEvent (CommandEventType command, object commandObject)
		{
			this.Command = command;
			this.commandObject = commandObject;
		}
		
		public CommandEvent (ICommand command)
		{
			this.Command = command.Command;
			this.commandObject = command;
		}

        public CommandEventType Command { get; }

        public object CommandObject
		{
			get { return commandObject; }
		}
	}
}

