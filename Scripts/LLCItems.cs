using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game;

namespace LockedLootContainers
{
    public class ItemThingy : DaggerfallUnityItem
    {
        public const int templateIndex = 4700;
        public const string baseName = "Thingy";

        public static int ringType = -1;

        public ItemThingy() : base(ItemGroups.Jewellery, templateIndex)
        {
            if (ringType >= 0)
                message = ringType;
            else
                message = 1;
                //message = JewelryAdditionsMain.GetRandomVariantType(true, true, 24);
                //message = Random.Range(1, 25);

            ringType = -1;

            switch (message)
            {
                case 1:
                    shortName = "Silver Ring";
                    value = 20;
					maxCondition = 250;
					enchantmentPoints = 600;
                    break;
                case 2:
                    shortName = "Gold Ring";
                    value = 80;
					maxCondition = 1000;
					enchantmentPoints = 300;
                    break;
                case 3:
                    shortName = "Silver Ruby Ring";
                    value = 235;
					maxCondition = 1000;
					enchantmentPoints = 2100;
                    break;
                case 4:
                    shortName = "Gold Ruby Ring";
                    value = 295;
					maxCondition = 2500;
					enchantmentPoints = 1050;
                    break;
                case 5:
                    shortName = "Silver Emerald Ring";
                    value = 320;
					maxCondition = 1315;
					enchantmentPoints = 3300;
                    break;
                case 6:
                    shortName = "Gold Emerald Ring";
                    value = 380;
					maxCondition = 3125;
					enchantmentPoints = 1650;
                    break;
                case 7:
                    shortName = "Silver Sapphire Ring";
                    value = 270;
					maxCondition = 1135;
					enchantmentPoints = 2550;
                    break;
                case 8:
                    shortName = "Gold Sapphire Ring";
                    value = 330;
					maxCondition = 2775;
					enchantmentPoints = 1275;
                    break;
                case 9:
                    shortName = "Silver Diamond Ring";
                    value = 420;
					maxCondition = 1750;
					enchantmentPoints = 4300;
                    break;
                case 10:
                    shortName = "Gold Diamond Ring";
                    value = 480;
					maxCondition = 4000;
					enchantmentPoints = 2150;
                    break;
                case 11:
                    shortName = "Silver Amethyst Ring";
                    value = 95;
					maxCondition = 515;
					enchantmentPoints = 1050;
                    break;
                case 12:
                    shortName = "Gold Amethyst Ring";
                    value = 155;
					maxCondition = 1525;
					enchantmentPoints = 525;
                    break;
                case 13:
                    shortName = "Silver Apatite Ring";
                    value = 45;
					maxCondition = 340;
					enchantmentPoints = 750;
                    break;
                case 14:
                    shortName = "Gold Apatite Ring";
                    value = 105;
					maxCondition = 1175;
					enchantmentPoints = 375;
                    break;
                case 15:
                    shortName = "Silver Aquamarine Ring";
                    value = 145;
					maxCondition = 690;
					enchantmentPoints = 1350;
                    break;
                case 16:
                    shortName = "Gold Aquamarine Ring";
                    value = 205;
					maxCondition = 1875;
					enchantmentPoints = 675;
                    break;
                case 17:
                    shortName = "Silver Garnet Ring";
                    value = 70;
					maxCondition = 425;
					enchantmentPoints = 900;
                    break;
                case 18:
                    shortName = "Gold Garnet Ring";
                    value = 130;
					maxCondition = 1350;
					enchantmentPoints = 450;
                    break;
                case 19:
                    shortName = "Silver Topaz Ring";
                    value = 120;
					maxCondition = 600;
					enchantmentPoints = 1200;
                    break;
                case 20:
                    shortName = "Gold Topaz Ring";
                    value = 180;
					maxCondition = 1700;
					enchantmentPoints = 600;
                    break;
                case 21:
                    shortName = "Silver Zircon Ring";
                    value = 195;
					maxCondition = 865;
					enchantmentPoints = 1650;
                    break;
                case 22:
                    shortName = "Gold Zircon Ring";
                    value = 255;
					maxCondition = 2225;
					enchantmentPoints = 825;
                    break;
                case 23:
                    shortName = "Silver Spinel Ring";
                    value = 170;
					maxCondition = 775;
					enchantmentPoints = 1500;
                    break;
                case 24:
                    shortName = "Gold Spinel Ring";
                    value = 230;
					maxCondition = 2050;
					enchantmentPoints = 750;
                    break;
                default:
                    shortName = "Broken Ring";
                    value = 1;
					maxCondition = 100;
					enchantmentPoints = 1;
                    break;
            }

            if (message % 2 == 0)
                weightInKg += 0.02f; // Gold

            if (message > 2)
                weightInKg += 0.01f; // Add 0.01 weight if jewelry item has a gemstone.

            currentCondition = maxCondition;

            CurrentVariant = message - 1;
        }

        public override int InventoryTextureArchive
        {
            get { return templateIndex; }
        }

        public override string ItemName
        {
            get { return shortName; }
        }

        public override string LongName
        {
            get { return shortName; }
        }

        /*private static string GetJewelryName(int message)
        {
            string material = "";
            string gemstone = "";

            if (message % 2 == 0)
                material = "Gold ";
            else
                material = "Silver ";

            gemstone = JewelryAdditionsMain.DetermineGemstone((int)Mathf.Floor(message / 2));

            return material + gemstone + baseName;
        }*/

        public override EquipSlots GetEquipSlot()
        {
            return GameManager.Instance.PlayerEntity.ItemEquipTable.GetFirstSlot(EquipSlots.Ring0, EquipSlots.Ring1);
        }

        public override int GetEnchantmentPower()
        {
            return enchantmentPoints;
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(ItemThingy).ToString();
            return data;
        }
    }
}

