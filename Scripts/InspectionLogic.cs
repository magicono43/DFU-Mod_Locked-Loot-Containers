using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static int IdentifyChestMaterialRoll(LLCObject chest)
        {
            ChestMaterials chestMat = chest.ChestMaterial;
            float blowOpenChestChance = (magicResist * -1f) + Mathf.Floor(sturdiness / -4f) + Mathf.Round(effectMagnitude * (elementModifier + ((float)Destr / 500f))) + (int)Mathf.Round(Luck / 10f);

            int identifyRoll = UnityEngine.Random.Range(0, 40 + 1);

            // + Int + Will + Lockpicking?
            // Next time I work on this, somehow do the formula for this identification roll, maybe have wooden chests give a bonus to identifying as well or something, will see then continue on.

            if (identifyRoll <= 9) { return (int)InfoVagueness.Unknown; }
            else if (identifyRoll >= 10 && identifyRoll <= 35) { return (int)InfoVagueness.Bare; }
            else if (identifyRoll >= 36 && identifyRoll <= 62) { return (int)InfoVagueness.Simple; }
            else if (identifyRoll >= 63 && identifyRoll <= 89) { return (int)InfoVagueness.Vivid; }
            else { return (int)InfoVagueness.Complete; }
        }
    }
}