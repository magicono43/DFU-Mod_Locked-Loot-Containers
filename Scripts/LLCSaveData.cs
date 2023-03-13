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
        public int chestHitPoints;
        public LockMaterials lockMaterial;
        public int lockSturdiness;
        public int lockMagicResist;
        public int lockComplexity;
        public int jamResist;
        public int lockHitPoints;
        public int picksAttempted;
        public int lockMechHitPoints;
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
        public Dictionary<ulong, OpenChestData> OpenChests;

        public Type SaveDataType
        {
            get { return typeof(LLCSaveData); }
        }

        public object NewSaveData()
        {
            LLCSaveData emptyData = new LLCSaveData();
            emptyData.ClosedChests = new Dictionary<ulong, ClosedChestData>();
            emptyData.OpenChests = new Dictionary<ulong, OpenChestData>();
            return emptyData;
        }

        public object GetSaveData()
        {
            Dictionary<ulong, ClosedChestData> closedChestEntries = new Dictionary<ulong, ClosedChestData>();
            Dictionary<ulong, OpenChestData> openChestEntries = new Dictionary<ulong, OpenChestData>();
            LLCObject[] closedChests = GameObject.FindObjectsOfType<LLCObject>();

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first.
            {
                for (int i = 0; i < closedChests.Length; i++)
                {
                    if (closedChests[i].LoadID <= 0)
                        continue;

                    ClosedChestData chest = new ClosedChestData
                    {
                        loadID = closedChests[i].LoadID,
                        currentPosition = closedChests[i].transform.position,
                        recentInspectValues = closedChests[i].RecentInspectValues,
                        isLockJammed = closedChests[i].IsLockJammed,
                        hasBeenBashed = closedChests[i].HasBeenBashed,
                        hasBeenInspected = closedChests[i].HasBeenInspected,
                        chestMaterial = closedChests[i].ChestMaterial,
                        chestSturdiness = closedChests[i].ChestSturdiness,
                        chestMagicResist = closedChests[i].ChestMagicResist,
                        chestHitPoints = closedChests[i].ChestHitPoints,
                        lockMaterial = closedChests[i].LockMaterial,
                        lockSturdiness = closedChests[i].LockSturdiness,
                        lockMagicResist = closedChests[i].LockMagicResist,
                        lockComplexity = closedChests[i].LockComplexity,
                        jamResist = closedChests[i].JamResist,
                        lockHitPoints = closedChests[i].LockHitPoints,
                        picksAttempted = closedChests[i].PicksAttempted,
                        lockMechHitPoints = closedChests[i].LockMechHitPoints,
                        attachedLoot = closedChests[i].AttachedLoot.SerializeItems()
                    };

                    if (chest != null)
                        closedChestEntries.Add(chest.loadID, chest);
                }

                DaggerfallLoot[] allLoot = GameObject.FindObjectsOfType<DaggerfallLoot>();
                List<DaggerfallLoot> openChestList = new List<DaggerfallLoot>();
                for (int i = 0; i < allLoot.Length; i++)
                {
                    if (allLoot[i].ContainerType == LootContainerTypes.Nothing && allLoot[i].ContainerImage == InventoryContainerImages.Chest && allLoot[i].customDrop == false)
                    {
                        openChestList.Add(allLoot[i]);
                    }
                }

                for (int i = 0; i < openChestList.Count; i++)
                {
                    if (openChestList[i].LoadID <= 0)
                        continue;

                    OpenChestData chest = new OpenChestData
                    {
                        loadID = openChestList[i].LoadID,
                        currentPosition = openChestList[i].transform.position,
                        textureArchive = openChestList[i].TextureArchive,
                        textureRecord = openChestList[i].TextureRecord,
                        items = openChestList[i].Items.SerializeItems()
                    };

                    if (chest != null)
                        openChestEntries.Add(chest.loadID, chest);
                }
            }

            LLCSaveData data = new LLCSaveData();
            data.ClosedChests = closedChestEntries;
            data.OpenChests = openChestEntries;
            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            LLCSaveData data = (LLCSaveData)dataIn;
            Dictionary<ulong, ClosedChestData> closedChests = data.ClosedChests;
            Dictionary<ulong, OpenChestData> openChests = data.OpenChests;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first.
            {
                foreach (KeyValuePair<ulong, ClosedChestData> chest in closedChests)
                {
                    if (chest.Value.loadID <= 0)
                        continue;

                    AddClosedChestToSceneFromSave(chest.Value);
                }

                foreach (KeyValuePair<ulong, OpenChestData> chest in openChests)
                {
                    if (chest.Value.loadID <= 0)
                        continue;

                    AddOpenChestToSceneFromSave(chest.Value);
                }

                if (closedChests.Count > 0 || openChests.Count > 0) // This is here to try and prevent the random static loot-piles from respawning when loading saves for whatever reason.
                {
                    DaggerfallLoot[] lootPiles = GameObject.FindObjectsOfType<DaggerfallLoot>();
                    List<GameObject> deleteQue = new List<GameObject>();

                    for (int i = 0; i < lootPiles.Length; i++)
                    {
                        if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure && lootPiles[i].ContainerImage == InventoryContainerImages.Chest)
                        {
                            DaggerfallBillboard pileBoard = lootPiles[i].GetComponent<DaggerfallBillboard>();
                            if (!pileBoard)
                                continue;

                            Vector3 pilePos = pileBoard.transform.position;

                            foreach (KeyValuePair<ulong, ClosedChestData> chest in closedChests)
                            {
                                if (LockedLootContainersMain.WithinMarginOfErrorPos(chest.Value.currentPosition, pilePos, 0.2f, 2.0f, 0.2f))
                                    deleteQue.Add(pileBoard.gameObject);
                            }

                            foreach (KeyValuePair<ulong, OpenChestData> chest in openChests)
                            {
                                if (LockedLootContainersMain.WithinMarginOfErrorPos(chest.Value.currentPosition, pilePos, 0.2f, 2.0f, 0.2f))
                                    deleteQue.Add(pileBoard.gameObject);
                            }
                        }
                    }

                    for (int i = 0; i < deleteQue.Count; i++)
                    {
                        GameObject.Destroy(deleteQue[i]);
                    }
                }
            }
        }

        public static void AddClosedChestToSceneFromSave(ClosedChestData data)
        {
            if (data.loadID <= 0)
                return;

            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, GameObjectHelper.GetBestParent());
            LLCObject chest = go.AddComponent<LLCObject>();

            go.transform.position = data.currentPosition;
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
            chest.ChestHitPoints = data.chestHitPoints;
            chest.LockMaterial = data.lockMaterial;
            chest.LockSturdiness = data.lockSturdiness;
            chest.LockMagicResist = data.lockMagicResist;
            chest.LockComplexity = data.lockComplexity;
            chest.JamResist = data.jamResist;
            chest.LockHitPoints = data.lockHitPoints;
            chest.PicksAttempted = data.picksAttempted;
            chest.LockMechHitPoints = data.lockMechHitPoints;
            chest.AttachedLoot = loot;

            Billboard chestBillboard = go.GetComponent<DaggerfallBillboard>();
            chestBillboard.SetMaterial(4733, 0);
            chestBillboard.transform.position = go.transform.position;
            chestBillboard.transform.position += new Vector3(0, chestBillboard.Summary.Size.y / 2, 0);
            GameObjectHelper.AlignBillboardToGround(go, chestBillboard.Summary.Size);
        }

        public static void AddOpenChestToSceneFromSave(OpenChestData data)
        {
            if (data.loadID <= 0)
                return;

            DaggerfallLoot openChest = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, data.currentPosition, GameObjectHelper.GetBestParent(), data.textureArchive, data.textureRecord, data.loadID, null, false);
            openChest.gameObject.name = GameObjectHelper.GetGoFlatName(data.textureArchive, data.textureRecord);
            openChest.Items.DeserializeItems(data.items);
            GameObject.Destroy(openChest.GetComponent<SerializableLootContainer>());
        }
    }
}