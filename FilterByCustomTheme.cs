using Kitchen;
using Kitchen.ShopBuilder;
using System.Collections.Generic;
using Unity.Entities;

namespace CustomThemes
{
    [UpdateInGroup(typeof(ShopOptionGroup))]
    public class FilterByCustomTheme : ShopBuilderFilter
    {
        private SCustomThemesList ActiveThemes;

        protected override void BeforeRun()
        {
            base.BeforeRun();
            ActiveThemes = GetOrDefault<SCustomThemesList>();
        }
        protected override void Filter(ref CShopBuilderOption option)
        {
            if (!option.IsRemoved && CustomThemeRegistry.TryGetApplianceRequiresThemes(option.Appliance, out HashSet<int> requiredThemes))
            {
                foreach (int theme in requiredThemes)
                {
                    if (!ActiveThemes.Has(theme))
                    {
                        option.IsRemoved = true;
                        option.FilteredBy = this;
                        return;
                    }
                }
            }
        }
    }
}
