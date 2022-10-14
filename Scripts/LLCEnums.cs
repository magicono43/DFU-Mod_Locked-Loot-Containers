using System;

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

    /*/// <summary>
    /// Flags for what material types are allowed to generate in certain context, such as wood not being allowed in volcanic caves, but daedric being allowed, etc.
    /// </summary>
    [Flags]
    public enum PermittedMaterials
    {
        None = 0,
        Wood = 1,
        Iron = 2,
        Steel = 4,
        Orcish = 8,
        Mithril = 16,
        Dwarven = 32,
        Adamantium = 64,
        Daedric = 128,
        Everything = 0xff,
    }*/
}
