using GarbageCollector.Data.Interfaces;

namespace GarbageCollector.Data
{
    public class RuntimeThread
    {
        private IGarbageCollector _collector;

        private Stack<RuntimeStackItem> _stack;
        private Dictionary<int, RuntimeStackItem> _roots;
        private int _stackSize;
        private int _index;

        public string Name { get; set; }

        public RuntimeThread(IGarbageCollector collector, int stackSize, string name)
        {
            _collector = collector;

            _stack = new Stack<RuntimeStackItem>(stackSize);
            _roots = new Dictionary<int, RuntimeStackItem>();
            _stackSize = stackSize;
            _index = 0;

            Name = name;
        }

        public Dictionary<int, RuntimeStackItem> Roots => _roots;        

        public void PushInstruction(string value)
        {
            _index++;
            if (_index > _stackSize) throw new StackOverflowException();

            var address = _collector.Allocate(value); 
            var stackItem = new RuntimeStackItem(address);

            _stack.Push(stackItem);
            _roots.Add(address, stackItem);
        }

        public void PopInstruction()
        {
            _index--;

            var item = _stack.Pop();
            _roots.Remove(item.StartIndexInTheHeap);
        }
    }

    public class RuntimeStackItem
    {
        public int StartIndexInTheHeap { get; set; }

        public RuntimeStackItem(int startIndexInTheHeap)
        {
            StartIndexInTheHeap = startIndexInTheHeap;
        }
    }
}
