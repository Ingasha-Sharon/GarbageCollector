using GarbageCollector.Data.Collectors;
using GarbageCollector.Data.Interfaces;
using GarbageCollector.Data.PartitionCollectors;

namespace GarbageCollector.Data
{
    public class Runtime
    {
        private List<Instruction> _instructions;
        private IGarbageCollector _collector;

        public Runtime(List<Instruction> instructions, string collectorStrategy)
        {
            _instructions = instructions;
            _collector = GetCollector(collectorStrategy);
        }

        #region Public Methods

        public void Run()
        {
            foreach (var instruction in _instructions)
            {
                _collector.ExecuteInstruction(instruction);
                _collector.PrintMemory();
            }
        }

        #endregion

        #region Private Methods

        private IGarbageCollector GetCollector(string strategy)
        {
            IGarbageCollector collector = strategy switch
            {
                "NONE" => new NoneGarbageCollector(),
                "MARK_AND_SWEEP" => new MarkAndSweepGarbageCollector(),
                "MARK_AND_COMPACT" => new MarkAndCompactGarbageCollector(),

                "BYAGE_MARK_AND_COMPACT" => new ByAgePartitionGarbageCollector(),

                _ => throw new NotImplementedException()
            };

            return collector;
        }

        #endregion
    }
}