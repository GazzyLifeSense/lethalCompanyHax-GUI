using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class KillCommand : ICommand {
    static void KillPlayer(PlayerControllerB player) =>
        player.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1);

    static Result KillSelf() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            return new Result(message: "Player not found!");
        }

        KillPlayer(localPlayer);
        return new Result(true);
    }

    static Result KillTargetPlayer(string arg) {
        if (!Helper.GetPlayer(arg).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        KillPlayer(targetPlayer);
        return new Result(true);
    }

    public static Result KillAllPlayers() {
        Helper.Players.ToList().ForEach(KillPlayer);
        return new Result(true);
    }

    public static void Execute(string arg) {
        Result result = arg.Length is 0
                      ? KillSelf()
                      : KillTargetPlayer(arg);

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
