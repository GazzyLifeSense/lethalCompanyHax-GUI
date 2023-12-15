
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    InteractTrigger? GetDoorTrigger(DoorLock door) =>
        Reflector.Target(door).GetInternalField<InteractTrigger>("doorTrigger");

    void UnlockDoor(DoorLock door) {
        if (!this.GetDoorTrigger(door).IsNotNull(out InteractTrigger doorTrigger)) return;

        door.UnlockDoorSyncWithServer();
        doorTrigger.timeToHold = 0.0f;
    }

    public void Execute(string[] _) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(this.UnlockDoor);
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalAccessibleObject => terminalAccessibleObject.SetDoorOpenServerRpc(true));

        Console.Print("All doors unlocked!");
    }
}
