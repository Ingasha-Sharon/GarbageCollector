using GarbageCollector.Data;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollector.Console
{
    public class Program
    {
        /// <summary>
        /// See https://www.elementsofcomputerscience.com/posts/garbage-collection-in-csharp-01 for details
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var instructions = new List<Instruction>();            
            var contents = File.ReadAllLines(AppContext.BaseDirectory + "/instructions.txt");
            foreach (var line in contents)
            {
                var c = line.Split(';');
                var intruction = new Instruction(c[0], c[1], c[2]);
                instructions.Add(intruction);
            }

            var runtime = new Runtime(instructions, "NONE");
            //var runtime = new Runtime(instructions, "MARK_AND_SWEEP");
            //var runtime = new Runtime(instructions, "MARK_AND_COMPACT");
            //var runtime = new Runtime(instructions, "BYAGE_MARK_AND_COMPACT");

            runtime.Run();
        }
    }
}