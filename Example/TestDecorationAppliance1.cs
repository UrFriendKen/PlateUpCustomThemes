using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CustomThemes.Example
{
    public class TestDecorationAppliance1 : CustomAppliance
    {
        public override string UniqueNameID => "testDecorationAppliance1";
        public override GameObject Prefab => null; // Appliance Prefab
        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CGivesDecoration(),
            new CGivesCustomDecoration(Main.NewTheme, 3)
        };
        public override bool IsPurchasable => true;
        public override PriceTier PriceTier => PriceTier.DecoMediumCheap;
        public override ShoppingTags ShoppingTags => ShoppingTags.Decoration;
        public override List<(Locale, ApplianceInfo)> InfoList => new List<(Locale, ApplianceInfo)>()
        {
            (Locale.English, new ApplianceInfo()
            {
                Name = "NewTheme Appliance",
                Description = "This is a new Decoration Appliance",
                Sections = new List<Appliance.Section>()
                {
                    new Appliance.Section()
                    {
                        Title = "Section",
                        Description = "Lorem ipsum dolor sit"
                    }
                }
            })
        };
    }
}
