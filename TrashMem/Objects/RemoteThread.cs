using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashMemCore.Objects
{
    public class RemoteThread
    {
        public uint Handle { get; private set; }

        public RemoteThread(uint handle)
        {
            Handle = handle;
        }

        public override string ToString()
        {
            return $"Thread => {Handle.ToString("X")}";
        }
    }
}
