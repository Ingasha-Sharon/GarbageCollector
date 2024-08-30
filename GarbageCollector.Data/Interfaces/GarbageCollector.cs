namespace GarbageCollector.Data.Interfaces
{
    public interface IGarbageCollector
    {
        void ExecuteInstruction(Instruction instruction);
        int Allocate(string field);        
        void PrintMemory();
    }

    public abstract class GarbageCollector : IGarbageCollector
    {
        public abstract void ExecuteInstruction(Instruction instruction);

        public abstract int Allocate(string field);

        public abstract void Collect();

        public abstract void PrintMemory();
    }
}
