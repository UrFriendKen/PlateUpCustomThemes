using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class ManageDecorationIndicators_Patch
    {
        [HarmonyPatch(typeof(ManageDecorationIndicators), "ShouldHaveIndicator")]
        [HarmonyPrefix]
        static bool ShouldHaveIndicator_Prefix(Entity candidate, ref bool __result)
        {
            if (PatchController.StaticHas<CGivesCustomDecoration>(candidate))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
