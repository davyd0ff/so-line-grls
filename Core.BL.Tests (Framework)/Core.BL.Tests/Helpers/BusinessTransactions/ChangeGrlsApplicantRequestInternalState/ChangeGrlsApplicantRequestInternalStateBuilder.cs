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
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;


internal partial class Create
{
    public ChangeGrlsApplicantRequestInternalStateBuilder ChangeGrlsApplicantRequestInternalState =>
        new ChangeGrlsApplicantRequestInternalStateBuilder(mockedUnitOfWork);
}

namespace Core.BL.Tests.Helpers.BusinessTransactions
{
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

        public ChangeGrlsApplicantRequestInternalStateBuilder WithUser(CoreUnitOfWorkUser user)
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

        public (ChangeGrlsApplicantRequestInternalState, TestService) PleaseWithTestService()
        {
            return (this.Please(), new TestService
            {
                ICoreUnitOfWork = this._unitOfWork,
                IDocumentStateRepository = this._documentStateRepository
            });
        }
    }
}
