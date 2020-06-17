using FileBackup.Core.Communication.Bucket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Core.Communication.Interfaces
{
    public interface IBucketRepository
    {
        Task<bool> DoesS3BucketExist(string bucketName);
        Task<CreateBucketResponse> CreateBucket(string bucketName);
        Task<IEnumerable<ListS3BucketResponse>> ListBuckets();
        Task DeleteBucket(string bucketName);
    }
}
