using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

public class HateCommand : ICommand {
    public static void Execute(string playerId = "") {
        if (playerId.Length is 0) {
            Console.Print("Please select a player!");
            return;
        }

        if (!Helper.GetPlayer(playerId).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        List<string> promptedEnemies = Helper.PromptEnemiesToTarget(targetPlayer);

        if (promptedEnemies.Count is 0) {
            Console.Print("No enemies found!");
            return;
        }

        Console.Print($"Enemies prompted: {promptedEnemies.Count}");
    }
}
