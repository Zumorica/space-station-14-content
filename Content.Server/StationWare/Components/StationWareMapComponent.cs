using Content.Shared.StationWare.Components;
using Robust.Server.Player;
using Robust.Shared.Players;

namespace Content.Server.StationWare.Components;

[RegisterComponent, Friend(typeof(StationWareSystem))]
public sealed class StationWareMapComponent : SharedStationWareMapComponent
{
    [ViewVariables] public StationWareMiniGame? MiniGame { get; set; } = null;

    /// <summary>
    ///     People actually playing the minigames.
    /// </summary>
    [ViewVariables] public HashSet<IPlayerSession> Players { get; } = default!;

    /// <summary>
    ///     UID of the main StationWare lobby grid.
    /// </summary>
    [ViewVariables] public EntityUid Lobby = EntityUid.Invalid;

    /// <summary>
    ///     People observing the minigames.
    /// </summary>
    //[ViewVariables] public IEnumerable<IPlayerSession> Observers { get; } = default!;
}
