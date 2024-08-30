using System.Reflection;

namespace GarbageCollector.Data
{
    public class RuntimeHeap
    {
        public RuntimeHeap(int size)
        {
            Size = size;
            Cells = new RuntimeHeapCell[size];
            Pointers = new List<RuntimeHeapPointer>();

            for (var i = 0; i < size; i++)
            {
                Cells[i] = new RuntimeHeapCell();
            }
        }

        //public RuntimeHeap(int size, string metadata)
        //{
        //    Size = size;
        //    Cells = new RuntimeHeapCell[size];
        //    Pointers = new List<RuntimeHeapPointer>();

        //    for (var i = 0; i < size; i++)
        //    {
        //        Cells[i] = new RuntimeHeapCell(metadata);
        //    }
        //}

        #region Properties

        public int Size { get; set; }

        public RuntimeHeapCell[] Cells { get; set; }

        public List<RuntimeHeapPointer> Pointers { get; set; }

        #endregion

        
        public void Print()
        {
            var length = Cells.Length;
            for (var i = 0; i < length; i++)
            {
                var display = $"{i.ToString()}".PadRight(4);
                Console.Write(display.Substring(0, 3));
            }

            Console.WriteLine();

            foreach (var cell in Cells)
            {
                var display = $"{cell.Cell}".PadRight(4);
                Console.Write(display.Substring(0, 3));
            }

            Console.WriteLine();
        }
    }

    public class RuntimeHeapPointer
    {
        public int StartCellIndex { get; set; }

        public int AllocationSize { get; set; }

        public bool IsMarked { get; set; }

        public string Metadata { get; set; }

        public RuntimeHeapPointer(int startCellIndex, int allocationSize, string metadata)
        {
            StartCellIndex = startCellIndex;
            AllocationSize = allocationSize;
            IsMarked = false;
            Metadata = metadata;
        }

        public void SetMarked(bool marked)
        {
            IsMarked = marked;
        }

        public void SetMetadata(string metadata)
        {
            Metadata = metadata;            
        }
    }

    public class RuntimeHeapCell
    {
        public char Cell { get; set; }
        public bool IsMarked { get; set; }

        public RuntimeHeapCell()
        {
            Cell = '\0';
            IsMarked = false;
        }

        public void SetMarked()
        {
            IsMarked = true;
        }
    }
}
