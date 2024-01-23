using System;
using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    static Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.Teleporter.IsNotNull(out ShipTeleporter teleporter)) {
            Console.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    static Result TeleportPlayerToBase(string playerId) {
        if (!Helper.GetPlayer(playerId).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(TeleportPlayerToBaseLater(targetPlayer));

        return new Result(true);
    }

    public static void Execute(string playerId = "") {
        if (playerId.Length == 0) {
            Helper.StartOfRound?.ForcePlayerIntoShip();
            return;
        }

        Result result = TeleportPlayerToBase(playerId);

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
