using GameNetcodeStuff;

namespace Hax;

public class HealCommand : ICommand {
    public static void Execute() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Player not found");
            return;
        }

        localPlayer.health = 100;
        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        Helper.HUDManager?.UpdateHealthUI(localPlayer.health, false);
    }
}
