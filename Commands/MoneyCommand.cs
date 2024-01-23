
namespace Hax;

public class MoneyCommand : ICommand {
    public static void Execute(string money) {
        if (money.Length is 0) {
            Console.Print("Please Input money amount!");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Console.Print("Terminal not found!");
            return;
        }

        if (!int.TryParse(money, out int amount)) {
            Console.Print("Invalid amount!");
            return;
        }

        terminal.groupCredits += amount;
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
    }
}
