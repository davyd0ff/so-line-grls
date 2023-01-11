using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions.ApplicantRequests.grls;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.LimitedPrice;
using Core.Repositories.Abstract;
using Moq;


namespace Core.BL.Tests.Helpers.BusinessTransactions
{
    public class UpdateLimitedPriceApplicantRequestBuilder
    {
        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
        private TestService testService;

        private Mock<IIdentifiedRepository>  mockedLimitedPriceApplicantRequestRepository;
        private Mock<IIdentifiedRepository> mockedCustomEventRepository;

        public UpdateLimitedPriceApplicantRequestBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            this.testService = new TestService();

            this.mockedLimitedPriceApplicantRequestRepository = new Mock<IIdentifiedRepository>();
            this.mockedLimitedPriceApplicantRequestRepository
                .Setup(repo => repo.Update(It.IsAny<ApplicantRequestLimitedPrice>()));
            this.mockedCustomEventRepository = new Mock<IIdentifiedRepository>();
            this.mockedCustomEventRepository
                .Setup(repo => repo.Add(It.IsAny<CustomEvent>()));
        }

        public UpdateLimitedPriceApplicantRequestBuilder FindByIdMustReturns(ApplicantRequestLimitedPrice applicantRequest)
        {
            this.mockedLimitedPriceApplicantRequestRepository
                .Setup(repo => repo.FindById(It.Is<int>(id => id.Equals(applicantRequest.Id))))
                .Returns(applicantRequest);

            return this;
        }

        public UpdateLimitedPriceApplicantRequestBuilder StatementIsNotArchived(IncomingPackageBase incomingPackage)
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

        public UpdateLimitedPriceApplicantRequest Please()
        {
            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IIdentifiedRepository>(It.Is<string>(code => code.Equals(typeof(ApplicantRequestLimitedPrice).Name))))
                .Returns(mockedLimitedPriceApplicantRequestRepository.Object);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IIdentifiedRepository>(It.Is<string>(code =>
                    code.Equals(typeof(CustomEvent).Name))))
                .Returns(mockedCustomEventRepository.Object);

            return new UpdateLimitedPriceApplicantRequest(mockedCoreUnitOfWork.Object);
        }


        public (UpdateLimitedPriceApplicantRequest, TestService) PleaseWithTestService()
        {
            this.testService.LimitedPriceApplicantRequestRepository = mockedLimitedPriceApplicantRequestRepository;
            this.testService.CustomEventRepository = mockedCustomEventRepository;

            return (this.Please(), this.testService);
        }
    }
}
