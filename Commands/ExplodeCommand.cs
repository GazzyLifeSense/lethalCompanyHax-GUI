using System.Linq;
using UnityEngine;

namespace Hax;

public class ExplodeCommand : ICommand {
    public static void Execute(string mode) {
        switch (mode)
        {
            case "landmine":
                Object
                .FindObjectsOfType<Landmine>()
                .ToList()
                .ForEach(mine => mine.TriggerMine());
                break;
            case "jetpack":
                Object
                .FindObjectsOfType<JetpackItem>()
                .ToList()
                .ForEach(jetpack => {
                    jetpack.ExplodeJetpackServerRpc();
                    });
                break;
        }
    }
}
