using Shared;
using Shared.Packet.Packets;

namespace Server
{
    /// <summary>
    /// This class checks for name instead of profile ID (ex: Ryujinx has the same ProfileID on the default user on every instance so that is easy to $ban but Yuzu's default ProfileID is random)
    /// </summary>
    public class NameCheck
    {
        private bool found = false;
        private string[] bannedNames = new string[]{"yuzu", "suyu", "ryujinx"};
        public async Task Main(Client client, Logger Logger)
        {
            Logger.Info(client.Name);
            if (bannedNames.Contains(client.Name) && found == false) 
            {
                found = true;
                int ms = 15000;
                Logger.Info($"found banned name {client.Name}");
                Logger.Info($"Sending to crash stage in {ms} ms");
                await Task.Delay(ms);
                Logger.Info("Attemping to send")
;               await client.Send(new ChangeStagePacket
                {
                    Id = "ChangeYourName",
                    Stage = "ChangeYourName",
                    Scenario = -1,
                });
                await Task.Delay(5000);
                found = false;
            }
            else if(bannedNames.Contains(client.Name) && found == true) { Logger.Info($"Currently processing last crash, will attempt again on next packet"); }
        }
    }
}
