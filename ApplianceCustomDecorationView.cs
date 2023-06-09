using Kitchen;
using Kitchen.Modules;
using KitchenData;
using KitchenLib.Utils;
using KitchenMods;
using MessagePack;
using Shapes;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Kitchen.Modules.DecorationSetElement;

namespace CustomThemes
{
    public class ApplianceCustomDecorationView : UpdatableObjectView<ApplianceCustomDecorationView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(new QueryHelper()
                    .All(typeof(CCustomDecorationIndicator), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CCustomDecorationIndicator> customDecorationIndicators = Views.ToComponentDataArray<CCustomDecorationIndicator>(Allocator.Temp);

                bool isNight = HasSingleton<SIsNightTime>();

                for (int i = 0; i < views.Length; i++)
                {
                    CLinkedView view = views[i];
                    CCustomDecorationIndicator customDecorationIndicator = customDecorationIndicators[i];
                    SendUpdate(view, new ViewData
                    {
                        Decorations = new Dictionary<DecorationType, int>()
                        {
                            { customDecorationIndicator.DecorationType, customDecorationIndicator.Value }   // To add support for more than 1 value? TBC
                        },
                        IsNight = isNight
                    });
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public Dictionary<DecorationType, int> Decorations;

            [Key(1)]
            public bool IsNight;

            public bool IsChangedFrom(ViewData check)
            {
                if (IsNight != check.IsNight || Decorations.Count != check.Decorations.Count)
                    return true;
                foreach (KeyValuePair<DecorationType, int> decoration in Decorations)
                {
                    if (!check.Decorations.TryGetValue(decoration.Key, out int value) || value != decoration.Value)
                        return true;
                }
                return false;
            }
        }

        public CustomDecorationSetElement Modules;

        private ViewData Data;

        protected override void UpdateData(ViewData view_data)
        {
            Data = view_data;
            base.gameObject.SetActive(view_data.IsNight);
            Modules.SetValues(view_data.Decorations);
        }
    }

    public class CustomDecorationSetElement : Element
    {
        public CustomDecorationValueElement Prefab;

        public CustomDecorationValueElement PrefabWithInfo;

        public CustomDecorationValueElement PrefabNumbered;

        public Vector3 Base;

        public Vector3 Padding;

        public bool Centre;

        public bool DrawHorizontal;

        public bool DrawSigns;

        public TextMeshPro Range;

        public GameObject Container;

        public DecorationSetElement.DrawMode PrefabDrawMode;

        private Bounds Bounds = default(Bounds);

        public override Bounds BoundingBox => Bounds;

        public void SetRange(string text)
        {
            if (!(Range == null))
            {
                Range.text = text;
            }
        }

        public void SetValues(Dictionary<DecorationType, int> values)
        {
            if (Container != null)
            {
                UnityEngine.Object.Destroy(Container);
            }
            Bounds = default(Bounds);
            Container = new GameObject();
            Container.transform.parent = base.transform;
            Container.transform.localPosition = Vector3.zero;
            Container.transform.localRotation = Quaternion.identity;
            Container.transform.localScale = Vector3.one;
            Vector3 basePos = Base;
            foreach (KeyValuePair<DecorationType, int> item in values)
            {
                DecorationType decorationType = item.Key;
                int progress = item.Value;
                if (!CustomThemeRegistry.TryGetTheme((int)decorationType, out CustomTheme theme, warn_if_fail: true))
                    continue;
                if (PrefabDrawMode != DrawMode.Number)
                {

                    int bonusLevel = CustomThemeRegistry.GetBonusLevel(item.Value, theme.MaxLevel);
                    for (int i = 0; i <= bonusLevel; i++)
                    {
                        if ((i > 0 && PrefabDrawMode == DrawMode.Info) || (i == bonusLevel && PrefabDrawMode != DrawMode.Info))
                        {
                            int partialLevel = (i == bonusLevel) ? CustomThemeRegistry.GetPartialLevel(progress) : 0;
                            if (partialLevel > 0 || i > 0)
                            {
                                Element element = CreateModule(decorationType, i, (PrefabDrawMode != DrawMode.Info) ? partialLevel : 0, basePos);
                                Vector3 vector = (DrawHorizontal ? (Vector3.right * element.BoundingBox.size.x) : (Vector3.back * element.BoundingBox.size.y));
                                basePos += vector + Padding;
                                Bounds.Encapsulate(element.BoundingBox);
                            }
                        }
                    }
                }
                else
                {
                    if (progress != 0)
                    {
                        Element element2 = CreateModule(decorationType, 0, progress, basePos);
                        Vector3 vector2 = (DrawHorizontal ? (Vector3.right * element2.BoundingBox.size.x) : (Vector3.back * element2.BoundingBox.size.y));
                        basePos += vector2 + Padding;
                        Bounds.Encapsulate(element2.BoundingBox);
                    }
                }
            }
            if (Centre)
            {
                Container.transform.localPosition = -Bounds.center;
            }
        }

        private Element CreateModule(DecorationType type, int count, int partial, Vector3 position)
        {
            CustomDecorationValueElement decorationValueElement = UnityEngine.Object.Instantiate(PrefabDrawMode switch
            {
                DrawMode.Normal => Prefab,
                DrawMode.Info => PrefabWithInfo,
                DrawMode.Number => PrefabNumbered,
                _ => Prefab,
            }, Container.transform, worldPositionStays: true);
            decorationValueElement.Set(type, count, partial, DrawSigns);
            Transform transform = decorationValueElement.transform;
            transform.localScale = Vector3.one;
            transform.localPosition = position + Vector3.back * decorationValueElement.BoundingBox.extents.y;
            return decorationValueElement;
        }
    }

    public class CustomDecorationValueElement : Element
    {
        [ColorUsage(true, true)]
        public Color ActiveColour;

        [ColorUsage(true, true)]
        public Color InactiveColour;

        public float ActiveHeight;

        public float InactiveHeight;

        public Bounds Bounds;

        public TextMeshPro Icon;

        public TextMeshPro Description;

        public List<Rectangle> Pips;

        public TextMeshPro Number;

        public bool DrawMultipleIcons;

        public override Bounds BoundingBox
        {
            get
            {
                Bounds.center = base.transform.localPosition;
                return Bounds;
            }
        }

        public void Set(DecorationType type, int level, int partial, bool draw_sign = false)
        {
            if (CustomThemeRegistry.TryGetTheme((int)type, out CustomTheme theme) && !theme.Icon.IsNullOrEmpty())
            {
                string value = theme.Icon;
                if (DrawMultipleIcons)
                {
                    int progress = level * 3 + partial;
                    StringBuilder stringBuilder = new StringBuilder(progress);
                    for (int i = 0; i < progress; i++)
                    {
                        stringBuilder.Append(value);
                    }
                    Icon.text = stringBuilder.ToString();
                }
                else
                {
                    Icon.text = value;
                }
                if (Description != null)
                {
                    Description.text = theme.GetBonusText(level);
                }
            }
            if (Pips != null)
            {
                for (int j = 0; j < Pips.Count; j++)
                {
                    Pips[j].Color = ((j < level) ? ActiveColour : InactiveColour);
                    Pips[j].Height = ((j < level) ? ActiveHeight : InactiveHeight);
                    if (j == level && partial > 0)
                    {
                        Pips[j].Color = Color.Lerp(InactiveColour, ActiveColour, 0.5f * (float)partial / 3f);
                        Pips[j].Height = Mathf.Lerp(InactiveHeight, ActiveHeight, 0.5f * (float)partial / 3f);
                    }
                }
            }
            if (Number != null)
            {
                int progress = level * 3 + partial;
                string sign = "";
                if (draw_sign && progress > 0)
                {
                    sign = "+";
                }
                Number.text = $"{sign}{progress}";
            }
        }
    }
}
