using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Entity.Models;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.Documents;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories;
using Core.Repositories.Abstract;
using Grls.Sync.Tests.Helpers.GRLS;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Grls.Sync.Tests.Helpers.Models.Common;
using grlsSync.Observers;
using Moq;
using System;

namespace Grls.Sync.Tests.Helpers
{
    public class ChangeGrlsApplicantRequestInternalStateObserverBuilder
    {
        private Mock<ICoreUnitOfWork> _mockedCoreUnitOfWork;
        private Mock<IDbContext> _mockedDbContext;
        private Mock<IBinaryBusinessTransaction<ChangeStateInfo, bool>> _mockedChangeLongDocumentInternalState;
        private Mock<IDocumentRepository> _mockedIDocumentRepository;


        private ChangeStateInfo changeStateInfo;

        public ChangeGrlsApplicantRequestInternalStateObserverBuilder(
            Mock<ICoreUnitOfWork> mockedCoreUnitOfWork,
            Mock<IDbContext> mockedDbContext
        )
        {
            this._mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            this._mockedDbContext = mockedDbContext;

            this._mockedChangeLongDocumentInternalState = new Mock<IBinaryBusinessTransaction<ChangeStateInfo, bool>>();
            this._mockedChangeLongDocumentInternalState
                .Setup(tran => tran.Run(It.IsAny<ChangeStateInfo>(), It.IsAny<bool>()))
                .Returns(TransactionResult.Succeeded());

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<IBinaryBusinessTransaction<ChangeStateInfo, bool>>())
                .Returns(this._mockedChangeLongDocumentInternalState.Object);


            this.changeStateInfo = new ChangeStateInfo();


            //this._mockedCoreUnitOfWork
            //    .Setup(u => u.UpdateQrCode(It.IsAny<ApplicantRequestBaseLong>()));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeDocumentInternalState>())
                .Returns(new MockChangeDocumentInternalState(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeLongDocumentInternalState>())
                .Returns(new Mock_Successed_ChangeLongDocumentInternalState(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeLongDocumentExternalState>())
                .Returns(new Mock_Successed_ChangeLongDocumentExternalState(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeGrlsApplicantRequestInternalState>())
                .Returns(new MockChangeGrlsApplicantRequestInternalState(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeGrlsApplicantRequestExternalState>())
                .Returns(new MockChangeGrlsApplicantRequestExternalState(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeLongDocumentInternalStateObserver>())
                .Returns(new Mock_Successed_ChangeLongDocumentInternalStateObserver(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<CreateRequestAnswer>())
                .Returns(new Mock_Successed_CreateRequestAnswer(this._mockedCoreUnitOfWork));

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<RequestAnswerBaseLongRepository>())
                .Returns(new MockRequestAnswerBaseLongRepository(this._mockedCoreUnitOfWork, this._mockedDbContext, null));


            this.MockInternalStates();
            this.MockIDocumentRepository();
        }

        private void MockIDocumentRepository()
        {
            this._mockedIDocumentRepository = new Mock<IDocumentRepository>();

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<IDocumentRepository>())
                .Returns(this._mockedIDocumentRepository.Object);

        }

        protected void MockInternalStates()
        {
            var mockedInternalStateThesaurusRepository = new Mock<IThesaurusRepository>();
            mockedInternalStateThesaurusRepository
                .Setup(r => r.FindByCode(It.Is<string>(code => code.Equals(InternalStates.Project))))
                .Returns(InternalStates.Project.Please());

            mockedInternalStateThesaurusRepository
                .Setup(r => r.FindByCode(It.Is<string>(code => code.Equals(InternalStates.Sending))))
                .Returns(InternalStates.Sending.Please());

            mockedInternalStateThesaurusRepository
                .Setup(r => r.FindByCode(It.Is<string>(code => code.Equals(InternalStates.Formated))))
                .Returns(InternalStates.Formated.Please());

            mockedInternalStateThesaurusRepository
                .Setup(r => r.FindByCode(It.Is<string>(code => code.Equals(InternalStates.HandledApplicant))))
                .Returns(InternalStates.HandledApplicant.Please());



            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<IThesaurusRepository>(It.Is<string>(code => code.Equals(typeof(InternalState).Name))))
                .Returns(mockedInternalStateThesaurusRepository.Object);
        }


        public ChangeGrlsApplicantRequestInternalStateObserverBuilder WithUser(OldUser user)
        {
            this._mockedCoreUnitOfWork
                .Setup(u => u.User)
                .Returns(user);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateObserverBuilder WithRequest(MedicamentRegistrationApplicantRequest applicantRequest)
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

            this._mockedCoreUnitOfWork.Setup(u => u.Get(typeof(IIdentifiedLongRepository<Document>)))
                                      .Returns(mockIIdentifiedLongRepositoryForDocument.Object);

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateObserverBuilder WithRequestAnswer(RequestAnswerBaseLong answer)
        {
            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<RequestAnswerBaseLongRepository>())
                .Returns(new MockRequestAnswerBaseLongRepository(this._mockedCoreUnitOfWork, this._mockedDbContext, answer));

            return this;
        }

        public ChangeGrlsApplicantRequestInternalStateObserverBuilder WithNextInternalState(InternalState internalState)
        {
            this.changeStateInfo.IsChanged = true;
            this.changeStateInfo.StateId = internalState.Id;

            var IIdentifiedRepositoryMock = new Mock<IIdentifiedRepository>();
            IIdentifiedRepositoryMock
                .Setup(r => r.FindById(It.IsAny<int>()))
                .Returns(internalState);

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(InternalState).Name))))
                .Returns(IIdentifiedRepositoryMock.Object);

            return this;
        }


        public ChangeGrlsApplicantRequestInternalStateObserverBuilder WithInternalStateTransitions(StateTransition transition)
        {
            this.changeStateInfo.Transition = transition;
            this.changeStateInfo.IsChanged = true;
            this.changeStateInfo.StateId = transition.ToStateId;


            var IIdentifiedRepositoryMock = new Mock<IIdentifiedRepository>();
            IIdentifiedRepositoryMock
                .Setup(r => r.FindById(It.IsAny<int>()))
                .Returns(transition.ToState);

            this._mockedCoreUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(InternalState).Name))))
                .Returns(IIdentifiedRepositoryMock.Object);


            return this;
        }



        public ChangeGrlsApplicantRequestInternalStateObserver Please()
        {
            return new ChangeGrlsApplicantRequestInternalStateObserver();
        }

        public (ChangeGrlsApplicantRequestInternalStateObserver, TestService) PleaseWithTestService()
        {
            return (this.Please(), new TestService
            {
                ICoreUnitOfWork = this._mockedCoreUnitOfWork,
                IncomingParams = new object[] { this.changeStateInfo },
                ChangeLongDocumentInternalState = this._mockedChangeLongDocumentInternalState,
                IDocumentRepository = this._mockedIDocumentRepository,
            });
        }
    }
}
