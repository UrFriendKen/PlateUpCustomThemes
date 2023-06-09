using HarmonyLib;
using KitchenData;
using KitchenLib.Utils;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class DecorationTypeExtensions_Patch
    {
        [HarmonyPatch(typeof(DecorationTypeExtensions), nameof(DecorationTypeExtensions.Icon))]
        [HarmonyPrefix]
        static bool Icon_Prefix(DecorationType type, ref string __result)
        {
            if (!CustomThemeRegistry.TryGetTheme((int)type, out CustomTheme customTheme))
                return true;
            __result = customTheme.Icon != "?" ? $"<sprite name=\"{customTheme.Icon}\" tint=1>" : "?";
            return false;
        }
    }
}
