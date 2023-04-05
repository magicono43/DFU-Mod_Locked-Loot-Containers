using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static string GetLockPickSuccessText()
        {
            int rand = UnityEngine.Random.Range(0, 9);
            if (rand == 0) { return "The locking mechanism disengages."; }
            else if (rand == 1) { return "With a satisfying click, the lock springs open."; }
            else if (rand == 2) { return "The locking mechanism finally yields"; }
            else if (rand == 3) { return "The lock finally surrenders, with a satisfying click..."; }
            else if (rand == 4) { return "The lock finally yields to your efforts..."; }
            else { return "The lock clicks open..."; }
        }

        public static string GetLockPickAttemptText()
        {
            int rand = UnityEngine.Random.Range(0, 5);
            if (rand == 0) { return "It does not unlock..."; }
            else if (rand == 1) { return "Unfortunately, it's still locked..."; }
            else { return "You fail to pick the lock..."; }
        }

        public static string GetJammedLockText()
        {
            int rand = UnityEngine.Random.Range(0, 5);
            if (rand == 0) { return "Looks like you jammed the lock, no other option but excessive force now."; }
            else if (rand == 1) { return "Due to bad luck or incompetence, the lock is now jammed, brute force is the only option now."; }
            else { return "You jammed the lock, now brute force is the only option."; }
        }

        public static string GetLockAlreadyJammedText()
        {
            int rand = UnityEngine.Random.Range(0, 5);
            if (rand == 0) { return "The locking mechanism is seized and inoperable..."; }
            else if (rand == 1) { return "The tumblers won't budge, jammed..."; }
            else { return "The lock is jammed and inoperable..."; }
        }

        public static string GetMagicLockPickSuccessText()
        {
            int rand = UnityEngine.Random.Range(0, 13);
            if (rand == 0) { return "The locking mechanism disengages."; }
            else if (rand == 1) { return "With a satisfying click, the lock springs open."; }
            else if (rand == 2) { return "The spell bypasses the lock's defenses, unlocking it with ease."; }
            else if (rand == 3) { return "The spell coaxes the lock open, with a satisfying click..."; }
            else if (rand == 4) { return "The lock surrenders to the spell, clicking open."; }
            else if (rand == 5) { return "The lock's resistance is broken by a surge of magical energy, granting access."; }
            else if (rand == 6) { return "The lock yields to your spell, granting access with a shimmering glow."; }
            else { return "The lock clicks open..."; }
        }

        public static string GetMagicLockPickAttemptText()
        {
            int rand = UnityEngine.Random.Range(0, 18);
            if (rand == 0) { return "The lock remains sealed..."; }
            else if (rand == 1) { return "The locking mechanism rattles and clicks, but holds firm against the magical assault."; }
            else if (rand == 2) { return "The lock remains unyielding, repelling the magical attempt to unlock it."; }
            else if (rand == 3) { return "The lock defies the magical attempt, remaining securely locked."; }
            else if (rand == 4) { return "The lock pulsates with a surge of magic, but remains unmoved, still locked."; }
            else if (rand == 5) { return "The lock shimmers with a magical aura, but refuses to yield."; }
            else if (rand == 6) { return "The lock repels the magical energy, remaining steadfastly locked."; }
            else if (rand == 7) { return "The lock emits a faint hum, but remains stubbornly sealed."; }
            else if (rand == 8) { return "The lock glows briefly with a faint light, but remains locked."; }
            else if (rand == 9) { return "The lock shudders slightly but resists the magical assault."; }
            else { return "The attempt fails against the lock..."; }
        }

        public static string GetMagicJammedLockText()
        {
            int rand = UnityEngine.Random.Range(0, 9);
            if (rand == 0) { return "The locking mechanism seizes up, leaving it jammed and inoperable."; }
            else if (rand == 1) { return "The magic assault backfires, jamming the lock rendering it unopenable."; }
            else if (rand == 2) { return "The locking mechanism seizes in place, rendering it impossible to unlock."; }
            else if (rand == 3) { return "The lock emits a sudden burst of smoke, it's now jammed and inoperable."; }
            else { return "The locking mechanism jams in place, rendering it completely inoperable."; }
        }

        public static string GetMagicLockAlreadyJammedText()
        {
            int rand = UnityEngine.Random.Range(0, 5);
            if (rand == 0) { return "The locking mechanism is seized and inoperable..."; }
            else if (rand == 1) { return "The tumblers won't budge, jammed..."; }
            else { return "The lock is jammed and inoperable..."; }
        }

        public static string GetBashedLockOffText()
        {
            int rand = UnityEngine.Random.Range(0, 10);
            if (rand == 0) { return "With a resounding snap, the lock breaks off..."; }
            else if (rand == 1) { return "The lock crumples and breaks, unable to withstand the assault..."; }
            else if (rand == 2) { return "The lock is torn apart, leaving behind a mangled mess."; }
            else if (rand == 3) { return "The lock succumbs to the unrelenting force, breaking off with a loud clang."; }
            else if (rand == 4) { return "With a forceful blow, the lock finally breaks off."; }
            else if (rand == 5) { return "The lock shatters into pieces, yielding to the relentless force."; }
            else if (rand == 6) { return "You strike the lock, and the severed part sails off in a arc!"; }
            else { return "Through use of brute force, the lock finally breaks off..."; }
        }

        public static string GetBashedOpenChestText()
        {
            int rand = UnityEngine.Random.Range(0, 7);
            if (rand == 0) { return "With a loud crash, the chest finally yields to your assault."; }
            else if (rand == 1) { return "After smashing it to bits, you finally can access the chest's contents."; }
            else if (rand == 2) { return "The chest's exterior crumbled under the relentless assault, revealing the contents inside."; }
            else if (rand == 3) { return "The unrelenting assault created a large hole in the chest's body, granting access to what's inside."; }
            else { return "You smash a large hole in the body of the chest, granting access to the contents."; }
        }

        public static string GetArrowSmashedOpenChestText()
        {
            int rand = UnityEngine.Random.Range(0, 7);
            if (rand == 0) { return "The wooden chest was no match for the barrage of arrows."; }
            else if (rand == 1) { return "The chest finally yields to the onslaught of arrows."; }
            else if (rand == 2) { return "The wooden chest was no match for the rain of arrows."; }
            else if (rand == 3) { return "After being pelted by arrows, the chest finally surrenders its contents."; }
            else { return "The arrow smashes a large hole in the chest, granting access to its contents..."; }
        }

        public static string GetChestBlownOpenBySpellText()
        {
            int rand = UnityEngine.Random.Range(0, 6);
            if (rand == 0) { return "The chest is reduced to a splittery pile by the force of the spell, granting access..."; }
            else if (rand == 1) { return "Reduced to a jagged pile, the chest's conents are now free to access."; }
            else if (rand == 2) { return "Blown open by the force of the spell, the chest's content can be accessed."; }
            else { return "The spell causes the chest to erupt into a chaotic mess, granting access to its contents..."; }
        }

        public static string GetChestDisintegratedBySpellText()
        {
            int rand = UnityEngine.Random.Range(0, 6);
            if (rand == 0) { return "The contents of the chest are now free to access, now that you have reduced it to ash..."; }
            else if (rand == 1) { return "The chest is now a pile of ash on the ground, maybe some of the contents survived..."; }
            else if (rand == 2) { return "What remains of the loot? You will have to sift through the pile of ashes to find out."; }
            else { return "The spell causes the chest to disintegrate into an unrecognizable pile, granting access to whatever is left..."; }
        }
    }
}
