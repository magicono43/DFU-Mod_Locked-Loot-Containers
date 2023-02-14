using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static string BuildName()
        {
            return GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.displayName;
        }

        public static string CityName()
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CurrentRegion()
        {   // %crn going to use for %reg as well here
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        public static string RegentTitle()
        {   // %rt %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion((int)FactionFile.FactionTypes.Province, gps.CurrentRegionIndex, out regionFaction);
            return GetRulerTitle(regionFaction.ruler);
        }

        private static string GetRulerTitle(int factionRuler)
        {
            switch (factionRuler)
            {
                case 1:
                    return TextManager.Instance.GetLocalizedText("King");
                case 2:
                    return TextManager.Instance.GetLocalizedText("Queen");
                case 3:
                    return TextManager.Instance.GetLocalizedText("Duke");
                case 4:
                    return TextManager.Instance.GetLocalizedText("Duchess");
                case 5:
                    return TextManager.Instance.GetLocalizedText("Marquis");
                case 6:
                    return TextManager.Instance.GetLocalizedText("Marquise");
                case 7:
                    return TextManager.Instance.GetLocalizedText("Count");
                case 8:
                    return TextManager.Instance.GetLocalizedText("Countess");
                case 9:
                    return TextManager.Instance.GetLocalizedText("Baron");
                case 10:
                    return TextManager.Instance.GetLocalizedText("Baroness");
                case 11:
                    return TextManager.Instance.GetLocalizedText("Lord");
                case 12:
                    return TextManager.Instance.GetLocalizedText("Lady");
                default:
                    return TextManager.Instance.GetLocalizedText("Lord");
            }
        }

        public static string RandomAlcohol() // Might want to add more "fantasy" sounding drinks to this at some point, but for now it should be alright hopefully.
        {
            DFLocation.ClimateBaseType climate = GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType;
            DaggerfallDateTime.Seasons season = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;

            int variant = UnityEngine.Random.Range(0, 2);

            switch (climate)
            {
                case DFLocation.ClimateBaseType.Desert:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Jalapeno tequila" : "Prickly pear margarita";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Coconut rum" : "Cucumber mojito";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Pineapple margarita" : "Cucumber mojito";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Cinnamon tequila" : "Spicy scorpion cocktail";
                    }
                case DFLocation.ClimateBaseType.Mountain:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Mulled cider" : "Cinnamon mead";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Elderberry cocktail" : "Juniper berry mead";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Barley wine" : "Golden ale";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Mulled wine" : "Wassail";
                    }
                case DFLocation.ClimateBaseType.Temperate:
                default:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Cranberry wine" : "Pumpkin sangria";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Watermelon Mojito" : "Tequila sunrise";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Mint julep" : "Elderflower champagne";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Mulled wine" : "Anise Liqueur";
                    }
                case DFLocation.ClimateBaseType.Swamp:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Banana liqueur" : "Grapefruit mimosa";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Sake sour" : "Grapefruit daiquiri";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Pineapple sangria" : "Sake mojito";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Snakebite cocktail" : "Coconut rum";
                    }
            }
        }
        
        public static string GetChestMaterialText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicMaterialText(chest.ChestMaterial); }
            else if (vagueness == (int)InfoVagueness.Vivid || vagueness == (int)InfoVagueness.Complete) { return GetCompleteMaterialText(chest.ChestMaterial); }
            else { return "ERROR"; }
        }

        public static string GetBasicMaterialText(ChestMaterials mat)
        {
            if (mat == ChestMaterials.Wood || mat == ChestMaterials.Dwarven || mat == ChestMaterials.Adamantium) { return "Soft"; }
            else if (mat == ChestMaterials.Iron || mat == ChestMaterials.Steel || mat == ChestMaterials.Orcish || mat == ChestMaterials.Mithril || mat == ChestMaterials.Daedric) { return "Hard"; }
            else return "ERROR";
        }

        public static string GetCompleteMaterialText(ChestMaterials mat)
        {
            if (mat == ChestMaterials.Wood) { return "Wooden"; }
            else if (mat == ChestMaterials.Iron) { return "Iron"; }
            else if (mat == ChestMaterials.Steel) { return "Steel"; }
            else if (mat == ChestMaterials.Orcish) { return "Orcish"; }
            else if (mat == ChestMaterials.Mithril) { return "Mithril"; }
            else if (mat == ChestMaterials.Dwarven) { return "Dwarven"; }
            else if (mat == ChestMaterials.Adamantium) { return "Adamantium"; }
            else if (mat == ChestMaterials.Daedric) { return "Daedric"; }
            else { return "ERROR"; }
        }

        public static string GetBasicMaterialText(LockMaterials mat)
        {
            if (mat == LockMaterials.Wood || mat == LockMaterials.Dwarven || mat == LockMaterials.Adamantium) { return "Soft"; }
            else if (mat == LockMaterials.Iron || mat == LockMaterials.Steel || mat == LockMaterials.Orcish || mat == LockMaterials.Mithril || mat == LockMaterials.Daedric) { return "Hard"; }
            else return "ERROR";
        }

        public static string GetCompleteMaterialText(LockMaterials mat)
        {
            if (mat == LockMaterials.Wood) { return "Wooden"; }
            else if (mat == LockMaterials.Iron) { return "Iron"; }
            else if (mat == LockMaterials.Steel) { return "Steel"; }
            else if (mat == LockMaterials.Orcish) { return "Orcish"; }
            else if (mat == LockMaterials.Mithril) { return "Mithril"; }
            else if (mat == LockMaterials.Dwarven) { return "Dwarven"; }
            else if (mat == LockMaterials.Adamantium) { return "Adamantium"; }
            else if (mat == LockMaterials.Daedric) { return "Daedric"; }
            else { return "ERROR"; }
        }

        public static string GetChestSturdinessText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicSturdinessText(chest.ChestSturdiness); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividSturdinessText(chest.ChestSturdiness); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteSturdinessText(chest.ChestSturdiness); }
            else { return "ERROR"; }
        }

        public static string GetBasicSturdinessText(int stab)
        {
            if (stab <= 35) { return "Flimsy"; }
            else if (stab >= 36 && stab <= 75) { return "Sturdy"; }
            else { return "Fortified"; }
        }

        public static string GetVividSturdinessText(int stab)
        {
            if (stab <= 16) { return "Fragile"; }
            else if (stab >= 17 && stab <= 35) { return "Flimsy"; }
            else if (stab >= 36 && stab <= 50) { return "Stable"; }
            else if (stab >= 51 && stab <= 75) { return "Sturdy"; }
            else if (stab >= 76 && stab <= 90) { return "Fortified"; }
            else { return "Impenetrable"; }
        }

        public static string GetCompleteSturdinessText(int stab)
        {
            if (stab <= 16) { return "Fragile" + " (" + stab.ToString() + ")"; }
            else if (stab >= 17 && stab <= 35) { return "Flimsy" + " (" + stab.ToString() + ")"; }
            else if (stab >= 36 && stab <= 50) { return "Stable" + " (" + stab.ToString() + ")"; }
            else if (stab >= 51 && stab <= 75) { return "Sturdy" + " (" + stab.ToString() + ")"; }
            else if (stab >= 76 && stab <= 90) { return "Fortified" + " (" + stab.ToString() + ")"; }
            else { return "Impenetrable" + " (" + stab.ToString() + ")"; }
        }

        public static string GetChestMagicResistText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicMagicResistText(chest.ChestMagicResist); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividMagicResistText(chest.ChestMagicResist); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteMagicResistText(chest.ChestMagicResist); }
            else { return "ERROR"; }
        }

        public static string GetBasicMagicResistText(int res)
        {
            if (res <= 35) { return "Defenseless"; }
            else if (res >= 36 && res <= 75) { return "Resistant"; }
            else { return "Immune"; }
        }

        public static string GetVividMagicResistText(int res)
        {
            if (res <= 16) { return "Defenseless"; }
            else if (res >= 17 && res <= 35) { return "Vulnerable"; }
            else if (res >= 36 && res <= 50) { return "Resistant"; }
            else if (res >= 51 && res <= 75) { return "Protected"; }
            else if (res >= 76 && res <= 90) { return "Warded"; }
            else { return "Immune"; }
        }

        public static string GetCompleteMagicResistText(int res)
        {
            if (res <= 16) { return "Defenseless" + " (" + res.ToString() + ")"; }
            else if (res >= 17 && res <= 35) { return "Vulnerable" + " (" + res.ToString() + ")"; }
            else if (res >= 36 && res <= 50) { return "Resistant" + " (" + res.ToString() + ")"; }
            else if (res >= 51 && res <= 75) { return "Protected" + " (" + res.ToString() + ")"; }
            else if (res >= 76 && res <= 90) { return "Warded" + " (" + res.ToString() + ")"; }
            else { return "Immune" + " (" + res.ToString() + ")"; }
        }

        public static string GetLockMaterialText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicMaterialText(chest.LockMaterial); }
            else if (vagueness == (int)InfoVagueness.Vivid || vagueness == (int)InfoVagueness.Complete) { return GetCompleteMaterialText(chest.LockMaterial); }
            else { return "ERROR"; }
        }

        public static string GetLockSturdinessText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicSturdinessText(chest.LockSturdiness); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividSturdinessText(chest.LockSturdiness); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteSturdinessText(chest.LockSturdiness); }
            else { return "ERROR"; }
        }

        public static string GetLockMagicResistText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicMagicResistText(chest.LockMagicResist); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividMagicResistText(chest.LockMagicResist); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteMagicResistText(chest.LockMagicResist); }
            else { return "ERROR"; }
        }

        public static string GetLockComplexityText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicComplexityText(chest.LockComplexity); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividComplexityText(chest.LockComplexity); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteComplexityText(chest.LockComplexity); }
            else { return "ERROR"; }
        }

        public static string GetBasicComplexityText(int comp)
        {
            if (comp <= 35) { return "Simple"; }
            else if (comp >= 36 && comp <= 75) { return "Complex"; }
            else { return "Elaborate"; }
        }

        public static string GetVividComplexityText(int comp)
        {
            if (comp <= 16) { return "Crude"; }
            else if (comp >= 17 && comp <= 35) { return "Simple"; }
            else if (comp >= 36 && comp <= 50) { return "Complex"; }
            else if (comp >= 51 && comp <= 75) { return "Elaborate"; }
            else if (comp >= 76 && comp <= 90) { return "Tricky"; }
            else { return "Devious"; }
        }

        public static string GetCompleteComplexityText(int comp)
        {
            if (comp <= 16) { return "Crude" + " (" + comp.ToString() + ")"; }
            else if (comp >= 17 && comp <= 35) { return "Simple" + " (" + comp.ToString() + ")"; }
            else if (comp >= 36 && comp <= 50) { return "Complex" + " (" + comp.ToString() + ")"; }
            else if (comp >= 51 && comp <= 75) { return "Elaborate" + " (" + comp.ToString() + ")"; }
            else if (comp >= 76 && comp <= 90) { return "Tricky" + " (" + comp.ToString() + ")"; }
            else { return "Devious" + " (" + comp.ToString() + ")"; }
        }

        public static string GetLockJamResistText(LLCObject chest, int vagueness)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { return "Unknown"; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { return GetBasicJamResistText(chest.JamResist); }
            else if (vagueness == (int)InfoVagueness.Vivid) { return GetVividJamResistText(chest.JamResist); }
            else if (vagueness == (int)InfoVagueness.Complete) { return GetCompleteJamResistText(chest.JamResist); }
            else { return "ERROR"; }
        }

        public static string GetBasicJamResistText(int jam)
        {
            if (jam <= 35) { return "Eroded"; }
            else if (jam >= 36 && jam <= 75) { return "Springy"; }
            else { return "Oiled"; }
        }

        public static string GetVividJamResistText(int jam)
        {
            if (jam <= 16) { return "Eroded"; }
            else if (jam >= 17 && jam <= 35) { return "Deteriorated"; }
            else if (jam >= 36 && jam <= 50) { return "Stiff"; }
            else if (jam >= 51 && jam <= 75) { return "Springy"; }
            else if (jam >= 76 && jam <= 90) { return "Oiled"; }
            else { return "Effortless"; }
        }

        public static string GetCompleteJamResistText(int jam)
        {
            if (jam <= 16) { return "Eroded" + " (" + jam.ToString() + ")"; }
            else if (jam >= 17 && jam <= 35) { return "Deteriorated" + " (" + jam.ToString() + ")"; }
            else if (jam >= 36 && jam <= 50) { return "Stiff" + " (" + jam.ToString() + ")"; }
            else if (jam >= 51 && jam <= 75) { return "Springy" + " (" + jam.ToString() + ")"; }
            else if (jam >= 76 && jam <= 90) { return "Oiled" + " (" + jam.ToString() + ")"; }
            else { return "Effortless" + " (" + jam.ToString() + ")"; }
        }
    }
}