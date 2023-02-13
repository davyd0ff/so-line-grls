using Core.BL.Tests.Helpers.BusinessTransactions;
using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.BusinessTransactions.MailSender;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.BusinessTransactions;
using Core.Models.Common.Abstract;
using Core.Repositories.Abstracts.core;
using Moq;


internal partial class Create
{
    internal SaveApplicantRequestDefectAppointedSignersBuilder SaveApplicantRequestDefectAppointedSigners =>
        new SaveApplicantRequestDefectAppointedSignersBuilder(this.mockedUnitOfWork);
}

namespace Core.BL.Tests.Helpers.BusinessTransactions
{
    internal class SaveApplicantRequestDefectAppointedSignersBuilder
    {
        private Mock<ICoreUnitOfWork> mockedUnitOfWork;
        private TestService testService;
        //private ApplicantRequestDefect ApplicantRequestDefect;
        private Mock<IDocumentSignersRepository> repository;
        private Mock<IBusinessTransaction<IMailSenderModel>> mailSenderService;
        public SaveApplicantRequestDefectAppointedSignersBuilder(Mock<ICoreUnitOfWork> mockedUnitOfWork)
        {
            this.mockedUnitOfWork = mockedUnitOfWork;
            this.testService = new TestService();

            this.repository = new Mock<IDocumentSignersRepository>();
            this.mockedUnitOfWork
                .Setup(unit => unit.Get<IDocumentSignersRepository>())
                .Returns(this.repository.Object);


            #region MailSender
            this.mailSenderService = new Mock<IBusinessTransaction<IMailSenderModel>>();
            this.mailSenderService
                .Setup(service => service.Run(It.IsAny<IMailSenderModel>()))
                .Returns(TransactionResult.Succeeded());

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<IBusinessTransaction<IMailSenderModel>>())
                .Returns(mailSenderService.Object);

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<MailSenderApplicantRequestDefectWasApproved>())
                .Returns(new MockedMailSenderApplicantRequestDefectWasApproved(this.mockedUnitOfWork));
            #endregion
        }

        public SaveApplicantRequestDefectAppointedSignersBuilder WithUser(CoreUnitOfWorkUser user)
        {

            this.mockedUnitOfWork
                .Setup(u => u.User)
                .Returns(user);

            return this;
        }

        public SaveApplicantRequestDefectAppointedSigners Please()
        {
            return new SaveApplicantRequestDefectAppointedSigners(this.mockedUnitOfWork.Object);
        }

        public (SaveApplicantRequestDefectAppointedSigners, TestService) PleaseWithTestService()
        {
            testService.IDocumentSignersRepository = this.repository;
            testService.MailSenderApplicantRequestDefectWasApproved = this.mailSenderService;

            return (this.Please(), this.testService);
        }

        private class MockedMailSenderApplicantRequestDefectWasApproved : MailSenderApplicantRequestDefectWasApproved
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;

            public MockedMailSenderApplicantRequestDefectWasApproved(Mock<ICoreUnitOfWork> mockedUnitOfWork)
                : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override TransactionResult PerformTransaction(IMailSenderModel model)
            {
                return this.mockedUnitOfWork
                    .Object
                    .Get<IBusinessTransaction<IMailSenderModel>>()
                    .Run(model);
            }

            protected override ValidationResult Validate(IMailSenderModel model)
            {
                return ValidationResult.Succeeded();
            }
        }
    }
}
