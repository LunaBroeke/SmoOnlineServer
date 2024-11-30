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

		public static string StartRandomGame()
		{
			string stage = RandomStage();
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
			return $"Starting Random Game on {stage}";
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
	}
}
