using System.Collections.Generic;
using System.Text;

namespace TerminalFormatter
{
    public class Header
    {
        public string CreateHeader(string text)
        {
            //   ╔════════╗
            // ╭─╢ HEADER ╟&╮
            // │ ╚════════╝^│

            StringBuilder builder = new();
            int length = text.Length;

            builder.Append("  ╔");
            for (int i = 0; i < length + 2; i++)
            {
                builder.Append("═");
            }
            builder.Append("╗\n");

            builder.Append("╭─╢ ");
            builder.Append(text);
            builder.Append(" ╟&╮\n");

            builder.Append("│ ╚");
            for (int i = 0; i < length + 2; i++)
            {
                builder.Append("═");
            }
            builder.Append("╝^│\n");

            return builder.ToString();
        }

        public string CreateHeaderWithoutLines(string text, int padLeft = 2)
        {
            //   ╔════════╗
            //   ║ HEADER ║
            //   ╚════════╝

            StringBuilder builder = new();
            int length = text.Length;

            for (int i = 0; i < padLeft; i++)
            {
                builder.Append(" ");
            }

            builder.Append("╔");
            for (int i = 0; i < length + 2; i++)
            {
                builder.Append("═");
            }
            builder.Append("╗\n");

            for (int i = 0; i < padLeft; i++)
            {
                builder.Append(" ");
            }

            builder.Append("║ ");
            builder.Append(text);
            builder.Append(" ║\n");

            for (int i = 0; i < padLeft; i++)
            {
                builder.Append(" ");
            }
            builder.Append("╚");
            for (int i = 0; i < length + 2; i++)
            {
                builder.Append("═");
            }
            builder.Append("╝\n");

            return builder.ToString();
        }

        internal string CreateNumberedHeader(
            string text,
            int padLeft = 2,
            Dictionary<int, string> replaceNumbers = null
        )
        {
            //   ╔════════╗0
            //   ║ HEADER ║1
            //   ╚════════╝2

            string Header = CreateHeaderWithoutLines(text, padLeft);

            StringBuilder builder = new();

            string[] lines = Header.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                builder.Append("  ");
                builder.Append(lines[i].Trim());

                builder.Append(
                    $"  {(replaceNumbers != null && replaceNumbers.ContainsKey(i) ? replaceNumbers[i] : "")}"
                );
                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}
