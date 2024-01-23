using UnityEngine;

namespace Hax;

public class LocationCommand : ICommand {
    public static void Execute() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Console.Print("Player not found!");
            return;
        }

        Vector3 currentPostion = camera.transform.position;
        Console.Print($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}
