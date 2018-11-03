using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsServer
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    // Define the data contract for the service
    [DataContract]
    // Declare the serializable properties.
    public class ProductData
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Quantity { get; set; }
    }

    // Define the service contract.
    [ServiceContract]
    interface IProducts
    {
        [OperationContract]
        IList<ProductData> GetProducts();

    }

    interface IProductsChannel : IProducts, IClientChannel
    {
    }
}
