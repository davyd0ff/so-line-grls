using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Entity.Models;
using Core.Enums;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.CommunicationModels;
using Core.Models.CommunicationModels.State;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Grls.Common.Abstract;
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

        #region Создание "Ответа заявителя"
        [TestMethod]
        public void Test_GrlsLPApplicantRequestUsual_ToSending_CreateRequestAnswerWasCalled()
        {
            var applicantRequest = Create.GrlsLPApplicantRequestUsual
                                         .ToStatement(Create.StatementLPRegLimPrice)
                                         .WithInternalState(InternalStates.Formated)
                                         .Please();

            var (observer, testService) =
                                    Create.ChangeGrlsApplicantRequestInternalStateObserver
                                          .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                          .WithRequest(applicantRequest)
                                          .WithNextInternalState(InternalStates.Transferred)
                                          .PleaseWithTestService();


            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Once());
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_ToSending_CreateRequestAnswerWasCalled()
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
        public void Test_GrlsMRApplicantRequestMZ_ToProject_CreateRequestAnswerWasNotCalled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, testService) =
                                     Create.ChangeGrlsApplicantRequestInternalStateObserver
                                           .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                           .WithRequest(applicantRequest)
                                           .WithNextInternalState(InternalStates.Project)
                                           .PleaseWithTestService();


            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Never());
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZWithAnswer_ToSending_CreateRequestAnswerWasNotCalled()
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

        
            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


            testService.ICoreUnitOfWork.Verify(u => u.Get<CreateRequestAnswer>(), Times.Never());
        }
        #endregion
        #region "Ответ заявителя" меняет состояние
            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToProject_AnswerDidNotChangeState()
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
                                               .WithNextInternalState(InternalStates.Project)
                                               .PleaseWithTestService();


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.IsAny<ChangeStateInfo>(), true), Times.Never());
            }


            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToSending_AnswerChangeStateToProject()
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


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.Is<ChangeStateInfo>(
                    csi => csi.Id == answer.Id 
                        && csi.StateId == InternalStates.Project
                        && csi.TypeId == answer.DocumentType.Id
                ), true), Times.Once());
            }


            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToSendApplicant_AnswerChangeStateToProject()
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
                                               .WithNextInternalState(InternalStates.SendApplicant)
                                               .PleaseWithTestService();


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.Is<ChangeStateInfo>(
                    csi => csi.Id == answer.Id
                        && csi.StateId == InternalStates.Project
                        && csi.TypeId == answer.DocumentType.Id
                ), true), Times.Once());
            }

            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToHandledApplicant_AnswerChangeStateToFormated()
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


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.ChangeLongDocumentInternalState.Verify(t => t.Run(It.Is<ChangeStateInfo>(
                    csi => csi.Id == answer.Id
                        && csi.StateId == InternalStates.Formated
                        && csi.TypeId == answer.DocumentType.Id
                ), true), Times.Once());
            }
        #endregion
        #region "Ответ заявителя" сохраняет QRCode
            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToHandledApplicant_AnswerGenerateQRCode()
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


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.IDocumentRepository.Verify(r => r.SaveQrCode(answer.Id, It.IsAny<BarCodeQr>()), Times.Once());
            }
        #endregion
        #region обновление запроса
        [TestMethod]
            public void Test_GrlsMRApplicantRequestMZ_ToSendApplicant_UpdateApplicantRequestWasCalledWithDtHandedFact()
            {
                var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                             .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                             .WithInternalState(InternalStates.Signed)
                                             .Please();

                var (observer, testService) =
                                         Create.ChangeGrlsApplicantRequestInternalStateObserver
                                               .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                               .WithRequest(applicantRequest)
                                               .WithNextInternalState(InternalStates.SendApplicant)
                                               .PleaseWithTestService();


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.UpdateApplicantRequest.Verify(t => t.Run(It.Is<IIdentifiedBase>(entity => entity.Id == applicantRequest.Id)));
            }
        #endregion
        #region Создание "AdditionalMaterialsReceived"
            [TestMethod]
            public void Test_GrlsMRApplicantRequestMZWithAnswer_ToResponseReceived_AdditionalMaterialsReceivedWasCreated()
            {
                var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                             .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                             .Please();

                var (observer, testService) =
                                         Create.ChangeGrlsApplicantRequestInternalStateObserver
                                               .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
                                               .WithRequest(applicantRequest)
                                               .WithNextInternalState(InternalStates.ResponseReceived)
                                               .PleaseWithTestService();


                observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);


                testService.CreateAddMaterialsReceived
                           .Verify(t => t.Run(It.Is<AddMaterialsReceivedCreateParams>(
                               p => p.RequestId == applicantRequest.Id
                                 && p.IncomingNumber == applicantRequest.IncomingPackage.Number
                            )));

            }
        #endregion
    }
}