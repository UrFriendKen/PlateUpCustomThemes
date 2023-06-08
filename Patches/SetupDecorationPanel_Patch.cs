using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class SetupDecorationPanel_Patch
    {
        [HarmonyPatch(typeof(UnlockSelectPopupView), "SetupDecorationPanel")]
        [HarmonyPrefix]
        static bool SetupDecorationPanel_Prefix(DecorationBonusSetElement element, Unlock unlock)
        {
            if (unlock is UnlockCard unlockCard)
            {
                foreach (UnlockEffect effect in unlockCard.Effects)
                {
                    if (effect is CustomThemeAddEffect themeAddEffect)
                    {
                        if (!CustomThemeRegistry.TryGetTheme(themeAddEffect.ID, out CustomTheme customTheme, warn_if_fail: true))
                            continue;
                        element.Set(customTheme.DecorationType);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
