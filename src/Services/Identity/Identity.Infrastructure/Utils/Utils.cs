using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Utils
{
    public static class Utils
    {
        public static bool ContainDuplicates<T>(List<T> list)
        {
            HashSet<T> set = new();
            foreach (var item in list)
            {
                if (!set.Add(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
