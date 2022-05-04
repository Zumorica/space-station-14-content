using Content.Shared.StationWare.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.StationWare;

public abstract class SharedStationWareSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedStationWareMapComponent, ComponentGetState>(GetWareState);
        SubscribeLocalEvent<SharedStationWareMapComponent, ComponentHandleState>(HandleWareState);
    }

    private void GetWareState(EntityUid uid, SharedStationWareMapComponent component, ref ComponentGetState args)
    {
        args.State = new StationWareMapComponentState(component.RunLevel, component.Difficulty, component.Speed);
    }

    private void HandleWareState(EntityUid uid, SharedStationWareMapComponent component, ref ComponentHandleState args)
    {
        if (args.Current is not StationWareMapComponentState state)
            return;

        component.RunLevel = state.RunLevel;
        component.Difficulty = state.Difficulty;
        component.Speed = state.Speed;
    }
}
