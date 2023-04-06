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
        public Quaternion currentRotation;
        public int[] recentInspectValues;
        public bool isLockJammed;
        public bool hasBeenBashed;
        public bool hasBeenInspected;
        public ChestMaterials chestMaterial;
        public int chestSturdiness;
        public int chestMagicResist;
        public int chestStartHP;
        public int chestCurrentHP;
        public LockMaterials lockMaterial;
        public int lockSturdiness;
        public int lockMagicResist;
        public int lockComplexity;
        public int jamResist;
        public int lockStartHP;
        public int lockCurrentHP;
        public int picksAttempted;
        public int lockMechStartHP;
        public int lockMechCurrentHP;
        public ItemData_v1[] attachedLoot;
    }

    public class OpenChestData
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public Quaternion currentRotation;
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

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon || GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
            {
                LLCObject[] closedChests = GameObject.FindObjectsOfType<LLCObject>();
                for (int i = 0; i < closedChests.Length; i++)
                {
                    if (closedChests[i].LoadID <= 0)
                        continue;

                    ClosedChestData chest = new ClosedChestData
                    {
                        loadID = closedChests[i].LoadID,
                        currentPosition = closedChests[i].transform.position,
                        currentRotation = closedChests[i].transform.rotation,
                        recentInspectValues = closedChests[i].RecentInspectValues,
                        isLockJammed = closedChests[i].IsLockJammed,
                        hasBeenBashed = closedChests[i].HasBeenBashed,
                        hasBeenInspected = closedChests[i].HasBeenInspected,
                        chestMaterial = closedChests[i].ChestMaterial,
                        chestSturdiness = closedChests[i].ChestSturdiness,
                        chestMagicResist = closedChests[i].ChestMagicResist,
                        chestStartHP = closedChests[i].ChestStartHP,
                        chestCurrentHP = closedChests[i].ChestCurrentHP,
                        lockMaterial = closedChests[i].LockMaterial,
                        lockSturdiness = closedChests[i].LockSturdiness,
                        lockMagicResist = closedChests[i].LockMagicResist,
                        lockComplexity = closedChests[i].LockComplexity,
                        jamResist = closedChests[i].JamResist,
                        lockStartHP = closedChests[i].LockStartHP,
                        lockCurrentHP = closedChests[i].LockCurrentHP,
                        picksAttempted = closedChests[i].PicksAttempted,
                        lockMechStartHP = closedChests[i].LockMechStartHP,
                        lockMechCurrentHP = closedChests[i].LockMechCurrentHP,
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
                        currentRotation = openChestList[i].transform.rotation,
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

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon || GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
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
                    MeshFilter[] validModels = GameObject.FindObjectsOfType<MeshFilter>();
                    DaggerfallLoot[] lootPiles = GameObject.FindObjectsOfType<DaggerfallLoot>();
                    List<GameObject> deleteQue = new List<GameObject>();

                    for (int i = 0; i < validModels.Length; i++) // Not necessarily my "complete" way I'd like to see this, but will see if it works well enough for now for the 3D model cases.
                    {
                        int modelID = -1;
                        bool validID = false;
                        string meshName = validModels[i].mesh.name;

                        if (meshName.Length > 0)
                        {
                            string properName = meshName.Substring(0, meshName.Length - 9);
                            validID = int.TryParse(properName, out modelID);
                        }

                        if (validID)
                        {
                            if (modelID >= 41811 && modelID <= 41813) // Vanilla DF Chest models
                                deleteQue.Add(validModels[i].gameObject);
                        }
                    }

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

            GameObject go = null;
            LLCObject chest = null;
            if (LockedLootContainersMain.ChestGraphicType == 0) // Use sprite based graphics for chests
            {
                go = GameObjectHelper.CreateDaggerfallBillboardGameObject(LockedLootContainersMain.ClosedChestSpriteID, 0, GameObjectHelper.GetBestParent());
                chest = go.AddComponent<LLCObject>();
            }
            else // Use 3D models for chests
            {
                GameObject usedModelPrefab = (LockedLootContainersMain.ChestGraphicType == 1) ? LockedLootContainersMain.Instance.LowPolyClosedChestPrefab : LockedLootContainersMain.Instance.HighPolyClosedChestPrefab;
                go = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(LockedLootContainersMain.ClosedChestModelID), GameObjectHelper.GetBestParent(), new Vector3(0, 0, 0));
                chest = go.AddComponent<LLCObject>();
                Collider col = go.AddComponent<BoxCollider>();
            }

            go.transform.position = data.currentPosition;
            go.transform.rotation = data.currentRotation;
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
            chest.ChestStartHP = data.chestStartHP;
            chest.ChestCurrentHP = data.chestCurrentHP;
            chest.LockMaterial = data.lockMaterial;
            chest.LockSturdiness = data.lockSturdiness;
            chest.LockMagicResist = data.lockMagicResist;
            chest.LockComplexity = data.lockComplexity;
            chest.JamResist = data.jamResist;
            chest.LockStartHP = data.lockStartHP;
            chest.LockCurrentHP = data.lockCurrentHP;
            chest.PicksAttempted = data.picksAttempted;
            chest.LockMechStartHP = data.lockMechStartHP;
            chest.LockMechCurrentHP = data.lockMechCurrentHP;
            chest.AttachedLoot = loot;

            if (LockedLootContainersMain.ChestGraphicType == 0) // Use sprite based graphics for chests
            {
                Billboard chestBillboard = go.GetComponent<DaggerfallBillboard>();
                chestBillboard.SetMaterial(LockedLootContainersMain.ClosedChestSpriteID, 0);
                chestBillboard.transform.position = go.transform.position;
            }
        }

        public static void AddOpenChestToSceneFromSave(OpenChestData data)
        {
            if (data.loadID <= 0)
                return;

            if (data.textureArchive == LockedLootContainersMain.SmashedChestSpriteID || data.textureArchive == LockedLootContainersMain.DisintegratedChestSpriteID || data.textureRecord == LockedLootContainersMain.SmashedChestModelID || data.textureRecord == LockedLootContainersMain.DisintegratedChestModelID)
            {
                if (LockedLootContainersMain.ChestGraphicType == 0) // Use sprite based graphics for chests
                {
                    DaggerfallLoot openChest = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, data.currentPosition, GameObjectHelper.GetBestParent(), data.textureArchive, data.textureRecord, data.loadID, null, false);
                    openChest.gameObject.name = GameObjectHelper.GetGoFlatName(data.textureArchive, data.textureRecord);
                    openChest.Items.DeserializeItems(data.items);
                    GameObject.Destroy(openChest.GetComponent<SerializableLootContainer>());
                }
                else // Use 3D models for chests
                {
                    GameObject usedModelPrefab = null;
                    if (data.textureRecord == LockedLootContainersMain.DisintegratedChestModelID)
                        usedModelPrefab = (LockedLootContainersMain.ChestGraphicType == 1) ? LockedLootContainersMain.Instance.LowPolyDisintegratedChestPrefab : LockedLootContainersMain.Instance.HighPolyDisintegratedChestPrefab;
                    else
                        usedModelPrefab = (LockedLootContainersMain.ChestGraphicType == 1) ? LockedLootContainersMain.Instance.LowPolySmashedChestPrefab : LockedLootContainersMain.Instance.HighPolySmashedChestPrefab;

                    GameObject go = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName((uint)data.textureRecord), GameObjectHelper.GetBestParent(), data.currentPosition);
                    go.transform.rotation = data.currentRotation;
                    LockedLootContainersMain.RotateBackToZero(go.transform);
                    Collider col = go.AddComponent<BoxCollider>();
                    DaggerfallLoot chestLoot = go.AddComponent<DaggerfallLoot>();
                    if (chestLoot)
                    {
                        chestLoot.ContainerType = LootContainerTypes.Nothing;
                        chestLoot.ContainerImage = InventoryContainerImages.Chest;
                        chestLoot.LoadID = data.loadID;
                        if (data.textureArchive == LockedLootContainersMain.SmashedChestSpriteID || data.textureRecord == LockedLootContainersMain.SmashedChestModelID)
                            chestLoot.TextureRecord = LockedLootContainersMain.SmashedChestModelID;
                        else
                            chestLoot.TextureRecord = LockedLootContainersMain.DisintegratedChestModelID;
                        chestLoot.Items.DeserializeItems(data.items);
                    }
                }
            }
            else if (data.items.Length == 0)
            {
                if (LockedLootContainersMain.ChestGraphicType == 0) // Use sprite based graphics for chests
                {
                    DaggerfallLoot openChest = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, data.currentPosition, GameObjectHelper.GetBestParent(), data.textureArchive, data.textureRecord, data.loadID, null, false);
                    openChest.gameObject.name = GameObjectHelper.GetGoFlatName(data.textureArchive, data.textureRecord);
                    openChest.Items.DeserializeItems(data.items);
                    GameObject.Destroy(openChest.GetComponent<SerializableLootContainer>());
                }
                else // Use 3D models for chests
                {
                    GameObject usedModelPrefab = (LockedLootContainersMain.ChestGraphicType == 1) ? LockedLootContainersMain.Instance.LowPolyOpenEmptyChestPrefab : LockedLootContainersMain.Instance.HighPolyOpenEmptyChestPrefab;
                    GameObject go = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(LockedLootContainersMain.OpenEmptyChestModelID), GameObjectHelper.GetBestParent(), data.currentPosition);
                    go.transform.rotation = data.currentRotation;
                    Collider col = go.AddComponent<BoxCollider>();
                    DaggerfallLoot chestLoot = go.AddComponent<DaggerfallLoot>();
                    if (chestLoot)
                    {
                        chestLoot.ContainerType = LootContainerTypes.Nothing;
                        chestLoot.ContainerImage = InventoryContainerImages.Chest;
                        chestLoot.LoadID = data.loadID;
                        chestLoot.TextureRecord = LockedLootContainersMain.OpenEmptyChestModelID;
                        chestLoot.Items.DeserializeItems(data.items);
                    }
                }
            }
            else
            {
                if (LockedLootContainersMain.ChestGraphicType == 0) // Use sprite based graphics for chests
                {
                    DaggerfallLoot openChest = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, data.currentPosition, GameObjectHelper.GetBestParent(), data.textureArchive, data.textureRecord, data.loadID, null, false);
                    openChest.gameObject.name = GameObjectHelper.GetGoFlatName(data.textureArchive, data.textureRecord);
                    openChest.Items.DeserializeItems(data.items);
                    GameObject.Destroy(openChest.GetComponent<SerializableLootContainer>());
                }
                else // Use 3D models for chests
                {
                    GameObject usedModelPrefab = (LockedLootContainersMain.ChestGraphicType == 1) ? LockedLootContainersMain.Instance.LowPolyOpenFullChestPrefab : LockedLootContainersMain.Instance.HighPolyOpenFullChestPrefab;
                    GameObject go = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(LockedLootContainersMain.OpenFullChestModelID), GameObjectHelper.GetBestParent(), data.currentPosition);
                    go.transform.rotation = data.currentRotation;
                    Collider col = go.AddComponent<BoxCollider>();
                    DaggerfallLoot chestLoot = go.AddComponent<DaggerfallLoot>();
                    if (chestLoot)
                    {
                        chestLoot.ContainerType = LootContainerTypes.Nothing;
                        chestLoot.ContainerImage = InventoryContainerImages.Chest;
                        chestLoot.LoadID = data.loadID;
                        chestLoot.TextureRecord = LockedLootContainersMain.OpenFullChestModelID;
                        chestLoot.Items.DeserializeItems(data.items);
                    }
                }
            }
        }
    }
}