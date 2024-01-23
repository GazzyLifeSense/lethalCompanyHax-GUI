using System.Linq;
using UnityEngine;

namespace Hax;

public class LockCommand : ICommand {
    public static void Execute() {
        Object.FindObjectsOfType<TerminalAccessibleObject>()
              .ToList()
              .ForEach(terminalObject => terminalObject.SetDoorOpenServerRpc(false));
    }
}
