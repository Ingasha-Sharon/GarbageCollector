namespace GarbageCollector.Data.Collectors
{
    public class MarkAndSweepGarbageCollector : Data.Interfaces.GarbageCollector
    {
        private List<RuntimeThread> _threads;
        private RuntimeHeap _heap;

        public MarkAndSweepGarbageCollector()
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
            if (r == -1)
            {
                Collect();
                r = AllocateHeap(field);
            }

            if (r == -1) throw new InsufficientMemoryException();

            return r;
        }

        public override void Collect()
        {
            MarkFromRoots();
            Sweep();
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

                // c = 0 => This cell is free.
                var subCells = _heap.Cells.Skip(begin).Take(requiredSize).ToArray();
                if (subCells.All(x => x.Cell == '\0'))
                {
                    for (var i = begin; i < begin + requiredSize; i++)
                    {
                        _heap.Cells[i].Cell = cells[i - begin];
                    }

                    var pointer = new RuntimeHeapPointer(begin, requiredSize, "");
                    _heap.Pointers.Add(pointer);

                    return begin;
                }

                begin++;
            }

            return -1;
        }

        private void MarkFromRoots()
        {
            var pointers = _heap.Pointers;
            foreach (var root in _threads.SelectMany(x => x.Roots))
            {
                var startIndexInTheHeap = root.Value.StartIndexInTheHeap;

                var pointer = pointers.FirstOrDefault(x => x.StartCellIndex == startIndexInTheHeap);
                if (pointer == null) return;

                pointer.SetMarked(true);
            }
        }

        private void Sweep()
        {            
            var cells = _heap.Cells;
            var pointers = _heap.Pointers;
            foreach (var pointer in pointers.Where(x => !x.IsMarked))
            {
                var address = pointer.StartCellIndex;
                for (var i = address; i < address + pointer.AllocationSize; i++)
                {
                    cells[i].Cell = '\0';
                }
            }

            var list = new List<RuntimeHeapPointer>();
            foreach (var pointer in pointers.Where(x => x.IsMarked))
            {
                pointer.SetMarked(false);
                list.Add(pointer);
            }

            pointers = list;
        }

        #endregion
    }
}
