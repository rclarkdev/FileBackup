using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBackup.Core.Communication.Files
{
    public class ListFileResponse
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string Owner { get; set; }
        public long Size { get; set; }
    }
}
