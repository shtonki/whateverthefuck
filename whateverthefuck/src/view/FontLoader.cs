using QuickFont;
using QuickFont.Configuration;
using System.Collections.Generic;
using System.Drawing.Text;

namespace whateverthefuck.src.view
{
    static class FontLoader
    {
        public static QFont DefaultFont { get; private set; }

        public static QFont BigFont { get; private set; }

        public static void LoadFonts()
        {
            var builderConfig = new QFontBuilderConfiguration(true)
            {
                ShadowConfig =
                {
                    BlurRadius = 2,
                    BlurPasses = 1,
                    Type = ShadowType.Blurred,
                },
                TextGenerationRenderHint = TextGenerationRenderHint.ClearTypeGridFit,
                Characters = CharacterSet.General | CharacterSet.Japanese | CharacterSet.Thai | CharacterSet.Cyrillic, // @dubious that we don't need all of these
            };

            // loop through some installed fonts and load them
            var ifc = new InstalledFontCollection();

            foreach (var fontFamily in ifc.Families)
            {
                DefaultFont = new QFont(fontFamily.Name, 14, new QFontBuilderConfiguration());
                BigFont = new QFont(fontFamily.Name, 20, new QFontBuilderConfiguration(), System.Drawing.FontStyle.Bold);
                break;
            }
        }
    }
}
