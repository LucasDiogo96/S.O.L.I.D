using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            RunProblemExample();

            RunSolutionExample();
        }

        public static void RunProblemExample()
        {
            LSP.Problem.Apple fruit = new LSP.Problem.Orange();
            Console.WriteLine("An apple is " + fruit.GetColor());
        }

        public static void RunSolutionExample()
        {
            LSP.Solution.Fruit fruit = new LSP.Solution.Apple();
            Console.WriteLine("An apple is " + fruit.GetColor());

            fruit = new LSP.Solution.Orange();
            Console.WriteLine("An orange is " + fruit.GetColor());
        }
    }
}
