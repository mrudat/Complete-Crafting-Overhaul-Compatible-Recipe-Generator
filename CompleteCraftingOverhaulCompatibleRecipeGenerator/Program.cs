using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompleteCraftingOverhaulCompatibleRecipeGenerator
{
    public class Program
    {
        private const string CCORGPrefix = "CCORGOverride_";

        private const string PrimaryMaterialCountOverrideKeywordPrefix = CCORGPrefix + "PrimaryMaterialCount_";
        private const string SecondaryMaterialCountOverrideKeywordPrefix = CCORGPrefix + "SecondaryMaterialCount_";
        private const string PrimaryMaterialOverrideKeywordPrefix = CCORGPrefix + "PrimaryMaterial_";
        private const string SecondaryMaterialOverrideKeywordPrefix = CCORGPrefix + "SecondaryMaterial_";
        private const string RecipeTypeOverrideKeywordPrefix = CCORGPrefix + "RecipeType_";
        private const string MaterialKeywordPrefix = CCORGPrefix + "CategoryForgeMaterial_";

        private readonly static FormLink<IMiscItemGetter> IngotGalatite = new(new FormKey(UpdateEsm, 0xCC0656));
        private readonly static FormLink<IMiscItemGetter> IngotCalcinium = new(new FormKey(UpdateEsm, 0xCC0654));

        private readonly static FormLink<IKeywordGetter> WAF_MaterialNordic = new(new FormKey(UpdateEsm, 0xAF0102));
        private readonly static FormLink<IKeywordGetter> WAF_MaterialChitin = new(new FormKey(UpdateEsm, 0xAF0133));

        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialAdvanced = new(new FormKey(UpdateEsm, 0xAF0119));
        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialDraugr = new(new FormKey(UpdateEsm, 0xAF0135));
        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialGuard = new(new FormKey(UpdateEsm, 0xAF0112));
        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialTGLinwe = new(new FormKey(UpdateEsm, 0xAF0100));
        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialTGSummerset = new(new FormKey(UpdateEsm, 0xAF0101));
        private readonly static FormLink<IKeywordGetter> WAF_ArmorMaterialThalmor = new(new FormKey(UpdateEsm, 0xAF0222));

        //private readonly static FormLink<IKeywordGetter> WAF_WeaponMaterialBlades = new(new FormKey(UpdateEsm, 0xAF0103));

        private readonly static Dictionary<IFormLinkGetter<IKeywordGetter>, (IFormLinkGetter<IItemGetter> Primary, IFormLinkGetter<IItemGetter> Secondary)> MaterialKeywordToMaterial = new()
        {
            { Dragonborn.Keyword.DLC2ArmorMaterialBonemoldHeavy, (Skyrim.Ingredient.BoneMeal, Skyrim.MiscItem.IngotIron) },
            { Dragonborn.Keyword.DLC2ArmorMaterialBonemoldLight, (Skyrim.Ingredient.BoneMeal, Skyrim.MiscItem.IngotIron) },
            { Dragonborn.Keyword.DLC2ArmorMaterialChitinHeavy, (Dragonborn.MiscItem.DLC2ChitinPlate, Dragonborn.MiscItem.DLC2NetchLeather) },
            { Dragonborn.Keyword.DLC2ArmorMaterialChitinLight, (Dragonborn.MiscItem.DLC2ChitinPlate, Dragonborn.MiscItem.DLC2NetchLeather) },
            { Dawnguard.Keyword.DLC1ArmorMaterialDawnguard, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.ingotSilver) },
            { Skyrim.Keyword.ArmorMaterialDaedric, (Skyrim.MiscItem.IngotEbony, Skyrim.Ingredient.DaedraHeart) },
            { Skyrim.Keyword.WeapMaterialDaedric, (Skyrim.MiscItem.IngotEbony, Skyrim.Ingredient.DaedraHeart) },
            { Skyrim.Keyword.ArmorMaterialDragonplate, (Skyrim.MiscItem.DragonBone, Skyrim.MiscItem.IngotEbony) },
            { Skyrim.Keyword.ArmorMaterialDragonscale, (Skyrim.MiscItem.DragonScales, Skyrim.MiscItem.DragonBone) },
            { Dawnguard.Keyword.DLC1WeapMaterialDragonbone, (Skyrim.MiscItem.DragonBone, Skyrim.MiscItem.IngotEbony) },
            { Skyrim.Keyword.WeapMaterialDraugr, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Firewood01) },
            { WAF_ArmorMaterialDraugr, (Skyrim.MiscItem.IngotIron, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialDraugrHoned, (Skyrim.MiscItem.IngotCorundum, Skyrim.MiscItem.IngotSteel) },
            { Skyrim.Keyword.ArmorMaterialDwarven, (Skyrim.MiscItem.IngotDwarven, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialDwarven, (Skyrim.MiscItem.IngotDwarven, Skyrim.MiscItem.IngotSteel) },
            { Skyrim.Keyword.ArmorMaterialEbony, (Skyrim.MiscItem.IngotEbony, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.WeapMaterialEbony, (Skyrim.MiscItem.IngotEbony, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.ArmorMaterialElven, (Skyrim.MiscItem.IngotIMoonstone, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.WeapMaterialElven, (Skyrim.MiscItem.IngotIMoonstone, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.ArmorMaterialElvenGilded, (Skyrim.MiscItem.IngotIMoonstone, Skyrim.MiscItem.IngotQuicksilver) },
            { Update.Keyword.ArmorMaterialFalmer, (Skyrim.MiscItem.ChaurusChitin, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialFalmer, (Skyrim.MiscItem.ChaurusChitin, Skyrim.MiscItem.IngotIron) },
            { Dawnguard.Keyword.DLC1ArmorMaterialFalmerHardened, (Skyrim.MiscItem.ChaurusChitin, Skyrim.MiscItem.ChaurusChitin) },
            { Dawnguard.Keyword.DLC1ArmorMaterielFalmerHeavy, (Dawnguard.MiscItem.DLC1ShellbugChitin, Skyrim.MiscItem.IngotEbony) },
            { Dawnguard.Keyword.DLC1ArmorMaterielFalmerHeavyOriginal, (Dawnguard.MiscItem.DLC1ShellbugChitin, Skyrim.MiscItem.IngotEbony) },
            { Skyrim.Keyword.WeapMaterialFalmerHoned, (Skyrim.MiscItem.ChaurusChitin, Skyrim.MiscItem.IngotIron) },
            { Skyrim.Keyword.ArmorMaterialGlass, (Skyrim.MiscItem.IngotMalachite, Skyrim.MiscItem.IngotIMoonstone) },
            { Skyrim.Keyword.WeapMaterialGlass, (Skyrim.MiscItem.IngotMalachite, Skyrim.MiscItem.IngotIMoonstone) },
            { Skyrim.Keyword.ArmorMaterialHide, (Skyrim.MiscItem.Leather01, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialImperial, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.IngotIron) },
            { Skyrim.Keyword.ArmorMaterialImperialHeavy, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialImperialLight, (Skyrim.MiscItem.Leather01, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialImperialStudded, (Skyrim.MiscItem.Leather01, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialIron, (Skyrim.MiscItem.IngotIron, Skyrim.MiscItem.IngotIron) },
            { Skyrim.Keyword.WeapMaterialIron, (Skyrim.MiscItem.IngotIron, Skyrim.MiscItem.IngotIron) },
            { Skyrim.Keyword.ArmorMaterialIronBanded, (Skyrim.MiscItem.IngotIron, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialLeather, (Skyrim.MiscItem.Leather01, Skyrim.MiscItem.Leather01) },
            { Dragonborn.Keyword.DLC2ArmorMaterialMoragTong, (Dragonborn.MiscItem.DLC2ChitinPlate, Skyrim.MiscItem.Leather01) },
            { Dragonborn.Keyword.DLC2ArmorMaterialNordicHeavy, (Skyrim.MiscItem.IngotOrichalcum, Skyrim.MiscItem.IngotQuicksilver) },
            { Dragonborn.Keyword.DLC2ArmorMaterialNordicLight, (Skyrim.MiscItem.IngotOrichalcum, Skyrim.MiscItem.IngotQuicksilver) },
            { Dragonborn.Keyword.DLC2WeaponMaterialNordic, (Skyrim.MiscItem.IngotOrichalcum, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.ArmorMaterialOrcish, (Skyrim.MiscItem.IngotOrichalcum, Skyrim.MiscItem.IngotSteel) },
            { Skyrim.Keyword.WeapMaterialOrcish, (Skyrim.MiscItem.IngotOrichalcum, Skyrim.MiscItem.IngotSteel) },
            { Skyrim.Keyword.ArmorMaterialScaled, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialSilver, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.ingotSilver) },
            { Dragonborn.Keyword.DLC2ArmorMaterialStalhrimHeavy, (Dragonborn.MiscItem.DLC2OreStalhrim, Skyrim.MiscItem.IngotQuicksilver) },
            { Dragonborn.Keyword.DLC2ArmorMaterialStalhrimLight, (Dragonborn.MiscItem.DLC2OreStalhrim, Skyrim.MiscItem.IngotQuicksilver) },
            { Dragonborn.Keyword.DLC2WeaponMaterialStalhrim, (Dragonborn.MiscItem.DLC2OreStalhrim, Skyrim.MiscItem.IngotQuicksilver) },
            { Skyrim.Keyword.ArmorMaterialSteel, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.WeapMaterialSteel, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.IngotSteel) },
            { Skyrim.Keyword.ArmorMaterialSteelPlate, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialStormcloak, (Skyrim.MiscItem.IngotSteel, Skyrim.MiscItem.Leather01) },
            { Skyrim.Keyword.ArmorMaterialStudded, (Skyrim.MiscItem.Leather01, Skyrim.MiscItem.IngotIron) },
            { Skyrim.Keyword.WeapMaterialWood, (Skyrim.MiscItem.Firewood01, Skyrim.MiscItem.IngotIron) },
        };

        private enum RecipeType
        {
            Gauntlets,
            Boots,
            Helmet,
            Cuirass,
            Shield,
            //Weapons
            Dagger,
            Sword,
            WarAxe,
            Mace,
            Greatsword,
            Battleaxe,
            Warhammer,
            Bow,
            Staff,
            // don't touch the recipes
            Custom
        }

        private static readonly Dictionary<RecipeType, (ushort primaryMaterialCount, ushort secondaryMaterialCount, ushort leatherCount, ushort leatherStripCount)> ArmorRecipeToMaterialCount = new()
        {
            { RecipeType.Gauntlets, (2, 1, 1, 2) },
            { RecipeType.Boots, (3, 1, 1, 2) },
            { RecipeType.Helmet, (3, 1, 1, 1) },
            { RecipeType.Cuirass, (5, 2, 2, 4) },
            { RecipeType.Shield, (4, 1, 0, 1) },
        };

        private static readonly Dictionary<RecipeType, (ushort primaryMaterialCount, ushort secondaryMaterialCount, ushort leatherStripCount)> WeaponRecipeToMaterialCount = new()
        {
            { RecipeType.Dagger, (1, 1, 1) },
            { RecipeType.Sword, (2, 1, 1) },
            { RecipeType.WarAxe, (2, 1, 2) },
            { RecipeType.Mace, (3, 1, 2) },
            { RecipeType.Greatsword, (3, 1, 2) },
            { RecipeType.Battleaxe, (3, 2, 3) },
            { RecipeType.Warhammer, (4, 2, 3) },
            { RecipeType.Bow, (3, 1, 2) },
            { RecipeType.Staff, (3, 1, 0) },
        };

        // Note: sorted in precedence order.
        private static readonly List<(BipedObjectFlag, RecipeType)> BipedFlagsToArmorRecipeTyoe = new()
        {
            (BipedObjectFlag.Body, RecipeType.Cuirass),
            (BipedObjectFlag.Hands, RecipeType.Gauntlets),
            (BipedObjectFlag.Feet, RecipeType.Boots),
            (BipedObjectFlag.Shield, RecipeType.Shield),
            (BipedObjectFlag.Circlet, RecipeType.Helmet)
        };

        private static readonly Dictionary<IFormLinkGetter<IKeywordGetter>, RecipeType> WeaponTypeKeywordToRecipeType = new()
        {
            { Skyrim.Keyword.WeapTypeBattleaxe, RecipeType.Battleaxe },
            { Skyrim.Keyword.WeapTypeBow, RecipeType.Bow },
            { Skyrim.Keyword.WeapTypeDagger, RecipeType.Dagger },
            { Skyrim.Keyword.WeapTypeGreatsword, RecipeType.Greatsword },
            { Skyrim.Keyword.WeapTypeMace, RecipeType.Mace },
            { Skyrim.Keyword.WeapTypeStaff, RecipeType.Staff },
            { Skyrim.Keyword.WeapTypeSword, RecipeType.Sword },
            { Skyrim.Keyword.WeapTypeWarAxe, RecipeType.WarAxe },
            { Skyrim.Keyword.WeapTypeWarhammer, RecipeType.Warhammer },
        };

        private static readonly Dictionary<IFormLinkGetter<IItemGetter>, string> PrimaryMaterialToCCOMaterialGlobal = new()
        {
            { Skyrim.MiscItem.Firewood01, "Wood" },
            // TODO Bone? { Skyrim.MiscItem.DragonBone, "Bone" },
            { Skyrim.Ingredient.BoneMeal, "Bonemold" },
            { HearthFires.MiscItem.BYOHMaterialStoneBlock, "Stone" },
            // TODO Hide
            { Skyrim.MiscItem.Leather01, "Leather" },
            { Skyrim.MiscItem.IngotIron, "Iron" },
            { Skyrim.MiscItem.IngotSteel, "Steel" },
            { Skyrim.MiscItem.IngotCorundum, "Copper" },
            { Skyrim.MiscItem.IngotDwarven, "Dwarven" },
            { Skyrim.MiscItem.ChaurusChitin, "Chitin" },
            { Skyrim.MiscItem.IngotIMoonstone, "Moonstone" },
            { Skyrim.MiscItem.IngotQuicksilver, "Quicksilver" },
            { IngotCalcinium, "Calcinium" },
            { IngotGalatite, "Galatite" },
            { Skyrim.MiscItem.IngotOrichalcum, "Orichalcum" },
            { Skyrim.MiscItem.IngotMalachite, "Glass" },
            { Dragonborn.MiscItem.DLC2OreStalhrim, "Stalhrim" },
            { Skyrim.MiscItem.ingotSilver, "Silver" },
            { Skyrim.MiscItem.IngotGold, "Gold" },
            { Skyrim.MiscItem.IngotEbony, "Ebony" },
            { Skyrim.MiscItem.DragonBone, "Dragon" },
            { Skyrim.MiscItem.DragonScales, "Dragon" },
            { Skyrim.Ingredient.DaedraHeart, "Daedric" },
            // TODO Arcane
            { Skyrim.MiscItem.RuinsLinenPile01, "Cloth" },
        };

        private static readonly ModKey CompleteCraftingOverhaulRemasteredEsp = ModKey.FromNameAndExtension("Complete Crafting Overhaul_Remastered.esp");

        private static readonly HashSet<IFormLinkGetter<IItemGetter>> NeedsForge = new()
        {
            Skyrim.MiscItem.IngotIron,
            Skyrim.MiscItem.IngotCorundum,
            Skyrim.MiscItem.IngotDwarven,
            Skyrim.MiscItem.IngotEbony,
            Skyrim.MiscItem.IngotGold,
            Skyrim.MiscItem.IngotIMoonstone,
            Skyrim.MiscItem.IngotMalachite,
            Skyrim.MiscItem.IngotOrichalcum,
            Skyrim.MiscItem.IngotQuicksilver,
            Skyrim.MiscItem.IngotSteel,
            Skyrim.MiscItem.ingotSilver,
            Dragonborn.MiscItem.DLC2OreStalhrim,
            HearthFires.MiscItem.BYOHMaterialStoneBlock,
            Skyrim.MiscItem.Firewood01,
            Skyrim.Ingredient.BoneMeal,
            IngotGalatite,
            IngotCalcinium
        };

        private static readonly Dictionary<IFormLinkGetter<IItemGetter>, IFormLinkGetter<IPerkGetter>> materialToCraftingPerk = new()
        {
            { Skyrim.MiscItem.IngotSteel, Skyrim.Perk.SteelSmithing },
            { Skyrim.MiscItem.IngotDwarven, Skyrim.Perk.DwarvenSmithing },
            { Skyrim.MiscItem.IngotIMoonstone, Skyrim.Perk.ElvenSmithing },
            { Skyrim.MiscItem.IngotOrichalcum, Skyrim.Perk.OrcishSmithing },
            { Skyrim.MiscItem.IngotMalachite, Skyrim.Perk.GlassSmithing },
            { Skyrim.MiscItem.IngotQuicksilver, Skyrim.Perk.AdvancedArmors },
            { Skyrim.MiscItem.IngotEbony, Skyrim.Perk.EbonySmithing },
            { Skyrim.Ingredient.DaedraHeart, Skyrim.Perk.DaedricSmithing },
            { Skyrim.Ingredient.BoneMeal, Dragonborn.Perk.DLC2Smithing },
            { Skyrim.MiscItem.DragonBone, Skyrim.Perk.DragonArmor },
            { Skyrim.MiscItem.DragonScales, Skyrim.Perk.DragonArmor },
        };

        private static readonly HashSet<IFormLinkGetter<IItemGetter>> clothingMaterials = new()
        {
            Skyrim.MiscItem.RuinsLinenPile01,
            Skyrim.MiscItem.Leather01,
            Skyrim.Ingredient.TundraCotton,
            Skyrim.MiscItem.LeatherStrips
        };

        private static readonly HashSet<IFormLink<IKeywordGetter>> jewelryKeywords = new()
        {
            Skyrim.Keyword.JewelryExpensive,
            Skyrim.Keyword.ArmorJewelry,
            Skyrim.Keyword.VendorItemJewelry
        };

        private static readonly ModKey UpdateEsm = ModKey.FromNameAndExtension("Update.esm");

        private static Lazy<Settings> settings = new();

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .SetAutogeneratedSettings("Settings", "settings.json", out settings)
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (state.LoadOrder.PriorityOrder.HasMod(CompleteCraftingOverhaulRemasteredEsp, true))
            {
                MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2WeaponMaterialStalhrim] = MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2ArmorMaterialStalhrimHeavy] = MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2ArmorMaterialStalhrimLight] = (Dragonborn.MiscItem.DLC2OreStalhrim, IngotGalatite);

                MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2ArmorMaterialNordicHeavy] = MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2ArmorMaterialNordicLight] = MaterialKeywordToMaterial[Dragonborn.Keyword.DLC2WeaponMaterialNordic] = (IngotGalatite, Skyrim.MiscItem.IngotOrichalcum);

                MaterialKeywordToMaterial[Skyrim.Keyword.ArmorMaterialElven] = MaterialKeywordToMaterial[Skyrim.Keyword.WeapMaterialElven] = (IngotCalcinium, Skyrim.MiscItem.IngotQuicksilver);

                MaterialKeywordToMaterial[Skyrim.Keyword.ArmorMaterialGlass] = MaterialKeywordToMaterial[Skyrim.Keyword.WeapMaterialGlass] = (Skyrim.MiscItem.IngotMalachite, IngotCalcinium);
            }

            var armorLookup = state.LoadOrder.PriorityOrder.Armor()
                .WinningOverrides()
                .Where(x => x.EditorID is not null)
                .Where(x => x.BodyTemplate is not null)
                .Where(x => x.Race.FormKey == Skyrim.Race.DefaultRace.FormKey)
                .Where(x => !x.BodyTemplate!.Flags.HasFlag(BodyTemplate.Flag.NonPlayable))
                .Where(x => x.Keywords is not null)
                .Where(x => !x.Keywords!.Any(x => jewelryKeywords.Contains(x)))
                .Where(x => x.ObjectEffect.IsNull)
                .ToDictionary(x => x.AsLinkGetter<IConstructibleGetter>());

            var weaponLookup = state.LoadOrder.PriorityOrder.Weapon()
                .WinningOverrides()
                .Where(x => x.EditorID is not null)
                .Where(x => x.Keywords is not null)
                .Where(x => x.Data is not null)
                .Where(x => x.ObjectEffect.IsNull)
                .Where(x => !x.Data!.Flags.HasFlag(WeaponData.Flag.NonPlayable))
                .Where(x => !x.Data!.Flags.HasFlag(WeaponData.Flag.CantDrop))
                .Where(x => !x.Keywords!.Contains(Skyrim.Keyword.Dummy))
                .ToDictionary(x => x.AsLinkGetter<IConstructibleGetter>());

            Dictionary<IFormLinkGetter<IKeywordGetter>, string>? overrideKeywordLookup = new();

            var costumeKeyword = new HashSet<IFormLinkGetter<IKeywordGetter>>();

            foreach (var keyword in state.LoadOrder.PriorityOrder.Keyword().WinningOverrides())
            {
                if (keyword.EditorID is null) continue;
                if (keyword.EditorID.StartsWith(CCORGPrefix))
                    overrideKeywordLookup.Add(keyword.AsLink(), keyword.EditorID);
                if (keyword.EditorID == "CostumeShop_CostumeKeyword")
                    costumeKeyword.Add(keyword.AsLink());
            }

            var ccoGlobalLookup = state.LoadOrder.PriorityOrder.Global()
                .WinningOverrides()
                .Where(x => x.EditorID?.StartsWith("CCO_") == true)
                .GroupBy(x => x.EditorID!)
                .ToDictionary(x => x.Key, x => x.First().AsLinkGetter());

            Dictionary<IFormLinkGetter<IItemGetter>, (float weight, uint value)> materialsWeightsAndValues = new();
            Dictionary<string, IFormLinkGetter<IItemGetter>> materialLookupByEditorID = new();

            foreach (var miscItem in state.LoadOrder.PriorityOrder.IItem().WinningOverrides())
            {
                var miscItemLink = miscItem.AsLink();

                if (miscItem.EditorID is not null)
                    materialLookupByEditorID[miscItem.EditorID] = miscItemLink;

                if (miscItem is IWeightValueGetter hasMass)
                    materialsWeightsAndValues[miscItemLink] = (hasMass.Weight, hasMass.Value);
            }

            var materialsByValueDensity = (from item in materialsWeightsAndValues
                                           let valueDensity = item.Value.value / item.Value.weight
                                           orderby valueDensity, item.Value.weight
                                           select (item.Key, valueDensity)).ToList();

            var armorCraftingRecipeLookup = new Dictionary<IFormLinkGetter<IConstructibleGetter>, IConstructibleObjectGetter>();
            var armorTemperingRecipeLookup = new Dictionary<IFormLinkGetter<IConstructibleGetter>, IConstructibleObjectGetter>();
            var weaponCraftingRecipeLookup = new Dictionary<IFormLinkGetter<IConstructibleGetter>, IConstructibleObjectGetter>();
            var weaponTemperingRecipeLookup = new Dictionary<IFormLinkGetter<IConstructibleGetter>, IConstructibleObjectGetter>();

            // TODO make this a setting
            bool replaceRecipes = settings.Value.ReplaceAllRecipes;

            foreach (var recipe in state.LoadOrder.PriorityOrder.ConstructibleObject()
                .WinningOverrides())
            {
                if (recipe.WorkbenchKeyword.IsNull) continue;
                if (recipe.CreatedObject.IsNull) continue;
                if ((recipe.CreatedObjectCount ?? 1) != 1) continue;

                if (recipe.WorkbenchKeyword.Equals(Skyrim.Keyword.CraftingSmithingForge)
                    || recipe.WorkbenchKeyword.Equals(Skyrim.Keyword.CraftingTanningRack)
                    || recipe.WorkbenchKeyword.Equals(Skyrim.Keyword.CraftingSmelter))
                {
                    if (armorLookup.ContainsKey(recipe.CreatedObject))
                    {
                        if (!armorCraftingRecipeLookup.ContainsKey(recipe.CreatedObject))
                            armorCraftingRecipeLookup.Add(recipe.CreatedObject, recipe);
                        else
                        {
                            if (replaceRecipes)
                                Delete(state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(recipe));
                        }
                    }
                    else if (weaponLookup.ContainsKey(recipe.CreatedObject))
                    {
                        if (!weaponCraftingRecipeLookup.ContainsKey(recipe.CreatedObject))
                            weaponCraftingRecipeLookup.Add(recipe.CreatedObject, recipe);
                        else
                        {
                            if (replaceRecipes)
                                Delete(state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(recipe));
                        }
                    }
                }
                else if (recipe.WorkbenchKeyword.Equals(Skyrim.Keyword.CraftingSmithingArmorTable))
                {
                    if (!armorTemperingRecipeLookup.ContainsKey(recipe.CreatedObject))
                        armorTemperingRecipeLookup.Add(recipe.CreatedObject, recipe);
                    else
                    {
                        if (replaceRecipes)
                            Delete(state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(recipe));
                    }
                }
                else if (recipe.WorkbenchKeyword.Equals(Skyrim.Keyword.CraftingSmithingSharpeningWheel))
                {
                    if (!weaponTemperingRecipeLookup.ContainsKey(recipe.CreatedObject))
                        weaponTemperingRecipeLookup.Add(recipe.CreatedObject, recipe);
                    else
                    {
                        if (replaceRecipes)
                            Delete(state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(recipe));
                    }
                }
            }

            Dictionary<IFormLinkGetter<IItemGetter>, Noggog.ExtendedList<ContainerEntry>> temperItemsDict = new();

            Lazy<Noggog.ExtendedList<Condition>> temperRecipeConditions = new(
                () => new Noggog.ExtendedList<Condition>
                {
                    new ConditionFloat()
                    {
                        CompareOperator = CompareOperator.NotEqualTo,
                        Flags = Condition.Flag.OR,
                        ComparisonValue = 1,
                        Data = new FunctionConditionData()
                        {
                            Function = Condition.Function.EPTemperingItemIsEnchanted,
                            RunOnType = Condition.RunOnType.Subject
                        }
                    },
                    new ConditionFloat()
                    {
                        CompareOperator = CompareOperator.EqualTo,
                        ComparisonValue = 1,
                        Data = new FunctionConditionData()
                        {
                            Function = Condition.Function.HasPerk,
                            ParameterOneRecord = Skyrim.Perk.ArcaneBlacksmith,
                            RunOnType = Condition.RunOnType.Subject
                        }
                    }
                }
            );

            var recipes = state.PatchMod.ConstructibleObjects;

            foreach (var armorEntry in armorLookup)
            {
                var armorLink = armorEntry.Key;
                var armor = armorEntry.Value;
                var armorEditorID = armor.EditorID!;

                var armorType = armor.BodyTemplate!.ArmorType;

                if (armorType == ArmorType.Clothing)
                {
                    armorCraftingRecipeLookup.TryGetValue(armorLink, out var recipe);
                    IConstructibleObject? newRecipe = null;
                    if (recipe is null)
                        newRecipe = recipes.AddNew("Recipe" + armorEditorID);
                    else if (replaceRecipes)
                        newRecipe = recipes.GetOrAddAsOverride(recipe);

                    if (newRecipe is null)
                        continue;

                    newRecipe.CreatedObject.SetTo(armor);
                    newRecipe.CreatedObjectCount = 1;
                    newRecipe.WorkbenchKeyword.SetTo(Skyrim.Keyword.CraftingTanningRack);

                    // you can't temper clothes; delete temper recipe if found.
                    if (armorTemperingRecipeLookup.TryGetValue(armorLink, out var temperRecipe))
                        Delete(state.PatchMod.ConstructibleObjects.GetOrAddAsOverride(temperRecipe));

                    var itemWeight = armor.Weight;

                    // minimum 10% value for crafting.
                    var itemValue = (uint)Math.Ceiling(armor.Value * 0.9);

                    var items = newRecipe.Items ??= new();

                    items.Clear();

                    if (costumeKeyword.Count > 0 && armor.Keywords!.Any(k => costumeKeyword.Contains(k)))
                    {
                        IFormLinkGetter<IItemGetter>? primaryMaterial = null;

                        foreach (var keyword in armor.Keywords!)
                            if (primaryMaterial is null && MaterialKeywordToMaterial.TryGetValue(keyword, out var materialData))
                                (primaryMaterial, _) = materialData;

                        primaryMaterial ??= Skyrim.MiscItem.IngotIron;

                        var (weight, value) = materialsWeightsAndValues[primaryMaterial];

                        itemWeight -= weight;
                        itemValue -= value;

                        if (itemWeight <= 0) itemWeight = 0.01f;
                        if (itemValue <= 0) itemValue = 1;

                        items.Add(new()
                        {
                            Item = new()
                            {
                                Item = primaryMaterial.AsSetter(),
                                Count = 1
                            }
                        });
                    }

                    float valueDensity = itemValue / itemWeight;

                    // the most value dense ingredient that has less value density than the finished article, logically additional value comes from the act of crafting.
                    var ingredient = materialsByValueDensity.FindAll(i => i.valueDensity <= valueDensity && clothingMaterials.Contains(i.Key)).Select(i => i.Key).FirstOrDefault() ?? Skyrim.MiscItem.RuinsLinenPile01;

                    /*
                    foreach (var candidate in materialsByValueDensity)
                    {
                        if (candidate.valueDensity <= valueDensity)
                            if (clothingMaterials.Contains(candidate.Key))
                            {
                                ingredient = candidate.Key;
                                break;
                            }
                    }
                    */

                    // ingredient weight >= final item weight (the extra is wasted material)
                    var count = (int)Math.Ceiling(itemWeight / materialsWeightsAndValues[ingredient].weight);

                    if (count <= 0)
                        count = 1;

                    items.Add(new()
                    {
                        Item = new()
                        {
                            Item = ingredient.AsSetter(),
                            Count = count
                        }
                    });
                }
                else
                {
                    IFormLinkGetter<IItemGetter>? primaryMaterial = null;
                    IFormLinkGetter<IItemGetter>? secondaryMaterial = null;

                    ushort? primaryMaterialCount = null;
                    ushort? secondaryMaterialCount = null;
                    ushort? leatherCount = null;
                    ushort? leatherStripCount = null;

                    RecipeType? recipeType = null;

                    ReadOverridesFromKeywords(overrideKeywordLookup,
                                              materialLookupByEditorID,
                                              armor.Keywords!,
                                              ref primaryMaterial,
                                              ref secondaryMaterial,
                                              ref primaryMaterialCount,
                                              ref secondaryMaterialCount,
                                              ref recipeType);

                    foreach (var keyword in armor.Keywords!)
                        if (primaryMaterial is null && MaterialKeywordToMaterial.TryGetValue(keyword, out var materialData))
                            (primaryMaterial, secondaryMaterial) = materialData;

                    primaryMaterial ??= Skyrim.MiscItem.IngotIron;
                    secondaryMaterial ??= Skyrim.MiscItem.IngotIron;

                    if (recipeType is null || !ArmorRecipeToMaterialCount.ContainsKey(recipeType.Value))
                    {
                        var firstPersonFlags = armor.BodyTemplate.FirstPersonFlags;

                        foreach (var item in BipedFlagsToArmorRecipeTyoe)
                        {
                            if ((firstPersonFlags & item.Item1) == item.Item1)
                            {
                                recipeType = item.Item2;
                                break;
                            }
                        }
                    }

                    recipeType ??= RecipeType.Gauntlets;

                    if (recipeType == RecipeType.Custom) continue;

                    var materialCounts = ArmorRecipeToMaterialCount[recipeType.Value];

                    primaryMaterialCount ??= materialCounts.primaryMaterialCount;
                    secondaryMaterialCount ??= materialCounts.secondaryMaterialCount;
                    leatherCount ??= materialCounts.leatherCount;
                    leatherStripCount ??= materialCounts.leatherStripCount;

                    if (secondaryMaterial.Equals(Skyrim.Ingredient.DaedraHeart))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        primaryMaterialCount += (ushort)(secondaryMaterialCount - 1);
                        secondaryMaterialCount = 1;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                    }
                    if (primaryMaterial.Equals(secondaryMaterial))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        primaryMaterialCount += secondaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        secondaryMaterialCount = 0;
                    }
                    if (primaryMaterial.Equals(Skyrim.MiscItem.Leather01))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        leatherCount += primaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        primaryMaterialCount = 0;
                    }
                    if (primaryMaterial.Equals(Skyrim.MiscItem.LeatherStrips))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        leatherStripCount += primaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        primaryMaterialCount = 0;
                    }
                    if (secondaryMaterial.Equals(Skyrim.MiscItem.Leather01))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        leatherCount += secondaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        secondaryMaterialCount = 0;
                    }
                    if (secondaryMaterial.Equals(Skyrim.MiscItem.LeatherStrips))
                    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        leatherStripCount += secondaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        secondaryMaterialCount = 0;
                    }

                    armorCraftingRecipeLookup.TryGetValue(armorLink, out var recipe);
                    ConstructibleObject? newRecipe = null;
                    if (recipe is null)
                        newRecipe = recipes.AddNew("Recipe" + armorEditorID);
                    else if (replaceRecipes)
                        newRecipe = recipes.GetOrAddAsOverride(recipe);

                    if (newRecipe is not null)
                    {
                        newRecipe.CreatedObject.SetTo(armor);
                        newRecipe.CreatedObjectCount = 1;
                        bool needsForge = false;
                        if (primaryMaterialCount > 0 && NeedsForge.Contains(primaryMaterial))
                            needsForge = true;
                        if (secondaryMaterialCount > 0 && NeedsForge.Contains(secondaryMaterial))
                            needsForge = true;
                        if (needsForge)
                            newRecipe.WorkbenchKeyword.SetTo(Skyrim.Keyword.CraftingSmithingForge);
                        else
                            newRecipe.WorkbenchKeyword.SetTo(Skyrim.Keyword.CraftingTanningRack);

                        newRecipe.Items = new();
                        if (primaryMaterialCount > 0)
                        {
                            newRecipe.Items.Add(new()
                            {
                                Item = new()
                                {
                                    Item = primaryMaterial.AsSetter(),
                                    Count = primaryMaterialCount.Value
                                }
                            });
                        }
                        if (secondaryMaterialCount > 0)
                        {
                            newRecipe.Items.Add(new()
                            {
                                Item = new()
                                {
                                    Item = secondaryMaterial.AsSetter(),
                                    Count = secondaryMaterialCount.Value
                                }
                            });
                        }
                        if (leatherCount > 0)
                        {
                            newRecipe.Items.Add(new()
                            {
                                Item = new()
                                {
                                    Item = Skyrim.MiscItem.Leather01,
                                    Count = leatherCount.Value
                                }
                            });
                        }
                        if (leatherStripCount > 0)
                        {
                            newRecipe.Items.Add(new()
                            {
                                Item = new()
                                {
                                    Item = Skyrim.MiscItem.LeatherStrips,
                                    Count = leatherStripCount.Value
                                }
                            });
                        }

                        if (ccoGlobalLookup.Count > 0)
                        {
                            //newRecipe.Conditions.Clear();

                            var globalSuffix = "Misc";
                            if (PrimaryMaterialToCCOMaterialGlobal.TryGetValue(primaryMaterial, out var temp))
                                globalSuffix = temp;
                            ccoGlobalLookup.TryGetValue("CCO_CategoryForgeMaterial_" + globalSuffix, out var globalLink);

                            // TODO conditions.
                        }
                    }

                    MakeTemperRecipe(armorTemperingRecipeLookup,
                                        temperItemsDict,
                                        temperRecipeConditions,
                                        replaceRecipes,
                                        recipes,
                                        Skyrim.Keyword.CraftingSmithingArmorTable,
                                        primaryMaterial,
                                        armorLink,
                                        armorEditorID);
                }

            }

            foreach (var weaponEntry in weaponLookup)
            {
                var weaponLink = weaponEntry.Key;
                var weapon = weaponEntry.Value;
                var weaponEditorID = weapon.EditorID!;

                IFormLinkGetter<IItemGetter>? primaryMaterial = null;
                IFormLinkGetter<IItemGetter>? secondaryMaterial = null;

                ushort? primaryMaterialCount = null;
                ushort? secondaryMaterialCount = null;
                ushort? leatherStripCount = null;

                RecipeType? recipeType = null;

                ReadOverridesFromKeywords(overrideKeywordLookup,
                                          materialLookupByEditorID,
                                          weapon.Keywords!,
                                          ref primaryMaterial,
                                          ref secondaryMaterial,
                                          ref primaryMaterialCount,
                                          ref secondaryMaterialCount,
                                          ref recipeType);

                foreach (var keyword in weapon.Keywords!)
                {
                    // TODO weapon-specific keywords?
                    if (primaryMaterial is null && MaterialKeywordToMaterial.TryGetValue(keyword, out var materialData))
                        (primaryMaterial, secondaryMaterial) = materialData;
                    if (recipeType is null && WeaponTypeKeywordToRecipeType.TryGetValue(keyword, out var temp))
                        recipeType = temp;
                }

                recipeType ??= (weapon.Data?.AnimationType) switch
                {
                    WeaponAnimationType.OneHandSword => RecipeType.Sword,
                    WeaponAnimationType.OneHandDagger => RecipeType.Dagger,
                    WeaponAnimationType.OneHandAxe => RecipeType.WarAxe,
                    WeaponAnimationType.OneHandMace => RecipeType.Mace,
                    WeaponAnimationType.TwoHandSword => RecipeType.Greatsword,
                    WeaponAnimationType.TwoHandAxe => RecipeType.Battleaxe,
                    WeaponAnimationType.Bow => RecipeType.Bow,
                    WeaponAnimationType.Staff => RecipeType.Staff,
                    WeaponAnimationType.Crossbow => RecipeType.Bow,
                    _ => RecipeType.Dagger,
                };

                primaryMaterial ??= Skyrim.MiscItem.IngotIron;
                secondaryMaterial ??= Skyrim.MiscItem.IngotIron;

                if (recipeType == RecipeType.Custom) continue;

                if (recipeType == RecipeType.Staff)
                {
                    secondaryMaterial = primaryMaterial;
                    primaryMaterial = Skyrim.MiscItem.Firewood01;
                }

                var materialCounts = WeaponRecipeToMaterialCount[recipeType.Value];

                primaryMaterialCount ??= materialCounts.primaryMaterialCount;
                secondaryMaterialCount ??= materialCounts.secondaryMaterialCount;
                leatherStripCount ??= materialCounts.leatherStripCount;

                if (secondaryMaterial.Equals(Skyrim.Ingredient.DaedraHeart))
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    primaryMaterialCount += (ushort)(secondaryMaterialCount - 1);
                    secondaryMaterialCount = 1;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                }
                if (primaryMaterial.Equals(secondaryMaterial))
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    primaryMaterialCount += secondaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                    secondaryMaterialCount = 0;
                }
                if (primaryMaterial.Equals(Skyrim.MiscItem.LeatherStrips))
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    leatherStripCount += primaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                    primaryMaterialCount = 0;
                }
                if (secondaryMaterial.Equals(Skyrim.MiscItem.LeatherStrips))
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    leatherStripCount += secondaryMaterialCount;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                    secondaryMaterialCount = 0;
                }

                weaponCraftingRecipeLookup.TryGetValue(weaponLink, out var recipe);
                ConstructibleObject? newRecipe = null;
                if (recipe is null)
                    newRecipe = recipes.AddNew("Recipe" + weaponEditorID);
                else if (replaceRecipes)
                    newRecipe = recipes.GetOrAddAsOverride(recipe);

                if (newRecipe is not null)
                {
                    newRecipe.CreatedObject.SetTo(weapon);
                    newRecipe.CreatedObjectCount = 1;
                    bool needsForge = false;
                    if (primaryMaterialCount > 0 && NeedsForge.Contains(primaryMaterial))
                        needsForge = true;
                    if (secondaryMaterialCount > 0 && NeedsForge.Contains(secondaryMaterial))
                        needsForge = true;
                    if (needsForge)
                        newRecipe.WorkbenchKeyword.SetTo(Skyrim.Keyword.CraftingSmithingForge);
                    else
                        newRecipe.WorkbenchKeyword.SetTo(Skyrim.Keyword.CraftingTanningRack);

                    newRecipe.Items = new();
                    if (primaryMaterialCount > 0)
                    {
                        newRecipe.Items.Add(new()
                        {
                            Item = new()
                            {
                                Item = primaryMaterial.AsSetter(),
                                Count = primaryMaterialCount.Value
                            }
                        });
                    }
                    if (secondaryMaterialCount > 0)
                    {
                        newRecipe.Items.Add(new()
                        {
                            Item = new()
                            {
                                Item = secondaryMaterial.AsSetter(),
                                Count = secondaryMaterialCount.Value
                            }
                        });
                    }
                    if (leatherStripCount > 0)
                    {
                        newRecipe.Items.Add(new()
                        {
                            Item = new()
                            {
                                Item = Skyrim.MiscItem.LeatherStrips,
                                Count = leatherStripCount.Value
                            }
                        });
                    }

                    if (ccoGlobalLookup.Count > 0)
                    {
                        //newRecipe.Conditions.Clear();

                        var globalSuffix = "Misc";
                        if (PrimaryMaterialToCCOMaterialGlobal.TryGetValue(primaryMaterial, out var temp))
                            globalSuffix = temp;
                        ccoGlobalLookup.TryGetValue("CCO_CategoryForgeMaterial_" + globalSuffix, out var globalLink);

                        // TODO conditions.
                    }
                }

                if (recipeType != RecipeType.Staff)
                    MakeTemperRecipe(weaponTemperingRecipeLookup,
                                     temperItemsDict,
                                     temperRecipeConditions,
                                     replaceRecipes,
                                     recipes,
                                     Skyrim.Keyword.CraftingSmithingSharpeningWheel,
                                     primaryMaterial,
                                     weaponLink,
                                     weaponEditorID);
            }
        }

        private static void Delete(IConstructibleObject constructibleObject)
        {
            constructibleObject.IsDeleted = true;
            constructibleObject.EditorID = null;
            constructibleObject.CreatedObject.SetToNull();
            constructibleObject.Items = null;
            constructibleObject.Conditions.Clear();
            constructibleObject.WorkbenchKeyword.SetToNull();
            constructibleObject.CreatedObjectCount = null;
        }

        private static void MakeTemperRecipe(
            Dictionary<IFormLinkGetter<IConstructibleGetter>, IConstructibleObjectGetter> armorTemperingRecipeLookup,
            Dictionary<IFormLinkGetter<IItemGetter>, Noggog.ExtendedList<ContainerEntry>> temperItemsDict,
            Lazy<Noggog.ExtendedList<Condition>> temperRecipeConditions,
            bool replaceRecipes,
            SkyrimGroup<ConstructibleObject> recipes,
            FormLink<IKeywordGetter> workbenchKeyword,
            IFormLinkGetter<IItemGetter> primaryMaterial,
            IFormLinkGetter<IConstructibleGetter> armorLink, string armorEditorID)
        {
            armorTemperingRecipeLookup.TryGetValue(armorLink, out var temperRecipe);
            ConstructibleObject? newTemperRecipe = null;
            if (temperRecipe is null)
                newTemperRecipe = recipes.AddNew("Temper" + armorEditorID);
            else if (replaceRecipes)
                newTemperRecipe = recipes.GetOrAddAsOverride(temperRecipe);

            if (newTemperRecipe is null) return;

            newTemperRecipe.CreatedObject.SetTo(armorLink);
            newTemperRecipe.CreatedObjectCount = 1;
            newTemperRecipe.WorkbenchKeyword.SetTo(workbenchKeyword);

            if (!temperItemsDict.TryGetValue(primaryMaterial, out var temperItems))
            {
                temperItems = new();

                temperItems.Add(new()
                {
                    Item = new ContainerItem()
                    {
                        Item = (IFormLink<IItemGetter>)primaryMaterial,
                        Count = 1
                    }
                });


                temperItemsDict.Add(primaryMaterial, temperItems);
            }

            newTemperRecipe.Items = temperItems;

            newTemperRecipe.Conditions.Clear();
            newTemperRecipe.Conditions.AddRange(temperRecipeConditions.Value);
        }

        private static void ReadOverridesFromKeywords(
            Dictionary<IFormLinkGetter<IKeywordGetter>, string> keywordLookup,
            Dictionary<string, IFormLinkGetter<IItemGetter>> materialLookup,
            IReadOnlyList<IFormLinkGetter<IKeywordGetter>> keywords,
            ref IFormLinkGetter<IItemGetter>? primaryMaterial,
            ref IFormLinkGetter<IItemGetter>? secondaryMaterial,
            ref ushort? primaryMaterialCount,
            ref ushort? secondaryMaterialCount,
            ref RecipeType? recipeType)
        {
            foreach (var keyword in keywords)
            {
                if (keywordLookup.TryGetValue(keyword, out var keywordEditorID))
                {
                    if (primaryMaterialCount is null && keywordEditorID.StartsWith(PrimaryMaterialCountOverrideKeywordPrefix))
                        if (ushort.TryParse(keywordEditorID[PrimaryMaterialCountOverrideKeywordPrefix.Length..], out var parsedCount))
                            primaryMaterialCount = parsedCount;
                    if (secondaryMaterialCount is null && keywordEditorID.StartsWith(SecondaryMaterialCountOverrideKeywordPrefix))
                        if (ushort.TryParse(keywordEditorID[SecondaryMaterialCountOverrideKeywordPrefix.Length..], out var parsedCount))
                            secondaryMaterialCount = parsedCount;
                    if (primaryMaterial is null && keywordEditorID.StartsWith(PrimaryMaterialOverrideKeywordPrefix))
                        materialLookup.TryGetValue(keywordEditorID[PrimaryMaterialOverrideKeywordPrefix.Length..], out primaryMaterial);
                    if (secondaryMaterial is null && keywordEditorID.StartsWith(SecondaryMaterialOverrideKeywordPrefix))
                        materialLookup.TryGetValue(keywordEditorID[SecondaryMaterialOverrideKeywordPrefix.Length..], out secondaryMaterial);
                    if (recipeType is null && keywordEditorID.StartsWith(RecipeTypeOverrideKeywordPrefix))
                        if (Enum.TryParse<RecipeType>(keywordEditorID[RecipeTypeOverrideKeywordPrefix.Length..], false, out var temp))
                            recipeType = temp;
                }
            }
        }
    }
}
