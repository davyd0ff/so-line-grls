using Core.BusinessTransactions.dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.GRLS.ApplicantRequests
{
    public static class ApplicantRequestsIdGeneator
    {
        private static int InitId = 2;

        public static int Next()
        {
            return InitId++;
        }
    }
}
