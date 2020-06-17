using System;
using System.Collections.Generic;
using System.Text;

namespace FileBackup.Core.Communication.Bucket
{
    public class CreateBucketResponse
    {
        public string RequestId { get; set; }
        public string BucketName { get; set; }
    }
}
