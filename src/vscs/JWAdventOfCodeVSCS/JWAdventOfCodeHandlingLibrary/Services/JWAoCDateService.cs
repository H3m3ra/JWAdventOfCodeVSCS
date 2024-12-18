namespace JWAdventOfCodeHandlingLibrary.Services;

public static class JWAocDateService
{
    public static int? GetShortYearOfFullYear(int year)
    {
        var currentFullYear = DateTime.Now.Year;
        if (year < currentFullYear - 99 || currentFullYear < year)
        {
            return null;
        }
        return year % 100;
    }

    public static int ToFullYearFromShortYear(int shortYear)
    {
        var fullYear = DateTime.Now.Year;
        var currentShortYear = fullYear % 100;
        if (shortYear > currentShortYear)
        {
            fullYear -= 100;
        }
        return fullYear;
    }
}