using ISP.Solution.Interfaces;
using System;

namespace ISP.Solution
{
    public class ProductService : IProductService
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
    }
}
