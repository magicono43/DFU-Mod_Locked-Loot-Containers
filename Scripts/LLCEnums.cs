namespace LockedLootContainers
{
    /// <summary>
    /// Types of materials chests can be made from.
    /// </summary>
    public enum ChestMaterials
    {
        None,
        Wood,
        Iron,
        Steel,
        Orcish,
        Mithril,
        Dwarven,
        Adamantium,
        Daedric,
    }

    /// <summary>
    /// Types of materials locks can be made from.
    /// </summary>
    public enum LockMaterials
    {
        None,
        Wood,
        Iron,
        Steel,
        Orcish,
        Mithril,
        Dwarven,
        Adamantium,
        Daedric,
    }

    /// <summary>
    /// Types of materials that are allowed to generate in certain context, such as wood not being allowed in volcanic caves, but daedric being allowed, etc.
    /// Have this for now, since the flag based enum stuff was causing me unnecessary confusion it seemed, so sticking with what I know for now atleast.
    /// </summary>
    public enum PermittedMaterials
    {
        Wood,
        Iron,
        Steel,
        Orcish,
        Mithril,
        Dwarven,
        Adamantium,
        Daedric,
    }

    /// <summary>
    /// Used to determine how clear or unclear the information given is when inspecting/identifying a chest.
    /// </summary>
    public enum InfoVagueness
    {
        Unknown = 0,
        Bare = 1,
        Simple = 2,
        Vivid = 3,
        Complete = 4,
    }

    /// <summary>
    /// Used to characterize individual item and item groups into different levels of "sturdiness", basically to determine how bashing a chest damages said items and to what degree.
    /// </summary>
    public enum LootItemSturdiness
    {
        Very_Fragile = 0,
        Fragile = 1,
        Solid = 2,
        Resilient = 3,
        Unbreakable = 4,
    }

    /// <summary>
    /// Used to determine what type of interaction the player is "using" on a chest, right now mainly to assist with determining if a crime has been committed or not.
    /// </summary>
    public enum ChestInteractionType
    {
        None = -1,
        Inspect = 0,
        Lockpick = 1,
        Magic_Lockpick = 2,
        Bash = 3,
        Magic_Bash = 4,
    }

    /// <summary>
    /// A list of all the body-parts/equip-slots that can be used to determine the parts needed when generating an "equipment set" as loot.
    /// </summary>
    public enum EquipSetSlots
    {
        Head = 0,
        RightArm = 1,
        LeftArm = 2,
        Chest = 3,
        Hands = 4,
        Legs = 5,
        Feet = 6,
        MainHand = 7,
        OffHand = 8,
        BothHands = 9,
    }

    /// <summary>
    /// The vanilla item groups that this mod will be checking for and caring about during loot generation.
    /// </summary>
    public enum ChestLootItemGroups
    {
        // Yeah, I think I'm just going to make my own item groups and then use those for my own mod implementation for the loot generation.
        Drugs = 0,
        LightArmor = 1,
        MediumArmor = 2,
        HeavyArmor = 3,
        Shields = 4,
        SmallWeapons = 5,
        MediumWeapons = 6,
        LargeWeapons = 7,
        MensClothing = 8,
        WomensClothing = 9,
        Books = 10,
        Gems = 11,
        Jewelry = 12,
        Tools = 13,
        Lights = 14,
        Supplies = 15,
        BlessedItems = 16,
        ReligiousStuff = 17,
        GenericPlants = 18,
        ColdClimatePlants = 19,
        WarmClimatePlants = 20,
        CommonCreatureParts = 21,
        UncommonCreatureParts = 22,
        RareCreatureParts = 23,
        LiquidSolvents = 24,
        TraceMetals = 25,
    }

    /// <summary>
    /// Includes the vanilla "drug" items, but will likely include modded drug items as well eventually.
    /// </summary>
    public enum Drugs
    {
        Indulcet = 78,
        Sursum = 79,
        Quaesto_Vil = 80,
        Aegrotat = 81,
    }

    /// <summary>
    /// Includes the light/leather armor added by Roleplay Realism: Items.
    /// </summary>
    public enum LightArmor
    {
        Jerkin = 520,
        Cuisse = 521,
        Helmet = 522,
        Boots = 523,
        Gloves = 524,
        Left_Vambrace = 525,
        Right_Vambrace = 526,
    }

    /// <summary>
    /// Includes the medium/chain armor added by Roleplay Realism: Items.
    /// </summary>
    public enum MediumArmor
    {
        Hauberk = 515,
        Chausses = 516,
        Left_Spaulders = 517,
        Right_Spaulders = 518,
        Sollerets = 519,
    }

    /// <summary>
    /// Includes the vanilla item IDs for the body covering armor pieces for now atleast.
    /// </summary>
    public enum HeavyArmor
    {
        Cuirass = 102,
        Gauntlets = 103,
        Greaves = 104,
        Left_Pauldron = 105,
        Right_Pauldron = 106,
        Helm = 107,
        Boots = 108,
    }

    /// <summary>
    /// Includes the vanilla item IDs for the shield types in the game.
    /// </summary>
    public enum Shields
    {
        Buckler = 109,
        Round_Shield = 110,
        Kite_Shield = 111,
        Tower_Shield = 112,
    }

    /// <summary>
    /// Includes weapons of a size that would be able to fit in hidden chest compartments, be it vanilla or modded, likely mostly going to be short-blades.
    /// </summary>
    public enum SmallWeapons
    {
        Dagger = 113,
        Tanto = 114,
        Shortsword = 116,
        Wakazashi = 117,
        Wand = 140, // I'd honestly consider a wand closer to a potential weapon than a piece of jewelry, so I'm going to put it here for now.
        Archers_Axe = 513,
        Light_Flail = 514,
    }

    /// <summary>
    /// Includes weapons of a size that would be able to fit in one hand, but too large to fit in hidden chest compartments, be it vanilla or modded, likely going to cover most weapons.
    /// </summary>
    public enum MediumWeapons
    {
        Broadsword = 118,
        Saber = 119,
        Longsword = 120,
        Katana = 121,
        Mace = 124,
        Battle_Axe = 127,
        Short_Bow = 129,
        Arrow = 131,
    }

    /// <summary>
    /// Includes weapons of a size that would be only be usable with two hands, be it vanilla or modded.
    /// </summary>
    public enum LargeWeapons
    {
        Staff = 115,
        Claymore = 122,
        Dai_Katana = 123,
        Flail = 125,
        Warhammer = 126,
        War_Axe = 128,
        Long_Bow = 130,
    }

    /// <summary>
    /// Includes all clothing items fitted for male character, be it vanilla or modded.
    /// </summary>
    public enum MensClothing
    {
        Straps = 141,
        Armbands = 142,
        Kimono = 143,
        Fancy_Armbands = 144,
        Sash = 145,
        Eodoric = 146,
        Shoes = 147,
        Tall_Boots = 148,
        Boots = 149,
        Sandals = 150,
        Casual_pants = 151,
        Breeches = 152,
        Short_skirt = 153,
        Casual_cloak = 154,
        Formal_cloak = 155,
        Khajiit_suit = 156,
        Dwynnen_surcoat = 157,
        Short_tunic = 158,
        Formal_tunic = 159,
        Toga = 160,
        Reversible_tunic = 161,
        Loincloth = 162,
        Plain_robes = 163,
        Priest_robes = 164,
        Short_shirt = 165,
        Short_shirt_with_belt = 166,
        Long_shirt = 167,
        Long_shirt_with_belt = 168,
        Short_shirt_closed_top = 169,
        Short_shirt_closed_top2 = 170,
        Long_shirt_closed_top = 171,
        Long_shirt_closed_top2 = 172,
        Open_Tunic = 173,
        Wrap = 174,
        Long_Skirt = 175,
        Anticlere_Surcoat = 176,
        Challenger_Straps = 177,
        Short_shirt_unchangeable = 178,
        Long_shirt_unchangeable = 179,
        Vest = 180,
        Champion_straps = 181,
    }

    /// <summary>
    /// Includes all clothing items fitted for female character, be it vanilla or modded.
    /// </summary>
    public enum WomensClothing
    {
        Brassier = 182,
        Formal_brassier = 183,
        Peasant_blouse = 184,
        Eodoric = 185,
        Shoes = 186,
        Tall_boots = 187,
        Boots = 188,
        Sandals = 189,
        Casual_pants = 190,
        Casual_cloak = 191,
        Formal_cloak = 192,
        Khajiit_suit = 193,
        Formal_eodoric = 194,
        Evening_gown = 195,
        Day_gown = 196,
        Casual_dress = 197,
        Strapless_dress = 198,
        Loincloth = 199,
        Plain_robes = 200,
        Priestess_robes = 201,
        Short_shirt = 202,
        Short_shirt_belt = 203,
        Long_shirt = 204,
        Long_shirt_belt = 205,
        Short_shirt_closed = 206,
        Short_shirt_closed_belt = 207,
        Long_shirt_closed = 208,
        Long_shirt_closed_belt = 209,
        Open_tunic = 210,
        Wrap = 211,
        Long_skirt = 212,
        Tights = 213,
        Short_shirt_unchangeable = 214,
        Long_shirt_unchangeable = 215,
        Vest = 216,
    }

    /// <summary>
    /// Includes vanilla and modded IDs for various generic books.
    /// </summary>
    public enum Books
    {
        // Not much idea what is with these vanilla values here.
        Book0 = 277,
        Book1 = 277,
        Book2 = 277,
        Book3 = 277,
        // Modded book items such as from Ralzar's "Skill Books" mod.
        Basic_Skill_Book = 551,
        Advanced_Skill_Book = 552,
        Tomes_of_Arcane_Knowledge = 553,
        Tablet_of_Arcane_Knowledge = 554,
    }

    /// <summary>
    /// Includes vanilla and modded IDs for various gems.
    /// </summary>
    public enum Gems
    {
        // Vanilla Gem IDs
        Ruby = 0,
        Emerald = 1,
        Sapphire = 2,
        Diamond = 3,
        Jade = 4,
        Turquoise = 5,
        Malachite = 6,
        Amber = 7,
        // Modded gem items such as from my "Jewelry Additions" mod.
        Amethyst = 4708,
        Apatite = 4709,
        Aquamarine = 4710,
        Garnet = 4711,
        Topaz = 4712,
        Zircon = 4713,
        Spinel = 4714,
    }

    /// <summary>
    /// Includes vanilla and modded IDs for various gems.
    /// </summary>
    public enum Jewelry
    {
        // Vanilla Gem IDs
        Amulet = 133,
        Bracer = 134,
        Ring = 135,
        Bracelet = 136,
        Mark = 137,
        Torc = 138,
        Cloth_amulet = 139,
        // Modded gem items such as from my "Jewelry Additions" mod.
        JA_Ring = 4700,
        Earring = 4701,
        Pendant = 4702,
        JA_Bracelet = 4703,
        Tiara = 4704,
        Crown = 4705,
        Armlet_Petal = 4706,
        Armlet_Snake = 4707,
    }

    /// <summary>
    /// Includes mostly modded items that I would file under tools in some way, such as "Repair Tools" and some of Ralzar's mod items and such.
    /// </summary>
    public enum Tools
    {
        // Items from my "Repair Tools" mod.
        Whetstone = 800,
        Sewing_Kit = 801,
        Armorers_Hammer = 802,
        Jewelers_Pliers = 803,
        Epoxy_Glue = 804,
        Charging_Powder = 805,
    }

    /// <summary>
    /// Includes the vanilla light source items.
    /// </summary>
    public enum Lights
    {
        Torch = 247,
        Lantern = 248,
        Candle = 253,
    }

    /// <summary>
    /// Includes various misc useful items from both vanilla (DFU) and added by some mods.
    /// </summary>
    public enum Supplies
    {
        // Vanilla items given some use in DFU, or through mods.
        Bandage = 249,
        Oil = 252,
        Parchment = 279,
        // Modded supply items such as from my "Climates & Calories" mod.
        Rations = 531,
    }

    /// <summary>
    /// Includes various "expensive" vanilla items that have the term "holy" in them, suggesting they are blessed by a temple or special in some way compared to their mundane base item.
    /// </summary>
    public enum BlessedItems
    {
        Holy_relic = 55,
        Holy_water = 262,
        Holy_candle = 269,
        Holy_dagger = 270,
        Holy_tome = 271,
    }

    /// <summary>
    /// Includes various vanilla items that are more religious in theme and likely eventual use, mostly trinkets and idols, etc.
    /// </summary>
    public enum ReligiousStuff
    {
        Prayer_beads = 258,
        Rare_symbol = 259,
        Common_symbol = 260,
        Bell = 261,
        Talisman = 263,
        Religious_item = 264,
        Small_statue = 265,
        Icon = 267,
        Scarab = 268,
    }

    /// <summary>
    /// Includes plant type items that can be found in any parts of the game, no matter the climate (currently), both vanilla and modded items for now.
    /// </summary>
    public enum GenericPlants
    {
        Twigs = 8,
        Green_leaves = 9,
        Red_flowers = 10,
        Yellow_flowers = 11,
        Root_tendrils = 12,
        Root_bulb = 13,
        Green_berries = 15,
        Red_berries = 16,
        Yellow_berries = 17,
    }

    /// <summary>
    /// Includes plant type items that can only be found in cold or cooler climates in the game, (currently), both vanilla and modded items for now.
    /// </summary>
    public enum ColdClimatePlants
    {
        Pine_branch = 14,
        Clover = 18,
        Red_rose = 19,
        Yellow_rose = 20,
        Red_poppy = 23,
        Golden_poppy = 25,
    }

    /// <summary>
    /// Includes plant type items that can only be found in hot or desert climates in the game, (currently), both vanilla and modded items for now.
    /// </summary>
    public enum WarmClimatePlants
    {
        Black_rose = 21,
        White_rose = 22,
        Black_poppy = 24,
        White_poppy = 26,
        Ginkgo_leaves = 27,
        Bamboo = 28,
        Palm = 29,
        Aloe = 30,
        Fig = 31,
        Cactus = 32,
    }

    /// <summary>
    /// Includes items that are generally from beasts and creatures that are fairly common and mundane in rarity and value, both vanilla and modded items for now.
    /// </summary>
    public enum CommonCreatureParts
    {
        Spider_venom = 41,
        Snake_venom = 43,
        Small_scorpion_stinger = 48,
        Giant_blood = 50,
        Big_tooth = 56,
        Medium_tooth = 57,
        Small_tooth = 58,
        Orcs_blood = 61,
    }

    /// <summary>
    /// Includes items that are generally from creatures that are fairly uncommon and possibly magical in nature and more rare as a result, both vanilla and modded items for now.
    /// </summary>
    public enum UncommonCreatureParts
    {
        Werewolfs_blood = 33,
        Wereboar_tusk = 34,
        Nymph_hair = 36,
        Wraith_essence = 38,
        Ectoplasm = 39,
        Ghouls_tongue = 40,
        Troll_blood = 42,
        Giant_scorpion_stinger = 47,
        Mummy_wrappings = 49,
        Gryphon_Feather = 52,
        Ivory = 76,
        Pearl = 77,
    }

    /// <summary>
    /// Includes items that are generally from creatures that are very uncommon and basically always magical in nature and more very rare as a result, both vanilla and modded items for now.
    /// </summary>
    public enum RareCreatureParts
    {
        Fairy_dragon_scales = 35,
        Unicorn_horn = 37,
        Gorgon_snake = 44,
        Lich_dust = 45,
        Dragons_scales = 46,
        Basilisk_eye = 51,
        Daedra_heart = 53,
        Saints_hair = 54,
    }

    /// <summary>
    /// Includes items that are primarily liquid and would likely be used as a solvent or base in alchemical mixture, both vanilla and modded items for now.
    /// </summary>
    public enum LiquidSolvents
    {
        Pure_water = 59,
        Rain_water = 60,
        Elixir_vitae = 62,
        Nectar = 63,
        Ichor = 64,
    }

    /// <summary>
    /// Includes items that metal in nature, but in small trace amounts that would most likely be primarily used in alchemical mixtures, both vanilla and modded items for now.
    /// </summary>
    public enum TraceMetals
    {
        Mercury = 65,
        Tin = 66,
        Brass = 67,
        Lodestone = 68,
        Sulphur = 69,
        Lead = 70,
        Iron = 71,
        Copper = 72,
        Silver = 73,
        Gold = 74,
        Platinum = 75,
    }
}
