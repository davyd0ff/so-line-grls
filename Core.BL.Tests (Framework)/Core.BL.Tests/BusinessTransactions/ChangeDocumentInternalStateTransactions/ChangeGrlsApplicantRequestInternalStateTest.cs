using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.PortalConfiguration;
using Core.Enums;
using Core.Infrastructure;


namespace Core.BL.Tests.BusinessTransactions
{
    [TestClass]
    public class ChangeGrlsApplicantRequestInternalStateTest
    {
        private readonly Create Create;
        public ChangeGrlsApplicantRequestInternalStateTest()
        {
            this.Create = new Create();
            GlobalProperties.SetApplicationJobUserID(1);
        }

        //[TestMethod]
        //public void Test_GrlsMRApplicantRequestFGBU // TODO у FGBU своя транзакция
        //public Test_CannotTransitionTo

        // TODO DEV: Нужны тесты для FGBU
        // TODO DEV: Нужны тесты для Inspect
        // TODO DEV: Нужны тесты для CS (clinical searches)
        // TODO DEV: Нужны тесты для LP (limit price)
        // TODO DEV: Нужен тест для пользователя с правами заявителя (см. DocRequest2.bRequestHandled_ServerClick())
        //[TestMethod]
        //public void Test_GrlsCSApplicantRequestMZ_UserHasPermission()
        //{
        //    var user = Create.User
        //                     .WithPermissions(Actions.InternalStateChange)
        //                     .Please();
        //    var statement = Create.CSStatementReg.WithInternalState(InternalStates.Entered);
        //    var csApplicantRequest = Create.GrlsCSApplicantRequest
        //                                   .ToStatement(statement)
        //                                   .WithInternalstate(InternalStates.Project)

        //}

        //[TestMethod]
        //public void Test_GrlsMRApplicantRequestMZ_To


        [TestMethod]
        public void Test_GrlsCSApplicantRequestMZ_ToSending()
        {
            
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_ToSending_AndUserHasPermissions()
        {
            var user = Create.User
                             .WithPermissions(Actions.InternalStateChange)
                             .Please();

            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered);

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(statement)
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var transition = Create.StateTransition
                                   .ForDocumentType(applicantRequest.DocumentType)
                                   .From(InternalStates.Signed)
                                   .To(InternalStates.Sending)
                                   .HasPermissions(Actions.InternalStateChange)
                                   .WithTriggers(
                                         Create.Trigger
                                               .ForDocument(statement)
                                               .ToInternalState(InternalStates.RequestFormed),
                                         Create.Trigger
                                               .ForDocument(applicantRequest)
                                               .ToOuterState(OuterStates.SendApplicant)
                                    )
                                   .Please();

            var (transaction, testService) =
                                     Create.ChangeGrlsApplicantRequestInternalState
                                           .WithUser(user)
                                           .WithDocument(applicantRequest)
                                           .WithNextInternalState(InternalStates.Sending)
                                           .WithInternalStateTransitions(transition)
                                           .PleaseWithTestService();

            int transitionId = transition.Id;
            long documentId = applicantRequest.DocumentId;
            int stateId = InternalStates.Sending;
            var changeStateInfo = new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                Guid = applicantRequest.RoutingGuid,
                StateId = InternalStates.Sending,
                Level = RegistrationStatementLevelEnum.ЭкспертныйМЗ
            };


            var result = transaction.Run(changeStateInfo, true);


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentStateRepository.Verify(repo => repo.SetState(documentId, stateId, It.IsAny<int?>()), Times.Once());
            testService.ICoreUnitOfWork.Verify(unit => unit.OnTransactionSuccess(It.IsAny<TransactionParams>()), Times.Once());
            testService.ICoreUnitOfWork.Verify(unit => unit.OnTransactionSuccess(It.Is<TransactionParams>(
                tp => tp.IncomingParams[0] is ChangeStateInfo
                   && ((ChangeStateInfo)tp.IncomingParams[0]).Id == documentId
                   && ((ChangeStateInfo)tp.IncomingParams[0]).StateId == stateId
                   && ((ChangeStateInfo)tp.IncomingParams[0]).Transition.Id == transitionId
                   && ((ChangeStateInfo)tp.IncomingParams[0]).IsChanged)));
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_ToCanceled()
        {
            var user = Create.User
                             .WithPermissions(Actions.InternalStateChange)
                             .Please();

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Project)
                                         .Please();

            var (transaction, testService) = Create.ChangeGrlsApplicantRequestInternalState
                                                   .WithUser(user)
                                                   .WithDocument(applicantRequest)
                                                   .WithNextInternalState(InternalStates.Canceled)
                                                   .WithInternalStateTransitions(
                                                       Create.StateTransition
                                                             .ForDocumentType(applicantRequest.DocumentType)
                                                             .From(InternalStates.Project)
                                                             .To(InternalStates.Canceled)
                                                             .HasPermissions(Actions.InternalStateChange)
                                                             .WithoutTriggers()
                                                    )
                                                   .PleaseWithTestService();


            long documentId = applicantRequest.DocumentId;
            int canceledStateId = InternalStates.Canceled;
            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                Guid = applicantRequest.RoutingGuid,
                StateId = InternalStates.Canceled,
                Level = RegistrationStatementLevelEnum.ЭкспертныйМЗ
            }, true);


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentStateRepository.Verify(
                repo => repo.SetState(documentId, canceledStateId, It.IsAny<int?>()),
                Times.Once()
            );
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_Level1_ToCanceled_AndUserHasPermissions()
        {
            var user = Create.User
                             .WithPermissions(Actions.InternalStateChange)
                             .Please();

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Project)
                                         .Please();

            var (transaction, testService) = Create.ChangeGrlsApplicantRequestInternalState
                                                   .WithUser(user)
                                                   .WithDocument(applicantRequest)
                                                   .WithNextInternalState(InternalStates.Canceled)
                                                   .WithInternalStateTransitions(
                                                       Create.StateTransition
                                                             .ForDocumentType(applicantRequest.DocumentType)
                                                             .From(InternalStates.Project)
                                                             .To(InternalStates.Canceled)
                                                             .HasPermissions(Actions.InternalStateChange)
                                                             .WithoutTriggers()
                                                    )
                                                   .PleaseWithTestService();


            long documentId = applicantRequest.DocumentId;
            int canceledStateId = InternalStates.Canceled;
            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                Guid = applicantRequest.RoutingGuid,
                StateId = InternalStates.Canceled,
                Level = RegistrationStatementLevelEnum.Заявительский
            }, true);


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentStateRepository.Verify(
                repo => repo.SetState(documentId, canceledStateId, It.IsAny<int?>()),
                Times.Once()
            );
        }

    }
}
