using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Entity.Models;
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
        public void Test_ChangeLongDocumentInternalStateObserver_RunTrigger_InternalState()
        {
            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered);

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(statement)
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, incomingParams, spy_UnitOfWork) =
                           Create.ChangeLongDocumentInternalStateObserver
                                 .WithDocument(applicantRequest)
                                 .WithInternalStateTransitions(
                                        Create.StateTransition
                                              .ForDocumentType(applicantRequest.DocumentType)
                                              .From(InternalStates.Signed)
                                              .To(InternalStates.Sending)
                                              .HasPermissions(Actions.InternalStateChange)
                                              .WithTriggers(
                                                    Create.Trigger
                                                          .ForDocument(statement)
                                                          .ToInternalState(InternalStates.RequestFormed)
                                               )
                                  )
                                 .PleaseWitIncomingParamsAndSpies();



            observer.Execute(spy_UnitOfWork.Object, incomingParams);



            Assert.IsNotNull(spy_UnitOfWork);
            spy_UnitOfWork.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Never());
            spy_UnitOfWork.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Once());
        }


        [TestMethod]
        public void Test_ChangeLongDocumentInternalStateObserver_RunTrigger_ExternalState()
        {
            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered);

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(statement)
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, incomingParams, spy_UnitOfWork) = 
                           Create.ChangeLongDocumentInternalStateObserver
                                 .WithDocument(applicantRequest)
                                 .WithInternalStateTransitions(
                                        Create.StateTransition
                                              .ForDocumentType(applicantRequest.DocumentType)
                                              .From(InternalStates.Signed)
                                              .To(InternalStates.Sending)
                                              .HasPermissions(Actions.InternalStateChange)
                                              .WithTriggers(
                                                    Create.Trigger
                                                          .ForDocument(applicantRequest)
                                                          .ToOuterState(OuterStates.SendApplicant)
                                               )
                                  )
                                 .PleaseWitIncomingParamsAndSpies();



            observer.Execute(spy_UnitOfWork.Object, incomingParams);



            Assert.IsNotNull(spy_UnitOfWork);
            spy_UnitOfWork.Verify(u => u.Get<ChangeLongDocumentExternalState>(), Times.Once());
            spy_UnitOfWork.Verify(u => u.Get<ChangeGrlsApplicantRequestExternalState>(), Times.Once());
        }
    }
}

namespace Grls.Sync.Tests.Helpers.GRLS
{
    public partial class Create
    {
        public ChangeLongDocumentInternalStateObserverBuilder ChangeLongDocumentInternalStateObserver =>
            new ChangeLongDocumentInternalStateObserverBuilder(this.unitOfWork);
    }
}
namespace Grls.Sync.Tests.Helpers
{
    public class ChangeLongDocumentInternalStateObserverBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;
        private ChangeStateInfo changeStateInfo;

        public ChangeLongDocumentInternalStateObserverBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this.changeStateInfo = new ChangeStateInfo();

            this._unitOfWork.Setup(u => u.Get<ChangeDocumentInternalState>())
                            .Returns(new MockChangeDocumentInternalState(this._unitOfWork));

            this._unitOfWork.Setup(u => u.Get<ChangeLongDocumentExternalState>())
                            .Returns(new MockChangeLongDocumentExternalState(this._unitOfWork));

            this._unitOfWork.Setup(u => u.Get<ChangeGrlsApplicantRequestInternalState>())
                            .Returns(new MockChangeGrlsApplicantRequestInternalState(this._unitOfWork));

            this._unitOfWork.Setup(u => u.Get<ChangeGrlsApplicantRequestExternalState>())
                            .Returns(new MockChangeGrlsApplicantRequestExternalState(this._unitOfWork));
        }


        public ChangeLongDocumentInternalStateObserverBuilder WithDocument(MedicamentRegistrationApplicantRequest applicantRequest)
        {

            this.changeStateInfo.Id = applicantRequest.DocumentId;
            this.changeStateInfo.TypeId = applicantRequest.DocumentType.Id;
            this.changeStateInfo.Guid = applicantRequest.RoutingGuid;

            var mockIIdentifiedLongRepositoryForDocument = new Mock<IIdentifiedLongRepository<Document>>();
            mockIIdentifiedLongRepositoryForDocument
                .Setup(r => r.GetById(applicantRequest.DocumentId))
                .Returns(new Document
                {
                    Id = applicantRequest.DocumentId,
                    DocumentType = applicantRequest.DocumentType,
                    DocumentTypeId = applicantRequest.DocumentType.Id,
                    //IncomingPackage = applicantRequest.IncomingPackage,
                    RoutingGuid = applicantRequest.RoutingGuid
                });

            this._unitOfWork.Setup(u => u.Get(typeof(IIdentifiedLongRepository<Document>)))
                            .Returns(mockIIdentifiedLongRepositoryForDocument.Object);  

            return this;
        }

        public ChangeLongDocumentInternalStateObserverBuilder WithInternalStateTransitions(StateTransition transition)
        {
            this.changeStateInfo.Transition = transition;
            this.changeStateInfo.IsChanged = true;
            this.changeStateInfo.StateId = transition.ToStateId;


            var IIdentifiedRepositoryMock = new Mock<IIdentifiedRepository>();
            IIdentifiedRepositoryMock
                .Setup(r => r.FindById(It.IsAny<int>()))
                .Returns(transition.ToState);

            this._unitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(InternalState).Name))))
                .Returns(IIdentifiedRepositoryMock.Object);


            return this;
        }

        public ChangeLongDocumentInternalStateObserver Please()
        {
            return new ChangeLongDocumentInternalStateObserver();
        }

        public (ChangeLongDocumentInternalStateObserver, object[], Mock<ICoreUnitOfWork>) PleaseWitIncomingParamsAndSpies()
        {
            return (
                this.Please(),
                new object[] { this.changeStateInfo },
                this._unitOfWork
            );
        }

        internal sealed class MockChangeDocumentInternalState : ChangeDocumentInternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;

            public MockChangeDocumentInternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateRequest request)
            {
                return ValidationResult.Succeeded();
            }
            protected override TransactionResult PerformTransaction(ChangeStateRequest request)
            {
                return TransactionResult.Succeeded();
            }
        }
        internal sealed class MockChangeGrlsApplicantRequestInternalState : ChangeGrlsApplicantRequestInternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;
            public MockChangeGrlsApplicantRequestInternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo changeStateInfo, bool withTriggers)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return TransactionResult.Succeeded();
            }
        }
        
        internal sealed class MockChangeLongDocumentExternalState : ChangeLongDocumentExternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;
            public MockChangeLongDocumentExternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object){ 
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo info)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info)
            {
                return TransactionResult.Succeeded();
            }
        }
        internal sealed class MockChangeGrlsApplicantRequestExternalState : ChangeGrlsApplicantRequestExternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;

            public MockChangeGrlsApplicantRequestExternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo changeExternalStateInfo)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo changeExternalStateInfo)
            {
                return TransactionResult.Succeeded();
            }
        }
    }
}