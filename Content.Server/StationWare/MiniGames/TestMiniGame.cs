using System.Linq;
using Content.Server.StationWare.Components;
using Robust.Server.Player;
using Robust.Shared.Player;

namespace Content.Server.StationWare.MiniGames;

public sealed class TestMiniGame : StationWareMiniGame
{
    [ViewVariables(VVAccess.ReadWrite)] public bool Ended = false;

    public override int MinimumPlayers => 0;

    public override void WareStart(EntityUid map, StationWareMapComponent? ware = null)
    {
        Logger.Info("yay, started!");
    }

    public override IEnumerable<IPlayerSession> WareEnd(EntityUid map, StationWareMapComponent? ware = null)
    {
        Logger.Info("yay, ended!");

        // kill kill kill kill
        return Filter.Broadcast().Recipients.Cast<IPlayerSession>();
    }

    public override bool WareUpdate(float frameTime, EntityUid map, StationWareMapComponent? ware = null)
    {
        Logger.Info("WARIO UPDATE! WAHHHH");

        return Ended;
    }
}
