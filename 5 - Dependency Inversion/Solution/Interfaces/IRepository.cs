using System.Threading.Tasks;

namespace DIP.Solution.Interfaces
{
    public interface IRepository
    {
        void Save(Person person);
    }
}
