using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace CustomThemes
{
    public struct CCustomDecorationIndicator : IComponentData, IModComponent
    {
        public DecorationType DecorationType;
        public int Value;
    }
}
