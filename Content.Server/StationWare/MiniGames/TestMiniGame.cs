using Content.Server.StationWare.Components;
using Robust.Server.Player;

namespace Content.Server.StationWare.MiniGames;

public sealed class TestMiniGame : StationWareMiniGame
{
    public override int MinimumPlayers => 0;

    public override void WareStart(EntityUid map, StationWareMapComponent? ware = null)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<IPlayerSession> WareEnd(EntityUid map, StationWareMapComponent? ware = null)
    {
        throw new NotImplementedException();
    }

    public override bool WareUpdate(float frameTime, EntityUid map, StationWareMapComponent? ware = null)
    {
        throw new NotImplementedException();
    }
}
