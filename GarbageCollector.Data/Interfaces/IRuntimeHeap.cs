using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCollector.Data.Collectors.Interfaces
{
    public interface IRuntimeHeap
    {
        public int Allocate(string field);
    }
}
