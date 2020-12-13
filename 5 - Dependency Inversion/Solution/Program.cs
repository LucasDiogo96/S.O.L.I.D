using DIP.Solution.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DIP.Solution
{
    public class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRepository, MongoDBRepository>()
                .AddSingleton<IBusiness, Business>()
                .BuildServiceProvider();

            var business = serviceProvider.GetService<IBusiness>();

            business.Save(new Person("Name"));
        }
    }
}
