using Microsoft.AspNetCore.Http;
using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(string containerName, IFormFile file);
    Task DeleteFile(string containerName, string FileRoute);
    Task<string> EditFile(string containerName, IFormFile file, string path);
    Task<List<BlobDTO>> ListAsync();
    Task<BlobReponseDTO> UploadAsync(IFormFile blob);
    Task<BlobDTO?> DownloadAsync(string blobFilename);
    Task<BlobReponseDTO> DeleteAsync(string blobFileName);
}