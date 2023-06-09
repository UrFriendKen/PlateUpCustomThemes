using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenLib.Utils;
using System.Reflection;
using UnityEngine;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static MethodInfo m_GetPrefab = typeof(LocalViewRouter).GetMethod("GetPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo f_Modules = typeof(ApplianceDecorationView).GetField("Modules", BindingFlags.NonPublic | BindingFlags.Instance);
        
        static GameObject Prefab;

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ref LocalViewRouter __instance, ViewType view_type, ref GameObject __result)
        {
            if (view_type == Main.CustomDecorationIndicatorViewType)
            {
                if (Prefab == null)
                {
                    System.Object obj;
                    obj = m_GetPrefab?.Invoke(__instance, new object[] { ViewType.DecorationIndicator });
                    if (obj == null)
                    {
                        Main.LogError("Failed to get Prefab for ViewType.DecorationIndicator!");
                        return false;
                    }
                    GameObject prefabHider = new GameObject("Prefab Hider");
                    prefabHider.SetActive(false);

                    Prefab = GameObject.Instantiate(obj as GameObject);
                    Prefab.name = "Custom Decoration Indicator";
                    Prefab.transform.SetParent(prefabHider.transform, false);
                    Prefab.transform.localPosition = Vector3.zero;
                    Prefab.transform.localRotation = Quaternion.identity;
                    Prefab.transform.localScale = Vector3.one;

                    ApplianceDecorationView origView = Prefab.GetComponent<ApplianceDecorationView>();
                    if (origView == null)
                    {
                        Main.LogError("Failed to get ApplianceDecorationView from Prefab!");
                        return false;
                    }
                    obj = f_Modules.GetValue(origView);
                    if (obj == null)
                    {
                        Main.LogError("Failed to get ApplianceDecorationView.Modules!");
                        return false;
                    }
                    ApplianceCustomDecorationView customView = Prefab.AddComponent<ApplianceCustomDecorationView>();

                    DecorationSetElement origModules = (DecorationSetElement)obj;
                    customView.Modules = origModules.gameObject.AddComponent<CustomDecorationSetElement>();
                    customView.Modules.Base = origModules.Base;
                    customView.Modules.Padding = origModules.Padding;
                    customView.Modules.Centre = origModules.Centre;
                    customView.Modules.DrawHorizontal = origModules.DrawHorizontal;
                    customView.Modules.DrawSigns = origModules.DrawSigns;
                    customView.Modules.Range = origModules.Range;
                    customView.Modules.PrefabDrawMode = origModules.PrefabDrawMode;

                    customView.Modules.Prefab = FromDecorationValueElement(origModules.Prefab);
                    customView.Modules.PrefabWithInfo = FromDecorationValueElement(origModules.PrefabWithInfo);
                    customView.Modules.PrefabNumbered = FromDecorationValueElement(origModules.PrefabNumbered);
                    CustomDecorationValueElement FromDecorationValueElement(DecorationValueElement orig)
                    {
                        if (orig == null)
                            return null;
                        CustomDecorationValueElement custom = orig.gameObject.AddComponent<CustomDecorationValueElement>();
                        custom.ActiveColour = orig.ActiveColour;
                        custom.InactiveColour = orig.InactiveColour;
                        custom.ActiveHeight = orig.ActiveHeight;
                        custom.InactiveHeight = orig.InactiveHeight;
                        custom.Bounds = orig.Bounds;
                        custom.Icon = orig.Icon;
                        custom.Description = orig.Description;
                        custom.Pips = orig.Pips;
                        custom.Number = orig.Number;
                        custom.DrawMultipleIcons = orig.DrawMultipleIcons;
                        return custom;
                    }
                    Component.DestroyImmediate(origView);
                }
                __result = Prefab;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPostfix]
        static void GetPrefab_Postfix(ViewType view_type, ref GameObject __result)
        {
            if (view_type == ViewType.ParametersDisplay && __result.GetComponent<RegistryProgressUpdateView>() == null)
            {
                __result.AddComponent<RegistryProgressUpdateView>();
            }
        }
    }
}
