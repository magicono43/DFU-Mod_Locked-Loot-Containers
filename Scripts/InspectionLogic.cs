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
        public static int[] GetInspectionValues(LLCObject chest)
        {
            int[] values = chest.RecentInspectValues;

            values[0] = IdentifyChestMaterialRoll(chest);
            values[1] = IdentifyChestSturdinessRoll(chest);
            values[2] = IdentifyMagicResistRoll(chest);
            values[3] = IdentifyLocktMaterialRoll(chest);
            values[4] = IdentifyLockSturdinessRoll(chest);
            values[5] = IdentifyMagicResistRoll(chest);
            values[6] = IdentifyLockComplexityRoll(chest);
            values[7] = IdentifyJamResistRoll(chest);

            return values;
        }

        public static int IdentifyChestMaterialRoll(LLCObject chest)
        {
            int identifyRoll = (chest.ChestMaterial == ChestMaterials.Wood) ? 10 : 0;
            identifyRoll += Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.7f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.4f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.35f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyChestSturdinessRoll(LLCObject chest)
        {
            int identifyRoll = Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.4f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.8f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.3f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyMagicResistRoll(LLCObject chest)
        {
            int identifyRoll = Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.4f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.3f) + Mathf.RoundToInt(Mathf.Clamp(Alter, 0, 130) * 0.2f) + Mathf.RoundToInt(Mathf.Clamp(Mysti, 0, 130) * 0.2f) + Mathf.RoundToInt(Mathf.Clamp(Thaum, 0, 130) * 0.2f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLocktMaterialRoll(LLCObject chest)
        {
            int identifyRoll = (chest.LockMaterial == LockMaterials.Wood) ? 10 : 0;
            identifyRoll += Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.6f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.3f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.45f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLockSturdinessRoll(LLCObject chest)
        {
            int identifyRoll = Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.3f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.7f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.4f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyLockComplexityRoll(LLCObject chest)
        {
            int identifyRoll = Mathf.RoundToInt(Mathf.Clamp(Intel, -50, 70) * 0.2f) + Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.4f) + Mathf.RoundToInt(Mathf.Clamp(Agili, -50, 70) * 0.4f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.45f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int IdentifyJamResistRoll(LLCObject chest)
        {
            int identifyRoll = Mathf.RoundToInt(Mathf.Clamp(Willp, -50, 70) * 0.5f) + Mathf.RoundToInt(Mathf.Clamp(Agili, -50, 70) * 0.8f) + Mathf.RoundToInt(Mathf.Clamp(LockP, 0, 130) * 0.3f);
            return DetermineInfoTextVagueness(identifyRoll);
        }

        public static int DetermineInfoTextVagueness(int identifyRoll)
        {
            identifyRoll += UnityEngine.Random.Range(0, 25 + 1);

            if (identifyRoll <= 9) { return (int)InfoVagueness.Unknown; }
            else if (identifyRoll >= 10 && identifyRoll <= 35) { return (int)InfoVagueness.Bare; }
            else if (identifyRoll >= 36 && identifyRoll <= 62) { return (int)InfoVagueness.Simple; }
            else if (identifyRoll >= 63 && identifyRoll <= 89) { return (int)InfoVagueness.Vivid; }
            else { return (int)InfoVagueness.Complete; }
        }
    }
}