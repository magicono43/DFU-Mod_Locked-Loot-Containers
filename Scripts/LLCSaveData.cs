using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Serialization;

namespace LockedLootContainers
{
    public class ClosedChestData
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public int[] recentInspectValues;
        public bool isLockJammed;
        public bool hasBeenBashed;
        public bool hasBeenInspected;
        public ChestMaterials chestMaterial;
        public int chestSturdiness;
        public int chestMagicResist;
        public LockMaterials lockMaterial;
        public int lockSturdiness;
        public int lockMagicResist;
        public int lockComplexity;
        public int jamResist;
        public int picksAttempted;
        public int lockBashedLightTimes;
        public int lockBashedHardTimes;
        public int chestBashedLightTimes;
        public int chestBashedHardTimes;
        public ItemData_v1[] attachedLoot;
    }

    public class OpenChestData
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public int textureArchive;
        public int textureRecord;
        public ItemData_v1[] items;
    }

    [FullSerializer.fsObject("v1")]
    public class LLCSaveData : IHasModSaveData
    {
        public Dictionary<ulong, ClosedChestData> ClosedChests;
        public Dictionary<ulong, OpenChestData> OpenChests; // Continue work on this tomorrow, adding the custom loot-pile save-data and save-loading logic stuff, also more bug-fixing with saves, etc.

        public Type SaveDataType
        {
            get { return typeof(LLCSaveData); }
        }

        public object NewSaveData()
        {
            LLCSaveData emptyData = new LLCSaveData();
            emptyData.ClosedChests = new Dictionary<ulong, ClosedChestData>();
            return emptyData;
        }

        public object GetSaveData()
        {
            Dictionary<ulong, ClosedChestData> chestEntries = new Dictionary<ulong, ClosedChestData>();
            LLCObject[] chests = GameObject.FindObjectsOfType<LLCObject>();

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                for (int i = 0; i < chests.Length; i++)
                {
                    if (chests[i].LoadID <= 0)
                        continue;

                    ClosedChestData chest = new ClosedChestData
                    {
                        loadID = chests[i].LoadID,
                        currentPosition = chests[i].transform.position,
                        recentInspectValues = chests[i].RecentInspectValues,
                        isLockJammed = chests[i].IsLockJammed,
                        hasBeenBashed = chests[i].HasBeenBashed,
                        hasBeenInspected = chests[i].HasBeenInspected,
                        chestMaterial = chests[i].ChestMaterial,
                        chestSturdiness = chests[i].ChestSturdiness,
                        chestMagicResist = chests[i].ChestMagicResist,
                        lockMaterial = chests[i].LockMaterial,
                        lockSturdiness = chests[i].LockSturdiness,
                        lockMagicResist = chests[i].LockMagicResist,
                        lockComplexity = chests[i].LockComplexity,
                        jamResist = chests[i].JamResist,
                        picksAttempted = chests[i].PicksAttempted,
                        lockBashedLightTimes = chests[i].LockBashedLightTimes,
                        lockBashedHardTimes = chests[i].LockBashedHardTimes,
                        chestBashedLightTimes = chests[i].ChestBashedLightTimes,
                        chestBashedHardTimes = chests[i].ChestBashedHardTimes,
                        attachedLoot = chests[i].AttachedLoot.SerializeItems()
                    };

                    if (chest != null)
                        chestEntries.Add(chest.loadID, chest);
                }
            }

            LLCSaveData data = new LLCSaveData();
            data.ClosedChests = chestEntries;
            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            LLCSaveData data = (LLCSaveData)dataIn;
            Dictionary<ulong, ClosedChestData> chests = data.ClosedChests;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                DaggerfallLoot[] allLoot = UnityEngine.Resources.FindObjectsOfTypeAll<DaggerfallLoot>();
                List<DaggerfallLoot> pileList = new List<DaggerfallLoot>();

                for (int i = 0; i < allLoot.Length; i++)
                {
                    if (allLoot[i].ContainerType == LootContainerTypes.RandomTreasure)
                    {
                        pileList.Add(allLoot[i]);
                    }
                }
                DaggerfallLoot[] lootPiles = pileList.ToArray();

                foreach (KeyValuePair<ulong, ClosedChestData> chest in chests)
                {
                    if (chest.Value.loadID <= 0)
                        continue;

                    GameObject obj = RecreateChestFromSaveData(chest.Value, lootPiles);

                    if (obj == null)
                        continue;

                    LockedLootContainersMain.AddChestToSceneFromSave(obj);
                }
            }
        }

        public static GameObject RecreateChestFromSaveData(ClosedChestData data, DaggerfallLoot[] lootPiles)
        {
            if (data.loadID <= 0)
                return null;

            DaggerfallLoot parentPile = null;
            for (int i = 0; i < lootPiles.Length; i++)
            {
                if (data.loadID == lootPiles[i].LoadID)
                {
                    parentPile = lootPiles[i];
                    break;
                }
            }

            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, (parentPile != null) ? parentPile.transform.parent.parent : GameObjectHelper.GetBestParent());
            LLCObject chest = go.AddComponent<LLCObject>();

            ItemCollection loot = new ItemCollection();
            loot.DeserializeItems(data.attachedLoot);

            chest.LoadID = data.loadID;
            chest.RecentInspectValues = data.recentInspectValues;
            chest.IsLockJammed = data.isLockJammed;
            chest.HasBeenBashed = data.hasBeenBashed;
            chest.HasBeenInspected = data.hasBeenInspected;
            chest.ChestMaterial = data.chestMaterial;
            chest.ChestSturdiness = data.chestSturdiness;
            chest.ChestMagicResist = data.chestMagicResist;
            chest.LockMaterial = data.lockMaterial;
            chest.LockSturdiness = data.lockSturdiness;
            chest.LockMagicResist = data.lockMagicResist;
            chest.LockComplexity = data.lockComplexity;
            chest.JamResist = data.jamResist;
            chest.PicksAttempted = data.picksAttempted;
            chest.LockBashedLightTimes = data.lockBashedLightTimes;
            chest.LockBashedHardTimes = data.lockBashedHardTimes;
            chest.ChestBashedLightTimes = data.chestBashedLightTimes;
            chest.ChestBashedHardTimes = data.chestBashedHardTimes;
            chest.AttachedLoot = loot;

            if (parentPile != null)
                go.transform.position = parentPile.transform.position;
            else
                go.transform.position = data.currentPosition;

            return go;
        }
    }
}