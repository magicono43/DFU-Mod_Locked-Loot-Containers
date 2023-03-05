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
    public class ChestData
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
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

    [FullSerializer.fsObject("v1")]
    public class LLCSaveData : IHasModSaveData
    {
        public Dictionary<ulong, ChestData> Chests;

        public Type SaveDataType
        {
            get { return typeof(LLCSaveData); }
        }

        public object NewSaveData()
        {
            LLCSaveData emptyData = new LLCSaveData();
            emptyData.Chests = new Dictionary<ulong, ChestData>();
            return emptyData;
        }

        public object GetSaveData()
        {
            Dictionary<ulong, ChestData> chestEntries = new Dictionary<ulong, ChestData>();
            LLCObject[] chests = GameObject.FindObjectsOfType<LLCObject>();

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                for (int i = 0; i < chests.Length; i++)
                {
                    if (chests[i].LoadID <= 0)
                        continue;

                    ChestData chest = new ChestData
                    {
                        loadID = chests[i].LoadID,
                        currentPosition = chests[i].transform.position,
                        localPosition = chests[i].transform.localPosition,
                        localRotation = chests[i].transform.localRotation,
                        localScale = chests[i].transform.localScale,
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
            data.Chests = chestEntries;
            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            LLCSaveData data = (LLCSaveData)dataIn;
            Dictionary<ulong, ChestData> chests = data.Chests;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                foreach (KeyValuePair<ulong, ChestData> chest in chests)
                {
                    if (chest.Value.loadID <= 0)
                        continue;

                    GameObject obj = RecreateChestFromSaveData(chest.Value);

                    if (obj == null)
                        continue;

                    LockedLootContainersMain.AddChestToSceneFromSave(obj);
                }
            }
        }

        public static GameObject RecreateChestFromSaveData(ChestData data)
        {
            if (data.loadID <= 0)
                return null;

            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, GameObjectHelper.GetBestParent());
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
            go.transform.position = data.currentPosition;
            go.transform.localPosition = data.localPosition;
            go.transform.localRotation = data.localRotation;
            go.transform.localScale = data.localScale;

            return go;
        }
    }
}