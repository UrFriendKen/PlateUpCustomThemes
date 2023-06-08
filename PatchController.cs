using Kitchen;
using KitchenMods;

namespace CustomThemes
{
    public class PatchController : GameSystemBase, IModSystem
    {
        private static PatchController _instance;
        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }
        protected override void OnUpdate()
        {
        }

        internal static void SetCustomTheme(int theme)
        {
            if (_instance == null)
                return;
            SCustomThemesList orCreate = _instance.GetOrCreate<SCustomThemesList>();
            orCreate.Set(theme);
            _instance.SetSingleton(orCreate);
        }
    }
}
