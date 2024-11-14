using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using MirrorMode.Menu;

using JumpKing.Mods;
using JumpKing.PauseMenu;

namespace MirrorMode
{
    [JumpKingMod(IDENTIFIER)]
    public static class MirrorMode
	{
        const string IDENTIFIER = "JeFi.MirrorMode";
        const string HARMONY_IDENTIFIER = "JeFi.MirrorMode.Harmony";
        const string SETTINGS_FILE = "Preferences.xml";

        public static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
            Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
#endif
            try
            {
                Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
                Preferences = new Preferences();
            }
            Preferences.PropertyChanged += SaveSettingsOnFile;

            Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

            new Patching.EndingPictureScreen(harmony);
            new Patching.InputComponent(harmony);
            new Patching.DebugTeleport(harmony);
            new Patching.FadeTextEntity(harmony);
            new Patching.MenuFactory(harmony);
            new Patching.SpeechBubbleFormat(harmony);
            new Patching.OldManEntity(harmony);
            // Caution: Transpiler will make the methods call in function body can't be redirect after same harmony instance patching, so we better move it to last.
            new Patching.JumpGame(harmony);
#if DEBUG
            Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", null);
#endif
        }

        #region Menu Items
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleMirrorMode ToggleMirrorMode(object factory, GuiFormat format)
        {
            return new ToggleMirrorMode();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleMirrorBabeArt ToggleMirrorBabeArt(object factory, GuiFormat format)
        {
            return new ToggleMirrorBabeArt();
        }
        #endregion

        private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            try
            {
                XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            }
        }

    }
}
