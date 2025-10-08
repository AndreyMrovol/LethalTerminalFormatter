using System;
using System.Linq;

namespace TerminalFormatter.Patches
{
  public static class String
  {
    public static string Sanitized(this string currentString) => new string(currentString.SkipToLetters().RemoveWhitespace().ToLowerInvariant());

    public static string RemoveWhitespace(this string input) => new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());

    public static string SkipToLetters(this string input) => new string(input.SkipWhile(c => !char.IsLetter(c)).ToArray());

    public static string StripSpecialCharacters(this string input)
    {
      string returnString = string.Empty;

      foreach (char charmander in input)
      {
        if ((char.IsLetterOrDigit(charmander)) || charmander.ToString() == " ")
        {
          returnString += charmander;
        }
      }

      return returnString;
    }
  }
}
