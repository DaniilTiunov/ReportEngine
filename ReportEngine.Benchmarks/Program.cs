using BenchmarkDotNet.Running;
using Mapster;

namespace ReportEngine.Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            var listOfA = new List<TypeA>
            {
                new TypeA("AA", 1),
                new TypeA("AA", 2)
            };

            var listOfB = listOfA.Adapt<List<TypeB>>();

            foreach (var b in listOfB)
            {
                Console.WriteLine(b.ToString());
                Console.WriteLine(b.Name);
            }
        }
    }

    public class TypeA
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TypeA(string Name, int Id)
        {
            this.Name = Name;
            this.Id = Id;
        }
    }

    public class TypeB
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TypeB(string Name, int Id)
        {
            this.Name = Name;
            this.Id = Id;
        }
    }
}
