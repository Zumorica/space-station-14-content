using Content.Server.StationWare.Components;
using Robust.Server.Player;

namespace Content.Server.StationWare;

/// <summary>
///     Base class for all StationWare minigames.
/// </summary>
public abstract class StationWareMiniGame : EntitySystem
{
    [Dependency] protected readonly StationWareSystem Ticker = default!;

    public abstract int MinimumPlayers { get; }

    /// <summary>
    ///     Method called to start this minigame on a certain map.
    /// </summary>
    /// <param name="map">The map entity where to start the minigame.</param>
    /// <param name="ware">The StationWare map component, resolved if null.</param>
    public abstract void WareStart(EntityUid map, StationWareMapComponent? ware = null);

    /// <summary>
    ///     Method called to end this minigame on a certain map.
    /// </summary>
    /// <param name="map">The map entity where this minigame is happening.</param>
    /// <param name="ware">The StationWare map component, resolved if null.</param>
    /// <returns>The winners for this minigame.</returns>
    public abstract IEnumerable<IPlayerSession> WareEnd(EntityUid map, StationWareMapComponent? ware = null);

    /// <summary>
    ///     Method called to update this minigame's state on a certain map.
    /// </summary>
    /// <param name="frameTime">Delta time since last frame.</param>
    /// <param name="map">The map entity where this minigame is happening.</param>
    /// <param name="ware">The StationWare map component, resolved if null.</param>
    /// <returns>Whether the minigame has ended or not.</returns>
    public abstract bool WareUpdate(float frameTime, EntityUid map, StationWareMapComponent? ware = null);
}
