using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.Enums;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.CommunicationModels.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.BL.Tests.GRLS.ChangeDocumentInternalStateTransactionRunnerBuilder;

namespace Core.BL.Tests.BusinessTransactions.ChangeDocumentInternalStateTransactions
{
    [TestClass]
    public class ChangeDocumentInternalStateTransactionRunnerTest
    {
        private readonly Create Create;

        public ChangeDocumentInternalStateTransactionRunnerTest()
        {
            this.Create = new Create();
        }

        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForEecApplicantRequestCp()
        {

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                It.IsAny<long>(),
                new DocumentType { Code = StatementTypeList.EecApplicantRequestCp },
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Never);
        }

        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_IIdentifiedBase() {

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                It.IsAny<int>(),
                new DocumentType { Code = StatementTypeList.AccreditationStatement},
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Never);
        }

        #region Запросы МЗ потока Регистрации модуля ГРЛС
        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestMZ_ByTypeCode()
        {
            var request = Create.GrlsMrApplicantRequestMZ
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                It.IsAny<int>(),
                new DocumentType { Code = StatementTypeList.ApplicantRequestMinistryOfHealth },
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Never);

            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }

        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestMZ_ByEntity()
        {
            var request = Create.GrlsMrApplicantRequestMZ
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                request,
                new DocumentType { Code = StatementTypeList.ApplicantRequestMinistryOfHealth },
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);
            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Never);

            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }
        #endregion
        #region Запросы ФГБУ потока Регистрации модуля ГРЛС
        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestFGBU_ByTypeCode()
        {
            var request = Create.GrlsMrApplicantRequestFGBU
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                It.IsAny<int>(),
                request.DocumentType,
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);

            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }

        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestFGBU_ByEntity()
        {
            var request = Create.GrlsMrApplicantRequestFGBU
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                request,
                request.DocumentType,
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);
            
            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }
        #endregion
        #region Запросы Инспектирования потока Регистрации модуля ГРЛС
        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestInspect_ByTypeCode()
        {
            var request = Create.GrlsMrApplicantRequestInspect
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                It.IsAny<int>(),
                request.DocumentType,
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);

            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }

        [TestMethod]
        public void Test_ChangeDocumentInternalStateTransactionRunner_ForMRApplicantRequestInspect_ByEntity()
        {
            var request = Create.GrlsMrApplicantRequestInspect
                                .Please();

            var (runner, spy) = Create.ChangeDocumentInternalStateTransactionRunner
                                      .PleaseWithSpy();

            runner.Run(
                request,
                request.DocumentType,
                It.IsAny<int>(),
                It.IsAny<RegistrationStatementLevelEnum>());


            Assert.IsNotNull(spy);
            spy.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Never);

            spy.Verify(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Once);
            spy.Verify(u => u.Get<ChangeLongDocumentInternalState>(), Times.Once);
        }
        #endregion

    }
}

namespace Core.BL.Tests.GRLS
{
    public partial class Create
    {

        public ChangeDocumentInternalStateTransactionRunnerBuilder ChangeDocumentInternalStateTransactionRunner =>
            new ChangeDocumentInternalStateTransactionRunnerBuilder(unitOfWork);


    }

    public class ChangeDocumentInternalStateTransactionRunnerBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;
        public ChangeDocumentInternalStateTransactionRunnerBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _unitOfWork.Setup(u => u.Get<ChangeDocumentInternalState>())
                .Returns(new MockChangeDocumentInternalState(this._unitOfWork.Object));

            _unitOfWork.Setup(u => u.Get<ChangeLongDocumentInternalState>())
                .Returns(new MockChangeLongDocumentInternalState(this._unitOfWork.Object));

            _unitOfWork.Setup(u => u.Get<ChangeGrlsApplicantRequestInternalState>())
                .Returns(new MockChangeGrlsApplicantRequestInternalState(this._unitOfWork.Object));

            _unitOfWork.Setup(u => u.Get<ChangeGrlsFgbuApplicantRequestInternalState>())
                .Returns(new MockChangeGrlsFgbuApplicantRequestInternalState(this._unitOfWork.Object));
        }

        public (ChangeDocumentInternalStateTransactionRunner, Mock<ICoreUnitOfWork>) PleaseWithSpy()
        {
            return (new ChangeDocumentInternalStateTransactionRunner(this._unitOfWork.Object), this._unitOfWork);
        }


        internal sealed class MockChangeDocumentInternalState : ChangeDocumentInternalState
        {
            public MockChangeDocumentInternalState(ICoreUnitOfWork unitOfWork) : base(unitOfWork) { }

            protected override ValidationResult Validate(ChangeStateRequest request)
            {
                return null;
            }

            protected override TransactionResult PerformTransaction(ChangeStateRequest request)
            {
                return TransactionResult.Succeeded();
            }
        }

        internal sealed class MockChangeGrlsFgbuApplicantRequestInternalState : ChangeGrlsFgbuApplicantRequestInternalState
        {
            public MockChangeGrlsFgbuApplicantRequestInternalState(ICoreUnitOfWork unitOfWork) : base(unitOfWork) { }

            protected override ValidationResult Validate(ChangeStateInfo changeStateInfo, bool withTriggers)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return TransactionResult.Succeeded();
            }
        }

        internal sealed class MockChangeGrlsApplicantRequestInternalState : ChangeGrlsApplicantRequestInternalState
        {
            public MockChangeGrlsApplicantRequestInternalState(ICoreUnitOfWork unitOfWork) : base(unitOfWork)
            {
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return TransactionResult.Succeeded();
            }
        }

        internal sealed class MockChangeLongDocumentInternalState : ChangeLongDocumentInternalState
        {
            public MockChangeLongDocumentInternalState(ICoreUnitOfWork unitOfWork) : base(unitOfWork) { }

            protected override ValidationResult Validate(ChangeStateInfo info, bool withTriggers)
            {
                return null;
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return TransactionResult.Succeeded();
            }
        }
    }



}
