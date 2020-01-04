using QuickFont;
using QuickFont.Configuration;

namespace whateverthefuck.src.view
{
    static class FontLoader
    {
        public static QFont DefaultFont { get; private set; }

        public static QFontDrawing Drawing { get; private set; }

        public static void LoadFonts()
        {
            Drawing = new QFontDrawing();

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

            DefaultFont = new QFont("Fonts/times.ttf", 14, builderConfig);
        }
    }
}
