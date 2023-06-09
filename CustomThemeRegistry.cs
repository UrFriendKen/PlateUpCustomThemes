using CustomThemes.Customs;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomThemes
{
    public class CustomTheme
    {
        public readonly int ID;
        public readonly string Name;
        public readonly int MaxLevel;
        public readonly string Icon;

        private CustomLimitedDecorationLocalisation _customLocalisation;

        public LimitedDecorationLocalisation Localisation;

        public DecorationType DecorationType => GetDecorationType();
        public int CurrentProgress => GetCurrentProgress();
        public int CurrentLevel => GetCurrentLevel();


        public CustomTheme(int id, string name, int maxLevel, string icon, CustomLimitedDecorationLocalisation levelLocalisation)
        {
            ID = id;
            Name = name;
            MaxLevel = maxLevel;
            Icon = icon;
            _customLocalisation = levelLocalisation;
            Complete();
        }

        public static implicit operator DecorationType(CustomTheme customTheme)
        {
            return customTheme.DecorationType;
        }

        internal void Complete()
        {
            Localisation = _customLocalisation?.GameDataObject;
        }

        public string GetBonusText(int level)
        {
            return Localisation?[level] ?? "";
        }

        public DecorationType GetDecorationType()
        {
            return CustomThemeRegistry.GetDecorationType(this);
        }

        public int GetCurrentProgress()
        {
            return CustomThemeRegistry.GetDecorationProgress(DecorationType);
        }

        public int GetCurrentLevel()
        {
            return Mathf.Clamp(CurrentProgress / 3, 0, MaxLevel);
        }
    }

    public struct CustomDecorationBonusInfo
    {
        public int Level;
        public Dictionary<Locale, string> Texts;

        public string this[Locale l]
        {
            get
            {
                return Texts.TryGetValue(l, out string text) ? text : string.Empty;
            }
            set
            {
                Texts[l] = value;
            }
        }
    }

    public static class CustomThemeRegistry
    {
        private static Dictionary<int, CustomTheme> _registeredThemes = new Dictionary<int, CustomTheme>();
        private static Dictionary<string, CustomTheme> _registeredThemesByName = new Dictionary<string, CustomTheme>();

        private static Dictionary<int, HashSet<int>> _applianceRequiresThemes = new Dictionary<int, HashSet<int>>();

        private static GameData _gameData;
        internal static List<(CustomAppliance customAppliance, CustomTheme customTheme)> AwaitingCustomApplianceRequiresThemes = new List<(CustomAppliance customAppliance, CustomTheme customTheme)>();

        private static readonly HashSet<int> baseGameThemes = Enum.GetValues(typeof(DecorationType)).Cast<int>().ToHashSet();

        private static Dictionary<DecorationType, int> _currentProgress = new Dictionary<DecorationType, int>();
        internal static Dictionary<DecorationType, int> CurrentProgress
        {
            get
            {
                return new Dictionary<DecorationType, int>(_currentProgress);
            }
            set
            {
                Dictionary<DecorationType, int> copy = new Dictionary<DecorationType, int>(value);
                bool isChanged = false;
                if (_currentProgress.Count != copy.Count)
                {
                    Main.LogInfo("Count change");
                    isChanged = true;
                }
                else
                {
                    foreach (KeyValuePair<DecorationType, int> item in copy)
                    {
                        if (!_currentProgress.TryGetValue(item.Key, out int itemValue) || itemValue != item.Value)
                        {
                            Main.LogInfo("Value change");
                            isChanged = true;
                            break;
                        }
                    }
                }

                if (isChanged)
                {
                    _currentProgress = copy;
                }
                ProgressIsChanged = isChanged;
            }
        }
        internal static bool ProgressIsChanged { get; private set; } = false;

        internal static void Complete(GameData gameData)
        {
            _gameData = gameData;
            foreach (var pair in AwaitingCustomApplianceRequiresThemes)
            {
                AddCustomApplianceRequiresTheme(pair.customAppliance, pair.customTheme);
            }

            foreach (CustomTheme customTheme in _registeredThemes.Values)
            {
                RegisterDecorationIcon(customTheme);
                customTheme.Complete();
            }
        }

        internal static IEnumerable<CustomTheme> GetAllCustomThemes()
        {
            return _registeredThemes.Values;
        }

        private static void RegisterDecorationIcon(CustomTheme customTheme)
        {
            DecorationType customDecorationType = customTheme.DecorationType;
            if (_gameData == null || customTheme.Icon.IsNullOrEmpty() || _gameData.GlobalLocalisation.DecorationIcons.ContainsKey(customDecorationType))
                return;
            _gameData.GlobalLocalisation.DecorationIcons.Add(customDecorationType, customTheme.Icon);
        }

        public static CustomTheme RegisterTheme(string themeName, CustomLimitedDecorationLocalisation levelLocalisation, int maxLevel = 3, string iconSpriteName = null)
        {
            if (_registeredThemesByName.TryGetValue(themeName, out CustomTheme theme))
            {
                Main.LogWarning($"{themeName} already registered!");
                return theme;
            }
            int id = VariousUtils.GetID(themeName);
            if (baseGameThemes.Contains(id))
            {
                Main.LogError($"Failed to register {themeName} due to ID conflict with base game DecorationType {(DecorationType)id} ({id}). Consider changing custom theme name.");
                return null;
            }
            if (_registeredThemes.TryGetValue(id, out CustomTheme conflictingCustomTheme))
            {
                string conflictThemeName = conflictingCustomTheme.Name;
                Main.LogError($"Failed to register {themeName} due to ID conflict with another custom theme {(!conflictThemeName.IsNullOrEmpty() ? conflictThemeName : "\"\"")}. Consider changing custom theme name.");
                return null;
            }

            theme = new CustomTheme(id, themeName, maxLevel, iconSpriteName, levelLocalisation);
            _registeredThemes.Add(id, theme);
            _registeredThemesByName.Add(themeName, theme);
            RegisterDecorationIcon(theme);
            return theme;
        }

        public static bool TryGetTheme(string themeName, out CustomTheme theme, bool warn_if_fail = false)
        {
            if (!_registeredThemesByName.TryGetValue(themeName, out theme))
            {
                if (warn_if_fail)
                    Main.LogWarning($"{themeName} is not registered!");
                return false;
            }
            return true;
        }

        public static bool TryGetTheme(int themeID, out CustomTheme theme, bool warn_if_fail = false)
        {
            if (!_registeredThemes.TryGetValue(themeID, out theme))
            {
                if (warn_if_fail)
                    Main.LogWarning($"{themeID} is not registered!");
                return false;
            }
            return true;
        }

        public static void AddCustomApplianceRequiresTheme(CustomAppliance customAppliance, CustomTheme theme)
        {
            if (!_registeredThemes.ContainsKey(theme.ID))
            {
                Main.LogError($"{theme.ID} is not registered!");
                return;
            }

            if (_gameData == null)
            {
                AwaitingCustomApplianceRequiresThemes.Add((customAppliance, theme));
                return;
            }

            int applianceID = customAppliance?.GameDataObject?.ID ?? 0;
            if (applianceID == 0)
            {
                Main.LogError($"{customAppliance} has not been properly initialized! Could not retrieve Appliance GameDataObject.");
                return;
            }

            if (!_applianceRequiresThemes.ContainsKey(applianceID))
            {
                _applianceRequiresThemes.Add(applianceID, new HashSet<int>());
            }
            if (_applianceRequiresThemes[applianceID].Contains(theme.ID))
            {
                return;
            }
            _applianceRequiresThemes[applianceID].Add(theme.ID);
        }

        public static bool TryGetApplianceRequiresThemes(int applianceID, out HashSet<int> requiredThemes)
        {
            return _applianceRequiresThemes.TryGetValue(applianceID, out requiredThemes);
        }

        public static DecorationType GetDecorationType(CustomTheme customTheme)
        {
            return (DecorationType)customTheme.ID;
        }

        public static int GetDecorationProgress(DecorationType Type)
        {
            return CurrentProgress.TryGetValue(Type, out int value) ? value : 0;
        }

        public static int GetDecorationLevel(DecorationType Type)
        {
            return _registeredThemes.TryGetValue((int)Type, out CustomTheme customTheme) ? customTheme.CurrentLevel : 0;
        }
    }
}
