using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        // Various colors in Daggerfall's palette
        public static Color32 Blank = new Color32(0, 0, 0, 0);
        public static Color32 Black = new Color32(0, 0, 0, 255);
        public static Color32 White = new Color32(255, 255, 255, 255);
        public static Color32 Gainsboro = new Color32(220, 220, 220, 255);
        public static Color32 BrownRed = new Color32(92, 33, 3, 255);
        public static Color32 BrightRed = new Color32(154, 24, 8, 255);

        public static Color32 Wood = new Color32(159, 99, 63, 255);
        public static Color32 Iron = new Color32(110, 110, 110, 255);
        public static Color32 Steel = new Color32(132, 132, 132, 255);
        public static Color32 Orcish = new Color32(130, 162, 77, 255);
        public static Color32 Mithril = new Color32(87, 137, 205, 255);
        public static Color32 Dwarven = new Color32(212, 203, 0, 255);
        public static Color32 Adamantium = new Color32(38, 51, 40, 255);
        public static Color32 Daedric = new Color32(162, 36, 12, 255);

        public static Color32 Brown1 = new Color32(139, 83, 43, 255);
        public static Color32 Brown2 = new Color32(115, 67, 35, 255);
        public static Color32 Gray1 = new Color32(147, 147, 147, 255);
        public static Color32 Gray2 = new Color32(119, 119, 119, 255);
        public static Color32 Blue1 = new Color32(123, 164, 230, 255);
        public static Color32 Blue2 = new Color32(87, 137, 205, 255);

        public static Color32 Pink1 = new Color32(220, 166, 188, 255);
        public static Color32 Pink2 = new Color32(188, 127, 158, 255);
        public static Color32 Pink3 = new Color32(155, 98, 130, 255);
        public static Color32 Purple1 = new Color32(127, 77, 106, 255);
        public static Color32 Purple2 = new Color32(101, 65, 96, 255);
        public static Color32 Purple3 = new Color32(75, 52, 71, 255);

        public static Color32 Yellow1 = new Color32(226, 220, 0, 255);
        public static Color32 Yellow2 = new Color32(197, 185, 0, 255);
        public static Color32 Yellow3 = new Color32(168, 150, 0, 255);
        public static Color32 Yellow4 = new Color32(139, 115, 0, 255);
        public static Color32 Yellow5 = new Color32(116, 97, 7, 255);
        public static Color32 Yellow6 = new Color32(93, 78, 14, 255);

        public static Color32 Teal1 = new Color32(109, 170, 170, 255);
        public static Color32 Teal2 = new Color32(70, 135, 135, 255);
        public static Color32 Teal3 = new Color32(54, 112, 112, 255);
        public static Color32 Green1 = new Color32(68, 99, 67, 255);
        public static Color32 Green2 = new Color32(52, 77, 44, 255);
        public static Color32 Green3 = new Color32(39, 60, 39, 255);

        public static int Stren { get { return Player.Stats.LiveStrength - 50; } }
        public static int Intel { get { return Player.Stats.LiveIntelligence - 50; } }
        public static int Willp { get { return Player.Stats.LiveWillpower - 50; } }
        public static int Agili { get { return Player.Stats.LiveAgility - 50; } }
        public static int Endur { get { return Player.Stats.LiveEndurance - 50; } }
        public static int Speed { get { return Player.Stats.LiveSpeed - 50; } }
        public static int Luck { get { return Player.Stats.LiveLuck - 50; } }
        public static int LockP { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Lockpicking); } }
        public static int PickP { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket); } }
        public static int Sneak { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Stealth); } }
        public static int Alter { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Alteration); } }
        public static int Destr { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Destruction); } }
        public static int Illus { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Illusion); } }
        public static int Mysti { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Mysticism); } }
        public static int Thaum { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Thaumaturgy); } }

        public static bool LockPickChance(LLCObject chest)
        {
            int lockComp = chest.LockComplexity;
            int attempts = chest.PicksAttempted - 1;

            if (lockComp >= 0 && lockComp <= 19)
            {
                int lockP = Mathf.CeilToInt(LockP * 1.7f);
                float successChance = (lockComp * -1.6f) + lockP + Mathf.Round(PickP * 0.1f) + Mathf.Round(Intel * 0.1f) + Mathf.Round(Agili * 0.7f) + Mathf.Round(Speed * 0.3f) + Mathf.Round(Luck * 0.2f);
                if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(successChance, 7f, 100f))))
                {
                    if (LockP >= 60) { } // Do nothing
                    else { Player.TallySkill(DFCareer.Skills.Lockpicking, (short)Mathf.Clamp(1 - attempts, 0, 1)); }
                    return true;
                }
                else
                    return false;
            }
            else if (lockComp >= 20 && lockComp <= 39)
            {
                int lockP = Mathf.CeilToInt(LockP * 1.7f);
                float successChance = (lockComp * -1.6f) + lockP + Mathf.Round(PickP * 0.1f) + Mathf.Round(Intel * 0.2f) + Mathf.Round(Agili * 0.6f) + Mathf.Round(Speed * 0.25f) + Mathf.Round(Luck * 0.2f);
                if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(successChance, 5f, 95f))))
                {
                    if (LockP >= 80) { } // Do nothing
                    else { Player.TallySkill(DFCareer.Skills.Lockpicking, (short)Mathf.Clamp(2 - attempts, 0, 2)); }
                    return true;
                }
                else
                    return false;
            }
            else if (lockComp >= 40 && lockComp <= 59)
            {
                int lockP = Mathf.CeilToInt(LockP * 1.7f);
                float successChance = (lockComp * -1.6f) + lockP + Mathf.Round(PickP * 0.1f) + Mathf.Round(Intel * 0.3f) + Mathf.Round(Agili * 0.5f) + Mathf.Round(Speed * 0.2f) + Mathf.Round(Luck * 0.2f);
                if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(successChance, 3f, 90f))))
                {
                    Player.TallySkill(DFCareer.Skills.Lockpicking, (short)Mathf.Clamp(3 - attempts, 0, 3));
                    return true;
                }
                else
                    return false;
            }
            else if (lockComp >= 60 && lockComp <= 79)
            {
                int lockP = Mathf.CeilToInt(LockP * 1.7f);
                float successChance = (lockComp * -1.6f) + lockP + Mathf.Round(PickP * 0.1f) + Mathf.Round(Intel * 0.4f) + Mathf.Round(Agili * 0.4f) + Mathf.Round(Speed * 0.15f) + Mathf.Round(Luck * 0.2f);
                if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(successChance, 1f, 80f))))
                {
                    Player.TallySkill(DFCareer.Skills.Lockpicking, (short)Mathf.Clamp(4 - attempts, 0, 4));
                    return true;
                }
                else
                    return false;
            }
            else
            {
                int lockP = Mathf.CeilToInt(LockP * 1.7f);
                float successChance = (lockComp * -1.6f) + lockP + Mathf.Round(PickP * 0.1f) + Mathf.Round(Intel * 0.5f) + Mathf.Round(Agili * 0.3f) + Mathf.Round(Speed * 0.1f) + Mathf.Round(Luck * 0.2f);
                if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(successChance, 0f, 70f)))) // Potentially add specific text depending on initial odds, like "Through dumb-Luck, you somehow unlocked it", etc.
                {
                    Player.TallySkill(DFCareer.Skills.Lockpicking, (short)Mathf.Clamp(5 - attempts, 0, 5));
                    return true;
                }
                else
                    return false;
            }
        }

        public static int LockDamageNegationChance(LLCObject chest, DaggerfallUnityItem weapon, int skillID)
        {
            int lockStab = (chest.LockMaterial == LockMaterials.Wood) ? Mathf.RoundToInt(chest.LockSturdiness / 0.7f) : chest.LockSturdiness;

            if (skillID == (int)DFCareer.Skills.HandToHand || weapon == null)
                return 15 + lockStab - (Mathf.RoundToInt(Stren * 0.7f) + Mathf.RoundToInt(Luck * 0.1f));
            else
                return lockStab - (Mathf.RoundToInt(Stren * 0.7f) + Mathf.RoundToInt(Luck * 0.1f));
        }

        public static int CalculateLockDamage(LLCObject chest, DaggerfallUnityItem weapon, int skillID, bool critBash)
        {
            int damage = 0;
            int lockmechDam = 0;
            int lockStab = (chest.LockMaterial == LockMaterials.Wood) ? Mathf.RoundToInt(chest.LockSturdiness / 0.7f) : chest.LockSturdiness;
            float lockDamRes = Mathf.Abs((lockStab * 0.35f / 100f) -1f);
            float luckMod = (Luck / 500f) + 1f;

            if (skillID == (int)DFCareer.Skills.HandToHand || weapon == null)
            {
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateHandToHandDamage() * 0.7f * lockDamRes * luckMod));
                lockmechDam = (critBash) ? Mathf.RoundToInt(damage * 0.5f * 0.8f) * 4 : Mathf.RoundToInt(damage * 0.5f * 0.8f);
            }
            else if (skillID == (int)DFCareer.Skills.BluntWeapon)
            {
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateWeaponDamage(weapon) * 0.8f * lockDamRes * luckMod));
                lockmechDam = (critBash) ? Mathf.RoundToInt(damage * 0.5f) * 4 : Mathf.RoundToInt(damage * 0.5f);
            }
            else
            {
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateWeaponDamage(weapon) * lockDamRes * luckMod));
                lockmechDam = (critBash) ? Mathf.RoundToInt(damage * 0.5f * 0.6f) * 4 : Mathf.RoundToInt(damage * 0.5f * 0.6f);
            }

            DoesLockJam(chest, lockmechDam);
            damage = (critBash) ? damage * 4 : damage;
            return damage;
        }

        public static int ChestDamageNegationChance(LLCObject chest, DaggerfallUnityItem weapon, int skillID)
        {
            int chestStab = (chest.ChestMaterial == ChestMaterials.Wood) ? Mathf.RoundToInt(chest.ChestSturdiness / 0.7f) : chest.ChestSturdiness;

            if (skillID == (int)DFCareer.Skills.HandToHand || weapon == null)
                return 30 + chestStab - (Mathf.RoundToInt(Stren * 0.7f) + Mathf.RoundToInt(Luck * 0.1f));
            else
                return 10 + chestStab - (Mathf.RoundToInt(Stren * 0.7f) + Mathf.RoundToInt(Luck * 0.1f));
        }

        public static int CalculateChestDamage(LLCObject chest, DaggerfallUnityItem weapon, int skillID, bool critBash)
        {
            int damage = 0;
            int chestStab = (chest.ChestMaterial == ChestMaterials.Wood) ? Mathf.RoundToInt(chest.ChestSturdiness / 0.7f) : chest.ChestSturdiness;
            float chestDamRes = Mathf.Abs((chestStab * 0.45f / 100f) - 1f);
            float luckMod = (Luck / 500f) + 1f;

            if (skillID == (int)DFCareer.Skills.HandToHand || weapon == null)
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateHandToHandDamage() * 0.6f * chestDamRes * luckMod));
            else if (skillID == (int)DFCareer.Skills.BluntWeapon)
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateWeaponDamage(weapon) * chestDamRes * luckMod));
            else
                damage = Mathf.Max(1, Mathf.RoundToInt(CalculateWeaponDamage(weapon) * 0.7f * chestDamRes * luckMod));

            damage = (critBash) ? damage * 4 : damage;
            return damage;
        }

        public static int ChestMagicDamageNegationChance(LLCObject chest, float elementMod)
        {
            int chestResist = chest.ChestMagicResist;
            int chestStab = chest.ChestSturdiness;

            return chestResist + Mathf.RoundToInt(chestStab / 5f) - Mathf.RoundToInt(elementMod * ((Destr * 0.4f) + (Willp * 0.6f) + (Luck * 0.2f)));
        }

        public static int CalculateChestMagicDamage(LLCObject chest, int magnitude, float elementMod)
        {
            int damage = 0;
            int chestResist = chest.ChestMagicResist;
            int chestStab = chest.ChestSturdiness;
            float chestDamRes = Mathf.Abs(((Mathf.RoundToInt(chestStab * 0.2f) + chestResist) * 0.4f / 100f) - 1f);
            float luckMod = (Luck / 250f) + 1f;

            damage = Mathf.Max(1, Mathf.RoundToInt(magnitude * elementMod * chestDamRes * luckMod));

            return damage;
        }

        public static int DetermineDamageToLockMechanism(LLCObject chest) // Determine how much damage the locking mechanism takes from a lock-pick attempt, if any.
        {
            if (Dice100.SuccessRoll(LockMechanismDamageNegationChance(chest)))
                return 0;
            else
                return CalculateLockMechanismDamage(chest);
        }

        public static int LockMechanismDamageNegationChance(LLCObject chest)
        {
            int jamResist = chest.JamResist;

            return jamResist - (chest.PicksAttempted * (10 - Mathf.RoundToInt(Luck * 0.1f)));
        }

        public static int CalculateLockMechanismDamage(LLCObject chest)
        {
            int damage = UnityEngine.Random.Range(30, 61);
            float mechDamRes = Mathf.Abs((chest.JamResist * 0.3f / 100f) - 1f);

            damage = damage - Mathf.RoundToInt(Luck * 0.3f);
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * mechDamRes));

            return damage;
        }

        public static bool DoesLockJam(LLCObject chest, int damage) // Determine if damage dealt to the locking mechanism should cause it to become jammed and inoperable.
        {
            int lockMechHP = chest.LockMechCurrentHP - damage;

            if (lockMechHP <= 0)
            {
                chest.LockMechCurrentHP = -1;
                chest.IsLockJammed = true;
                return true;
            }
            else
            {
                chest.LockMechCurrentHP = lockMechHP;
                return false;
            }
        }

        public static void IsThisACrime(ChestInteractionType interactionType)
        {
            if (IsValidCrimeLocation())
            {
                PlayerGPS.DiscoveredBuilding buildingData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
                DFLocation.BuildingTypes buildingType = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.buildingType;

                switch (interactionType)
                {
                    case ChestInteractionType.Lockpick:
                        GameManager.Instance.PlayerEntity.TallyCrimeGuildRequirements(true, 1);
                        if (LockpickDetectionCheck(buildingData, buildingType))
                            RegisterDetectedPunishment(buildingData, buildingType);
                        break;
                    case ChestInteractionType.Magic_Lockpick:
                        GameManager.Instance.PlayerEntity.TallyCrimeGuildRequirements(true, 1);
                        if (MagicLockpickDetectionCheck(buildingData, buildingType))
                            RegisterDetectedPunishment(buildingData, buildingType);
                        break;
                    case ChestInteractionType.Bash:
                    case ChestInteractionType.Magic_Bash:
                        RegisterDetectedPunishment(buildingData, buildingType);
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool IsValidCrimeLocation()
        {
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                return false;

            DFLocation.BuildingTypes bT = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.buildingType;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                if (IsValidShop(bT) || bT == DFLocation.BuildingTypes.Tavern || bT == DFLocation.BuildingTypes.Temple || bT == DFLocation.BuildingTypes.GuildHall ||
                    IsValidTownHouse(bT) || bT == DFLocation.BuildingTypes.Palace)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool LockpickDetectionCheck(PlayerGPS.DiscoveredBuilding buildingData, DFLocation.BuildingTypes buildingType)
        {
            int detectionChance = (20 + (buildingData.quality * 3)) * -1;
            int closedMod = BuildingOpenCheck(buildingData, buildingType) ? 0 : 30;
            int sneakChance = Sneak + Mathf.RoundToInt(PickP * 0.2f) + Mathf.RoundToInt(Agili * 0.6f) + Mathf.RoundToInt(Luck * 0.4f) + closedMod;

            Player.TallySkill(DFCareer.Skills.Stealth, 1);
            Player.TallySkill(DFCareer.Skills.Pickpocket, 1);

            if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(detectionChance + sneakChance, 5f, 90f))))
                return false;
            else
                return true;
        }

        public static bool MagicLockpickDetectionCheck(PlayerGPS.DiscoveredBuilding buildingData, DFLocation.BuildingTypes buildingType)
        {
            int detectionChance = (20 + (buildingData.quality * 3)) * -1;
            int closedMod = BuildingOpenCheck(buildingData, buildingType) ? 0 : 30;
            int sneakChance = Sneak + Mathf.RoundToInt(Illus * 0.2f) + Mathf.RoundToInt(Speed * 0.4f) + Mathf.RoundToInt(Luck * 0.6f) + closedMod;

            Player.TallySkill(DFCareer.Skills.Stealth, 1);

            if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(detectionChance + sneakChance, 5f, 90f))))
                return false;
            else
                return true;
        }

        public static void RegisterDetectedPunishment(PlayerGPS.DiscoveredBuilding buildingData, DFLocation.BuildingTypes bT)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            if (playerEntity != null)
            {
                if (IsValidShop(bT) || bT == DFLocation.BuildingTypes.Tavern || IsValidTownHouse(bT) || bT == DFLocation.BuildingTypes.Palace)
                {
                    DaggerfallUI.AddHUDText("You were detected...", 2f);
                    playerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                    playerEntity.SpawnCityGuards(true);
                }
                else if (bT == DFLocation.BuildingTypes.Temple)
                {
                    int factionID = buildingData.factionID;
                    FactionFile.FactionData factionData = playerEntity.FactionData.FactionDict[factionID];
                    string factionName = factionData.name;
                    DaggerfallUI.AddHUDText("You were detected, " + factionName + " disproves of this transgression...", 3f);
                    playerEntity.FactionData.ChangeReputation(factionID, -2, true);
                    playerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                    playerEntity.SpawnCityGuards(true);
                }
                else if (bT == DFLocation.BuildingTypes.GuildHall)
                {
                    int factionID = buildingData.factionID;

                    if (factionID == (int)FactionFile.FactionIDs.The_Mages_Guild || factionID == (int)FactionFile.FactionIDs.The_Fighters_Guild || OwnedByKnightlyOrder(factionID))
                    {
                        FactionFile.FactionData factionData = playerEntity.FactionData.FactionDict[factionID];
                        string factionName = factionData.name;
                        DaggerfallUI.AddHUDText("You were detected, " + factionName + " disproves of this transgression...", 3f);
                        playerEntity.FactionData.ChangeReputation(factionID, -2, true);
                        playerEntity.CrimeCommitted = PlayerEntity.Crimes.Theft;
                        playerEntity.SpawnCityGuards(true);
                    }
                    else if (factionID == (int)FactionFile.FactionIDs.The_Thieves_Guild)
                    {
                        DaggerfallUI.AddHUDText("You were detected, bad thieves bring shame to the entire guild...", 3f);
                        playerEntity.FactionData.ChangeReputation(factionID, -4, true);
                        FactionFile.FactionData factionData = playerEntity.FactionData.FactionDict[factionID];
                        if (factionData.rep < 0)
                        {
                            GameObjectHelper.CreateFoeSpawner(false, MobileTypes.Rogue, 2, 3, 8); // Make 2 instances so maybe they will spawn more quickly?
                            GameObjectHelper.CreateFoeSpawner(false, MobileTypes.Rogue, 2, 3, 8);
                        }
                    }
                    else if (factionID == (int)FactionFile.FactionIDs.The_Dark_Brotherhood)
                    {
                        DaggerfallUI.AddHUDText("You were detected, The Dark Brotherhood does not approve stealing from your own kin...", 3f);
                        playerEntity.FactionData.ChangeReputation(factionID, -4, true);
                        FactionFile.FactionData factionData = playerEntity.FactionData.FactionDict[factionID];
                        if (factionData.rep < 0)
                        {
                            GameObjectHelper.CreateFoeSpawner(false, MobileTypes.Assassin, 2, 3, 8); // Make 2 instances so maybe they will spawn more quickly?
                            GameObjectHelper.CreateFoeSpawner(false, MobileTypes.Assassin, 2, 3, 8);
                        }
                    }
                }
            }
        }

        public static bool BuildingOpenCheck(PlayerGPS.DiscoveredBuilding buildingData, DFLocation.BuildingTypes buildingType)
        {
            /*
             * Open Hours For Specific Places:
             * Temples, Dark Brotherhood, Thieves Guild: 24/7
             * All Other Guilds: 11:00 - 23:00
             * Fighters Guild & Mages Guild, Rank 6 = 24/7 Access
             * 
             * Alchemists: 07:00 - 22:00
             * Armorers: 09:00 - 19:00
             * Banks: 08:00 - 15:00
             * Bookstores: 	09:00 - 21:00
             * Clothing Stores: 10:00 - 19:00
             * Gem Stores: 09:00 - 18:00
             * General Stores + Furniture Stores: 06:00 - 23:00
             * Libraries: 09:00 - 23:00
             * Pawn Shops + Weapon Smiths: 09:00 - 20:00
            */

            int buildingInt = (int)buildingType;
            int hour = DaggerfallUnity.Instance.WorldTime.Now.Hour;
            IGuild guild = GameManager.Instance.GuildManager.GetGuild(buildingData.factionID);
            if (buildingType == DFLocation.BuildingTypes.GuildHall && (PlayerActivate.IsBuildingOpen(buildingType) || guild.HallAccessAnytime()))
                return true;
            if (buildingInt < 18)
                return PlayerActivate.IsBuildingOpen(buildingType);
            else if (buildingInt <= 22)
                return hour < 6 || hour > 18 ? false : true;
            else
                return true;
        }

        public static bool OwnedByKnightlyOrder(int factionID)
        {
            switch (factionID)
            {
                case (int)FactionFile.FactionIDs.The_Host_of_the_Horn:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Dragon:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Flame:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Hawk:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Owl:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Rose:
                case (int)FactionFile.FactionIDs.The_Knights_of_the_Wheel:
                case (int)FactionFile.FactionIDs.The_Order_of_the_Candle:
                case (int)FactionFile.FactionIDs.The_Order_of_the_Raven:
                case (int)FactionFile.FactionIDs.The_Order_of_the_Scarab:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValidTownHouse(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType) // Just including basically everything that I don't know what they represent, just in-case, can remove some if necessary when I do know more.
            {
                case DFLocation.BuildingTypes.AnyHouse:
                case DFLocation.BuildingTypes.Town4:
                case DFLocation.BuildingTypes.Town23:
                case DFLocation.BuildingTypes.HouseForSale:
                case DFLocation.BuildingTypes.Special1:
                case DFLocation.BuildingTypes.Special2:
                case DFLocation.BuildingTypes.Special3:
                case DFLocation.BuildingTypes.Special4:
                case DFLocation.BuildingTypes.House1:
                case DFLocation.BuildingTypes.House2:
                case DFLocation.BuildingTypes.House3:
                case DFLocation.BuildingTypes.House4:
                case DFLocation.BuildingTypes.House5:
                case DFLocation.BuildingTypes.House6:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item, DaggerfallUnityItem bashingWep, int wepSkillID) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                int itemMat = item.NativeMaterialValue;
                int weapMat = (bashingWep != null) ? bashingWep.NativeMaterialValue : -1;
                int matDiff = weapMat - itemMat;

                if (bashingWep == null && wepSkillID == (short)DFCareer.Skills.HandToHand)
                {
                    float conditionMod = (float)Mathf.Max(3, UnityEngine.Random.Range(3, 12 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (wepSkillID == (short)DFCareer.Skills.BluntWeapon)
                {
                    float conditionMod = (float)Mathf.Max(6, UnityEngine.Random.Range(6, 15 + Mathf.Clamp(Mathf.Round(matDiff / 2), -6, 6) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else
                {
                    float conditionMod = (float)Mathf.Max(4, UnityEngine.Random.Range(4, 13 + Mathf.Clamp(matDiff * 2, -6, 14) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                int itemMat = (item.NativeMaterialValue - 200 < 0) ? -3 : item.NativeMaterialValue - 200;
                int weapMat = (bashingWep != null) ? bashingWep.NativeMaterialValue : -1;
                int matDiff = weapMat - itemMat;

                if (bashingWep == null && wepSkillID == (short)DFCareer.Skills.HandToHand)
                {
                    float conditionMod = (float)Mathf.Max(3, UnityEngine.Random.Range(3, 12 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (wepSkillID == (short)DFCareer.Skills.BluntWeapon)
                {
                    float conditionMod = (float)Mathf.Max(6, UnityEngine.Random.Range(6, 15 + Mathf.Clamp(matDiff, -6, 12) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else
                {
                    float conditionMod = (float)Mathf.Max(5, UnityEngine.Random.Range(5, 13 + Mathf.Clamp(matDiff, -6, 10) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                float conditionMod = (float)Mathf.Max(12, UnityEngine.Random.Range(12, 23 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range(1, Mathf.Max(2, Mathf.CeilToInt(item.stackCount * 0.25f) + 1));
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item, int damOrDisin, int spellMag) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(40, UnityEngine.Random.Range(40, 61 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(15, UnityEngine.Random.Range(15, 17 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 50) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(40, UnityEngine.Random.Range(40, 61 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(10, UnityEngine.Random.Range(10, 13 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 50) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(90, UnityEngine.Random.Range(90, 110 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(40, UnityEngine.Random.Range(40, 43 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 100) + Mathf.RoundToInt(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range(Mathf.CeilToInt(item.stackCount * 0.55f), item.stackCount + 1);
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                float conditionMod = (float)Mathf.Max(18, UnityEngine.Random.Range(18, 38 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                float conditionMod = (float)Mathf.Max(15, UnityEngine.Random.Range(15, 35 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                float conditionMod = (float)Mathf.Max(70, UnityEngine.Random.Range(70, 110 + Mathf.RoundToInt(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range(Mathf.CeilToInt(item.stackCount * 0.35f), item.stackCount + 1);
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool RemoveOrDamageBasedOnCondition(LLCObject chest, DaggerfallUnityItem item, int damAmount)
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (damAmount < item.currentCondition)
            {
                item.currentCondition -= damAmount;
                return false;
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);
                return true;
            }
        }

        public static LootItemSturdiness DetermineLootItemSturdiness(DaggerfallUnityItem item)
        {
            if (item == null)
                return LootItemSturdiness.Unbreakable;

            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch (item.ItemGroup)
                {
                    case ItemGroups.UselessItems1:
                        return LootItemSturdiness.Very_Fragile;
                    case ItemGroups.Drugs:
                    case ItemGroups.MensClothing:
                    case ItemGroups.Books:
                    case ItemGroups.UselessItems2:
                    case ItemGroups.ReligiousItems:
                    case ItemGroups.Maps:
                    case ItemGroups.WomensClothing:
                    case ItemGroups.Paintings:
                    case ItemGroups.PlantIngredients1:
                    case ItemGroups.PlantIngredients2:
                    case ItemGroups.MiscellaneousIngredients1:
                    case ItemGroups.Jewellery:
                    case ItemGroups.MiscItems:
                        return LootItemSturdiness.Fragile;
                    case ItemGroups.Weapons:
                    case ItemGroups.Gems:
                    case ItemGroups.CreatureIngredients1:
                    case ItemGroups.CreatureIngredients2:
                    case ItemGroups.CreatureIngredients3:
                    case ItemGroups.MetalIngredients:
                    case ItemGroups.MiscellaneousIngredients2:
                    case ItemGroups.Currency:
                    default:
                        return LootItemSturdiness.Solid;
                    case ItemGroups.Armor:
                        return LootItemSturdiness.Resilient;
                }
            }
            else // Modded Item Templates
            {
                if (RepairToolsCheck && item.TemplateIndex >= 800 && item.TemplateIndex <= 805)
                {
                    if (item.TemplateIndex == (int)Tools.Whetstone) { return LootItemSturdiness.Solid; }
                    else if (item.TemplateIndex == (int)Tools.Sewing_Kit) { return LootItemSturdiness.Fragile; }
                    else if (item.TemplateIndex == (int)Tools.Armorers_Hammer) { return LootItemSturdiness.Solid; }
                    else if (item.TemplateIndex == (int)Tools.Jewelers_Pliers) { return LootItemSturdiness.Solid; }
                    else if (item.TemplateIndex == (int)Tools.Epoxy_Glue) { return LootItemSturdiness.Very_Fragile; }
                    else if (item.TemplateIndex == (int)Tools.Charging_Powder) { return LootItemSturdiness.Fragile; }
                    else { return LootItemSturdiness.Unbreakable; }
                }
                else if (JewelryAdditionsCheck && item.TemplateIndex >= 4700 && item.TemplateIndex <= 4714)
                {
                    if (item.TemplateIndex >= 4700 && item.TemplateIndex <= 4707) { return LootItemSturdiness.Fragile; }
                    else { return LootItemSturdiness.Solid; }
                }
                else if (ClimatesAndCaloriesCheck && item.TemplateIndex == (int)Supplies.Rations)
                {
                    return LootItemSturdiness.Fragile;
                }
                else if (SkillBooksCheck && item.TemplateIndex >= 551 && item.TemplateIndex <= 554)
                {
                    return LootItemSturdiness.Fragile;
                }
                else if (RolePlayRealismNewWeaponCheck && (item.TemplateIndex == (int)SmallWeapons.Archers_Axe || item.TemplateIndex == (int)SmallWeapons.Light_Flail))
                {
                    return LootItemSturdiness.Solid;
                }
                else if (RolePlayRealismNewArmorCheck && item.TemplateIndex >= 515 && item.TemplateIndex <= 526)
                {
                    return LootItemSturdiness.Resilient;
                }
                else { return LootItemSturdiness.Unbreakable; }
            }
        }

        public static void DetermineDestroyedLootRefuseType(DaggerfallUnityItem item, out DaggerfallUnityItem wasteItem, out int wasteAmount)
        {
            wasteAmount = 1;

            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch (item.ItemGroup)
                {
                    case ItemGroups.UselessItems1:
                        if (item.IsPotion) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlassFragments.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    case ItemGroups.Armor:
                        wasteItem = LLCItemBuilder.CreateScrapMaterial(WeaponMaterialTypes.None, (ArmorMaterialTypes)item.NativeMaterialValue); wasteAmount = GetWasteAmount(item); return;
                    case ItemGroups.Weapons:
                        if (item.TemplateIndex == (int)Weapons.Arrow) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemBrokenArrow.templateIndex); return; }
                        else { wasteItem = LLCItemBuilder.CreateScrapMaterial((WeaponMaterialTypes)item.NativeMaterialValue); wasteAmount = GetWasteAmount(item); return; }
                    case ItemGroups.MensClothing:
                    case ItemGroups.WomensClothing:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemTatteredCloth.templateIndex); return;
                    case ItemGroups.Books:
                    case ItemGroups.Maps:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemPaperShreds.templateIndex); wasteAmount = GetWasteAmount(item); return;
                    case ItemGroups.UselessItems2:
                        if (item.TemplateIndex == (int)UselessItems2.Oil || item.TemplateIndex == (int)UselessItems2.Lantern) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlassFragments.templateIndex); return; }
                        else if (item.TemplateIndex == (int)UselessItems2.Bandage) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemTatteredCloth.templateIndex); return; }
                        else if (item.TemplateIndex == (int)UselessItems2.Parchment) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemPaperShreds.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    case ItemGroups.Gems:
                    case ItemGroups.MiscellaneousIngredients2:
                    case ItemGroups.MetalIngredients:
                        if (item.TemplateIndex == (int)MiscellaneousIngredients2.Ivory) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemIvoryFragments.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemShinyRubble.templateIndex); return; }
                    case ItemGroups.Drugs:
                    case ItemGroups.PlantIngredients1:
                    case ItemGroups.PlantIngredients2:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemClumpofPlantMatter.templateIndex); return;
                    case ItemGroups.MiscellaneousIngredients1:
                        if (item.TemplateIndex >= 59 && item.TemplateIndex <= 64) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlassFragments.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemIvoryFragments.templateIndex); return; }
                    case ItemGroups.CreatureIngredients1:
                    case ItemGroups.CreatureIngredients2:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlobofGore.templateIndex); return;
                    case ItemGroups.CreatureIngredients3:
                        if (item.TemplateIndex == (int)CreatureIngredients3.Nymph_hair) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlobofGore.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemIvoryFragments.templateIndex); return; }
                    case ItemGroups.Jewellery:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemDestroyedJewelry.templateIndex); return;
                    case ItemGroups.MiscItems:
                        if (item.TemplateIndex == (int)MiscItems.Soul_trap) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemShinyRubble.templateIndex); return; }
                        else if (item.TemplateIndex == (int)MiscItems.Letter_of_credit || item.TemplateIndex == (int)MiscItems.Potion_recipe || item.TemplateIndex == (int)MiscItems.Map) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemPaperShreds.templateIndex); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    case ItemGroups.Currency:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemRuinedCoin.templateIndex); return;
                    default:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return;
                }
            }
            else // Modded Item Templates
            {
                if (RepairToolsCheck && item.TemplateIndex >= 800 && item.TemplateIndex <= 805)
                {
                    if (item.TemplateIndex == (int)Tools.Whetstone) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    else if (item.TemplateIndex == (int)Tools.Sewing_Kit) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    else if (item.TemplateIndex == (int)Tools.Armorers_Hammer) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    else if (item.TemplateIndex == (int)Tools.Jewelers_Pliers) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                    else if (item.TemplateIndex == (int)Tools.Epoxy_Glue) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemGlassFragments.templateIndex); return; }
                    else if (item.TemplateIndex == (int)Tools.Charging_Powder) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemShinyRubble.templateIndex); return; }
                    else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
                }
                else if (JewelryAdditionsCheck && item.TemplateIndex >= 4700 && item.TemplateIndex <= 4714)
                {
                    if (item.TemplateIndex >= 4700 && item.TemplateIndex <= 4707) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemDestroyedJewelry.templateIndex); return; }
                    else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemShinyRubble.templateIndex); return; }
                }
                else if (ClimatesAndCaloriesCheck && item.TemplateIndex == (int)Supplies.Rations)
                {
                    wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return;
                }
                else if (SkillBooksCheck && item.TemplateIndex >= 551 && item.TemplateIndex <= 554)
                {
                    wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemPaperShreds.templateIndex); wasteAmount = UnityEngine.Random.Range(3, 10 + 1); return;
                }
                else if (RolePlayRealismNewWeaponCheck && (item.TemplateIndex == (int)SmallWeapons.Archers_Axe || item.TemplateIndex == (int)SmallWeapons.Light_Flail))
                {
                    wasteItem = LLCItemBuilder.CreateScrapMaterial((WeaponMaterialTypes)item.NativeMaterialValue); wasteAmount = GetWasteAmount(item); return;
                }
                else if (RolePlayRealismNewArmorCheck && item.TemplateIndex >= 515 && item.TemplateIndex <= 526)
                {
                    wasteItem = LLCItemBuilder.CreateScrapMaterial(WeaponMaterialTypes.None, (ArmorMaterialTypes)item.NativeMaterialValue); wasteAmount = GetWasteAmount(item); return;
                }
                else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemUselessRefuse.templateIndex); return; }
            }
        }

        public static int GetWasteAmount(DaggerfallUnityItem item)
        {
            float luckMod = ((float)Luck / 200f) + 0.5f;

            if (item == null)
                return 1;

            if (item.ItemGroup == ItemGroups.Armor)
            {
                switch (item.TemplateIndex)
                {
                    case (int)Armor.Cuirass:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(20f * luckMod) + 1));
                    case (int)Armor.Gauntlets:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(6f * luckMod) + 1));
                    case (int)Armor.Greaves:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(12f * luckMod) + 1));
                    case (int)Armor.Left_Pauldron:
                    case (int)Armor.Right_Pauldron:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)Armor.Helm:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(12f * luckMod) + 1));
                    case (int)Armor.Boots:
                    case (int)Armor.Buckler:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)Armor.Round_Shield:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(14f * luckMod) + 1));
                    case (int)Armor.Kite_Shield:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(18f * luckMod) + 1));
                    case (int)Armor.Tower_Shield:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(25f * luckMod) + 1));
                    case (int)MediumArmor.Hauberk:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)MediumArmor.Chausses:
                    case (int)MediumArmor.Left_Spaulders:
                    case (int)MediumArmor.Right_Spaulders:
                    case (int)MediumArmor.Sollerets:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(5f * luckMod) + 1));
                    case (int)LightArmor.Jerkin:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(6f * luckMod) + 1));
                    case (int)LightArmor.Cuisse:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(4f * luckMod) + 1));
                    case (int)LightArmor.Helmet:
                    case (int)LightArmor.Boots:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(3f * luckMod) + 1));
                    case (int)LightArmor.Gloves:
                    case (int)LightArmor.Left_Vambrace:
                    case (int)LightArmor.Right_Vambrace:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(2f * luckMod) + 1));
                    default:
                        return 1;
                }
            }
            else if (item.ItemGroup == ItemGroups.Weapons)
            {
                switch (item.TemplateIndex)
                {
                    case (int)Weapons.Dagger:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(2f * luckMod) + 1));
                    case (int)Weapons.Tanto:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(3f * luckMod) + 1));
                    case (int)Weapons.Staff:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)SmallWeapons.Archers_Axe:
                    case (int)SmallWeapons.Light_Flail:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(6f * luckMod) + 1));
                    case (int)Weapons.Broadsword:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(13f * luckMod) + 1));
                    case (int)Weapons.Saber:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(9f * luckMod) + 1));
                    case (int)Weapons.Longsword:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)Weapons.Katana:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(8f * luckMod) + 1));
                    case (int)Weapons.Claymore:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(18f * luckMod) + 1));
                    case (int)Weapons.Dai_Katana:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(15f * luckMod) + 1));
                    case (int)Weapons.Mace:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)Weapons.Flail:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(16f * luckMod) + 1));
                    case (int)Weapons.Warhammer:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(14f * luckMod) + 1));
                    case (int)Weapons.Battle_Axe:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(10f * luckMod) + 1));
                    case (int)Weapons.War_Axe:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(14f * luckMod) + 1));
                    case (int)Weapons.Short_Bow:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(2f * luckMod) + 1));
                    case (int)Weapons.Long_Bow:
                        return Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.RoundToInt(4f * luckMod) + 1));
                    default:
                        return 1;
                }
            }
            else if (item.ItemGroup == ItemGroups.Books)
                return UnityEngine.Random.Range(3, 10 + 1);
            else
                return 1;
        }

        public static void ApplyLockPickAttemptCosts()
        {
            Player.TallySkill(DFCareer.Skills.Lockpicking, 1);
            int timePassed = 450 - (Speed * 3);
            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.RaiseTime(timePassed);
            Player.DecreaseFatigue(Mathf.CeilToInt(PlayerEntity.DefaultFatigueLoss * (timePassed / 60)));
        }

        public static void ApplyInspectionCosts()
        {
            Player.TallySkill(DFCareer.Skills.Lockpicking, 1);
            int timePassed = 1800 - (Speed * 12);
            DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.RaiseTime(timePassed);
            Player.DecreaseFatigue(Mathf.CeilToInt(PlayerEntity.DefaultFatigueLoss * (timePassed / 60)));
        }

        public static int CalculateHandToHandDamage()
        {
            int minBaseDamage = (Player.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand) / 10) + 1;
            int maxBaseDamage = (Player.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand) / 5) + 1;
            int damage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);
            damage += FormulaHelper.DamageModifier(Player.Stats.LiveStrength);
            return damage;
        }

        public static int CalculateWeaponDamage(DaggerfallUnityItem weapon)
        {
            if (weapon == null)
                return 1;

            int wepDamage = UnityEngine.Random.Range(weapon.GetBaseDamageMin(), weapon.GetBaseDamageMax() + 1);
            wepDamage += FormulaHelper.DamageModifier(Player.Stats.LiveStrength);
            wepDamage += weapon.GetWeaponMaterialModifier();
            return wepDamage;
        }

        public static int DaggerfallMatsToLLCValue(int nativeMaterialValue) // For determining "material difference" between weapon and LLC material estimated equivalent, mostly placeholder for now.
        {
            switch ((WeaponMaterialTypes)nativeMaterialValue)
            {
                default:
                case WeaponMaterialTypes.Iron:
                    return 0;
                case WeaponMaterialTypes.Steel:
                case WeaponMaterialTypes.Silver:
                    return 1;
                case WeaponMaterialTypes.Elven:
                case WeaponMaterialTypes.Dwarven:
                    return 2;
                case WeaponMaterialTypes.Mithril:
                    return 3;
                case WeaponMaterialTypes.Adamantium:
                case WeaponMaterialTypes.Ebony:
                    return 4;
                case WeaponMaterialTypes.Orcish:
                    return 5;
                case WeaponMaterialTypes.Daedric:
                    return 6;
            }
        }

        public static int ChestMaterialToDaggerfallValue(ChestMaterials chestMat)
        {
            switch (chestMat)
            {
                default:
                case ChestMaterials.Wood:
                    return -1;
                case ChestMaterials.Iron:
                    return 0;
                case ChestMaterials.Steel:
                    return 1;
                case ChestMaterials.Orcish:
                    return 5;
                case ChestMaterials.Mithril:
                    return 3;
                case ChestMaterials.Dwarven:
                    return 2;
                case ChestMaterials.Adamantium:
                    return 4;
                case ChestMaterials.Daedric:
                    return 6;
            }
        }

        public static int LockMaterialToDaggerfallValue(LockMaterials lockMat)
        {
            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    return -1;
                case LockMaterials.Iron:
                    return 0;
                case LockMaterials.Steel:
                    return 1;
                case LockMaterials.Orcish:
                    return 5;
                case LockMaterials.Mithril:
                    return 3;
                case LockMaterials.Dwarven:
                    return 2;
                case LockMaterials.Adamantium:
                    return 4;
                case LockMaterials.Daedric:
                    return 6;
            }
        }

        public static void ToggleChestShadowsOrCollision(GameObject chestGo)
        {
            Collider coll = chestGo.GetComponent<Collider>();
            MeshRenderer meshR = chestGo.GetComponent<MeshRenderer>();

            if (coll != null)
            {
                if (AllowChestCollision) { coll.isTrigger = false; }
                else { coll.isTrigger = true; }
            }

            if (meshR != null)
            {
                if (AllowChestShadows) { meshR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On; }
                else { meshR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; }
            }
        }

        public static void RotateBackToZero(Transform goTransform) // Rotates the transform of this gameobject back to the "0, 0, 0" positions.
        {
            float oriX = goTransform.rotation.x;
            float oriY = goTransform.rotation.y;
            float oriZ = goTransform.rotation.z;

            float corrX = oriX < 0 ? Mathf.Abs(oriX) : oriX * -1f;
            float corrY = oriY < 0 ? Mathf.Abs(oriY) : oriY * -1f;
            float corrZ = oriZ < 0 ? Mathf.Abs(oriZ) : oriZ * -1f;

            goTransform.Rotate(oriX + corrX, oriY + corrY, oriZ + corrZ);
        }

        public static void CloneLLCObjectProperties(LLCObject newObj, LLCObject oldObj) // Will have to remember to add to this when I add more properties/features later on.
        {
            newObj.LoadID = oldObj.LoadID;
            newObj.RecentInspectValues = oldObj.RecentInspectValues;
            newObj.IsLockJammed = oldObj.IsLockJammed;
            newObj.HasBeenBashed = oldObj.HasBeenBashed;
            newObj.HasBeenInspected = oldObj.HasBeenInspected;
            newObj.ChestMaterial = oldObj.ChestMaterial;
            newObj.ChestSturdiness = oldObj.ChestSturdiness;
            newObj.ChestMagicResist = oldObj.ChestMagicResist;
            newObj.ChestStartHP = oldObj.ChestStartHP;
            newObj.ChestCurrentHP = oldObj.ChestCurrentHP;
            newObj.LockMaterial = oldObj.LockMaterial;
            newObj.LockSturdiness = oldObj.LockSturdiness;
            newObj.LockMagicResist = oldObj.LockMagicResist;
            newObj.LockComplexity = oldObj.LockComplexity;
            newObj.JamResist = oldObj.JamResist;
            newObj.LockStartHP = oldObj.LockStartHP;
            newObj.LockCurrentHP = oldObj.LockCurrentHP;
            newObj.PicksAttempted = oldObj.PicksAttempted;
            newObj.LockMechStartHP = oldObj.LockMechStartHP;
            newObj.LockMechCurrentHP = oldObj.LockMechCurrentHP;
            newObj.AttachedLoot = new ItemCollection();
            newObj.AttachedLoot.TransferAll(oldObj.AttachedLoot);
        }

        public static AudioClip RollRandomAudioClip(AudioClip[] clips)
        {
            int randChoice = UnityEngine.Random.Range(0, clips.Length);
            AudioClip clip = clips[randChoice];

            if (clip == LastSoundPlayed)
            {
                if (randChoice == 0)
                    randChoice++;
                else if (randChoice == clips.Length - 1)
                    randChoice--;
                else
                    randChoice = CoinFlip() ? randChoice + 1 : randChoice - 1;

                clip = clips[randChoice];
            }
            LastSoundPlayed = clip;
            return clip;
        }

        public static AudioClip GetChestBashAudioClip(LLCObject chest, DaggerfallUnityItem weapon, bool bashedOpen, bool isHardBash)
        {
            short skillUsed = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(skillUsed);
            AudioClip clip = null;

            if (wepSkill == (int)Skills.HandToHand)
            {
                if (chest.ChestMaterial == ChestMaterials.Wood)
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenWoodChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(UnarmedHitWoodHardClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(UnarmedHitWoodLightClips);
                    }
                }
                else
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenMetalChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(UnarmedHitMetalClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(UnarmedHitMetalClips);
                    }
                }
            }
            else if (wepSkill == (int)Skills.BluntWeapon)
            {
                if (chest.ChestMaterial == ChestMaterials.Wood)
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenWoodChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(BluntHitWoodHardClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(BluntHitWoodLightClips);
                    }
                }
                else
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenMetalChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(BluntHitMetalHardClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(BluntHitMetalLightClips);
                    }
                }
            }
            else // Bladed weapons
            {
                if (chest.ChestMaterial == ChestMaterials.Wood)
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenWoodChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(BladeHitWoodHardClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(BladeHitWoodLightClips);
                    }
                }
                else
                {
                    if (bashedOpen)
                    {
                        clip = RollRandomAudioClip(BashOpenMetalChestClips);
                    }
                    else if (isHardBash)
                    {
                        clip = RollRandomAudioClip(BladeHitMetalHardClips);
                    }
                    else
                    {
                        clip = RollRandomAudioClip(BladeHitMetalLightClips);
                    }
                }
            }

            if (clip == null)
                clip = UnarmedHitWoodLightClips[0];

            return clip;
        }

        public static AudioClip GetLockBashAudioClip(LLCObject chest, bool bashedOff)
        {
            AudioClip clip = null;

            if (chest.LockMaterial == LockMaterials.Wood)
            {
                if (bashedOff)
                {
                    clip = RollRandomAudioClip(BashOffLockClips);
                }
                else
                {
                    AudioClip[] hitWoodLockClips = { HitMetalLockClips[2], BladeHitWoodLightClips[2], BladeHitWoodHardClips[0] };
                    clip = RollRandomAudioClip(hitWoodLockClips);
                }
            }
            else
            {
                if (bashedOff)
                {
                    clip = RollRandomAudioClip(BashOffLockClips);
                }
                else
                {
                    clip = RollRandomAudioClip(HitMetalLockClips);
                }
            }

            if (clip == null)
                clip = HitMetalLockClips[0];

            return clip;
        }

        public static AudioClip GetArrowHitChestAudioClip(LLCObject chest, bool bashedOpen)
        {
            AudioClip clip = null;

            if (chest.ChestMaterial == ChestMaterials.Wood)
            {
                if (bashedOpen)
                {
                    clip = BashOpenWoodChestClips[3];
                }
                else
                {
                    clip = RollRandomAudioClip(ArrowHitWoodClips);
                }
            }
            else
            {
                clip = RollRandomAudioClip(ArrowHitMetalClips);
            }

            if (clip == null)
                clip = ArrowHitMetalClips[0];

            return clip;
        }

        public static AudioClip GetSpellImpactChestAudioClip(LLCObject chest, bool blownOpen, bool disintegrated)
        {
            AudioClip clip = null;

            if (blownOpen)
            {
                if (disintegrated)
                {
                    clip = RollRandomAudioClip(ChestDisintegratedSpellClips);
                }
                else
                {
                    clip = RollRandomAudioClip(ChestBlownOpenSpellClips);
                }
            }
            else
            {
                clip = RollRandomAudioClip(ChestResistedSpellClips);
            }

            if (clip == null)
                clip = ChestResistedSpellClips[0];

            return clip;
        }

        public static AudioClip GetLockpickAttemptClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(LockpickAttemptClips);

            if (clip == null)
                clip = LockpickAttemptClips[0];

            return clip;
        }

        public static AudioClip GetLockpickJammedClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(LockpickJammedClips);

            if (clip == null)
                clip = LockpickJammedClips[0];

            return clip;
        }

        public static AudioClip GetLockAlreadyJammedClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(LockAlreadyJammedClips);

            if (clip == null)
                clip = LockAlreadyJammedClips[0];

            return clip;
        }

        public static AudioClip GetLockpickSuccessClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(LockpickSuccessfulClips);

            if (clip == null)
                clip = LockpickSuccessfulClips[0];

            return clip;
        }

        public static AudioClip GetMagicLockpickAttemptClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(MagicLockpickAttemptClips);

            if (clip == null)
                clip = MagicLockpickAttemptClips[0];

            return clip;
        }

        public static AudioClip GetMagicLockpickJammedClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(MagicLockpickJammedClips);

            if (clip == null)
                clip = MagicLockpickJammedClips[0];

            return clip;
        }

        public static AudioClip GetMagicLockAlreadyJammedClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(MagicLockAlreadyJammedClips);

            if (clip == null)
                clip = MagicLockAlreadyJammedClips[0];

            return clip;
        }

        public static AudioClip GetMagicLockpickSuccessClip()
        {
            AudioClip clip = null;

            clip = RollRandomAudioClip(MagicLockpickSuccessfulClips);

            if (clip == null)
                clip = MagicLockpickSuccessfulClips[0];

            return clip;
        }

        public static bool CoinFlip()
        {
            if (UnityEngine.Random.Range(0, 1 + 1) == 0)
                return false;
            else
                return true;
        }

        public static bool WithinMarginOfErrorPos(Vector3 value1, Vector3 value2, float xAcceptDif, float yAcceptDif, float zAcceptDif)
        {
            bool xEqual = Mathf.Abs(value1.x - value2.x) <= xAcceptDif;
            bool yEqual = Mathf.Abs(value1.y - value2.y) <= yAcceptDif;
            bool zEqual = Mathf.Abs(value1.z - value2.z) <= zAcceptDif;

            return xEqual && yEqual && zEqual;
        }

        public static T[] FillArray<T>(List<T> list, int start, int count, T value)
        {
            for (var i = start; i < start + count; i++)
            {
                list.Add(value);
            }

            return list.ToArray();
        }

        public static int PickOneOf(params int[] values) // Pango provided assistance in making this much cleaner way of doing the random value choice part, awesome.
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
}