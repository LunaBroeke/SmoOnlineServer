using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;
using System.Security.Principal;
using System.Diagnostics;

namespace Server
{
	public class FirewallCheck
	{
		private Settings settings = Settings.Instance;
		private static readonly Logger logger = new Logger("Firewall");
		private bool success = false;
		public void CheckManager()
		{
			string result = CheckFirewall();
			logger.Info(result);
			Thread.Sleep(5000);
			result = result.ToLower();
			if (result.Contains("admin"))
			{
				RestartWithAdmin();
			}
		}
		public string CheckFirewall()
		{
			string port = settings.Server.Port.ToString();
			string name = $"SMOO{port}";
			try
			{
				Type type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
				INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(type);

				// Create a new firewall rule
				INetFwRule newRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

				foreach (INetFwRule rule in firewallPolicy.Rules)
				{
					if (rule.LocalPorts != null && rule.LocalPorts.Contains(port) && rule.Protocol == (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP)
					{
						if (rule.Enabled == true)
						{
							success = true;
							return $"We have already found a port open on {port} meaning that everything is going according to plan, please check if you have also opened the port on your router, if you're still facing issues refer to the #help channel in the CraftyBoss community server https://discord.gg/w3TnB899ww";
						}
						else
						{
							if (IsAdministrator())
							{
								rule.Enabled = true;
								success = true;
								return $"We found the port was closed but we have successfully managed to open it again.";
							}
							else
							{
								success = false;
								return $"We have found an open port {port}, but it seems to be disabled, requesting Admin privileges for restart in 5 seconds";
							}
						}
					}
				}
				if (IsAdministrator())
				{
					newRule.Name = name;
					newRule.Description = $"Allow inbound TCP traffic on port {port}";
					newRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
					newRule.LocalPorts = port;
					newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
					newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
					newRule.Enabled = true;
					newRule.EdgeTraversal = true;

					firewallPolicy.Rules.Add(newRule);
					success = true;
					return $"Firewall rule added successfully on port {port} with name {name}";
				}
				else
				{
					success = false;
					return $"No admin privileges, Server requires a restart with Administrator to create a Firewall rule as without it your device's security won't allow any incoming connections. We will automatically attempt a restart in 5 seconds";
				}
			}
			catch (Exception ex)
			{
				return $"Error: {ex.Message}";
			}
		}

		static bool IsAdministrator()
		{
			using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
		static void RestartWithAdmin()
		{

			ProcessStartInfo processInfo = new ProcessStartInfo()
			{
				FileName = Process.GetCurrentProcess().MainModule.FileName,
				UseShellExecute = true,
				Verb = "runas"
			};

			try
			{
				Process.Start(processInfo);
				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				logger.Error($"Failed to start because {ex}");
			}


		}
	}
}
