using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace CustomThemes.Example
{
    [UpdateInGroup(typeof(ApplyEffectsGroup))]
    public class ApplyTestThemeDecorationEffects : GameSystemBase, IModSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            int level = Main.NewTheme.GetCurrentLevel();
            if (level > 0)
            {

            }
            if (level > 1)
            {

            }
            if (level > 2)
            {

            }
        }
    }
}
