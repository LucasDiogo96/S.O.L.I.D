using DIP.Solution.Interfaces;

namespace DIP.Solution
{
    public class Business : IBusiness
    {
        private readonly IRepository _repository;

        public Business(IRepository repository)
        {
            _repository = repository;
        }

        public void Save(Person person)
        {
            _repository.Save(person);
        }
    }
}
