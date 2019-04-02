﻿using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using System;
using TrueCraft.API.Entities;
using TrueCraft.Core;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.Core.AI;
using TrueCraft.Core.Entities;
using System.Threading.Tasks;

namespace TrueCraft.Commands
{
    public class PositionCommand : Command
    {
        public override string Name
        {
            get { return "pos"; }
        }

        public override string Description
        {
            get { return "Shows your position."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.SendMessage(client.Entity.Position.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Shows your position.");
        }

        public PositionCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class SaveCommand : Command
    {
        public override string Name
        {
            get { return "save"; }
        }

        public override string Description
        {
            get { return "Saves the world!"; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.World.Save();
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/save: Saves the world!");
        }

        public SaveCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class SkyLightCommand : Command
    {
        public override string Name
        {
            get { return "sl"; }
        }

        public override string Description
        {
            get { return "Shows sky light at your current position."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            int mod = 0;
            if (arguments.Length == 1)
                int.TryParse(arguments[0], out mod);
            client.SendMessage(client.World.GetSkyLight(
                (Coordinates3D)(client.Entity.Position + new Vector3(0, -mod, 0))).ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/sl");
        }

        public SkyLightCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class SpawnCommand : Command
    {
        public override string Name
        {
            get { return "spawn"; }
        }

        public override string Description
        {
            get { return "Spawns a mob."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 1)
            {
                Help(client, alias, arguments);
                return;
            }
            var entityTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (typeof(IEntity).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                        entityTypes.Add(t);
                }
            }

            arguments[0] = arguments[0].ToUpper();
            var type = entityTypes.SingleOrDefault(t => t.Name.ToUpper() == arguments[0] + "ENTITY");
            if (type == null)
            {
                client.SendMessage(ChatColor.Red + "Unknown entity type.");
                return;
            }
            var entity = (IEntity)Activator.CreateInstance(type);
            var em = client.Server.GetEntityManagerForWorld(client.World);
            entity.Position = client.Entity.Position + MathHelper.FowardVector(client.Entity.Yaw) * 3;
            em.SpawnEntity(entity);
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/spawn [type]: Spawns a mob of that type.");
        }

        public SpawnCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class ToMeCommand : Command
    {
        public override string Name
        {
            get { return "tome"; }
        }

        public override string Description
        {
            get { return "Moves a mob towards your position."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 1)
            {
                Help(client, alias, arguments);
                return;
            }

            int id;
            if (!int.TryParse(arguments[0], out id))
            {
                Help(client, alias, arguments);
                return;
            }

            var manager = client.Server.GetEntityManagerForWorld(client.World);
            var entity = manager.GetEntityByID(id) as MobEntity;
            if (entity == null)
            {
                client.SendMessage(ChatColor.Red + "An entity with that ID does not exist in this world.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var astar = new AStarPathFinder();
                var path = astar.FindPath(client.World, entity.BoundingBox, (Coordinates3D)entity.Position, (Coordinates3D)client.Entity.Position);
                if (path == null)
                {
                    client.SendMessage(ChatColor.Red + "It is impossible for this entity to reach you.");
                }
                else
                {
                    client.SendMessage(string.Format(ChatColor.Blue
                        + "Executing path with {0} waypoints", path.Waypoints.Count()));
                    entity.CurrentPath = path;
                }
            });
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/tome [id]: Moves a mob to your position.");
        }

        public ToMeCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class EntityInfoCommand : Command
    {
        public override string Name
        {
            get { return "entity"; }
        }

        public override string Description
        {
            get { return "Provides information about an entity ID."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 1)
            {
                Help(client, alias, arguments);
                return;
            }

            int id;
            if (!int.TryParse(arguments[0], out id))
            {
                Help(client, alias, arguments);
                return;
            }

            var manager = client.Server.GetEntityManagerForWorld(client.World);
            var entity = manager.GetEntityByID(id);
            if (entity == null)
            {
                client.SendMessage(ChatColor.Red + "An entity with that ID does not exist in this world.");
                return;
            }
            client.SendMessage(string.Format(
                "{0} {1}", entity.GetType().Name, entity.Position));
            if (entity is MobEntity)
            {
                var mob = entity as MobEntity;
                client.SendMessage(string.Format(
                    "{0}/{1} HP, {2} State, moving to to {3}",
                    mob.Health, mob.MaxHealth,
                    mob.CurrentState?.GetType().Name ?? "null",
                    mob.CurrentPath?.Waypoints.Last().ToString() ?? "null"));
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/entity [id]: Shows information about this entity.");
        }

        public EntityInfoCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class DestroyCommand : Command
    {
        public override string Name
        {
            get { return "destroy"; }
        }

        public override string Description
        {
            get { return "Destroys a mob. Violently."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 1)
            {
                Help(client, alias, arguments);
                return;
            }

            int id;
            if (!int.TryParse(arguments[0], out id))
            {
                Help(client, alias, arguments);
                return;
            }

            var manager = client.Server.GetEntityManagerForWorld(client.World);
            var entity = manager.GetEntityByID(id) as MobEntity;
            if (entity == null)
            {
                client.SendMessage(ChatColor.Red + "An entity with that ID does not exist in this world.");
                return;
            }

            manager.DespawnEntity(entity);
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/destroy [id]: " + Description);
        }

        public DestroyCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class TrashCommand : Command
    {
        public override string Name
        {
            get { return "trash"; }
        }

        public override string Description
        {
            get { return "Discards selected item, hotbar, or entire inventory."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                if (arguments[0] == "hotbar")
                {
                    // Discard hotbar items
                    for (short i = 36; i <= 44; i++)
                    {
                        client.Inventory[i] = ItemStack.EmptyStack;
                    }
                }
                else if (arguments[0] == "all")
                {
                    // Discard all inventory items, including armor and crafting area contents
                    for (short i = 0; i <= 44; i++)
                    {
                        client.Inventory[i] = ItemStack.EmptyStack;
                    }
                }
                else
                {
                    Help(client, alias, arguments);
                    return;
                }
            }
            else
            {
                // Discards selected item.
                client.Inventory[client.SelectedSlot] = ItemStack.EmptyStack;
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /trash <hotbar/all> or leave blank to clear\nselected slot.");
        }

        public TrashCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class WhatCommand : Command
    {
        public override string Name
        {
            get { return "what"; }
        }

        public override string Description
        {
            get { return "Tells you what you're holding."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.SendMessage(client.SelectedItem.ToString());
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/what: Tells you what you're holding.");
        }

        public WhatCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class LogCommand : Command
    {
        public override string Name
        {
            get { return "log"; }
        }

        public override string Description
        {
            get { return "Toggles client logging."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.EnableLogging = !client.EnableLogging;
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/pos: Toggles client logging.");
        }

        public LogCommand(CommandManager commands) : base(commands)
        {
        }
    }
    
    public class ResendInvCommand : Command
    {
        public override string Name
        {
            get { return "reinv"; }
        }

        public override string Description
        {
            get { return "Resends your inventory to the selected client."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/reinv: Resends your inventory.");
        }

        public ResendInvCommand(CommandManager commands) : base(commands)
        {
        }
    }

    public class RelightCommand : Command
    {
        public override string Name
        {
            get { return "relight"; }
        }

        public override string Description
        {
            get { return "Relights the chunk you're standing in."; }
        }

        public override string[] Aliases
        {
            get { return new string[0]; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length != 0)
            {
                Help(client, alias, arguments);
                return;
            }
            var server = client.Server as MultiplayerServer;
            var chunk = client.World.FindChunk((Coordinates3D)client.Entity.Position);
            var lighter = server.WorldLighters.SingleOrDefault(l => l.World == client.World);
            if (lighter != null)
            {
                lighter.InitialLighting(chunk, true);
                (client as RemoteClient).UnloadChunk(chunk.Coordinates);
                (client as RemoteClient).LoadChunk(chunk);
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("/reinv: Resends your inventory.");
        }

        public RelightCommand(CommandManager commands) : base(commands)
        {
        }
    }
}