﻿namespace GarbageCollector.Data.Collectors
{
    public class MarkAndCompactGarbageCollector : Data.Interfaces.GarbageCollector
    {
        private List<RuntimeThread> _threads;
        private RuntimeHeap _heap;

        private int _currentPointerInTheHeap;

        public MarkAndCompactGarbageCollector()
        {
            _threads = new List<RuntimeThread>();
            _heap = new RuntimeHeap(64);

            _currentPointerInTheHeap = 0;
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
            Compact();
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

            var begin = _currentPointerInTheHeap;

            // Do not continue if there is not enough space.
            if ((size - begin) < requiredSize) return -1;

            for (var i = begin; i < begin + requiredSize; i++)
            {
                _heap.Cells[i].Cell = cells[i - begin];
            }

            var pointer = new RuntimeHeapPointer(begin, requiredSize, "");
            _heap.Pointers.Add(pointer);

            _currentPointerInTheHeap = _currentPointerInTheHeap + requiredSize;

            return begin;
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

        private void Compact()
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

            _heap.Pointers = pointers = list;

            var offset = 0;
            foreach (var pointer in pointers)
            {
                var begin = pointer.StartCellIndex;
                if (begin == 0)
                {
                    offset = offset + pointer.AllocationSize;
                    continue;
                }

                // Update roots in the stack
                // This code is perfectible.
                var thread = _threads.FirstOrDefault(x => x.Roots.Any(t => t.Key == begin));
                var root = thread.Roots.FirstOrDefault(x => x.Key == begin);
                root.Value.StartIndexInTheHeap = offset;
                thread.Roots[offset] = root.Value; thread.Roots.Remove(begin);

                // Update pointers in the heap
                pointer.StartCellIndex = offset;
                for (var i = begin; i < begin + pointer.AllocationSize; i++)
                {
                    cells[offset + i - begin].Cell = cells[i].Cell;
                    cells[i].Cell = '\0';
                    
                }

                offset = offset + pointer.AllocationSize;
            }

            _currentPointerInTheHeap = offset;
        }

        #endregion
    }
}
