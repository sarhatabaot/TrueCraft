﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TrueCraft.Server
{
	public class QueryProtocol
	{
		private readonly ServerConfiguration _configuration;

		private readonly byte[] ProtocolVersion = {0xFE, 0xFD};
		private readonly byte Type_Handshake = 0x09;
		private readonly byte Type_Stat = 0x00;
		private CancellationTokenSource CToken;
		private int Port;
		private readonly Random Rnd;
		private readonly IMultiPlayerServer Server;
		private Timer Timer;
		private UdpClient Udp;

		private ConcurrentDictionary<IPEndPoint, QueryUser> UserList;

		public QueryProtocol(IMultiPlayerServer server, ServerConfiguration configuration)
		{
			Rnd = new Random();
			Server = server;
			_configuration = configuration;
		}

		public void Start()
		{
			Port = _configuration.QueryPort;
			Udp = new UdpClient(Port);
			UserList = new ConcurrentDictionary<IPEndPoint, QueryUser>();
			Timer = new Timer(ResetUserList, null, 0, 30000);
			CToken = new CancellationTokenSource();
			Udp.BeginReceive(HandleReceive, null);
		}

		private void HandleReceive(IAsyncResult ar)
		{
			if (CToken.IsCancellationRequested) return;

			try
			{
				var clientEP = new IPEndPoint(IPAddress.Any, Port);
				var buffer = Udp.EndReceive(ar, ref clientEP);

				DoReverseEndian(buffer);

				if (CheckVersion(buffer))
				{
					if (buffer[2] == Type_Handshake)
						HandleHandshake(buffer, clientEP);
					else if (buffer[2] == Type_Stat)
					{
						if (buffer.Length == 11)
							HandleBasicStat(buffer, clientEP);
						else if (buffer.Length == 15)
							HandleFullStat(buffer, clientEP);
					}
				}
			}
			catch
			{
			}

			if (CToken.IsCancellationRequested) return;

			Udp.BeginReceive(HandleReceive, null);
		}

		private void HandleHandshake(byte[] buffer, IPEndPoint clientEP)
		{
			using (var ms = new MemoryStream(buffer))
			using (var stream = new BinaryReader(ms))
			{
				var sessionId = GetSessionId(stream);

				var user = new QueryUser {SessionId = sessionId, ChallengeToken = Rnd.Next()};

				if (UserList.ContainsKey(clientEP))
				{
					QueryUser u;
					while (!UserList.TryRemove(clientEP, out u))
						Thread.Sleep(1);
				}

				UserList[clientEP] = user;

				using (var response = new MemoryStream())
				using (var writer = new BinaryWriter(response))
				{
					WriteHead(Type_Handshake, user, writer);
					WriteStringToStream(user.ChallengeToken.ToString(), response);
					SendResponse(response.ToArray(), clientEP);
				}
			}
		}

		private void HandleBasicStat(byte[] buffer, IPEndPoint clientEP)
		{
			using (var ms = new MemoryStream(buffer))
			using (var stream = new BinaryReader(ms))
			{
				var sessionId = GetSessionId(stream);
				var token = GetToken(stream);

				var user = GetUser(clientEP);
				if (user.ChallengeToken != token || user.SessionId != sessionId)
					throw new Exception("Invalid credentials");

				var stats = GetStats();
				using (var response = new MemoryStream())
				using (var writer = new BinaryWriter(response))
				{
					WriteHead(Type_Stat, user, writer);
					WriteStringToStream(stats["hostname"], response);
					WriteStringToStream(stats["gametype"], response);
					WriteStringToStream(stats["numplayers"], response);
					WriteStringToStream(stats["maxplayers"], response);
					var hostport = BitConverter.GetBytes(ushort.Parse(stats["hostport"]));
					Array.Reverse(hostport); //The specification needs little endian short
					writer.Write(hostport);
					WriteStringToStream(stats["hostip"], response);

					SendResponse(response.ToArray(), clientEP);
				}
			}
		}

		private void HandleFullStat(byte[] buffer, IPEndPoint clientEP)
		{
			using (var stream = new MemoryStream(buffer))
			using (var reader = new BinaryReader(stream))
			{
				var sessionId = GetSessionId(reader);
				var token = GetToken(reader);

				var user = GetUser(clientEP);
				if (user.ChallengeToken != token || user.SessionId != sessionId)
					throw new Exception("Invalid credentials");

				var stats = GetStats();
				using (var response = new MemoryStream())
				using (var writer = new BinaryWriter(response))
				{
					WriteHead(Type_Stat, user, writer);
					WriteStringToStream("SPLITNUM\0\0", response);
					foreach (var pair in stats)
					{
						WriteStringToStream(pair.Key, response);
						WriteStringToStream(pair.Value, response);
					}

					writer.Write((byte) 0x00);
					writer.Write((byte) 0x01);
					WriteStringToStream("player_\0", response);
					var players = GetPlayers();
					foreach (var player in players)
						WriteStringToStream(player, response);
					writer.Write((byte) 0x00);

					SendResponse(response.ToArray(), clientEP);
				}
			}
		}

		private bool CheckVersion(byte[] ver)
		{
			return ver[0] == ProtocolVersion[0] && ver[1] == ProtocolVersion[1];
		}

		private int GetSessionId(BinaryReader stream)
		{
			stream.BaseStream.Position = 3;
			return stream.ReadInt32();
		}

		private int GetToken(BinaryReader stream)
		{
			stream.BaseStream.Position = 7;
			return stream.ReadInt32();
		}

		private void WriteHead(byte type, QueryUser user, BinaryWriter stream)
		{
			stream.Write(type);
			stream.Write(user.SessionId);
		}

		private void SendResponse(byte[] res, IPEndPoint destination)
		{
			Udp.Send(res, res.Length, destination);
		}

		private QueryUser GetUser(IPEndPoint ipe)
		{
			if (!UserList.ContainsKey(ipe))
				throw new Exception("Undefined user");

			return UserList[ipe];
		}

		private Dictionary<string, string> GetStats()
		{
			var stats = new Dictionary<string, string>
			{
				{"hostname", _configuration.MOTD},
				{"gametype", "SMP"},
				{"game_id", "TRUECRAFT"},
				{"version", "1.0"},
				{"plugins", "TrueCraft"},
				{"map", Server.Worlds.First().Name},
				{"numplayers", Server.Clients.Count.ToString()},
				{"maxplayers", "64"},
				{"hostport", _configuration.ServerPort.ToString()},
				{"hostip", _configuration.ServerAddress}
			};
			return stats;
		}

		private List<string> GetPlayers()
		{
			var names = new List<string>();
			lock (Server.ClientLock)
				foreach (var client in Server.Clients)
					names.Add(client.Username);
			return names;
		}

		private void DoReverseEndian(byte[] buffer)
		{
			if (buffer.Length >= 7)
			{
				Swap(ref buffer[3], ref buffer[6]);
				Swap(ref buffer[4], ref buffer[5]);
			}

			if (buffer.Length >= 11)
			{
				Swap(ref buffer[7], ref buffer[10]);
				Swap(ref buffer[8], ref buffer[9]);
			}
		}

		private void Swap(ref byte a, ref byte b)
		{
			var c = a;
			a = b;
			b = c;
		}

		public void Stop()
		{
			Timer.Dispose();
			CToken.Cancel();
			Udp.Close();
		}

		private void ResetUserList(object state)
		{
			UserList.Clear();
		}

		private byte[] String0ToBytes(string s)
		{
			return Encoding.UTF8.GetBytes(s + "\0");
		}

		private void WriteToStream(byte[] bytes, Stream stream)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		private void WriteStringToStream(string s, Stream stream)
		{
			WriteToStream(String0ToBytes(s), stream);
		}

		private struct QueryUser
		{
			public int SessionId;
			public int ChallengeToken;
		}
	}
}