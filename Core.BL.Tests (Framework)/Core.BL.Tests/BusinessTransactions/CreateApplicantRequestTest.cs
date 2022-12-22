using Core.BL.Tests.GRLS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions
{
    [TestClass]
    public class CreateApplicantRequestTest
    {
        private readonly Create Create;

        public CreateApplicantRequestTest()
        {
            this.Create = new Create();
        }



        [TestMethod]
        public void Test_CreateApplicantRequest_ForCSApplicantRequestFGBUWithClinicalStudyChangeResolutionStatement()
        {
            //var changeStatement = Create.ChangeResolutionStatementCS
            //                            .WithParentStatement(Create.StatementPermissionCS)
            //                            .Please();

            //var applicantReqeustModel = Create.GrlsCSApplicantRequestFGBU
            //                                  .ToStatement(changeStatement)
            //                                  .Please();

            //var (transaction, spyOfUnit) = 
            //                  Create.CreateApplicantRequestBase
            //                        .WithDocument()
            //                        .PleaseWithSpy();


            //var result = transaction.Run();

        }

    }
}
