using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static string BuildName()
        {
            return GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.displayName;
        }

        public static string CityName()
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CurrentRegion()
        {   // %crn going to use for %reg as well here
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        public static string RegentTitle()
        {   // %rt %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion((int)FactionFile.FactionTypes.Province, gps.CurrentRegionIndex, out regionFaction);
            return GetRulerTitle(regionFaction.ruler);
        }

        private static string GetRulerTitle(int factionRuler)
        {
            switch (factionRuler)
            {
                case 1:
                    return TextManager.Instance.GetLocalizedText("King");
                case 2:
                    return TextManager.Instance.GetLocalizedText("Queen");
                case 3:
                    return TextManager.Instance.GetLocalizedText("Duke");
                case 4:
                    return TextManager.Instance.GetLocalizedText("Duchess");
                case 5:
                    return TextManager.Instance.GetLocalizedText("Marquis");
                case 6:
                    return TextManager.Instance.GetLocalizedText("Marquise");
                case 7:
                    return TextManager.Instance.GetLocalizedText("Count");
                case 8:
                    return TextManager.Instance.GetLocalizedText("Countess");
                case 9:
                    return TextManager.Instance.GetLocalizedText("Baron");
                case 10:
                    return TextManager.Instance.GetLocalizedText("Baroness");
                case 11:
                    return TextManager.Instance.GetLocalizedText("Lord");
                case 12:
                    return TextManager.Instance.GetLocalizedText("Lady");
                default:
                    return TextManager.Instance.GetLocalizedText("Lord");
            }
        }

        public static string RandomAlcohol() // Might want to add more "fantasy" sounding drinks to this at some point, but for now it should be alright hopefully.
        {
            DFLocation.ClimateBaseType climate = GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType;
            DaggerfallDateTime.Seasons season = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;

            int variant = UnityEngine.Random.Range(0, 2);

            switch (climate)
            {
                case DFLocation.ClimateBaseType.Desert:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Jalapeno tequila" : "Prickly pear margarita";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Coconut rum" : "Cucumber mojito";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Pineapple margarita" : "Cucumber mojito";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Cinnamon tequila" : "Spicy scorpion cocktail";
                    }
                case DFLocation.ClimateBaseType.Mountain:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Mulled cider" : "Cinnamon mead";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Elderberry cocktail" : "Juniper berry mead";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Barley wine" : "Golden ale";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Mulled wine" : "Wassail";
                    }
                case DFLocation.ClimateBaseType.Temperate:
                default:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Cranberry wine" : "Pumpkin sangria";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Watermelon Mojito" : "Tequila sunrise";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Mint julep" : "Elderflower champagne";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Mulled wine" : "Anise Liqueur";
                    }
                case DFLocation.ClimateBaseType.Swamp:
                    switch (season)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            return variant == 0 ? "Banana liqueur" : "Grapefruit mimosa";
                        case DaggerfallDateTime.Seasons.Spring:
                        default:
                            return variant == 0 ? "Sake sour" : "Grapefruit daiquiri";
                        case DaggerfallDateTime.Seasons.Summer:
                            return variant == 0 ? "Pineapple sangria" : "Sake mojito";
                        case DaggerfallDateTime.Seasons.Winter:
                            return variant == 0 ? "Snakebite cocktail" : "Coconut rum";
                    }
            }
        }

        public static TextFile.Token[] sdfsf()
        {

        }

        public static void testerslser()
        {
            TextFile.Token[] choiceText;

            choiceText = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
                "As you go to unlatch the pouch, you notice what looks like a magical rune inscribed onto the clasp. You can only imagine it's a magical trap of some sort",
                "(likely as an attempt to discourage behavior such as this.) ",
                "Do you attempt disarming it to get to the contents of the pouch, at risk of triggering the trap?", "",
                "(Odds Determined By: Thaumaturgy, Destruction, Lockpicking, and of course, Luck)");
        }
		
		public static TextFile.Token[] TextTokenFromRawString(string rawString)
        {
            var listOfCompLines = new List<string>();
            int partLength = 115;
            if (!DaggerfallUnity.Settings.SDFFontRendering)
                partLength = 65;
            string sentence = rawString;
            string[] words = sentence.Split(' ');
            TextDelay = 5f + (words.Length * 0.25f);
            var parts = new Dictionary<int, string>();
            string part = string.Empty;
            int partCounter = 0;
            foreach (var word in words)
            {
                if (part.Length + word.Length < partLength)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    parts.Add(partCounter, part);
                    part = word;
                    partCounter++;
                }
            }
            parts.Add(partCounter, part);

            foreach (var item in parts)
            {
                listOfCompLines.Add(item.Value);
            }

            return DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter, listOfCompLines.ToArray());
        }
		
		public void ShowFlavorText_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            WeatherType weatherType = GetCurrentWeatherType();
            DaggerfallDateTime.Seasons currentSeason = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;
            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;

            ulong currentTimeSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds(); // 15 * 86400 = Number of seconds in 15 days.

            TextFile.Token[] tokens = null;

            if (playerEnterExit.IsPlayerInside)
            {
                if (playerGPS.CurrentRegionIndex == 17 && locationData.Name == "Daggerfall" && (currentTimeSeconds - lastSeenCastleDFText) > 86400 * (uint)CastleDFTextCooldown) // Daggerfall Region
                {
                    lastSeenCastleDFText = currentTimeSeconds;

                    switch (currentSeason)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = FallDFPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = FallDFPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = FallDFPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Spring:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SpringDFPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SpringDFPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SpringDFPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Summer:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SummerDFPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SummerDFPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SummerDFPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Winter:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = WinterDFPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = WinterDFPalaceText(1); break;
                                case WeatherType.Snow: tokens = WinterDFPalaceText(3); break;
                            }
                            break;
                    }
                }
                else if (playerGPS.CurrentRegionIndex == 20 && locationData.Name == "Sentinel" && (currentTimeSeconds - lastSeenCastleSentText) > 86400 * (uint)CastleSentTextCooldown) // Sentinel Region
                {
                    lastSeenCastleSentText = currentTimeSeconds;

                    switch (currentSeason)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = FallSentPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = FallSentPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = FallSentPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Spring:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SpringSentPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SpringSentPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SpringSentPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Summer:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SummerSentPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SummerSentPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SummerSentPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Winter:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = WinterSentPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = WinterSentPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = WinterSentPalaceText(2); break;
                            }
                            break;
                    }
                }
                else if (playerGPS.CurrentRegionIndex == 23 && locationData.Name == "Wayrest" && (currentTimeSeconds - lastSeenCastleWayText) > 86400 * (uint)CastleWayTextCooldown) // Wayrest Region
                {
                    lastSeenCastleWayText = currentTimeSeconds;

                    switch (currentSeason)
                    {
                        case DaggerfallDateTime.Seasons.Fall:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = FallWayPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = FallWayPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = FallWayPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Spring:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SpringWayPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SpringWayPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SpringWayPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Summer:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = SummerWayPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = SummerWayPalaceText(1); break;
                                case WeatherType.Rain:
                                case WeatherType.Thunder: tokens = SummerWayPalaceText(2); break;
                            }
                            break;
                        case DaggerfallDateTime.Seasons.Winter:
                            switch (weatherType)
                            {
                                case WeatherType.Sunny:
                                default: tokens = WinterWayPalaceText(0); break;
                                case WeatherType.Overcast:
                                case WeatherType.Fog: tokens = WinterWayPalaceText(1); break;
                                case WeatherType.Snow: tokens = WinterWayPalaceText(3); break;
                            }
                            break;
                    }
                }
            }

            if (tokens != null)
            {
                if (MinDisplayDuration != 0 && TextDelay < MinDisplayDuration) // If not set to 0, the minimum number of seconds a message can show for
                    TextDelay = MinDisplayDuration;

                if (MaxDisplayDuration != 0 && TextDelay > MaxDisplayDuration) // If not set to 0, the maximum number of seconds a message can show for
                    TextDelay = MaxDisplayDuration;

                if (TextDisplayType == 0) // For HUD display of text
                {
                    DaggerfallUI.AddHUDText(tokens, TextDelay);
                }
                else // For MessageBox display of text
                {
                    DaggerfallMessageBox textBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    textBox.SetTextTokens(tokens);
                    textBox.ClickAnywhereToClose = true;
                    textBox.Show();
                }
            }
        }


        #region Shop Text

        public static TextFile.Token[] HotFallShopText(int weatherID)
        {
            int variant = UnityEngine.Random.Range(0, 3);
            string raw = "";

            if (weatherID == 1) // Cloudy
            {
                if (variant == 0)
                    raw = "You enter " + BuildName() + ". Many items of interest sit on display shelves around the shopkeeper...";
                else if (variant == 1)
                    raw = "It is a miserable autumn day outside, and it is good to get inside " + BuildName() + ". You glance over the new shipments of supplies and wares.";
                else
                    raw = "It is a bit early in the year for this sort of slightly colder weather, but new supplies arrived at " + BuildName() + " with the chill. You find a few useful items immediately...";
            }
            else if (weatherID == 2) // Rainy
            {
                if (variant == 0)
                    raw = "You enter " + BuildName() + ", happy to be out of the warm shower. As you dry off you notice the many items of interest that sit on the display shelves around the shop...";
                else if (variant == 1)
                    raw = "You are handed a towel as you come into " + BuildName() + " from the autumn thunderstorm. There is a new shipment of wares and supplies, and you notice several pieces worth looking at...";
                else
                    raw = "You are dripping with warm rain water as you enter " + BuildName() + ". It is a neat and clean chamber with a wide assortment of supplies and wares from this shop's speciality...";
            }
            else // Sunny or anything else
            {
                if (variant == 0)
                    raw = "As you enter " + BuildName() + ", golden glints from the autumn sun reflect off of the many items of interest scattered about...";
                else if (variant == 1)
                    raw = "You enter " + BuildName() + " from the sunny autumn day. Display shelves of this store's speciality are featured next to some of the more peaceful supplies and gear...";
                else
                    raw = "The pleasant autumn weather has given " + BuildName() + " an air of joviality. You browse through cases and displays of various supplies...";
            }
            return TextTokenFromRawString(raw);
        }

        #endregion
    }
}