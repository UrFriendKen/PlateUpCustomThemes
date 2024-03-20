using CustomThemes.Example;
using Kitchen;
using KitchenLib;
using KitchenLib.Customs;
using KitchenLib.Event;
using KitchenLib.Utils;
using KitchenMods;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace CustomThemes
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.CustomThemes";
        public const string MOD_NAME = "Custom Themes";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.9";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        public static AssetBundle Bundle;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        internal static CustomTheme NewTheme;

        internal static ViewType CustomDecorationIndicatorViewType = (ViewType)VariousUtils.GetID("CustomDecorationIndicator");

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }
        


        // See here
        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            AddGameDataObject<TestThemeUnlock>();
            NewTheme = CustomThemeRegistry.RegisterTheme("New Theme", AddGameDataObject<TestThemeLocalisation>());
            AddDecorationApplianceGameDataObject<TestDecorationAppliance1>(NewTheme);

            LogInfo("Done loading game data.");
        }

        private CustomAppliance AddDecorationApplianceGameDataObject<T>(CustomTheme customTheme) where T : CustomAppliance, new()
        {
            CustomAppliance customAppliance = AddGameDataObject<T>();
            CustomThemeRegistry.AddCustomApplianceRequiresTheme(customAppliance, customTheme);
            return customAppliance;
        }





        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {

            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            //LogInfo("Attempting to load asset bundle...");
            //Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            //LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            //AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                CustomThemeRegistry.Complete(args.gamedata);
            };
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
