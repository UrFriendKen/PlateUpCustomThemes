using KitchenData;
using KitchenMods;
using Unity.Entities;

namespace CustomThemes
{
    public struct CGivesCustomDecoration : IApplianceProperty, IAttachableProperty, IComponentData, IModComponent
    {
        public DecorationType Type;
        public int Value;

        public CGivesCustomDecoration(CustomTheme customTheme, int value)
        {
            Type = CustomThemeRegistry.GetDecorationType(customTheme);
            Value = value;
        }
    }
}
