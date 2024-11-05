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
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

            // new Models.SpriteBatchManager();
            Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

            new Patching.InputComponent(harmony);
            new Patching.OldManEntity(harmony);
            // new Patching.LevelScreen(harmony);
            new Patching.JumpGame(harmony);
            new Patching.FadeTextEntity(harmony);
            new Patching.MenuFactory(harmony);
            new Patching.SpeechBubbleFormat(harmony);
            new Patching.DebugTeleport(harmony);
            // new Patching.RaymanWallEntity(harmony);
        }

        [OnLevelStart]
        public static void OnLevelStart()
        {
        }

        [OnLevelEnd]
        public static void OnLevelEnd()
        {
        }

        #region Menu Items
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleMirrorMode ToggleMirrorMode(object factory, GuiFormat format)
        {
            return new ToggleMirrorMode();
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
