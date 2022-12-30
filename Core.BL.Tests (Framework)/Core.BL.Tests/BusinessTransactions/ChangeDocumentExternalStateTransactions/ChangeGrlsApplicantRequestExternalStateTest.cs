using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Infrastructure.DocumentsRouter;
using Core.Models.BusinessTransactions;
using Core.PortalConfiguration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Core.BL.Tests.BusinessTransactions.ChangeDocumentExternalStateTransactions
{
    [TestClass]
    public class ChangeGrlsApplicantRequestExternalStateTest
    {
        private readonly Create Create;

        public ChangeGrlsApplicantRequestExternalStateTest()
        {
            GlobalProperties.SetApplicationJobUserID(1);
            this.Create = new Create();
        }

        [TestMethod]
        public void Test_ChangeGrlsApplicantRequestExternalState_()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .WithOuterState(OuterStates.Project)
                                         .Please();

            var (transaction, testService) =
                    Create.ChangeGrlsApplicantRequestExternalState
                          .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                          .WithDocument(applicantRequest)
                          .WithNextOuterState(OuterStates.SendApplicant)
                          .WithOuterStateTransitions(
                                Create.OuterStateTransition
                                      .ForDocumentType(applicantRequest.DocumentType)
                                      .From(OuterStates.Project)
                                      .To(OuterStates.SendApplicant)
                                      .HasPermissions(Actions.InternalStateChange)
                           )
                          .PleaseWithTestService();



            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                StateId = OuterStates.SendApplicant,
                TypeId = applicantRequest.DocumentType.Id,
            });



            testService.IsTrue(result.IsSuccess);
            testService.IDocumentStateRepository.Verify(
                r => r.SetState(applicantRequest.DocumentId, It.IsAny<int>(), It.IsAny<int?>()),
                Times.Once());
        }
    }
}