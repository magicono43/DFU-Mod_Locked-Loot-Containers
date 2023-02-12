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
            int identifyRoll = (chest.ChestMaterial == ChestMaterials.Wood) ? 10 : 0;
            identifyRoll += (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 1.66f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 2.5f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 5f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyChestSturdinessRoll(LLCObject chest)
        {
            int identifyRoll = (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 3.33f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 1.42f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 5f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyMagicResistRoll(LLCObject chest)
        {
            int identifyRoll = (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 5f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 3.33f) + (int)Mathf.Round(Mathf.Clamp(Alter, 0, 130) / 6.66f) + (int)Mathf.Round(Mathf.Clamp(Mysti, 0, 130) / 6.66f) + (int)Mathf.Round(Mathf.Clamp(Thaum, 0, 130) / 6.66f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLocktMaterialRoll(LLCObject chest)
        {
            int identifyRoll = (chest.LockMaterial == LockMaterials.Wood) ? 10 : 0;
            identifyRoll += (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 2.5f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 3.33f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 2.85f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLockComplexityRoll(LLCObject chest)
        {
            int identifyRoll = (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 5f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 3.33f) + (int)Mathf.Round(Mathf.Clamp(Agili, -50, 70) / 5f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 2.85f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyJamResistRoll(LLCObject chest)
        {
            int identifyRoll = (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 2.5f) + (int)Mathf.Round(Mathf.Clamp(Agili, -50, 70) / 2.5f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 3.33f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLockSturdinessRoll(LLCObject chest)
        {
            int identifyRoll = (int)Mathf.Round(Mathf.Clamp(Intel, -50, 70) / 5f) + (int)Mathf.Round(Mathf.Clamp(Willp, -50, 70) / 1.66f) + (int)Mathf.Round(Mathf.Clamp(LockP, 0, 130) / 3.33f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int DetermineInfoTextVagueness(int identifyRoll)
        {
            identifyRoll += UnityEngine.Random.Range(0, 35 + 1);

            if (identifyRoll <= 9) { return (int)InfoVagueness.Unknown; }
            else if (identifyRoll >= 10 && identifyRoll <= 35) { return (int)InfoVagueness.Bare; }
            else if (identifyRoll >= 36 && identifyRoll <= 62) { return (int)InfoVagueness.Simple; }
            else if (identifyRoll >= 63 && identifyRoll <= 89) { return (int)InfoVagueness.Vivid; }
            else { return (int)InfoVagueness.Complete; }
        }
    }
}