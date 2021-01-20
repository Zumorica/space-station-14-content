using Content.Client.Chat;
using Content.Client.Interfaces.Chat;
using Content.Client.UserInterface;
using Content.Shared.Input;
using Robust.Client.Interfaces.Input;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.State
{
    public class GameScreen : GameScreenBase
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IGameHud _gameHud = default!;
        [Dependency] private readonly IInputManager _inputManager = default!;
        [Dependency] private readonly IChatManager _chatManager = default!;

        private ViewportContainer _viewport;
        [ViewVariables] private ChatBox _gameChat;
        [ViewVariables] private HSplitContainer _root;


        public override void Startup()
        {
            base.Startup();

            _root = new HSplitContainer()
            {
                MouseFilter = Control.MouseFilterMode.Ignore
            };
            LayoutContainer.SetAnchorAndMarginPreset(_root, LayoutContainer.LayoutPreset.Wide);

            _gameChat = new ChatBox()
            {
                MouseFilter = Control.MouseFilterMode.Ignore
            };

            _viewport = _userInterfaceManager.MainViewport;
            _viewport.SizeFlagsStretchRatio = 0.60f;
            _viewport.Parent!.RemoveChild(_userInterfaceManager.MainViewport);

            _root.AddChild(_userInterfaceManager.MainViewport);
            LayoutContainer.SetAnchorPreset(_userInterfaceManager.MainViewport, LayoutContainer.LayoutPreset.Wide);

            _root.AddChild(_gameChat);
            //LayoutContainer.SetAnchorAndMarginPreset(_gameChat, LayoutContainer.LayoutPreset.TopRight, margin: 10);
            //LayoutContainer.SetAnchorAndMarginPreset(_gameChat, LayoutContainer.LayoutPreset.TopRight, margin: 10);
            //LayoutContainer.SetMarginLeft(_gameChat, -475);
            //LayoutContainer.SetMarginBottom(_gameChat, 235);

            _viewport.AddChild(_gameHud.RootControl);

            _userInterfaceManager.StateRoot.AddChild(_root);

            _chatManager.SetChatBox(_gameChat);
            _gameChat.DefaultChatFormat = "say \"{0}\"";
            _gameChat.Input.PlaceHolder = Loc.GetString("Say something! [ for OOC");

            _inputManager.SetInputCommand(ContentKeyFunctions.FocusChat,
                InputCmdHandler.FromDelegate(s => FocusChat(_gameChat)));

            _inputManager.SetInputCommand(ContentKeyFunctions.FocusOOC,
                InputCmdHandler.FromDelegate(s => FocusOOC(_gameChat)));

            _inputManager.SetInputCommand(ContentKeyFunctions.FocusAdminChat,
                InputCmdHandler.FromDelegate(s => FocusAdminChat(_gameChat)));
        }

        public override void Shutdown()
        {
            base.Shutdown();

            _gameChat.Dispose();
            _gameHud.RootControl.Orphan();
        }

        internal static void FocusChat(ChatBox chat)
        {
            if (chat == null || chat.UserInterfaceManager.KeyboardFocused != null)
            {
                return;
            }

            chat.Input.IgnoreNext = true;
            chat.Input.GrabKeyboardFocus();
        }
        internal static void FocusOOC(ChatBox chat)
        {
            if (chat == null || chat.UserInterfaceManager.KeyboardFocused != null)
            {
                return;
            }

            chat.Input.IgnoreNext = true;
            chat.Input.GrabKeyboardFocus();
            chat.Input.InsertAtCursor("[");
        }

        internal static void FocusAdminChat(ChatBox chat)
        {
            if (chat == null || chat.UserInterfaceManager.KeyboardFocused != null)
            {
                return;
            }

            chat.Input.IgnoreNext = true;
            chat.Input.GrabKeyboardFocus();
            chat.Input.InsertAtCursor("]");
        }
    }
}
