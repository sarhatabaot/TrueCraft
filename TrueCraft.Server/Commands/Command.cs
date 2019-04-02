using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public abstract class Command : ICommand
    {
        protected readonly CommandManager Commands;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual string[] Aliases { get { return new string[0]; } }

        public virtual void Handle(IRemoteClient client, string alias, string[] arguments) { Help(client, alias, arguments); }

        public virtual void Help(IRemoteClient client, string alias, string[] arguments) { client.SendMessage("Command \"" + alias + "\" is not functional!"); }

        protected Command(CommandManager commands)
        {
            Commands = commands;
        }
    }
}
