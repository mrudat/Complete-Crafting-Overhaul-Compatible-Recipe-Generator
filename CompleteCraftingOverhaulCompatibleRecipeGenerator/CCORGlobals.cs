using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace CompleteCraftingOverhaulCompatibleRecipeGenerator
{
    public class CCORGlobals
    {
        private static ModKey master = Update.Global.Survival_ColdAttributePenaltyPercent.FormKey.ModKey;

        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Wood = new(new FormKey(master, 0xCC0150));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Bone = new(new FormKey(master, 0xCC0150));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Bonemold = new(new FormKey(master, 0xCC0158));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Stone = new(new FormKey(master, 0xCC0149));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Hide = new(new FormKey(master, 0xCC0151));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Leather = new(new FormKey(master, 0xCC0152));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Iron = new(new FormKey(master, 0xCC0153));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Steel = new(new FormKey(master, 0xCC0154));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Copper = new(new FormKey(master, 0xCC0454));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Dwarven = new(new FormKey(master, 0xCC0162));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Chitin = new(new FormKey(master, 0xCC0159));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Moonstone = new(new FormKey(master, 0xCC0161));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Quicksilver = new(new FormKey(master, 0xCC0457));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Calcinium = new(new FormKey(master, 0xCC0456));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Galatite = new(new FormKey(master, 0xCC0155));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Orichalcum = new(new FormKey(master, 0xCC0163));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Glass = new(new FormKey(master, 0xCC0164));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Stalhrim = new(new FormKey(master, 0xCC0166));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Silver = new(new FormKey(master, 0xCC0157));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Gold = new(new FormKey(master, 0xCC0453));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Ebony = new(new FormKey(master, 0xCC0165));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Dragon = new(new FormKey(master, 0xCC0167));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Daedric = new(new FormKey(master, 0xCC0168));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Arcane = new(new FormKey(master, 0xCC0183));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Cloth = new(new FormKey(master, 0xCC0197));
        public static readonly FormLink<IGlobalGetter> CCO_CategoryForgeMaterial_Misc = new(new FormKey(master, 0xCC0169));


    }
}
