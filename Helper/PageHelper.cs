using System.Text;

namespace NewsAggregatorConsoleApp.Helper
{
    class PageHelper
    {
        public static void DisplayHeader() 
        {
            Console.Clear();
            Random random = new();
            ConsoleColor[] colors = Enum.GetValues<ConsoleColor>();
            ConsoleColor randomColor = colors[random.Next(1, colors.Length)];
            DrawLine(ConsoleWidth(), 0, randomColor);
            Console.WriteLine("\n");
            string[] title =
            [
            "  ███╗   ██╗███████╗██╗    ██╗███████╗ ",
            "  ████╗  ██║██╔════╝██║    ██║██╔════╝ ",
            "  ██╔██╗ ██║█████╗  ██║ █╗ ██║███████╗ ",
            "  ██║╚██╗██║██╔══╝  ██║███╗██║╚════██║ ",
            "  ██║ ╚████║███████╗╚███╔███╔╝███████║ ",
            "  ╚═╝  ╚═══╝╚══════╝ ╚══╝╚══╝ ╚══════╝ "
            ];

            CenterLines(title,0, randomColor);
            Console.WriteLine();
            DrawLine(ConsoleWidth(), 0, randomColor);
            Console.WriteLine("\n");
        }
        public static void DisplaySubHeader(string subHeader)
        {
            int lineLength = subHeader.Length + 10;
            DrawLine(lineLength, 0, lineSymbol: '-');
            Console.WriteLine();
            CenterText(subHeader);
            Console.WriteLine();
            DrawLine(lineLength, 0, lineSymbol: '-');
            Console.WriteLine();
        }
        public record MenuOption(ConsoleKey Key, string Title, Func<Task> Action);
        public static void DisplayMenu(List<MenuOption> menuOptions, int withSpace = 16, int totalLength = 20)
        {
            PageHelper.CenterLines([.. menuOptions.Select(option =>
                PageHelper.CenterAlignTwoTexts($"{option.Key.ToString().Replace("D", "")}.", option.Title, withSpace, totalLength))]);
        }
        public static void DrawLine(int max = int.MaxValue, int leftOffset = 0, ConsoleColor color = ConsoleColor.White, char lineSymbol = '=')
        {
            CenterText(new string(lineSymbol, Math.Min(Math.Max(ConsoleWidth(), 50), max)), leftOffset, color);
        }

        public static async Task PrintToast(string message, ConsoleColor color, int delayTime)
        {
            CenterText(message, 0, color);
            await Task.Delay(delayTime);
        }
        public static async Task ShowSuccessToast(string message, int delayTime = 1000)
        {
            await PrintToast(message, ConsoleColor.Green, delayTime);
        }
        public static async Task ShowErrorToast(string message, int delayTime = 1000)
        {
            await PrintToast(message, ConsoleColor.Red, delayTime);
        }
        public static async Task ShowInfoToast(string message, int delayTime = 1000)
        {
            await PrintToast(message, ConsoleColor.Yellow, delayTime);
        }

        public static void CenterLines(string[] lines, int leftOffset = 0, ConsoleColor color = ConsoleColor.White)
        {
            foreach (string line in lines)
            {
                CenterText(line, leftOffset, color);
                Console.WriteLine();
            }
        }
        public static void CenterText(string line, int leftOffset = 0, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition((ConsoleWidth() - line.Length) / 2 + leftOffset, Console.CursorTop);
            Console.Write(line);
            Console.ResetColor();
        }
        public static string JoinWithSpacing(string[] words, int totalLength)
        {
            int totalWordLength = 0;
            foreach (var word in words)
            {
                totalWordLength += word.Length;
            }

            int totalSpaces = totalLength - totalWordLength;
            int spacesBetween = words.Length - 1;
            int baseSpaceSize = spacesBetween > 0 ? totalSpaces / spacesBetween : 0;
            int extraSpaces = spacesBetween > 0 ? totalSpaces % spacesBetween : 0;

            StringBuilder result = new();
            for (int i = 0; i < words.Length; i++)
            {
                result.Append(words[i]);

                if (i < spacesBetween)
                {
                    int spaceCount = baseSpaceSize + (i < extraSpaces ? 1 : 0);
                    result.Append(new string(' ', spaceCount));
                }
            }

            return result.ToString();
        }
        public static string CenterAlignTwoTexts(string first, string second, int withSpace, int totalLength)
        {
            return JoinWithSpacing([first, JoinWithSpacing([second, " "], withSpace)], totalLength);
        }
        public static void PrintTwoColoredTexts(
            int leftOffset,
            int nextOffset,
            string first,
            string second,
            ConsoleColor firstColor = ConsoleColor.White,
            ConsoleColor secondColor = ConsoleColor.White)
        {
            Console.SetCursorPosition(leftOffset, Console.CursorTop);

            Console.ForegroundColor = firstColor;
            Console.Write(first);
            Console.ResetColor();

            Console.Write(new string(' ', nextOffset-first.Length));

            Console.ForegroundColor = secondColor;
            Console.Write(second);
            Console.ResetColor();
        }

        public static async Task<bool> CheckRequiredFields(string errorMessage, params string[] fields)
        {
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                {
                    await PageHelper.ShowErrorToast(errorMessage, 2000);
                    return false;
                }
            }
            return true;
        }
        public static int ConsoleWidth()
        {
            return Console.WindowWidth;
        }
    }
}
