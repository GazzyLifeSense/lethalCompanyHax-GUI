
namespace Hax;

public class ShovelCommand : ICommand {
    public static void Execute(string force) {
        if (force.Length is 0) {
            Console.Print("Please input hit force!");
            return;
        }

        if (!int.TryParse(force, out int shovelHitForce)) {
            Console.Print("Invalid hit force!");
            return;
        }

        Settings.ShovelHitForce = shovelHitForce;
        Console.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
