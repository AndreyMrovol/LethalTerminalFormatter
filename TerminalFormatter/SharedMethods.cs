namespace TerminalFormatter
{
  public class SharedMethods
  {
    public static string GetWeather(SelectableLevel level, bool shortened = false)
    {
      return TerminalUtils.Utils.GetWeatherName.GetWeather(level, shortened);
    }

    public static string GetNumberlessPlanetName(SelectableLevel level)
    {
      return MrovLib.SharedMethods.GetNumberlessPlanetName(level);
    }

    public static int GetPrice(int beforeDiscountPrice)
    {
      if (TerminalUtils.Plugin.LGUCompat.IsModPresent)
      {
        return TerminalUtils.Plugin.LGUCompat.GetMoonPrice(beforeDiscountPrice);
      }
      else
      {
        return beforeDiscountPrice;
      }
    }
  }
}
