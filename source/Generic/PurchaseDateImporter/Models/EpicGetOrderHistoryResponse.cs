using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseDateImporter.Models
{
    public class EpicGetOrderHistoryResponse
    {
        [SerializationPropertyName("orders")]
        public EpicOrder[] Orders { get; set; }

        [SerializationPropertyName("nextPageToken")]
        public string NextPageToken { get; set; }
    }

    public class EpicOrder
    {
        [SerializationPropertyName("createdAtMillis")]
        public long CreatedAtMillis { get; set; }

        [SerializationPropertyName("items")]
        public EpicOrderItem[] Items { get; set; }
    }

    public class EpicOrderItem
    {
        [SerializationPropertyName("description")]
        public string Description { get; set; }
    }
}
