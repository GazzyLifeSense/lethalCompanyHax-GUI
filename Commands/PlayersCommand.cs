
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hax;

public class PlayersCommand : ICommand {
    public static string[] Execute() {
        return Helper.StartOfRound.allPlayerScripts.Select((player,index)=> $"{(player.isPlayerDead ? 'd':'s')}{index}: {player.playerUsername}").ToArray();
    }
}
