using HarmonyLib;
using Kitchen.Modules;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class DecorationBonusSetElement_Patch
    {
        static MethodInfo m_ClearAfterIndex = typeof(DecorationBonusSetElement).GetMethod("ClearAfterIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo m_ModuleAtIndex = typeof(DecorationBonusSetElement).GetMethod("ModuleAtIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(DecorationBonusSetElement), nameof(DecorationBonusSetElement.Set), new Type[] { typeof(DecorationType) })]
        [HarmonyPrefix]
        static bool Set_Prefix(ref DecorationBonusSetElement __instance, DecorationType type)
        {
            if (CustomThemeRegistry.TryGetTheme((int)type, out CustomTheme customTheme))
            {
                int index = 0;
                for (int i = 1; i <= customTheme.MaxLevel; i++)
                {
                    SetModule(__instance, index++, customTheme, i, 0);
                }
                m_ClearAfterIndex.Invoke(__instance, new object[] { index });
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DecorationBonusSetElement), nameof(DecorationBonusSetElement.Set), new Type[] { typeof(DecorationValues) })]
        [HarmonyPostfix]
        static void Set_Postfix(ref DecorationBonusSetElement __instance)
        {
            int index = __instance.Modules.Count;
            foreach (CustomTheme customTheme in CustomThemeRegistry.GetAllCustomThemes())
            {
                if (customTheme.CurrentProgress > 0)
                {
                    int num = Mathf.Clamp(customTheme.CurrentLevel + 1, 1, customTheme.MaxLevel);
                    for (int j = 1; j <= num; j++)
                    {
                        SetModule(__instance, index++, customTheme, j, customTheme.CurrentProgress);
                    }
                }
            }
            m_ClearAfterIndex.Invoke(__instance, new object[] { index });
        }

        static void SetModule(DecorationBonusSetElement instance, int index, CustomTheme customTheme, int level, int progress)
        {
            DecorationBonusElement decorationBonusElement = (DecorationBonusElement)m_ModuleAtIndex.Invoke(instance, new object[] { index });
            decorationBonusElement.Set(customTheme.GetBonusText(level), customTheme.DecorationType, progress, level * 3);
        }
    }
}
