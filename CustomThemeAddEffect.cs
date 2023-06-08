using KitchenData;
using System;

namespace CustomThemes
{
    public class CustomThemeAddEffect : UnlockEffect
    {
        public int ID;

        public CustomThemeAddEffect(CustomTheme theme)
        {
            ID = theme.ID;
        }
    }
}
