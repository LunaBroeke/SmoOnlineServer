using Shared.Packet.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class HnSRandom
	{
		public static Server server;
		public static readonly string[] stageNames = { "CapWorldHomeStage","WaterfallWorldHomeStage","SandWorldHomeStage","LakeWorldHomeStage","ForestWorldHomeStage","ClashWorldHomeStage","CityWorldHomeStage","SnowWorldHomeStage","SeaWorldHomeStage","LavaWorldHomeStage","SkyWorldHomeStage","MoonWorldHomeStage","PeachWorldHomeStage"};

		public static string StartRandomGame(string arg)
		{
			string stage = RandomStage();
			int num = 0;
			try { num = int.Parse(arg); }
			catch { return "Invalid number"; }
			server.Logger.Warn(server.Clients.Count.ToString());
			server.Logger.Warn(num.ToString());
			if (num > server.Clients.Count)
				return "Number is higher than the amount of players";
			else if (num == server.Clients.Count)
				return "Please have at least one seeker available";
			List<string> players = new List<string>();
			for (int i = 0; i < num; i++)
			{
				string n = RandomPlayer();
				if (players.Contains(n)) { i--; continue; }
				players.Add(n);
			}
			foreach (Client client in server.Clients)
			{

#pragma warning disable CS4014
				client.Send(new ChangeStagePacket
				{
					Stage = stage,
					Scenario = -1,
				});
#pragma warning restore CS4014 
			}
			// Still needs logic for selecting a random seeker(s) (amount would be selectable by doing 'randomgame [int]') and then displaying the seekers names. I haven't seen a way to broadcast messages so my current concept is to have a fake player display the seekers names.
			// Remember to add a debug command to: Save Player Locations, Test the Fake Player.
			string play = string.Join(',', players.ToArray());
			return $"Starting Random Game on {stage} with seekers: {play}";
		}

		public static string RandomStage()
		{
			Random r = new Random();
			int i = r.Next(0,stageNames.Length);
			try
			{
				return stageNames[i];
			}
			catch (IndexOutOfRangeException ex)
			{
				server.Logger.Warn("Please tell Luwuna that she doesnt know how Random works and that the array overflowed");
				return stageNames[i - 1];//I honestly dont remember how random works so I just have this incase it does go out of bounds
			}
		}

		private static string RandomPlayer()
		{
			Random r = new Random();
			int i = r.Next(0, server.Clients.Count);
			try
			{
				return server.Clients[i].Name;
			}
			catch
			{
				return server.Clients[i-1].Name;
			}
		}
	}
}
