using AsciiPhoto.Entities;
using System.Collections.Generic;
using System.Drawing;

namespace AsciiPhoto
{
    /// <summary>
    /// Lucida Console monospace console font properties.
    /// </summary>
    internal static class EmojiOverlay
    {
        /// <summary>
        /// Lucida Console font dimensions at 14pt size
        /// </summary>
        public static readonly Size CharacterSize = new Size(FontWidth, FontHeight);

        private const int FontHeight = 14;
        private const int FontWidth = 8;

        /// <summary>
        /// Gets a dictionary containing the alphabet characters and their pixel maps.
        /// </summary>
        private static Dictionary<string, string> Map => new Dictionary<string, string>()
        {
            ["#"] = "🦓",
            ["'"] = "😀",
            ["("] = "😃",
            [")"] = "😀",
            ["*"] = "😃",
            ["+"] = "😄",
            [","] = "😁",
            ["-"] = "😆",
            ["."] = "😅",
            ["/"] = "😂",
            [":"] = "😾",
            ["<"] = "💎",
            ["="] = "😊",
            [">"] = "💎",
            ["o"] = "📀",
            ["O"] = "📀",
            ["T"] = "🚀",
            ["U"] = "🚀",
            ["V"] = "💎",
            ["X"] = "⚡️",
            ["Y"] = "🚀",
            ["["] = "🚀",
            ["\\"] = "😙",
            ["]"] = "🧲",
            ["_"] = "😋",
            ["`"] = "😛",
            ["{"] = "😝",
            ["|"] = "👽",
            ["}"] = "🧲",
            ["~"] = "🌫",
            ["⌐"] = "🧐",
            ["¬"] = "🤓",
            ["x"] = "⚡️",
            ["¡"] = "🤩",
            ["«"] = "🥳",
            ["»"] = "🎲",
            ["░"] = "🎲",
            ["▒"] = "🎲",
            ["▓"] = "🎲",
            ["│"] = "🗾",
            ["┤"] = "🗾",
            ["╡"] = "🗾",
            ["╢"] = "🔥",
            ["╖"] = "😣",
            ["╕"] = "😖",
            ["╣"] = "😫",
            ["║"] = "😩",
            ["╗"] = "🥺",
            ["╝"] = "😢",
            ["╜"] = "😭",
            ["╛"] = "😤",
            ["┐"] = "🗾",
            ["└"] = "🗾",
            ["┴"] = "🗾",
            ["┬"] = "🗾",
            ["├"] = "🗾",
            ["─"] = "🗾",
            ["┼"] = "🗾",
            ["╞"] = "🗾",
            ["╟"] = "💦",
            ["╚"] = "💦",
            ["╔"] = "💦",
            ["╩"] = "💦",
            ["╦"] = "💦",
            ["╠"] = "💦",
            ["═"] = "💦",
            ["╬"] = "💦",
            ["╧"] = "💦",
            ["╨"] = "💦",
            ["╤"] = "💦",
            ["╥"] = "💦",
            ["╙"] = "💦",
            ["╘"] = "💦",
            ["╒"] = "💦",
            ["╓"] = "💦",
            ["╫"] = "💦",
            ["╪"] = "💦",
            ["┘"] = "😲",
            ["┌"] = "🥱",
            ["▌"] = "🔧",
            ["▐"] = "🥖",
            ["Γ"] = "😪",
            ["∩"] = "🧲",
            ["≡"] = "🤐",
            ["⌠"] = "🥴",
            ["⌡"] = "🤢",
            ["≈"] = "🌫",
            ["°"] = "💿",
            ["∙"] = "💿",
            ["√"] = "🎷",
            ["▀"] = "💙",
            ["■"] = "🤑",
            ["▄"] = "🤠",
            ["█"] = "🦠",
            ["\""] = "👍",
        };

        public static string ApplyOverlay(string characterToReplace)
        {
            if (Map.ContainsKey(characterToReplace))
            {
                return Map[characterToReplace];
            }

            return " ";
        }

        public static Letter[,] ApplyOverlay(Letter[,] lettersToReplace)
        {
            for (int y = 0; y < lettersToReplace.GetLength(1); y++)
            {
                for (int x = 0; x < lettersToReplace.GetLength(0); x++)
                {
                    lettersToReplace[x, y].Character = ApplyOverlay(lettersToReplace[x, y].Character);
                }
            }

            return lettersToReplace;
        }
    }
}
