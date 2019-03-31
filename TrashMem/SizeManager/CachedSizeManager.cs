using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TrashMem.SizeManager
{
    public class CachedSizeManager
    {
        public Dictionary<Type, int> SizeCache { get; private set; }

        public CachedSizeManager()
        {
            SizeCache = new Dictionary<Type, int>();
        }

        public int SizeOf(Type t)
        {
            if (SizeCache.ContainsKey(t)) return SizeCache[t];

            DynamicMethod dm = new DynamicMethod("SizeOf", typeof(int), new Type[] { });
            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Sizeof, t);
            il.Emit(OpCodes.Ret);

            int size = (int)dm.Invoke(null, null);
            SizeCache.Add(t, size);

            return size;
        }
    }
}
