using HarmonyLib;
using Kitchen;
using KitchenData;
using System.Reflection;
using System.Text;

namespace CustomThemes.Patches
{
    [HarmonyPatch]
    static class ApplianceInfoView_Patch
    {
        static MethodInfo m_AddSection = typeof(ApplianceInfoView).GetMethod("AddSection", BindingFlags.NonPublic | BindingFlags.Instance);

        static int _applianceID;

        [HarmonyPatch(typeof(ApplianceInfoView), "UpdateData")]
        [HarmonyPrefix]
        static void UpdateData_Prefix(ApplianceInfoView.ViewData data)
        {
            _applianceID = data.ID;
        }

        [HarmonyPatch(typeof(ApplianceInfoView), "AddDecorationInfo")]
        [HarmonyPrefix]
        static bool AddDecorationInfo_Prefix(ref ApplianceInfoView __instance, float offset, DecorationValues values, ref float __result)
        {
            if (!GameData.Main.TryGet(_applianceID, out Appliance gdo) || m_AddSection == null)
                return true;

            StringBuilder stringBuilder = new StringBuilder();
            int count = 0;
            foreach (IApplianceProperty property in gdo.Properties)
            {
                if (property is CGivesCustomDecoration cGivesCustomDecoration && cGivesCustomDecoration.Value > 0)
                {
                    for (int j = 0; j < cGivesCustomDecoration.Value; j++)
                    {
                        stringBuilder.Append(GameData.Main.GlobalLocalisation.GetIcon(cGivesCustomDecoration.Type));
                        stringBuilder.Append(" ");
                    }
                    count += cGivesCustomDecoration.Value;
                }
            }

            DecorationType[] types = DecorationValues.Types;
            foreach (DecorationType decorationType in types)
            {
                if (values[decorationType] > 0)
                {
                    ConcatenateDecorationIcons(decorationType, values[decorationType]);
                    count += values[decorationType];
                }
            }

            void ConcatenateDecorationIcons(DecorationType decorationType, int value)
            {
                for (int j = 0; j < value; j++)
                {
                    stringBuilder.Append(GameData.Main.GlobalLocalisation.GetIcon(decorationType));
                    stringBuilder.Append(" ");
                }
            }

            if (count > 0)
            {
                __result += (float)m_AddSection.Invoke(__instance, new object[]
                {
                    offset + __result,
                    new Appliance.Section()
                    {
                        Title = GameData.Main.GlobalLocalisation["ADDS_DECORATION"],
                        Description = stringBuilder.ToString(),
                        RangeDescription = ""
                    },
                    true
                });
                return false;
            }
            return true;
        }
    }
}
