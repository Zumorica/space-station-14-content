using Content.Server.UserInterface;
using Content.Shared.Instruments;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.Instruments;

[RegisterComponent, ComponentReference(typeof(SharedInstrumentComponent))]
public sealed class InstrumentComponent : SharedInstrumentComponent
{
    [ViewVariables]
    public float Timer = 0f;

    [ViewVariables]
    public int BatchesDropped = 0;

    [ViewVariables]
    public int LaggedBatches = 0;

    [ViewVariables]
    public int MidiEventCount = 0;

    public IPlayerSession? InstrumentPlayer =>
        IoCManager.Resolve<IEntityManager>().GetComponentOrNull<ActivatableUIComponent>(Owner.Uid)?.CurrentSingleUser
        ?? IoCManager.Resolve<IEntityManager>().GetComponentOrNull<ActorComponent>(Owner.Uid)?.PlayerSession;

    [ViewVariables] public BoundUserInterface? UserInterface => Owner.GetUIOrNull(InstrumentUiKey.Key);
}
