using System.Threading.Tasks;

namespace ISP.Problem
{
    public interface IRegisterService
    {
        object Get(int Id);
        Task<object> SearchTaxId(string taxId);
    }
}
