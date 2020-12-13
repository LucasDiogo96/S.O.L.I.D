using System.Threading.Tasks;

namespace ISP.Solution.Interfaces
{
    public interface ICompanyService
    {
        object Get(int Id);
        Task<object> SearchTaxId(string cnpj);
    }
}
