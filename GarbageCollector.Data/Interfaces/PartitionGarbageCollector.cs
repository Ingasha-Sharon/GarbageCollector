namespace GarbageCollector.Data.Interfaces
{
    public abstract class PartitionGarbageCollector : IGarbageCollector
    {
        public abstract void ExecuteInstruction(Instruction instruction);

        public abstract int Allocate(string field);

        public abstract void Collect(string partition);

        public abstract void PrintMemory();
    }
}
