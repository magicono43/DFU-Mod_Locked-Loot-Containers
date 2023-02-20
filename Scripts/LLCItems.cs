using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game;

namespace LockedLootContainers
{
    public class ItemMatScraps : DaggerfallUnityItem
    {
        public const int templateIndex = 4721;

        public ItemMatScraps() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override int InventoryTextureRecord
        {
            get { return CurrentVariant; } // This is here, because otherwise items in group "UselessItems1" by default have their texture record set to 0 basically.
        }

        public override string ItemName
        {
            get { return shortName; }
        }

        public override string LongName
        {
            get { return shortName; }
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemMatScraps).ToString();
            return data;
        }
    }

    public class ItemBrokenArrow : DaggerfallUnityItem
    {
        public const int templateIndex = 4722;

        public ItemBrokenArrow() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemBrokenArrow).ToString();
            return data;
        }
    }

    public class ItemTatteredCloth : DaggerfallUnityItem
    {
        public const int templateIndex = 4723;

        public ItemTatteredCloth() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemTatteredCloth).ToString();
            return data;
        }
    }

    public class ItemGlassFragments : DaggerfallUnityItem
    {
        public const int templateIndex = 4724;

        public ItemGlassFragments() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemGlassFragments).ToString();
            return data;
        }
    }

    public class ItemPaperShreds : DaggerfallUnityItem
    {
        public const int templateIndex = 4725;

        public ItemPaperShreds() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemPaperShreds).ToString();
            return data;
        }
    }

    public class ItemShinyRubble : DaggerfallUnityItem
    {
        public const int templateIndex = 4726;

        public ItemShinyRubble() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemShinyRubble).ToString();
            return data;
        }
    }

    public class ItemIvoryFragments : DaggerfallUnityItem
    {
        public const int templateIndex = 4727;

        public ItemIvoryFragments() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemIvoryFragments).ToString();
            return data;
        }
    }

    public class ItemDestroyedJewelry : DaggerfallUnityItem
    {
        public const int templateIndex = 4728;

        public ItemDestroyedJewelry() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemDestroyedJewelry).ToString();
            return data;
        }
    }

    public class ItemRuinedCoin : DaggerfallUnityItem
    {
        public const int templateIndex = 4729;

        public ItemRuinedCoin() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemRuinedCoin).ToString();
            return data;
        }
    }

    public class ItemClumpofPlantMatter : DaggerfallUnityItem
    {
        public const int templateIndex = 4730;

        public ItemClumpofPlantMatter() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemClumpofPlantMatter).ToString();
            return data;
        }
    }

    public class ItemGlobofGore : DaggerfallUnityItem
    {
        public const int templateIndex = 4731;

        public ItemGlobofGore() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemGlobofGore).ToString();
            return data;
        }
    }

    public class ItemUselessRefuse : DaggerfallUnityItem
    {
        public const int templateIndex = 4732;

        public ItemUselessRefuse() : base(ItemGroups.UselessItems1, templateIndex)
        {
        }

        public override bool IsStackable()
        {
            return true;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemUselessRefuse).ToString();
            return data;
        }
    }

    public static class LLCItemBuilder
    {
        public static DaggerfallUnityItem CreateScrapMaterial(WeaponMaterialTypes wepMat = WeaponMaterialTypes.None, ArmorMaterialTypes armMat = ArmorMaterialTypes.None)
        {
            int scrapMat = 0;

            if (wepMat != WeaponMaterialTypes.None)
            {
                switch (wepMat)
                {
                    default:
                    case WeaponMaterialTypes.Iron:
                        scrapMat = 1; break;
                    case WeaponMaterialTypes.Steel:
                        scrapMat = 2; break;
                    case WeaponMaterialTypes.Silver:
                        scrapMat = 3; break;
                    case WeaponMaterialTypes.Elven:
                        scrapMat = 4; break;
                    case WeaponMaterialTypes.Dwarven:
                        scrapMat = 5; break;
                    case WeaponMaterialTypes.Mithril:
                        scrapMat = 6; break;
                    case WeaponMaterialTypes.Adamantium:
                        scrapMat = 7; break;
                    case WeaponMaterialTypes.Ebony:
                        scrapMat = 8; break;
                    case WeaponMaterialTypes.Orcish:
                        scrapMat = 9; break;
                    case WeaponMaterialTypes.Daedric:
                        scrapMat = 10; break;
                }
            }
            else if (armMat != ArmorMaterialTypes.None)
            {
                switch (armMat)
                {
                    default:
                    case ArmorMaterialTypes.Leather:
                        scrapMat = 0; break;
                    case ArmorMaterialTypes.Chain:
                    case ArmorMaterialTypes.Chain2:
                    case ArmorMaterialTypes.Iron:
                        scrapMat = 1; break;
                    case ArmorMaterialTypes.Steel:
                        scrapMat = 2; break;
                    case ArmorMaterialTypes.Silver:
                        scrapMat = 3; break;
                    case ArmorMaterialTypes.Elven:
                        scrapMat = 4; break;
                    case ArmorMaterialTypes.Dwarven:
                        scrapMat = 5; break;
                    case ArmorMaterialTypes.Mithril:
                        scrapMat = 6; break;
                    case ArmorMaterialTypes.Adamantium:
                        scrapMat = 7; break;
                    case ArmorMaterialTypes.Ebony:
                        scrapMat = 8; break;
                    case ArmorMaterialTypes.Orcish:
                        scrapMat = 9; break;
                    case ArmorMaterialTypes.Daedric:
                        scrapMat = 10; break;
                }
            }

            DaggerfallUnityItem item = ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemMatScraps.templateIndex);
            switch (scrapMat)
            {
                default:
                case 0:
                    item.shortName = "Leather Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Leather;
                    item.weightInKg = 0.1f;
                    item.value = 1;
                    item.CurrentVariant = 0; break;
                case 1:
                    item.shortName = "Iron Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Iron;
                    item.weightInKg = 0.2f;
                    item.value = 3;
                    item.CurrentVariant = 1; break;
                case 2:
                    item.shortName = "Steel Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Steel;
                    item.weightInKg = 0.25f;
                    item.value = 6;
                    item.CurrentVariant = 2; break;
                case 3:
                    item.shortName = "Silver Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Silver;
                    item.weightInKg = 0.2f;
                    item.value = 12;
                    item.CurrentVariant = 3; break;
                case 4:
                    item.shortName = "Elven Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Elven;
                    item.weightInKg = 0.2f;
                    item.value = 24;
                    item.CurrentVariant = 4; break;
                case 5:
                    item.shortName = "Dwarven Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Dwarven;
                    item.weightInKg = 0.15f;
                    item.value = 48;
                    item.CurrentVariant = 5; break;
                case 6:
                    item.shortName = "Mithril Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Mithril;
                    item.weightInKg = 0.2f;
                    item.value = 96;
                    item.CurrentVariant = 6; break;
                case 7:
                    item.shortName = "Adamantium Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Adamantium;
                    item.weightInKg = 0.2f;
                    item.value = 192;
                    item.CurrentVariant = 7; break;
                case 8:
                    item.shortName = "Ebony Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Ebony;
                    item.weightInKg = 0.1f;
                    item.value = 384;
                    item.CurrentVariant = 8; break;
                case 9:
                    item.shortName = "Orcish Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Orcish;
                    item.weightInKg = 0.2f;
                    item.value = 768;
                    item.CurrentVariant = 9; break;
                case 10:
                    item.shortName = "Daedric Scrap";
                    item.nativeMaterialValue = (int)ArmorMaterialTypes.Daedric;
                    item.weightInKg = 0.25f;
                    item.value = 1536;
                    item.CurrentVariant = 10; break;
            }
            item.message = scrapMat;

            return item;
        }
    }
}

