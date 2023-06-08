using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace CustomThemes
{
    [UpdateAfter(typeof(DetermineDecorationLevels))]
    public class DetermineCustomDecorationLevels : GameSystemBase, IModSystem
    {
        private Dictionary<DecorationType, int> Scores = new Dictionary<DecorationType, int>();

        EntityQuery CustomDecorationAppliances;

        protected override void Initialise()
        {
            base.Initialise();
            CustomDecorationAppliances = GetEntityQuery(new QueryHelper()
                .All(typeof(CGivesCustomDecoration))
                .None(typeof(CHeldBy), typeof(CDestroyApplianceAtDay)));
        }

        protected override void OnUpdate()
        {
            Scores.Clear();
            using NativeArray<CGivesCustomDecoration> givesCustomDecorations = CustomDecorationAppliances.ToComponentDataArray<CGivesCustomDecoration>(Allocator.Temp);
            
            for (int i = 0; i < givesCustomDecorations.Length; i++)
            {
                CGivesCustomDecoration givesCustomDecoration = givesCustomDecorations[i];
                if (!Scores.TryGetValue(givesCustomDecoration.Type, out int value))
                {
                    Scores.Add(givesCustomDecoration.Type, givesCustomDecoration.Value);
                    continue;
                }
                Scores[givesCustomDecoration.Type] = value + givesCustomDecoration.Value;
            }
            CustomThemeRegistry.CurrentProgresses.Clear();
            foreach (KeyValuePair<DecorationType, int> score in Scores)
            {
                CustomThemeRegistry.CurrentProgresses.Add(score.Key, score.Value);
            }
        }
    }
}
