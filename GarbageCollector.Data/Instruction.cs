namespace GarbageCollector.Data
{
    public class Instruction
    {
        public string Name { get; set; }

        public string Operation { get; set; }

        public string Value { get; set; }

        public Instruction(string name, string operation, string value) 
        { 
            Name = name;
            Operation = operation;
            Value = value;
        }
    }
}
