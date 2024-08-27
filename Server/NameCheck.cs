using Shared;
using Shared.Packet.Packets;

namespace Server
{
    /// <summary>
    /// This class checks for name instead of profile ID (ex: Ryujinx has the same ProfileID on the default user on every instance so that is easy to $ban but Yuzu's default ProfileID is random)
    /// </summary>
    public class NameCheck
    {
        private bool found = false; // This bool exists as a check so that the server doesn't accidentally initiate this script twice
        private string[] bannedNames = new string[] { "yuzu", "suyu", "ryuplayer" }; // list of banned names
        public bool darker = true;
        public async Task Main(Client client, Logger Logger)
        {
            if (bannedNames.Contains(client.Name.ToLower()) && found == false)
            {
                if (darker == true) { await SendToDarker(client, Logger); return; }
                found = true; // ensures that this doesnt accidentally get initiated twice
                int ms = 15000; // Wait time to ensure that the client is loaded in the stage before attempting to send.
                Logger.Info($"found banned name {client.Name}");
                Logger.Info($"Sending to crash stage in {ms} ms");
                await Task.Delay(ms);
                Logger.Info("Attemping to send");
                await client.Send(new ChangeStagePacket
                {
                    Id = "ChangeYourName",
                    Stage = "ChangeYourName",
                    Scenario = -1,
                }); // Sends the player to a stage which doesnt exist
                await Task.Delay(5000); // waits to ensure the client doesnt send another packet.
                found = false; // opens the script back up again.
            }
            else if (bannedNames.Contains(client.Name) && found == true) { Logger.Info($"Currently processing last crash, will attempt again on next packet"); }
        }

        public async Task SendToDarker(Client client, Logger Logger)
        {
            int ms = 30000; // Wait time to ensure that the client is loaded in the stage before attempting to send.
            Logger.Info($"found banned name {client.Name}");
            Logger.Info($"Sending to crash stage in {ms} ms");
            await Task.Delay(ms);
            Logger.Info("Attemping to send");
            await client.Send(new ChangeStagePacket
            {
                Id = "CP_Entrance",
                Stage = "Special2WorldLavaStage",
                Scenario = -1,
            }); // Sends the player to Darker Side.
        }
    }
}

