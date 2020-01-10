namespace whateverthefuck.src.util
{
    using System;
    using System.Drawing;

    // @dubious entire class
    internal static class Coloring
    {
        // https://xkcd.com/221/
        private static Random r = new Random(3434);

        public static Color RandomColor()
        {
            return Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255));
        }

        public static Color Opposite(Color c)
        {
            return Color.FromArgb(c.ToArgb() ^ 0xffffff);
        }

        /*public static Color FromRarity(Rarity Rarity)
        {
            switch (Rarity)
            {
                case Rarity.Common: return Color.Gray;
                case Rarity.Uncommon: return Color.GreenYellow; ;
                case Rarity.Rare: return Color.DarkBlue;
                case Rarity.Epic: return Color.Purple;
                case Rarity.Legendary: return Color.DarkOrange;
                default: return Color.Cornsilk;
            }
        }*/
    }
}
