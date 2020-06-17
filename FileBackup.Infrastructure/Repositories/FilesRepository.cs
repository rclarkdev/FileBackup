using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Amazon.S3.Transfer;
using FileBackup.Core.Communication.Files;
using FileBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Infrastructure.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly IAmazonS3 _s3Client;

        public FilesRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<AddFileResponse> AddFiles (string bucketName, IList<IFormFile> formFiles)
        {
            var response = new List<string>();

            foreach (var formFile in formFiles)
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = formFile.OpenReadStream(),
                    Key = formFile.FileName,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using (var fileTransferUtility = new TransferUtility(_s3Client))
                {
                    await fileTransferUtility.UploadAsync(uploadRequest);  
                }

                var expiryUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = formFile.FileName,
                    Expires = DateTime.Now.AddDays(1)
                };

                var url = _s3Client.GetPreSignedURL(expiryUrlRequest);

                response.Add(url);

            }

            return new AddFileResponse
            {
                 PreSignedUrl = response
            };
        }

        public async Task<IEnumerable<ListFileResponse>> ListFiles (string bucketName)
        {
            var responses = await _s3Client.ListObjectsAsync(bucketName);

            return responses.S3Objects.Select(b => new ListFileResponse
            {
                BucketName = b.BucketName,
                Key = b.Key,
                Owner = b.Owner.DisplayName,
                Size = b.Size
            });
        }

        public async Task DownloadFile(string bucketName, string fileName)
        {
            var pathAndFileName = $"C:\\S3Temp\\{fileName}";

            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = pathAndFileName
            };

            using (var transferUtility = new TransferUtility(_s3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }

        public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
        {
            var multiObjectDeletRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName
            };

            multiObjectDeletRequest.AddKey(fileName);

            var response = await _s3Client.DeleteObjectsAsync(multiObjectDeletRequest);

            return new DeleteFileResponse
            {
                NumberOfDeletedObjects = response.DeletedObjects.Count
            };

        }

        public async Task AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            var createdOnUtc = DateTime.UtcNow;

            var s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:dd}/{request.Id}";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = JsonConvert.SerializeObject(request)
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<GetJsonObjectResponse> GetJsonObject(string bucketName, string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(request);

            using (var reader = new StreamReader(response.ResponseStream))
            {
                var contents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<GetJsonObjectResponse>(contents);
            }
        }
    }
}
