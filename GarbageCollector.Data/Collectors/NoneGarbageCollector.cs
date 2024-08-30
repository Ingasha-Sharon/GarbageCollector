namespace GarbageCollector.Data.Collectors
{
    public class NoneGarbageCollector : Data.Interfaces.GarbageCollector
    {
        private List<RuntimeThread> _threads;
        private RuntimeHeap _heap;

        public NoneGarbageCollector()
        {
            _threads = new List<RuntimeThread>();
            _heap = new RuntimeHeap(64);
        }

        public override void ExecuteInstruction(Instruction instruction)
        {
            if (instruction.Operation == "CREATE_THREAD")
            {
                AddThread(instruction.Name);
                return;
            }

            var thread = _threads.FirstOrDefault(x => x.Name == instruction.Name);
            if (thread == null) return;

            if (instruction.Operation == "PUSH_ON_STACK")
            {
                thread.PushInstruction(instruction.Value);
            }
            if (instruction.Operation == "POP_FROM_STACK")
            {
                thread.PopInstruction();
            }
        }

        public override int Allocate(string field)
        {
            var r = AllocateHeap(field);
            if (r == -1) throw new InsufficientMemoryException();

            return r;
        }

        public override void Collect()
        {
            return;
        }

        public override void PrintMemory()
        {
            _heap.Print();
        }

        #region Private Methods

        private void AddThread(string name)
        {
            var thread = new RuntimeThread(this, 16, name);
            _threads.Add(thread);
        }


        private int AllocateHeap(string field)
        {
            var size = _heap.Size;
            var cells = field.ToArray();
            var requiredSize = cells.Length;

            // We need to find a contiguous space of requiredSize char.
            var begin = 0;
            while (begin < size)
            {
                var c = _heap.Cells[begin];
                if (c.Cell != '\0')
                {
                    begin++;
                    continue;
                }

                // Do not continue if there is not enough space.
                if ((size - begin) < requiredSize) return -1;

                for (var i = begin; i < begin + requiredSize; i++)
                {
                    _heap.Cells[i].Cell = cells[i - begin];
                }

                var pointer = new RuntimeHeapPointer(begin, requiredSize, "");
                _heap.Pointers.Add(pointer);

                return begin;
            }

            return -1;
        }


        #endregion
    }
}
