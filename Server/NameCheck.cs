using Shared;
using Shared.Packet.Packets;

namespace Server
{
    /// <summary>
    /// This class checks for name instead of profile ID (ex: Ryujinx has the same ProfileID on the default user on every instance so that is easy to $ban but Yuzu's default ProfileID is random)
    /// </summary>
    public class NameCheck
    {
        private string[] bannedNames = Settings.Instance.BannedName.names; // list of banned names
        public bool darker = true;
        public async Task Main(Client client, Logger Logger)
        {
            bannedNames = Settings.Instance.BannedName.names;
            if (bannedNames.Contains(client.Name.ToLower()))
            {
                await SendToDarker(client, Logger); return;
            }
        }

        public async Task SendToDarker(Client client, Logger Logger)
        {
            int ms = 30000; // Wait time to ensure that the client is loaded in the stage before attempting to send.
            Logger.Info($"found banned name {client.Name}");
            Logger.Info($"Sending to crash stage in {ms} ms");
            await Task.Delay(ms);
            Logger.Info("Attemping to send");
            try
            {
                await client.Send(new ChangeStagePacket
                {
                    Stage = Settings.Instance.BannedName.StageToSend,
                    Scenario = -1,
                }); // Sends the player to Darker Side.
            }
            catch (Exception ex)
            {
                Logger.Warn($"client probably got annoyed and left the game to change their profile name ({ex.Message})");
            }
        }
    }
}

