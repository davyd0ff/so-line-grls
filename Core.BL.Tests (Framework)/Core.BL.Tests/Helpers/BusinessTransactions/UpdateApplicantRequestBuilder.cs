using Core.BL.Tests.Helpers.BusinessTransactions;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.LimitedPrice;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;


internal partial class Create
{
    public UpdateApplicantRequestBuilder UpdateApplicantRequest =>
        new UpdateApplicantRequestBuilder(this.mockedUnitOfWork);
}

namespace Core.BL.Tests.Helpers.BusinessTransactions
{
    public class UpdateApplicantRequestBuilder
    {
        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
        private TestService testService;

        private Mock<IIdentifiedRepository> mockedLimitedPriceApplicantRequestRepository;
        private Mock<IIdentifiedRepository> mockedMedicamentRegistrationApplicantRequestRepository;
        private Mock<IIdentifiedRepository> mockedCustomEventRepository;


        public UpdateApplicantRequestBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            this.testService = new TestService();


            this.mockedLimitedPriceApplicantRequestRepository = new Mock<IIdentifiedRepository>();
            this.mockedLimitedPriceApplicantRequestRepository
                .Setup(repo => repo.Update(It.IsAny<ApplicantRequestLimitedPrice>()));
            this.mockedMedicamentRegistrationApplicantRequestRepository = new Mock<IIdentifiedRepository>();
            this.mockedMedicamentRegistrationApplicantRequestRepository
                .Setup(repo => repo.Update(It.IsAny<MedicamentRegistrationApplicantRequest>()));          
            this.mockedCustomEventRepository = new Mock<IIdentifiedRepository>();
            this.mockedCustomEventRepository
                .Setup(repo => repo.Add(It.IsAny<CustomEvent>()));
        }

        public UpdateApplicantRequestBuilder FindByIdMustReturns(MedicamentRegistrationApplicantRequest applicantRequest)
        {
            this.mockedMedicamentRegistrationApplicantRequestRepository
                .Setup(repo => repo.FindById(It.Is<int>(id => id.Equals(applicantRequest.Id))))
                .Returns(applicantRequest);

            return this;
        }

        public UpdateApplicantRequestBuilder FindByIdMustReturn(ApplicantRequestLimitedPrice applicantRequest)
        {
            this.mockedLimitedPriceApplicantRequestRepository
                .Setup(repo => repo.FindById(It.Is<int>(id => id.Equals(applicantRequest.Id))))
                .Returns(applicantRequest);

            return this;
        }

        public UpdateApplicantRequestBuilder StatementIsNotArchived(IncomingPackageBase incomingPackage)
        {
            var mockRepo = new Mock<IInternalStateBindingRepository>();
            mockRepo
                .Setup(repo => repo.GetDocumentInternalState(
                    It.Is<int>(id => id.Equals(incomingPackage.Document.Id)),
                    It.Is<int>(id => id.Equals(incomingPackage.Document.DocumentType.Id)))
                 )
                .Returns(new DocumentToInternalStateBinding
                {
                    InternalState = InternalStates.Entered
                });

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IInternalStateBindingRepository>())
                .Returns(mockRepo.Object);

            return this;
        }

        public UpdateApplicantRequest Please()
        {
            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IIdentifiedRepository>(It.Is<string>(code =>
                    code.Equals(typeof(ApplicantRequestLimitedPrice).Name))))
                .Returns(mockedLimitedPriceApplicantRequestRepository.Object);
            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IIdentifiedRepository>(It.Is<string>(code =>
                    code.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                .Returns(mockedMedicamentRegistrationApplicantRequestRepository.Object);
            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IIdentifiedRepository>(It.Is<string>(code =>
                    code.Equals(typeof(CustomEvent).Name))))
                .Returns(mockedCustomEventRepository.Object);


            return new UpdateApplicantRequest(mockedCoreUnitOfWork.Object);
        }


        public (UpdateApplicantRequest, TestService) PleaseWithTestService()
        {
            this.testService.LimitedPriceApplicantRequestRepository = mockedLimitedPriceApplicantRequestRepository;
            this.testService.MedicamentRegistrationApplicantRequestRepository = mockedMedicamentRegistrationApplicantRequestRepository;
            this.testService.CustomEventRepository = mockedCustomEventRepository;

            return (this.Please(), this.testService);
        }
    }
}
