using System;
using System.Threading.Tasks;

namespace ISP.Problem
{
    public class ProductService : IRegisterService
    {
        public object Get(int Id)
        {
            return new
            {
                Id = Id,
                GTIN = "7891910000197",
                Price = 12.000,
                Name = "IPHONE XII"
            };
        }

        public async Task<object> SearchTaxId(string taxId)
        {
            throw new System.NotImplementedException();
        }
    }
}
