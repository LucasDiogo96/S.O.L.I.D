using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISP.Problem
{
    public class CompanyService : IRegisterService
    {
        public static HttpClient client = new HttpClient();

        public object Get(int Id)
        {
            return new
            {
                Id = Id,
                Name = "COMPANY NAME",
                TradeName = "COMPANY NAME LTDA",
                IE = "668.284.686.980",
                TaxId = "17.416.651/0001-60",
                Email = "diretoria@ritaeedsonconstrucoesltda.com.br"
            };
        }

        public async Task<object> SearchTaxId(string taxId)
        {
            string token = "xpto";

            string Url = String.Format("https://www.sintegraws.com.br/api/v1/execute-api.php?token={0}&cnpj={1}&plugin=RF", token, taxId);

            var result = await client.GetAsync(Url);

            var content = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject(content);
        }
    }
}
