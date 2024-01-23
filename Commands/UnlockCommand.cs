
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    static InteractTrigger? GetDoorTrigger(DoorLock door) =>
        Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger");

    static void UnlockDoor(DoorLock door) {
        if (!GetDoorTrigger(door).IsNotNull(out InteractTrigger doorTrigger)) return;

        door.UnlockDoorSyncWithServer();
        doorTrigger.timeToHold = 0.0f;
    }

    public static void Execute() {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(UnlockDoor);
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalAccessibleObject => terminalAccessibleObject.SetDoorOpenServerRpc(true));

        Console.Print("All doors unlocked!");
    }
}
