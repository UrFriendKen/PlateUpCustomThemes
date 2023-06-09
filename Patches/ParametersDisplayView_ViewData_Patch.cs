using HarmonyLib;
using Kitchen;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class ParametersDisplayView_ViewData_Patch
    {
        [HarmonyPatch(typeof(ParametersDisplayView.ViewData), "IsChangedFrom")]
        [HarmonyPostfix]
        static void IsChanged_Postfix(ref bool __result)
        {
            if (PatchController.IsParameterDisplayForceUpdate())
                __result = true;
        }
    }
}
