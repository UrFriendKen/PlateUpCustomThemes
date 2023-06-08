using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;
using UnityEngine;

namespace CustomThemes.Customs
{
    public class LimitedDecorationBonusInfo : Localisation
    {
        public Dictionary<int, string> Text;

        public override void Export(LocalisationContext context)
        {
            foreach (KeyValuePair<int, string> item in Text)
            {
                context.Add(item.Key.ToString(), item.Value);
            }
        }

        public override void Import(LocalisationContext context)
        {
            base.SetContext(context);
            Text = new Dictionary<int, string>();
            Dictionary<string, string> temp = new Dictionary<string, string>();
            context.GetAll(temp);
            foreach (KeyValuePair<string, string> kvp in temp)
            {
                if (!int.TryParse(kvp.Key, out int key))
                    continue;
                Text[key] = kvp.Value;
            }
        }
    }

    public class LimitedDecorationLocalisation : LocalisationSet<LimitedDecorationBonusInfo>
    {
        public LocalisationObject<LimitedDecorationBonusInfo> Info;

        public Dictionary<int, string> Text;

        public string this[int level]
        {
            get
            {
                if (Text.TryGetValue(level, out var value))
                {
                    return value;
                }
                return "";
            }
        }

        public override LocalisationObject<LimitedDecorationBonusInfo> LocalisationInfo => Info;

        protected override void InitialiseDefaults()
        {
        }

        public override bool Localise(KitchenData.Locale locale, StringSubstitutor subs)
        {
            if (Info == null)
            {
                return false;
            }
            LimitedDecorationBonusInfo decorationBonusInfo = Info.Get(locale);
            if (decorationBonusInfo == null)
            {
                return false;
            }
            Text = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> item in decorationBonusInfo.Text)
            {
                Text.Add(item.Key, subs.Parse(item.Value));
            }
            return true;
        }
    }

    public abstract class CustomLimitedDecorationLocalisation : CustomLocalisedGameDataObject<LimitedDecorationLocalisation, LimitedDecorationBonusInfo>
    {
        public override sealed int BaseGameDataObjectID => base.BaseGameDataObjectID;

        public override void Convert(GameData gameData, out GameDataObject gameDataObject)
        {
            LimitedDecorationLocalisation decorationLocalisation = ScriptableObject.CreateInstance<LimitedDecorationLocalisation>();
            if (decorationLocalisation.ID != ID)
            {
                decorationLocalisation.ID = ID;
            }
            if (InfoList.Count > 0)
            {
                decorationLocalisation.Info = new LocalisationObject<LimitedDecorationBonusInfo>();
                foreach (var info in InfoList)
                {
                    decorationLocalisation.Info.Add(info.Item1, info.Item2);
                }
            }
            if (decorationLocalisation.Info == null)
            {
                decorationLocalisation.Info = new LocalisationObject<LimitedDecorationBonusInfo>();
                if (!decorationLocalisation.Info.Has(Locale.English))
                {
                    decorationLocalisation.Info.Add(Locale.English, new LimitedDecorationBonusInfo
                    {
                        Text = new Dictionary<int, string>(),
                        Locale = Locale.English,
                        Parent = decorationLocalisation
                    });
                }
            }
            gameDataObject = decorationLocalisation;
        }
    }
}
