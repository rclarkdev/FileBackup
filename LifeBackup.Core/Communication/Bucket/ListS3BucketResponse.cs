using System;
using System.Collections.Generic;
using System.Text;

namespace FileBackup.Core.Communication.Bucket
{
    public class ListS3BucketResponse
    {
        public string BucketName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
