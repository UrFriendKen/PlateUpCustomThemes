using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace CustomThemes
{
    public struct SDecoIndicatorForceUpdate : IComponentData, IModComponent
    {
        public float Remaining;

        public SDecoIndicatorForceUpdate()
        {
            Reset();
        }

        public void Tick(float dt)
        {
            Remaining -= dt;
        }

        public void Reset()
        {
            Remaining = 30f;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CShouldDecoIndictorForceUpdate : IComponentData, IModComponent { }

    public class ForceUpdateCustomDecorationIndicator : RestaurantSystem, IModSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (!Require(out SDecoIndicatorForceUpdate forceUpdate))
            {
                Entity e = EntityManager.CreateEntity(typeof(SDecoIndicatorForceUpdate), typeof(CDoNotPersist));
                Set<SDecoIndicatorForceUpdate>(e);
                Set<CDoNotPersist>(e);
                return;
            }
            forceUpdate.Tick(Time.DeltaTime);
            if (forceUpdate.Remaining < 0f)
            {
                Entity e = EntityManager.CreateEntity(typeof(CShouldDecoIndictorForceUpdate), typeof(CDoNotPersist));
                Set<CShouldDecoIndictorForceUpdate>(e);
                Set<CDoNotPersist>(e);
                forceUpdate.Reset();
            }
            Set(forceUpdate);
        }
    }
}
