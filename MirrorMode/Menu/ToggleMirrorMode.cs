using JumpKing.PauseMenu.BT.Actions;

namespace MirrorMode.Menu
{
    public class ToggleMirrorMode : ITextToggle
    {
        public ToggleMirrorMode() : base(MirrorMode.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Mirror Mode";

        protected override void OnToggle()
        {
            MirrorMode.Preferences.IsEnabled = toggle;
        }
    }
}
