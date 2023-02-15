using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static void GetChestMaterialText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicMaterialText(chest.ChestMaterial, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid || vagueness == (int)InfoVagueness.Complete) { GetCompleteMaterialText(chest.ChestMaterial, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicMaterialText(ChestMaterials mat, out string text, out Color32 color)
        {
            if (mat == ChestMaterials.Wood || mat == ChestMaterials.Dwarven || mat == ChestMaterials.Adamantium) { text = "Soft"; color = Gainsboro; }
            else if (mat == ChestMaterials.Iron || mat == ChestMaterials.Steel || mat == ChestMaterials.Orcish || mat == ChestMaterials.Mithril || mat == ChestMaterials.Daedric) { text = "Hard"; color = Steel; }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetCompleteMaterialText(ChestMaterials mat, out string text, out Color32 color)
        {
            if (mat == ChestMaterials.Wood) { text = "Wooden"; color = Wood; }
            else if (mat == ChestMaterials.Iron) { text = "Iron"; color = Iron; }
            else if (mat == ChestMaterials.Steel) { text = "Steel"; color = Steel; }
            else if (mat == ChestMaterials.Orcish) { text = "Orcish"; color = Orcish; }
            else if (mat == ChestMaterials.Mithril) { text = "Mithril"; color = Mithril; }
            else if (mat == ChestMaterials.Dwarven) { text = "Dwarven"; color = Dwarven; }
            else if (mat == ChestMaterials.Adamantium) { text = "Adamantium"; color = Adamantium; }
            else if (mat == ChestMaterials.Daedric) { text = "Daedric"; color = Daedric; }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicMaterialText(LockMaterials mat, out string text, out Color32 color)
        {
            if (mat == LockMaterials.Wood || mat == LockMaterials.Dwarven || mat == LockMaterials.Adamantium) { text = "Soft"; color = Gainsboro; }
            else if (mat == LockMaterials.Iron || mat == LockMaterials.Steel || mat == LockMaterials.Orcish || mat == LockMaterials.Mithril || mat == LockMaterials.Daedric) { text = "Hard"; color = Steel; }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetCompleteMaterialText(LockMaterials mat, out string text, out Color32 color)
        {
            if (mat == LockMaterials.Wood) { text = "Wooden"; color = Wood; }
            else if (mat == LockMaterials.Iron) { text = "Iron"; color = Iron; }
            else if (mat == LockMaterials.Steel) { text = "Steel"; color = Steel; }
            else if (mat == LockMaterials.Orcish) { text = "Orcish"; color = Orcish; }
            else if (mat == LockMaterials.Mithril) { text = "Mithril"; color = Mithril; }
            else if (mat == LockMaterials.Dwarven) { text = "Dwarven"; color = Dwarven; }
            else if (mat == LockMaterials.Adamantium) { text = "Adamantium"; color = Adamantium; }
            else if (mat == LockMaterials.Daedric) { text = "Daedric"; color = Daedric; }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetChestSturdinessText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicSturdinessText(chest.ChestSturdiness, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividSturdinessText(chest.ChestSturdiness, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteSturdinessText(chest.ChestSturdiness, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicSturdinessText(int stab, out string text, out Color32 color)
        {
            if (stab <= 35) { text = "Flimsy"; color = Brown1; }
            else if (stab >= 36 && stab <= 75) { text = "Sturdy"; color = Gray1; }
            else { text = "Fortified"; color = Blue2; }
        }

        public static void GetVividSturdinessText(int stab, out string text, out Color32 color)
        {
            if (stab <= 16) { text = "Fragile"; color = Brown2; }
            else if (stab >= 17 && stab <= 35) { text = "Flimsy"; color = Brown1; }
            else if (stab >= 36 && stab <= 50) { text = "Stable"; color = Gray2; }
            else if (stab >= 51 && stab <= 75) { text = "Sturdy"; color = Gray1; }
            else if (stab >= 76 && stab <= 90) { text = "Fortified"; color = Blue2; }
            else { text = "Impenetrable"; color = Blue1; }
        }

        public static void GetCompleteSturdinessText(int stab, out string text, out Color32 color)
        {
            if (stab <= 16) { text = "Fragile" + " (" + stab.ToString() + ")"; color = Brown2; }
            else if (stab >= 17 && stab <= 35) { text = "Flimsy" + " (" + stab.ToString() + ")"; color = Brown1; }
            else if (stab >= 36 && stab <= 50) { text = "Stable" + " (" + stab.ToString() + ")"; color = Gray2; }
            else if (stab >= 51 && stab <= 75) { text = "Sturdy" + " (" + stab.ToString() + ")"; color = Gray1; }
            else if (stab >= 76 && stab <= 90) { text = "Fortified" + " (" + stab.ToString() + ")"; color = Blue2; }
            else { text = "Impenetrable" + " (" + stab.ToString() + ")"; color = Blue1; }
        }

        public static void GetChestMagicResistText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicMagicResistText(chest.ChestMagicResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividMagicResistText(chest.ChestMagicResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteMagicResistText(chest.ChestMagicResist, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicMagicResistText(int res, out string text, out Color32 color)
        {
            if (res <= 35) { text = "Defenseless"; color = Purple3; }
            else if (res >= 36 && res <= 75) { text = "Resistant"; color = Purple1; }
            else { text = "Immune"; color = Pink1; }
        }

        public static void GetVividMagicResistText(int res, out string text, out Color32 color)
        {
            if (res <= 16) { text = "Defenseless"; color = Purple3; }
            else if (res >= 17 && res <= 35) { text = "Vulnerable"; color = Purple2; }
            else if (res >= 36 && res <= 50) { text = "Resistant"; color = Purple1; }
            else if (res >= 51 && res <= 75) { text = "Protected"; color = Pink3; }
            else if (res >= 76 && res <= 90) { text = "Warded"; color = Pink2; }
            else { text = "Immune"; color = Pink1; }
        }

        public static void GetCompleteMagicResistText(int res, out string text, out Color32 color)
        {
            if (res <= 16) { text = "Defenseless" + " (" + res.ToString() + ")"; color = Purple3; }
            else if (res >= 17 && res <= 35) { text = "Vulnerable" + " (" + res.ToString() + ")"; color = Purple2; }
            else if (res >= 36 && res <= 50) { text = "Resistant" + " (" + res.ToString() + ")"; color = Purple1; }
            else if (res >= 51 && res <= 75) { text = "Protected" + " (" + res.ToString() + ")"; color = Pink3; }
            else if (res >= 76 && res <= 90) { text = "Warded" + " (" + res.ToString() + ")"; color = Pink2; }
            else { text = "Immune" + " (" + res.ToString() + ")"; color = Pink1; }
        }

        public static void GetLockMaterialText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicMaterialText(chest.LockMaterial, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid || vagueness == (int)InfoVagueness.Complete) { GetCompleteMaterialText(chest.LockMaterial, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetLockSturdinessText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicSturdinessText(chest.LockSturdiness, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividSturdinessText(chest.LockSturdiness, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteSturdinessText(chest.LockSturdiness, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetLockMagicResistText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicMagicResistText(chest.LockMagicResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividMagicResistText(chest.LockMagicResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteMagicResistText(chest.LockMagicResist, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetLockComplexityText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicComplexityText(chest.LockComplexity, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividComplexityText(chest.LockComplexity, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteComplexityText(chest.LockComplexity, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicComplexityText(int comp, out string text, out Color32 color)
        {
            if (comp <= 35) { text = "Simple"; color = Yellow5; }
            else if (comp >= 36 && comp <= 75) { text = "Complex"; color = Yellow4; }
            else { text = "Elaborate"; color = Yellow3; }
        }

        public static void GetVividComplexityText(int comp, out string text, out Color32 color)
        {
            if (comp <= 16) { text = "Crude"; color = Yellow6; }
            else if (comp >= 17 && comp <= 35) { text = "Simple"; color = Yellow5; }
            else if (comp >= 36 && comp <= 50) { text = "Complex"; color = Yellow4; }
            else if (comp >= 51 && comp <= 75) { text = "Elaborate"; color = Yellow3; }
            else if (comp >= 76 && comp <= 90) { text = "Tricky"; color = Yellow2; }
            else { text = "Devious"; color = Yellow1; }
        }

        public static void GetCompleteComplexityText(int comp, out string text, out Color32 color)
        {
            if (comp <= 16) { text = "Crude" + " (" + comp.ToString() + ")"; color = Yellow6; }
            else if (comp >= 17 && comp <= 35) { text = "Simple" + " (" + comp.ToString() + ")"; color = Yellow5; }
            else if (comp >= 36 && comp <= 50) { text = "Complex" + " (" + comp.ToString() + ")"; color = Yellow4; }
            else if (comp >= 51 && comp <= 75) { text = "Elaborate" + " (" + comp.ToString() + ")"; color = Yellow3; }
            else if (comp >= 76 && comp <= 90) { text = "Tricky" + " (" + comp.ToString() + ")"; color = Yellow2; }
            else { text = "Devious" + " (" + comp.ToString() + ")"; color = Yellow1; }
        }

        public static void GetLockJamResistText(LLCObject chest, int vagueness, out string text, out Color32 color)
        {
            if (vagueness == (int)InfoVagueness.Unknown) { text = "Unknown"; color = BrownRed; }
            else if (vagueness == (int)InfoVagueness.Bare || vagueness == (int)InfoVagueness.Simple) { GetBasicJamResistText(chest.JamResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Vivid) { GetVividJamResistText(chest.JamResist, out text, out color); }
            else if (vagueness == (int)InfoVagueness.Complete) { GetCompleteJamResistText(chest.JamResist, out text, out color); }
            else { text = "ERROR"; color = BrightRed; }
        }

        public static void GetBasicJamResistText(int jam, out string text, out Color32 color)
        {
            if (jam <= 35) { text = "Eroded"; color = Green3; }
            else if (jam >= 36 && jam <= 75) { text = "Springy"; color = Teal3; }
            else { text = "Oiled"; color = Teal2; }
        }

        public static void GetVividJamResistText(int jam, out string text, out Color32 color)
        {
            if (jam <= 16) { text = "Eroded"; color = Green3; }
            else if (jam >= 17 && jam <= 35) { text = "Deteriorated"; color = Green2; }
            else if (jam >= 36 && jam <= 50) { text = "Stiff"; color = Green1; }
            else if (jam >= 51 && jam <= 75) { text = "Springy"; color = Teal3; }
            else if (jam >= 76 && jam <= 90) { text = "Oiled"; color = Teal2; }
            else { text = "Effortless"; color = Teal1; }
        }

        public static void GetCompleteJamResistText(int jam, out string text, out Color32 color)
        {
            if (jam <= 16) { text = "Eroded" + " (" + jam.ToString() + ")"; color = Green3; }
            else if (jam >= 17 && jam <= 35) { text = "Deteriorated" + " (" + jam.ToString() + ")"; color = Green2; }
            else if (jam >= 36 && jam <= 50) { text = "Stiff" + " (" + jam.ToString() + ")"; color = Green1; }
            else if (jam >= 51 && jam <= 75) { text = "Springy" + " (" + jam.ToString() + ")"; color = Teal3; }
            else if (jam >= 76 && jam <= 90) { text = "Oiled" + " (" + jam.ToString() + ")"; color = Teal2; }
            else { text = "Effortless" + " (" + jam.ToString() + ")"; color = Teal1; }
        }
    }
}