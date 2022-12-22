using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.Entity.Models;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.Common;
using Core.Models.Documents.MedicamentRegistration;
using Core.PortalConfiguration;
using Core.Repositories.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var (transaction, spy_DocumentStateRepository, spy_unitOfWork) =
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
                          .PleaseWithSpies();



            var result = transaction.Run(new ChangeStateInfo
            {
                Id = applicantRequest.DocumentId,
                StateId = OuterStates.SendApplicant,
                TypeId = applicantRequest.DocumentType.Id,
            });



            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(spy_DocumentStateRepository);
            Assert.IsNotNull(spy_unitOfWork);
            spy_DocumentStateRepository.Verify(
                r => r.SetState(applicantRequest.DocumentId, It.IsAny<int>(), It.IsAny<int?>()),
                Times.Once());
        }
    }
}

namespace Core.BL.Tests.GRLS
{
    public partial class Create
    {
        public ChangeGrlsApplicantRequestExternalStateBuilder ChangeGrlsApplicantRequestExternalState =>
            new ChangeGrlsApplicantRequestExternalStateBuilder(this.mockedUnitOfWork);
    }


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

            this._documentStateRepository
                .Setup(r => r.GetState(It.Is<long>(p => p.Equals(applicantRequest.DocumentId))))
                .Returns(new State
                {
                    Id = applicantRequest.State.Id,
                    Code = applicantRequest.State.Code
                });


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
    }
}