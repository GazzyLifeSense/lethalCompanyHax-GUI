using UnityEngine;

namespace Hax;

public class TimescaleCommand : ICommand {
    public static void Execute(string scale) {
        if (scale.Length is 0) {
            Console.Print("Please input timescale amount!");
            return;
        }

        if (!float.TryParse(scale, out float timescale)) {
            Console.Print("Invalid timescale!");
            return;
        }

        Time.timeScale = timescale;
    }
}
