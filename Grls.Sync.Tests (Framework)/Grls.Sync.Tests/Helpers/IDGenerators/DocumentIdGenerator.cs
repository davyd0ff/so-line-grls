using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.IDGenerator
{
    public static class DocumentIdGenerator
    {
        private static long InitId = 20000000;

        public static long Next()
        {
            return InitId++;
        }
    }
}
