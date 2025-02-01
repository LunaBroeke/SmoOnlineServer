using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class LogSender
	{
		private readonly string Address = Settings.Instance.LogListener.Address;
		private readonly int Port = Settings.Instance.LogListener.Port;
		private Logger logger = new Logger("UDP");
		public LogSender()
		{
			Start();
		}
		public async void Start()
		{
			if (Settings.Instance.LogListener.Enabled == false) return;
			Logger.AddLogHandler(Log);
		}
		private async void Log(string source, string level, string text, ConsoleColor _)
		{
			try
			{
				using (UdpClient udpClient = new UdpClient())
				{
					DateTime logTime = DateTime.Now;
					string message = Logger.PrefixNewLines(text, $"{{{logTime}}} {level} [{source}]");
					byte[] data = Encoding.UTF8.GetBytes(message);
					await udpClient.SendAsync(data, data.Length, Address, Port);
				}
			}
			catch(Exception ex) 
			{
				logger.Error(ex);
			}
		}
	}
}
