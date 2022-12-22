using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Entity.Models;
using Core.Enums;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.CommunicationModels.State;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Grls.Sync.Tests.Helpers.GRLS;
using Grls.Sync.Tests.Helpers.Models;
using Grls.Sync.Tests.Helpers.Models.Common;
using grlsSync.Observers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Grls.Sync.Tests.Observers.ChangeDocumentInternalStateObservers
{
    [TestClass]
    public class ChangeGrlsApplicantRequestInternalStateObserverTest
    {
        private readonly Create Create;

        public ChangeGrlsApplicantRequestInternalStateObserverTest()
        {
            this.Create = new Create();
        }


        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_ToSending_AndUserHasPermissions_CreateRequestAnswerWasCalled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, testService) =
                                     Create.ChangeGrlsApplicantRequestInternalStateObserver
                                           .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                           .WithRequest(applicantRequest)
                                           .WithNextInternalState(InternalStates.Sending)
                                           .PleaseWithTestService();


            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Once());
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZWithAnswer_ToSending_CreateRequestAnswerWasNotCalledAndAnswerChangeStateToProject()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var answer = Create.RequestAnswerBaseLong
                               .WithRequest(applicantRequest)
                               .Please();

            var (observer, testService) =
                                     Create.ChangeGrlsApplicantRequestInternalStateObserver
                                           .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                           .WithRequest(applicantRequest)
                                           .WithRequestAnswer(answer)
                                           .WithNextInternalState(InternalStates.Sending)
                                           .PleaseWithTestService();

            // TODO надо проинициализировать DocumentType в RequestAnswer


            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Never());
            testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.Is<ChangeStateInfo>(
                csi => csi.Id == answer.Id 
                    && csi.StateId == InternalStates.Project
                    && csi.TypeId == answer.DocumentType.Id
            ), true));
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZWithAnswer_ToHandledApplicant_CreateRequestAnswerWasNotCalledAndAnswerChangeStateToFormated()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         //.WithInternalState(InternalStates.Signed)
                                         .Please();

            var answer = Create.RequestAnswerBaseLong
                               .WithRequest(applicantRequest)
                               .Please();

            var (observer, testService) =
                                     Create.ChangeGrlsApplicantRequestInternalStateObserver
                                           .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                           .WithRequest(applicantRequest)
                                           .WithRequestAnswer(answer)
                                           .WithNextInternalState(InternalStates.HandledApplicant)
                                           .PleaseWithTestService();

            // TODO надо проинициализировать DocumentType в RequestAnswer


            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Never());
            testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.Is<ChangeStateInfo>(
                csi => csi.Id == answer.Id
                    && csi.StateId == InternalStates.Formated
                    && csi.TypeId == answer.DocumentType.Id
            ), true));
            testService.IDocumentRepository.Verify(r => r.SaveQrCode(answer.Id, It.IsAny<BarCodeQr>()), Times.Once());
        }
    }
}