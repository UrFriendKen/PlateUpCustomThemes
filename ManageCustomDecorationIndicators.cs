using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace CustomThemes
{
    public class ManageCustomDecorationIndicators : IndicatorManager, IModSystem
    {
        protected override ViewType ViewType => Main.CustomDecorationIndicatorViewType;

        protected override EntityQuery GetSearchQuery()
        {
            return GetEntityQuery(new QueryHelper()
                .All(typeof(CGivesCustomDecoration), typeof(CPosition)));
        }

        protected override bool ShouldHaveIndicator(Entity candidate)
        {
            if (Has<CHeldBy>(candidate))
            {
                return false;
            }
            return true;
        }

        protected override Entity CreateIndicator(Entity source)
        {
            if (!Require(source, out CGivesCustomDecoration customDecoration) ||
                !Require(source, out CPosition position))
            {
                return default;
            }
            Entity entity = base.CreateIndicator(source);
            base.EntityManager.AddComponentData(entity, new CCustomDecorationIndicator
            {
                DecorationType = customDecoration.Type,
                Value = customDecoration.Value
            });
            base.EntityManager.AddComponentData(entity, new CPosition(position));
            return entity;
        }

        protected override void UpdateIndicator(Entity indicator, Entity source)
        {
            base.UpdateIndicator(indicator, source);
            if (Require(source, out CGivesCustomDecoration customDecoration))
            {
                base.EntityManager.SetComponentData(indicator, new CCustomDecorationIndicator
                {
                    DecorationType = customDecoration.Type,
                    Value = customDecoration.Value
                });
            }
        }
    }
}
