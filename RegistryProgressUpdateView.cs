using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace CustomThemes
{
    public class RegistryProgressUpdateView : UpdatableObjectView<RegistryProgressUpdateView.ViewData>
    {
        [UpdateAfter(typeof(ApplianceCustomDecorationView.UpdateView))]
        public class UpdateView : ViewSystemBase, IModSystem
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(new QueryHelper()
                    .All(typeof(CParametersDisplay), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                if (CustomThemeRegistry.ProgressIsChanged)
                {
                    using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                    Dictionary<DecorationType, int> progress = CustomThemeRegistry.CurrentProgress;
                    for (int i = 0; i < views.Length; i++)
                    {
                        CLinkedView view = views[i];
                        SendUpdate(view, new ViewData()
                        {
                            Decorations = progress
                        });
                    }
                }
            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData
        {
            [Key(0)]
            public Dictionary<DecorationType, int> Decorations;

            public IUpdatableObject GetRelevantSubview(IObjectView view)
            {
                return view.GetSubView<RegistryProgressUpdateView>();
            }
        }

        protected override void UpdateData(ViewData data)
        {
            if (Session.CurrentGameNetworkMode == GameNetworkMode.Client)
            {
                CustomThemeRegistry.CurrentProgress = data.Decorations;
            }
        }
    }
}
