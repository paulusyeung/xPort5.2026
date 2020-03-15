using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xPort5.Bot.Models
{
    public class CloudDisk
    {
        public class ResourceInfo
        {
            public int idx { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public int Size { get; set; }
            public string ETag { get; set; }
            public string ContentType { get; set; }
            public DateTime LastModified { get; set; }
            public DateTime Created { get; set; }
            public int QuotaUsed { get; set; }
            public int QuotaAvailable { get; set; }
            public bool Checked { get; set; }
        }

        public class ResourceInfoEx : ResourceInfo
        {
            public string CupsJobId { get; set; }
            public string CupsJobTitle { get; set; }
            public string PlateSize { get; set; }
            public string VpsFileName { get; set; }
            public int VpsPrintQueueId { get; set; }
        }

        public class ActionEmail
        {
            public string Recipient { get; set; }
            public string Remarks { get; set; }
            public bool ExpiryChecked { get; set; }
            public DateTime ExpiredOn { get; set; }
            public string Password { get; set; }
            public List<ResourceInfo> Items { get; set; }
        }

        public class ActionReprintEx
        {
            public int ClientId { get; set; }
            public string Remarks { get; set; }
            public List<ResourceInfoEx> Items { get; set; }
        }

        public class ActionOutputEx
        {
            public int ClientId { get; set; }
            public int Priority { get; set; }
            public string Workshop { get; set; }
            public bool Pickup { get; set; }
            public bool Deliver { get; set; }
            public int DeliverTo { get; set; }
            public string Remarks { get; set; }
            public List<ResourceInfoEx> Items { get; set; }
        }
    }
}