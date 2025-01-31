using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life;
using ModKit.Helper;
using ModKit.Internal;
using ModKit.Interfaces;
using Life.Network;
using System.Diagnostics;
using Mirror;
using Life.DB;
using System.IO;
using Newtonsoft.Json;
using Connexion;
using _100STConnect;
using ModKit.Helper.DiscordHelper;

namespace Connexion
{
    public class Connect : ModKit.ModKit
    {
        private readonly MyEvents _events;

        public Connect(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "2.1.1", "COLE100ST");
            _events = new MyEvents(api);
        }

        public static DiscordWebhookClient WebhookClient { get; internal set; }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            _events.Init(Nova.server);
            ModKit.Internal.Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }

        internal class Config
        {
        }
    }
}


public class MyEvents : ModKit.Helper.Events
{
    private MyPlugin.Config config;
    private object player;

    public DiscordWebhookClient WebhookClient { get; private set; }

    public MyEvents(IGameAPI api) : base(api)
    {
    }

    public override void OnPlayerSpawnCharacter(Player player)
    {
        Nova.server.SendMessageToAdmins($"<color=#0ffe04>[100STConnect] Un joueur vient de se connecter :");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- Pseudo steam : <color=#00FF00>{player.account.username}<color=#62418b>.");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- Nom et prénom RP : <color=#00FF00>{player.character.Firstname} {player.character.Lastname}<color=#62418b>.");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- ID : <color=#00FF00>{player.character.Id}<color=#62418b>.");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- Le joueur est dans l'entreprise : <color=#00FF00>{player.character.BizId}<color=#62418b>.");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- Le joueur possède : <color=#00FF00>{player.character.Bank}<color=#62418b>. en banque");
        Nova.server.SendMessageToAdmins($"<color=#7000ff>- Le joueur possède : <color=#00FF00>{player.character.Money}<color=#62418b>. en liquide");
        Nova.server.SendMessageToAdmins($"<color=#02dcfa>- Un STAFF viens en jeux : <color=#00FF00>{player.IsAdmin}<color=#3f7789>.");

        Logger.LogSuccess("100STConnect", $"[Connexion] Un joueur vient de se connecter :");
        Logger.LogSuccess("100STConnect", $"- Pseudo steam : {player.account.username}.");
        Logger.LogSuccess("100STConnect", $"- Nom et prénom RP : {player.character.Firstname} {player.character.Lastname}.");
        Logger.LogSuccess("100STConnect", $"- ID : {player.character.Id}.");
    }

    private static void PlaySoundForAdmins(Player player, bool isAdmin)
    {
        bool isConect = Mirror.NetworkConnection.LocalConnectionId == 0;


        if (isConect == true)
        {
            if (isAdmin == true)
            {
                player.setup.TargetPlayClaironById(0.2f, Nova.server.config.roleplayConfig.ticketAlertSound);

            }
        }
    }

    public void CreateConfig()
    {
        string directoryPath = pluginsPath + "/100STConnect";

        string configFilePath = directoryPath + "/config.json";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (!File.Exists(configFilePath))
        {
            Config defaultConfig = new Config { WebhookLogs = "Votre lien Wehbook Discord" };

            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(defaultConfig, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(configFilePath, jsonContent);
        }

        config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
    }

    public class Config
    {
        public string WebhookLogs { get; set; }

        public int HealRegenerationConfig { get; set; }
    }


    public override void OnPlayerDisconnect(Mirror.NetworkConnection conn)
    {
        Nova.server.SendMessageToAdmins($"<color=#800000>[100STConnect] Le joueur <color=#00FF00>{conn.identity.name} <color=#c7fd00>vient de se déconecter.");
    }

    public async void votreFonction()
    {
        await DiscordHelper.SendMsg(Connect.WebhookClient, $"config.WebhookLogs");

        await DiscordHelper.SendMsg(WebhookClient, $"# [100STConnect]"
            + $"\nAction **:** Réanimation d'une personne avec les premier secours."
            + $"\n"
            + $"\n***Information du Policier***"
            + $"\nNom & Prénom RP **:** {player.GetFullName()}"
            + $"\nPseudo Steam **:** {player.account.username}"
            + $"\nSteam ID **:** {player.steamId}"
            + $"\n"
            + $"\n***Information Victime***"
            + $"\nNom & Prénom RP **:** {closestPlayer.GetFullName()}"
            + $"\nPseudo Steam **:** {closestPlayer.account.username}"
            + $"\nSteam ID **:** {closestPlayer.steamId}");


    }

    public override void OnPlayerConnect(Player player)
    {
        Nova.server.SendMessageToAdmins("<color=#00eefd>[100STConnect] Un joueur est en train de se connecter :");
        Nova.server.SendMessageToAdmins($"<color=#00eefd>- Pseudo steam : <color=#00FF00>{player.account.username}<color=#00eefd>.");

        Logger.LogWarning("100STConnect", "Un joueur est en train de se connecter :");
        Logger.LogWarning("100STConnect", $"- Pseudo steam : {player.account.username}.");
        Logger.LogWarning("100STConnect", $"- Steam ID : {player.account.steamId}");

        bool isAdmin = player.IsAdmin;

        if (isAdmin == true)
        {
            Logger.LogWarning("100STConnect", $"Un membres du staff vient de ce connecter (Pseudo steam : {player.account.username}).");
        }
    }
}
        
    

