﻿using Core.DataAcquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.IDGenerator
{
    public static class IncomingNumberGenerator
    {
        private static int InitId = 2;

        public static int Next()
        {
            return InitId++;
        }
    }
}
