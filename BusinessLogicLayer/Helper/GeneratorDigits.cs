namespace BusinessLogicLayer.Helper;

public class GeneratorDigits
{
    public static string GenerateSixDigitCode()
    {
        Random random = new Random();
        return random.Next(0, 1000000).ToString("D6");
    }
}