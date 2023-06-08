using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;

namespace CustomThemes.Customs
{
    public class TestThemeUnlock : CustomUnlockCard
    {
        public override string UniqueNameID => "testTheme";
        public override List<UnlockEffect> Effects => new List<UnlockEffect>()
        {
            new CustomThemeAddEffect(Main.NewTheme)
        };
        public override bool IsUnlockable => true;
        public override UnlockGroup UnlockGroup => UnlockGroup.PrimaryTheme;
        public override CardType CardType => CardType.ThemeUnlock;
        public override int MinimumFranchiseTier => 0;
        public override bool IsSpecificFranchiseTier => false;
        public override DishCustomerChange CustomerMultiplier => DishCustomerChange.None;
        public override float SelectionBias => 0f;
        public override List<Unlock> HardcodedRequirements => new List<Unlock>();
        public override List<Unlock> HardcodedBlockers => new List<Unlock>();
        public override List<(Locale, UnlockInfo)> InfoList => new List<(Locale, UnlockInfo)>()
        {
            (Locale.English, new UnlockInfo()
            {
                Name = "New Theme",
                FlavourText = "Lorem Ipsum",
                Description = "This is a test",
                Locale = Locale.English
            })
        };
    }

    public class TestThemeLocalisation : CustomLimitedDecorationLocalisation
    {
        public override string UniqueNameID => "testThemeLocalisation";

        public override List<(Locale, LimitedDecorationBonusInfo)> InfoList => new List<(Locale, LimitedDecorationBonusInfo)>()
        {
            (Locale.English, new LimitedDecorationBonusInfo()
            {
                Locale = Locale.English,
                Text = new Dictionary<int, string>()
                {
                    { 1, "Level 1 Bonus" },
                    { 2, "Level 2 Bonus" },
                    { 3, "Level 3 Bonus" }
                }
            })
        };
    }
}
