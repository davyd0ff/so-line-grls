using Core.BL.Tests.GRLS;
using Core.Models.Documents.LimitedPrice;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Core.BL.Tests.BusinessTransactions
{
    [TestClass]
    public class UpdateLimitedPriceApplicantRequestTest
    {
        private readonly Create Create;

        public UpdateLimitedPriceApplicantRequestTest()
        {
            this.Create = new Create();
        }


        [TestMethod]
        public void Test_LimitedPriceApplicantRequestRepository_Update_WasCalled()
        {
            var applicantRequest = Create.GrlsLPApplicantRequestUsual.ToStatement(Create.StatementRegLimPrice).Please();
            var (transaction, testService) = Create.UpdateLimitedPriceApplicantRequest
                                                   .FindByIdMustReturns(applicantRequest)
                                                   .StatementIsNotArchived(applicantRequest.IncomingPackage)
                                                   .PleaseWithTestService();

            var verifyApplicantRequestId = applicantRequest.Id;
            var verifyDocumentId = applicantRequest.DocumentId;
            var verifyRoutingGuid = applicantRequest.RoutingGuid;


            transaction.Run(applicantRequest);


            testService.LimitedPriceApplicantRequestRepository.Verify(repo =>
                repo.Update(It.Is<ApplicantRequestLimitedPrice>(ar =>
                    ar.Id.Equals(verifyApplicantRequestId) &&
                    ar.DocumentId.Equals(verifyDocumentId) &&
                    ar.RoutingGuid.Equals(verifyRoutingGuid)
                ))
                , Times.Once());
        }
    }
}
