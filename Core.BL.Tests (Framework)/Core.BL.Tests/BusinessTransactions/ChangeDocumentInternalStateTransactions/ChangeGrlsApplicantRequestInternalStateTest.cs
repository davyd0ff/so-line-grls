using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.Common;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Core.PortalConfiguration;
using Core.Enums;
using Core.Models.BusinessTransactions;
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

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_Level3_ToSending_AndUserHasPermissions()
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

            var (transaction, spy_IDocumentStateRepository, spy_UnitOfWork) = 
                                     Create.ChangeGrlsApplicantRequestInternalState
                                           .WithUser(user)
                                           .WithDocument(applicantRequest)
                                           .WithNextInternalState(InternalStates.Sending)
                                           .WithInternalStateTransitions(transition)
                                           .PleaseWithSpies();

            int transitionId = transition.Id;
            long documentId = applicantRequest.DocumentId;
            int stateId = InternalStates.Sending;
            var changeStateInfo = new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                StateId = InternalStates.Sending,
                Level = RegistrationStatementLevelEnum.ЭкспертныйМЗ
            };
            
            
            var result = transaction.Run(changeStateInfo, true);


            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(spy_IDocumentStateRepository);
            Assert.IsNotNull(spy_UnitOfWork);

            spy_IDocumentStateRepository.Verify(
                repo => repo.SetState(documentId, stateId, It.IsAny<int?>()),
                Times.Once()
            );

            spy_UnitOfWork.Verify(
                unit => unit.OnTransactionSuccess(It.IsAny<Core.Infrastructure.TransactionParams>()),
                Times.Once()
            );

            spy_UnitOfWork.Verify(
                unit => unit.OnTransactionSuccess(It.Is<TransactionParams>(
                    tp => tp.IncomingParams[0] is ChangeStateInfo 
                       && ((ChangeStateInfo) tp.IncomingParams[0]).Id == documentId
                       && ((ChangeStateInfo) tp.IncomingParams[0]).StateId == stateId
                       && ((ChangeStateInfo) tp.IncomingParams[0]).Transition.Id == transitionId
                       && ((ChangeStateInfo) tp.IncomingParams[0]).IsChanged)));
        }

        [TestMethod]
        public void Test_GrlsMRApplicantRequestMZ_Level3_ToCanceled_AndUserHasPermissions()
        {
            var user = Create.User
                             .WithPermissions(Actions.InternalStateChange)
                             .Please();

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(Create.StatementMR.WithInternalState(InternalStates.Entered))
                                         .WithInternalState(InternalStates.Project)
                                         .Please();

            var (transaction, spy) = Create.ChangeGrlsApplicantRequestInternalState
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
                                           .PleaseWithSpy();


            long documentId = applicantRequest.DocumentId;
            int canceledStateId = InternalStates.Canceled;
            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                StateId = InternalStates.Canceled,
                Level = RegistrationStatementLevelEnum.ЭкспертныйМЗ
            }, true);


            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(spy);
            spy.Verify(
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

            var (transaction, spy) = Create.ChangeGrlsApplicantRequestInternalState
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
                                           .PleaseWithSpy();


            long documentId = applicantRequest.DocumentId;
            int canceledStateId = InternalStates.Canceled;
            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                TypeId = applicantRequest.DocumentType.Id,
                StateId = InternalStates.Canceled,
                Level = RegistrationStatementLevelEnum.Заявительский
            }, true);


            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(spy);
            spy.Verify(
                repo => repo.SetState(documentId, canceledStateId, It.IsAny<int?>()),
                Times.Once()
            );
        }

    }
}

namespace Core.BL.Tests.GRLS
{
    public partial class Create
    {
        public ChangeGrlsApplicantRequestInternalStateBuilder ChangeGrlsApplicantRequestInternalState =>
            new ChangeGrlsApplicantRequestInternalStateBuilder(unitOfWork);
    }

    public class ChangeGrlsApplicantRequestInternalStateBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;
        private Mock<IDocumentStateRepository> _documentStateRepository;

        public ChangeGrlsApplicantRequestInternalStateBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._documentStateRepository = new Mock<IDocumentStateRepository>();

            this._documentStateRepository
                .Setup(r => r.SetState(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>()));
        }

        public ChangeGrlsApplicantRequestInternalStateBuilder WithDocument(MedicamentRegistrationApplicantRequest applicantRequest)
        {
            var IRoutableRepositoryMock = new Mock<IRoutableRepository>();
            IRoutableRepositoryMock
                .Setup(r => r.FindByGuid(It.IsAny<Guid>()))
                .Returns(applicantRequest);


            this._documentStateRepository
                .Setup(r => r.GetState(It.Is<long>(p => p.Equals(applicantRequest.DocumentId))))
                .Returns(new InternalState
                {
                    Id = applicantRequest.InternalState.Id,
                    Code = applicantRequest.InternalState.Code
                });

            this._unitOfWork
                .Setup(u => u.Get<IRoutableRepository>(It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                .Returns(IRoutableRepositoryMock.Object);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateBuilder WithNextInternalState(InternalState internalState)
        {
            var IIdentifiedRepositoryMock = new Mock<IIdentifiedRepository>();
            IIdentifiedRepositoryMock
                .Setup(r => r.FindById(It.IsAny<int>()))
                .Returns(internalState);

            this._unitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(InternalState).Name))))
                .Returns(IIdentifiedRepositoryMock.Object);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateBuilder WithUser(OldUser user)
        {
            this._unitOfWork.Setup(u => u.User)
                            .Returns(user);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateBuilder WithInternalStateTransitions(params StateTransition[] transitions)
        {
            var IStateTransitionRepositioryMock = new Mock<IStateTransitionRepositiory>();
            IStateTransitionRepositioryMock
                .Setup(r => r.GetByType(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(transitions);

            this._unitOfWork
                .Setup(u => u.Get<IStateTransitionRepositiory>(It.Is<string>(p => p.Equals(typeof(InternalStateTransition).Name))))
                .Returns(IStateTransitionRepositioryMock.Object);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalState Please()
        {
            this._unitOfWork
                .Setup(u => u.Get<IDocumentStateRepository>(It.Is<string>(p => p.Equals(typeof(InternalStateTransition).Name))))
                .Returns(this._documentStateRepository.Object);

            return new ChangeGrlsApplicantRequestInternalState(this._unitOfWork.Object);

        }

        public (ChangeGrlsApplicantRequestInternalState, Mock<IDocumentStateRepository>) PleaseWithSpy()
        {
            return (this.Please(), this._documentStateRepository);
        }

        public (ChangeGrlsApplicantRequestInternalState, Mock<IDocumentStateRepository>, Mock<ICoreUnitOfWork>) PleaseWithSpies()
        {
            return (this.Please(), this._documentStateRepository, this._unitOfWork);
        }
    }
}
