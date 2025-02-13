using System;
using Content.Server.Administration;
using Content.Server.Commands;
using Content.Shared.Administration;
using Content.Shared.Alert;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Alert.Commands
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class ShowAlert : IConsoleCommand
    {
        public string Command => "showalert";
        public string Description => "Shows an alert for a player, defaulting to current player";
        public string Help => "showalert <alertType> <severity, -1 if no severity> <name or userID, omit for current player>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player as IPlayerSession;
            if (player?.AttachedEntity == null)
            {
                shell.WriteLine("You cannot run this from the server or without an attached entity.");
                return;
            }

            var attachedEntity = player.AttachedEntity.Value;

            if (args.Length > 2)
            {
                var target = args[2];
                if (!CommandUtils.TryGetAttachedEntityByUsernameOrId(shell, target, player, out attachedEntity)) return;
            }

            if (!IoCManager.Resolve<IEntityManager>().TryGetComponent(attachedEntity, out ServerAlertsComponent? alertsComponent))
            {
                shell.WriteLine("user has no alerts component");
                return;
            }

            var alertType = args[0];
            var severity = args[1];
            var alertMgr = IoCManager.Resolve<AlertManager>();
            if (!alertMgr.TryGet(Enum.Parse<AlertType>(alertType), out var alert))
            {
                shell.WriteLine("unrecognized alertType " + alertType);
                return;
            }
            if (!short.TryParse(severity, out var sevint))
            {
                shell.WriteLine("invalid severity " + sevint);
                return;
            }
            alertsComponent.ShowAlert(alert.AlertType, sevint == -1 ? null : sevint);
        }
    }
}
