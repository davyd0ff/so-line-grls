using Core.BL.Tests.Helpers.BusinessTransactions.States;
using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.BusinessTransactions.MailSender;
using Core.BusinessTransactions.States;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.Common.Abstract;
using Core.Models.CommunicationModels.State;
using Moq;


internal partial class Create
{
    public ChangeApplicantRequesDefectInternalStateBuilder ChangeApplicantRequesDefectInternalState =>
        new ChangeApplicantRequesDefectInternalStateBuilder(this.mockedUnitOfWork);
}


namespace Core.BL.Tests.Helpers.BusinessTransactions.States
{
    public class ChangeApplicantRequesDefectInternalStateBuilder
    {
        private Mock<ICoreUnitOfWork> mockedUnitOfWork;
        private TestService testService;
        private Mock<IBusinessTransaction<ChangeStateRequest>> ChangeApplicantRequestInternalState;
        private Mock<IBusinessTransaction<IMailSenderModel>> MailSenderApplicantRequestDefectUpdateState;

        public ChangeApplicantRequesDefectInternalStateBuilder(Mock<ICoreUnitOfWork> mockedUnitOfWork)
        {
            this.mockedUnitOfWork = mockedUnitOfWork;
            this.testService = new TestService();

            #region ChangeApplicantRequestInternalState
            this.ChangeApplicantRequestInternalState = new Mock<IBusinessTransaction<ChangeStateRequest>>();
            this.ChangeApplicantRequestInternalState
                .Setup(tran => tran.Run(It.IsAny<ChangeStateRequest>()))
                .Returns(TransactionResult.Succeeded());

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<IBusinessTransaction<ChangeStateRequest>>())
                .Returns(this.ChangeApplicantRequestInternalState.Object);

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<ChangeApplicantRequestInternalState>())
                .Returns(new MockedChangeApplicantRequestInternalState(this.mockedUnitOfWork));
            #endregion

            #region MailSenderApplicantRequestDefectUpdateState
            this.MailSenderApplicantRequestDefectUpdateState = new Mock<IBusinessTransaction<IMailSenderModel>>();
            this.MailSenderApplicantRequestDefectUpdateState
                .Setup(sender => sender.Run(It.IsAny<IMailSenderModel>()))
                .Returns(TransactionResult.Succeeded());

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<IBusinessTransaction<IMailSenderModel>>())
                .Returns(this.MailSenderApplicantRequestDefectUpdateState.Object);

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<MailSenderApplicantRequestDefectUpdateState>())
                .Returns(new MockedMailSenderApplicantRequestDefectUpdateState(this.mockedUnitOfWork));
            #endregion
        }


        public ChangeApplicantRequesDefectInternalState Please()
        {
            return new ChangeApplicantRequesDefectInternalState(this.mockedUnitOfWork.Object);
        }

        public (ChangeApplicantRequesDefectInternalState, TestService) PleaseWithTestService()
        {
            this.testService.ChangeApplicantRequestInternalState = this.ChangeApplicantRequestInternalState;
            this.testService.MailSenderApplicantRequestDefectUpdateState = this.MailSenderApplicantRequestDefectUpdateState;

            return (this.Please(), testService);
        }


        private class MockedChangeApplicantRequestInternalState : ChangeApplicantRequestInternalState
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;

            public MockedChangeApplicantRequestInternalState(Mock<ICoreUnitOfWork> mockedUnitOfWork) 
                : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateRequest request)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateRequest request)
            {
                return this.mockedUnitOfWork
                           .Object
                           .Get<IBusinessTransaction<ChangeStateRequest>>()
                           .Run(request);
            }

            protected override void OnConcreteTransactionSuccess(ChangeStateRequest request, TransactionResult result)
            {
                
            }
        }

        private class MockedMailSenderApplicantRequestDefectUpdateState : MailSenderApplicantRequestDefectUpdateState
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;
            public MockedMailSenderApplicantRequestDefectUpdateState(Mock<ICoreUnitOfWork> mockedUnitOfWork) 
                : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override ValidationResult Validate(IMailSenderModel t)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(IMailSenderModel model)
            {
                return this.mockedUnitOfWork
                           .Object
                           .Get<IBusinessTransaction<IMailSenderModel>>()
                           .Run(model);
            }
        }
    }
}
