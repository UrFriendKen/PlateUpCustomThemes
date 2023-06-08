using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace CustomThemes
{
    public struct SCustomThemesList : IComponentData, IModComponent
    {
        public FixedListInt512 Themes = new FixedListInt512();
        
        public SCustomThemesList()
        {
        }

        public void Set(int theme)
        {
            if (!Themes.Contains(theme))
            {
                Themes.Add(theme);
            }
        }

        public void Clear(int theme)
        {
            if (Themes.Contains(theme))
            {
                Themes.Remove(theme);
            }
        }

        public bool Has(int theme)
        {
            return Themes.Contains(theme);
        }
    }
}
