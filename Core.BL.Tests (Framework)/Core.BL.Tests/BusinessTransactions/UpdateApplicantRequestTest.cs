using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models.Common;
using Core.Models.Common;
using Core.Models.Documents.MedicamentRegistration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Core.BL.Tests.BusinessTransactions
{
    [TestClass]
    public class UpdateApplicantRequestTest
    {
        private readonly Create Create;
        public UpdateApplicantRequestTest()
        {
            this.Create = new Create();
        }


        [TestMethod]
        public void Test_MedicamentRegistrationApplicantRequestRepository_Update_WasCalled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ.ToStatement(Create.StatementMR).Please();
            var (transaction, testService) = Create.UpdateApplicantRequest
                                                   .FindByIdMustReturns(applicantRequest)
                                                   .StatementIsNotArchived(applicantRequest.IncomingPackage)
                                                   .PleaseWithTestService();

            var verifyApplicantRequestId = applicantRequest.Id;
            var verifyDocumentId = applicantRequest.DocumentId;
            var verifyRoutingGuid = applicantRequest.RoutingGuid;


            transaction.Run(applicantRequest);


            testService.MedicamentRegistrationApplicantRequestRepository.Verify(
                repo => repo.Update(It.Is<MedicamentRegistrationApplicantRequest>(ar => 
                    ar.Id.Equals(verifyApplicantRequestId) &&
                    ar.DocumentId.Equals(verifyDocumentId) &&
                    ar.RoutingGuid.Equals(verifyRoutingGuid)
                )),
                Times.Once());
        }


        [TestMethod]
        public void Test_CustomEventRepository_Add_WasCalled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ.ToStatement(Create.StatementMR).Please();
            var (transaction, testService) = Create.UpdateApplicantRequest
                                                   .FindByIdMustReturns(applicantRequest)
                                                   .StatementIsNotArchived(applicantRequest.IncomingPackage)
                                                   .PleaseWithTestService();

            var verifyApplicantRequestId = applicantRequest.Id;
            var verifyDocumentId = applicantRequest.DocumentId;
            var verifyRoutingGuid = applicantRequest.RoutingGuid;


            transaction.Run(applicantRequest);


            testService.CustomEventRepository.Verify(
                repo => repo.Add(It.Is<CustomEvent>(evnt => evnt.EventType.Id.Equals(1003))),
                Times.Once());
        }
    }
}
