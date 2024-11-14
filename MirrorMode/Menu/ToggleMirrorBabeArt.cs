using JumpKing.PauseMenu.BT.Actions;

namespace MirrorMode.Menu
{
    public class ToggleMirrorBabeArt : ITextToggle
    {
        public ToggleMirrorBabeArt() : base(MirrorMode.Preferences.IsMirrorBabeArt)
        {
        }

        protected override string GetName() => "Mirror Babe Art";

        protected override void OnToggle()
        {
            MirrorMode.Preferences.IsMirrorBabeArt = toggle;
        }
    }
}
