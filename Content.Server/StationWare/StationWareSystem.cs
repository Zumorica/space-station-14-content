using System.Linq;
using Content.Server.Players;
using Content.Server.StationWare.Components;
using Content.Shared.StationWare;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility.Collections;

namespace Content.Server.StationWare;

public sealed partial class StationWareSystem : SharedStationWareSystem
{
    [Dependency] private readonly IEntitySystemManager _entitySystemMan = default!;
    [Dependency] private readonly IReflectionManager _reflectionMan = default!;
    [Dependency] private readonly IPlayerManager _playerMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private StationWareMiniGame[] _minigames = Array.Empty<StationWareMiniGame>();

    public override void Initialize()
    {
        base.Initialize();

        _playerMan.PlayerStatusChanged += OnPlayerStatusChanged;

        var minigames = new ValueList<StationWareMiniGame>();

        foreach (var type in _reflectionMan.GetAllChildren<StationWareMiniGame>())
        {
            // As far as I'm aware, GetAllChildren guarantees no abstract types...
            minigames.Add((StationWareMiniGame)_entitySystemMan.GetEntitySystem(type));
        }

        _minigames = minigames.ToArray();
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _playerMan.PlayerStatusChanged -= OnPlayerStatusChanged;
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        if (e.NewStatus != SessionStatus.Disconnected || e.Session.AttachedEntity is not {} uid)
            return;

        if (!HasComp<StationWareMindComponent>(uid))
            return;

        var xform = Transform(uid);

        if (xform.MapID == MapId.Nullspace)
            return;

        var map = _mapManager.GetMapEntityId(xform.MapID);

        if (!TryComp(map, out StationWareMapComponent? ware))
            return; // Not on a stationware map, adminbus moment?

        RemovePlayer(map, e.Session, ware);
    }

    public EntityUid CreateStationWareMap()
    {
        var mapId = _mapManager.CreateMap();
        var mapUid = _mapManager.GetMapEntityId(mapId);

        var ware = AddComp<StationWareMapComponent>(mapUid);

        // TODO: Cvar with set time
        ware.NextRunLevelTime = _gameTiming.CurTime + TimeSpan.FromSeconds(30);

        Dirty(ware);

        return mapUid;
    }

    public bool AddPlayer(EntityUid map, IPlayerSession session, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware))
            return false;

        return ware.Players.Add(session);
    }

    public bool RemovePlayer(EntityUid map, IPlayerSession session, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware))
            return false;

        return ware.Players.Remove(session);
    }

    /// <summary>
    ///     Attempts to starts the StationWare game on a given map, but only if it's currently in the initial lobby.
    ///     Will fail if there's not at least two players.
    /// </summary>
    public void StartStationWare(EntityUid map, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware) || ware.RunLevel != StationWareRunLevel.InitialLobby)
            return;

        // TODO: better mincount (set to 1 for debugging)
        if (ware.Players.Count < 1)
            return;

        ware.RunLevel = StationWareRunLevel.GamePrepareLobby;
        Dirty(ware);
    }

    /// <summary>
    ///     Starts a minigame on a given StationWare game.
    /// </summary>
    public void StartStationWareMiniGame(EntityUid map, StationWareMiniGame miniGame, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware) || ware.RunLevel != StationWareRunLevel.GamePrepareLobby)
            return;

        ware.RunLevel = StationWareRunLevel.MiniGame;
        ware.MiniGame = miniGame;
        Dirty(ware);
    }

    public IEnumerable<IPlayerSession> EndStationWareMiniGame(EntityUid map, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware) || ware.RunLevel != StationWareRunLevel.MiniGame)
            return Enumerable.Empty<IPlayerSession>();

        if (ware.MiniGame is not {} miniGame)
        {
            ware.RunLevel = StationWareRunLevel.GamePrepareLobby;
            Dirty(ware);

            return Enumerable.Empty<IPlayerSession>();
        }


        var winners = miniGame.WareEnd(map, ware);

        ware.RunLevel = StationWareRunLevel.GameEndLobby;
        ware.NextRunLevelTime = TimeSpan.FromSeconds(10); // TODO: unhardcode these
        Dirty(ware);

        // TODO: do something with the winners here (give them points, show their names or something?)

        return winners;
    }

    public void NextStationWareRound(EntityUid map, StationWareMapComponent? ware = null)
    {
        if (!Resolve(map, ref ware))
            return;

        ware.RunLevel = StationWareRunLevel.GamePrepareLobby;
        ware.NextRunLevelTime = TimeSpan.FromSeconds(10); // TODO: unhardcode these
        Dirty(ware);

        return;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var ware in EntityQuery<StationWareMapComponent>())
        {
            var map = ware.Owner;

            switch (ware.RunLevel)
            {
                case StationWareRunLevel.InitialLobby:
                    if (_gameTiming.CurTime >= ware.NextRunLevelTime)
                    {
                        StartStationWare(map, ware);
                        break;
                    }

                    break;

                case StationWareRunLevel.GamePrepareLobby:
                    if (_gameTiming.CurTime >= ware.NextRunLevelTime)
                    {
                        // TODO: improve minigame picking algo
                        StartStationWareMiniGame(map, _random.Pick(_minigames), ware);
                        break;
                    }

                    break;

                case StationWareRunLevel.MiniGame:
                    if (ware.MiniGame is not {} miniGame)
                        return; // TODO: handle this better, this shouldn't happen ideally

                    if (miniGame.WareUpdate(frameTime, map, ware))
                    {
                        EndStationWareMiniGame(map, ware);
                        break;
                    }

                    break;

                case StationWareRunLevel.GameEndLobby:
                    if (_gameTiming.CurTime >= ware.NextRunLevelTime)
                    {
                        NextStationWareRound(map, ware);
                        break;
                    }

                    break;

                case StationWareRunLevel.EndLobby:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


}
