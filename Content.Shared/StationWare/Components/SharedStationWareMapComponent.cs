using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.StationWare.Components;

[NetworkedComponent, Friend(typeof(SharedStationWareSystem))]
public abstract class SharedStationWareMapComponent : Component
{
    /// <summary>
    ///     Current "Run Level" of this StationWare game.
    /// </summary>
    public StationWareRunLevel RunLevel { get; set; } = StationWareRunLevel.InitialLobby;

    /// <summary>
    ///     Current minigame difficulty.
    /// </summary>
    public int Difficulty { get; set; } = 1;

    /// <summary>
    ///     Current minigame speed.
    /// </summary>
    public int Speed { get; set; } = 1;

    /// <summary>
    ///     Time at which the run level will increment.
    /// </summary>
    public TimeSpan NextRunLevelTime { get; set; }
}

[Serializable, NetSerializable]
public sealed class StationWareMapComponentState : ComponentState
{
    public StationWareRunLevel RunLevel { get; }

    public int Difficulty { get; }

    public int Speed { get; }

    public StationWareMapComponentState(StationWareRunLevel runLevel, int difficulty, int speed)
    {
        RunLevel = runLevel;
        Difficulty = difficulty;
        Speed = speed;
    }
}
