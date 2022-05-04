namespace Content.Shared.StationWare;

public enum StationWareRunLevel
{
    /// <summary>
    ///     Before the game has started at all.
    /// </summary>
    InitialLobby,

    /// <summary>
    ///     The game has started, we're waiting for the next minigame.
    /// </summary>
    GamePrepareLobby,

    /// <summary>
    ///     We're in the middle of a minigame.
    /// </summary>
    MiniGame,

    /// <summary>
    ///     The minigame has ended, show who won.
    /// </summary>
    GameEndLobby,

    /// <summary>
    ///     The game has ended, and we (probably) have a winner.
    /// </summary>
    EndLobby
}
