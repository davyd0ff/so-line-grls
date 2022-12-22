using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.BusinessTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.CommunicationModels.State;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using grlsSync.Observers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Models;
using Grls.Sync.Tests.Helpers.GRLS;

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
                            .Returns(new Mock_Successed_ChangeLongDocumentExternalState(this._unitOfWork));

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

        public (ChangeLongDocumentInternalStateObserver, TestService) PleaseWithTestService()
        {
            return (
                this.Please(),
                new TestService
                {
                    ICoreUnitOfWork = this._unitOfWork,
                    IncomingParams = new object[] { this.changeStateInfo },
                }
            );
        }
    }
}
