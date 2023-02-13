using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.MedicamentRegistration;
using Core.Models;
using Core.Repositories.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.Entity.Models;


internal partial class Create
{
    public ChangeGrlsApplicantRequestExternalStateBuilder ChangeGrlsApplicantRequestExternalState =>
        new ChangeGrlsApplicantRequestExternalStateBuilder(this.mockedUnitOfWork);
}


namespace Core.BL.Tests.Helpers.BusinessTransactions
{
    public class ChangeGrlsApplicantRequestExternalStateBuilder
    {
        private Mock<ICoreUnitOfWork> _mockedUnitOfWork;
        private Mock<IDocumentStateRepository> _documentStateRepository;
        private DocumentType _documentType;

        public ChangeGrlsApplicantRequestExternalStateBuilder(Mock<ICoreUnitOfWork> mockedUnitOfWork)
        {
            this._mockedUnitOfWork = mockedUnitOfWork;
            this._documentStateRepository = new Mock<IDocumentStateRepository>();

            this._documentStateRepository
                .Setup(r => r.SetState(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>()));
        }

        public ChangeGrlsApplicantRequestExternalStateBuilder WithDocument(MedicamentRegistrationApplicantRequest applicantRequest)
        {
            this._documentType = applicantRequest.DocumentType;

            if(applicantRequest.State != null)
            {
                this._documentStateRepository
                    .Setup(r => r.GetState(It.Is<long>(p => p.Equals(applicantRequest.DocumentId))))
                    .Returns(new State
                    {
                        Id = applicantRequest.State.Id,
                        Code = applicantRequest.State.Code
                    });
            }
       

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

            this._mockedUnitOfWork.Setup(u => u.Get(typeof(IIdentifiedLongRepository<Document>)))
                                  .Returns(mockIIdentifiedLongRepositoryForDocument.Object);


            return this;
        }

        public ChangeGrlsApplicantRequestExternalStateBuilder WithUser(OldUser user)
        {
            this._mockedUnitOfWork.Setup(u => u.User)
                                  .Returns(user);

            return this;
        }

        public ChangeGrlsApplicantRequestExternalStateBuilder WithNextOuterState(State state)
        {
            var IIdentifiedRepositoryMock = new Mock<IIdentifiedRepository>();
            IIdentifiedRepositoryMock
                .Setup(r => r.FindById(It.IsAny<int>()))
                .Returns(state);

            this._mockedUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(State).Name))))
                .Returns(IIdentifiedRepositoryMock.Object);

            return this;
        }

        public ChangeGrlsApplicantRequestExternalStateBuilder WithOuterStateTransitions(params StateTransition[] transitions)
        {
            var IStateTransitionRepositioryMock = new Mock<IStateTransitionRepositiory>();
            IStateTransitionRepositioryMock
                .Setup(r => r.FindTransition(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns((int typeId, int fromStateId, int toStateId) =>
                                     transitions.FirstOrDefault(t => t.DocumentTypeId == typeId
                                                                  && t.FromState.Id == fromStateId
                                                                  && t.ToState.Id == toStateId));

            this._mockedUnitOfWork
                .Setup(u => u.Get<IStateTransitionRepositiory>(It.Is<string>(p => p.Equals(typeof(OuterStateTransition).Name))))
                .Returns(IStateTransitionRepositioryMock.Object);

            return this;
        }


        public ChangeGrlsApplicantRequestExternalState Please()
        {
            this._mockedUnitOfWork
                .Setup(u => u.Get<IDocumentStateRepository>(
                                It.Is<string>(p => p.Equals(typeof(OuterStateTransition).Name))))
                .Returns(this._documentStateRepository.Object);


            return new ChangeGrlsApplicantRequestExternalState(this._mockedUnitOfWork.Object);
        }

        public (ChangeGrlsApplicantRequestExternalState, Mock<IDocumentStateRepository>, Mock<ICoreUnitOfWork>) PleaseWithSpies()
        {
            return (this.Please(), this._documentStateRepository, this._mockedUnitOfWork);
        }

        public (ChangeGrlsApplicantRequestExternalState, TestService) PleaseWithTestService()
        {
            return (this.Please(), new TestService
            {
                ICoreUnitOfWork = this._mockedUnitOfWork,
                IDocumentStateRepository = this._documentStateRepository,
            });
        }
    }
}
