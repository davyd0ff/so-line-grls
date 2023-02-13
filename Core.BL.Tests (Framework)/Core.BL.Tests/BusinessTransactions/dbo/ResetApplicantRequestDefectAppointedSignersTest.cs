using Core.BL.Tests.GRLS;
using Core.BL.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions.dbo
{
    [TestClass]
    public class ResetApplicantRequestDefectAppointedSignersTest
    {
        private readonly Create Create;

        public ResetApplicantRequestDefectAppointedSignersTest()
        {
            this.Create = new Create();
            Loader.DocumentTypes();
        }

        [TestMethod]
        public void Test_Succeded_FromProjectToIndorse()
        {


            //var (transaction, testService) = Create.ResetApplicantRequestDefectAppointedSigners
            //                                       .

        }
    }
}
