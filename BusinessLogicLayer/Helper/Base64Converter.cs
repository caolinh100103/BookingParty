using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Helper;

public class Base64Converter
{
    public static string ConvertToBase64(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        { 
            throw new ArgumentException("Invalid image file");
        }
        using (var memoryStream = new MemoryStream())
        { 
            imageFile.CopyTo(memoryStream); 
            byte[] imageBytes = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(imageBytes); 
            return base64String;
        }
    }
}