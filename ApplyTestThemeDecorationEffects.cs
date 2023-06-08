using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace CustomThemes
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
            //Main.LogInfo(Main.NewTheme.GetCurrentProgress());
        }
    }
}
