using Kitchen;
using KitchenMods;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using Unity.Collections;
using Unity.Entities;

namespace CustomThemes
{
    public class PatchController : GameSystemBase, IModSystem
    {
        private static PatchController _instance;

        public enum DelayReason
        {
            CustomThemeProgressUpdate
        }

        public struct CParameterDisplayDelay : IComponentData, IModComponent
        {
            public int Frames;

            public CParameterDisplayDelay()
            {
                Frames = 5;
            }
            public CParameterDisplayDelay(int frames)
            {
                Frames = frames;
            }
        }

        EntityQuery ParameterDisplayDelays;

        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
            ParameterDisplayDelays = GetEntityQuery(typeof(CParameterDisplayDelay));
        }
        protected override void OnUpdate()
        {
            using NativeArray<Entity> paramDelayEntities = ParameterDisplayDelays.ToEntityArray(Allocator.Temp);
            using NativeArray<CParameterDisplayDelay> paramDelays = ParameterDisplayDelays.ToComponentDataArray<CParameterDisplayDelay>(Allocator.Temp);
            for (int i = paramDelayEntities.Length - 1; i > -1; i--)
            {
                Entity entity = paramDelayEntities[i];
                CParameterDisplayDelay delay = paramDelays[i];
                delay.Frames--;
                if (delay.Frames < 1)
                    EntityManager.DestroyEntity(entity);
                Set(entity, delay);
            }

            if (CustomThemeRegistry.ProgressIsChanged)
            {
                Set<CParameterDisplayDelay>(EntityManager.CreateEntity());
            }
        }

        internal static bool StaticHas<T>(Entity e, bool errorReturn = false) where T : struct, IComponentData
        {
            return _instance?.Has<T>(e) ?? errorReturn;
        }

        internal static bool IsParameterDisplayForceUpdate()
        {
            if (_instance == null)
                return false;
            return !_instance.ParameterDisplayDelays.IsEmpty;
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
