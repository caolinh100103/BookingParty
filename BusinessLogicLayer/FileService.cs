using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model.DTO;

namespace BusinessLogicLayer;

public class FileService : IFileService
{
    private readonly string _storageAccount = "fileforbookingparty";
    private readonly string _key = "4dB+tPJFMEHWfvZOzde3VspNluP5cux+5xUP6yw/QWfv5L5PANdlIUJS1qLGlwMX5mLlsgK6Lr6B+AStQb42tw==";
    private readonly BlobContainerClient _fileContainer;
    private readonly string _storageConnectionString;
    private readonly string _storageContainerName;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;
    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _configuration = configuration;
        var creadential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), creadential);
        _fileContainer = blobServiceClient.GetBlobContainerClient("files");
        _storageConnectionString = _configuration.GetValue<string>("BlobConnectionString");
        _storageContainerName = _configuration.GetValue<string>("BlobContainerName");
        _logger = logger;
    }
    public async Task<string> SaveFile(string containerName, IFormFile file)
    {
        containerName = "fileforbookingparty";
        string connectionString =
            @"DefaultEndpointsProtocol=https;AccountName=savingfilesforbooking;AccountKey=4dB+tPJFMEHWfvZOzde3VspNluP5cux+5xUP6yw/QWfv5L5PANdlIUJS1qLGlwMX5mLlsgK6Lr6B+AStQb42tw==;EndpointSuffix=core.windows.net";
        BlobContainerClient blobClientContainer = new BlobContainerClient(connectionString, containerName);

        BlobClient blobClient = blobClientContainer.GetBlobClient(file.FileName);
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        await blobClient.UploadAsync(memoryStream);
        var path = blobClient.Uri.AbsoluteUri;
        return path;
    }
    public Task DeleteFile(string containerName, string FileRoute)
    { 
        throw new NotImplementedException();
    }

    public Task<string> EditFile(string containerName, IFormFile file, string path)
    { 
        throw new NotImplementedException();
    }

    public async Task<List<BlobDTO>> ListAsync()
    {
        List<BlobDTO> files = new List<BlobDTO>();
        await foreach (var file in _fileContainer.GetBlobsAsync())
        {
            string uri = _fileContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}//{name}";
            
            files.Add(new BlobDTO
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }

        return files;
    }

    public async Task<BlobReponseDTO> UploadAsync(IFormFile blob)
    {
        // Create new upload response object that we can return to the requesting method
        BlobReponseDTO response = new();

        // Get a reference to a container named in appsettings.json and then create it
        BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
        //await container.CreateAsync();
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = container.GetBlobClient(blob.FileName);

            // Open a stream for the file we want to upload
            await using (Stream? data = blob.OpenReadStream())
            {
                // Upload the file async
                await client.UploadAsync(data);
            }

            // Everything is OK and file got uploaded
            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
           when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError($"File with name {blob.FileName} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
            response.Status = $"File with name {blob.FileName} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;
    }

    public async Task<BlobDTO?> DownloadAsync(string blobFilename)
    {
        BlobClient file = _fileContainer.GetBlobClient(blobFilename);
        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();
            string name = blobFilename;
            string contentType = content.Value.Details.ContentType;

            return new BlobDTO
            {
                Content = blobContent,
                Name = name,
                ContentType = contentType
            };
        }

        return null;
    }

    public async Task<BlobReponseDTO> DeleteAsync(string blobFileName)
    {
        BlobClient file = _fileContainer.GetBlobClient(blobFileName);

        await file.DeleteAsync();

        return new BlobReponseDTO()
        {
            Error = false,
            Status = $"File: {blobFileName} has been successfully deleted"
        };
    }
    // public async Task<string> Index(IFormFile file)
    // {
    //     FileStream fs;
    //     FileStream ms;  
    //     if (file.Length > 0)
    //     {
    //         string folderName = "room";
    //         string path = Path.Combine(_env.WebRootPath, $"images/{folderName}");
    //         ms = new FileStream(Path.Combine(path, file.FileName), FileMode.Open);
    //         var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
    //         var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
    //
    //         // you can use CancellationTokenSource to cancel the upload midway
    //         var cancellation = new CancellationTokenSource();
    //
    //         var task = new FirebaseStorage(
    //                 Bucket,
    //                 new FirebaseStorageOptions
    //                 {
    //                     AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
    //                     ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
    //                 })
    //             .Child("receipts")
    //             .Child("test")
    //             .Child($"aspcore.png")
    //             .PutAsync(ms, cancellation.Token);
    //
    //         task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
    //
    //
    //         try
    //         {
    //             ViewBag.link = await task;
    //             return Ok();
    //         }
    //         catch (Exception ex)
    //         {
    //             ViewBag.error = $"Exception was thrown: {ex}";
    //         }
    //
    //     }
    //     return BadRequest();
    // }
}

    