using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace CustomThemes.Patches
{
    static class DelegateUnlockSystem_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(DelegateUnlockSystem), t => t.Name.Contains("c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        static void Prefix(Entity e, CProgressionOption unlock)
        {
            if (GameData.Main.TryGet(unlock.ID, out Unlock gdo))
            {
                CardType cardType = gdo.CardType;
                if ((cardType != CardType.FranchiseTier || unlock.FromFranchise) && (gdo is UnlockCard unlockCard))
                {
                    for (int i = 0; i < unlockCard.Effects.Count; i++)
                    {
                        UnlockEffect unlockEffect = unlockCard.Effects[i];
                        if (unlockEffect is CustomThemeAddEffect customThemeAddEffect)
                        {
                            PatchController.SetCustomTheme(customThemeAddEffect.ID);
                        }
                    }
                }
            }
        }
    }
}
