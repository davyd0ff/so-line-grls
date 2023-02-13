using Core.BL.Tests.Helpers.BusinessTransactions;
using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.BusinessTransactions.States;
using Core.DataAcquisition;
using Core.DataAcquisition.Abstract;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.Documents.ApplicantRequest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;


internal partial class Create
{
    public UpdateApplicantRequestDefectBuilder UpdateApplicantRequestDefect =>
        new UpdateApplicantRequestDefectBuilder(this.mockedUnitOfWork);
}


namespace Core.BL.Tests.Helpers.BusinessTransactions
{
    public class UpdateApplicantRequestDefectBuilder
    {
        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
        private TestService testService;

        private Mock<IBusinessTransaction<IIdentifiedBase>> UpdateApplicantRequest;
        private Mock<IDataAcquisition<ApplicantRequestDefect, Guid>> GetApplicantRequestDefectByGuid;
        private Mock<IBinaryBusinessTransaction<ApplicantRequestDefect, State>> ChangeApplicantRequesDefectInternalState;
        private Mock<IBinaryBusinessTransaction<ApplicantRequestDefect, IEnumerable<ApprovingSigner>>> SaveApplicantRequestDefectAppointedSigners;

        public UpdateApplicantRequestDefectBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            this.testService = new TestService();
        }

        public UpdateApplicantRequestDefectBuilder WithUser(CoreUnitOfWorkUser user)
        {

            this.mockedCoreUnitOfWork
                .Setup(u => u.User)
                .Returns(user);

            return this;
        }

        public UpdateApplicantRequestDefectBuilder ForRequest(ApplicantRequestDefect applicantRequest)
        {
            ApplicantRequestDefect changedApplicantRequest = null;
            #region Mock<GetApplicantRequestDefectByGuid>
            this.GetApplicantRequestDefectByGuid = new Mock<IDataAcquisition<ApplicantRequestDefect, Guid>>();
            this.GetApplicantRequestDefectByGuid
                .Setup(ops => ops.Do(It.Is<Guid>(guid => guid.Equals(applicantRequest.RoutingGuid))))
                .Returns(applicantRequest);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IDataAcquisition<ApplicantRequestDefect, Guid>>())
                .Returns(this.GetApplicantRequestDefectByGuid.Object);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<GetApplicantRequestDefectByGuid>())
                .Returns(new MockedGetApplicantRequestDefectByGuid(this.mockedCoreUnitOfWork));
            #endregion
            #region Mock<ChangeApplicantRequesDefectInternalState>
            
            this.ChangeApplicantRequesDefectInternalState = new Mock<IBinaryBusinessTransaction<ApplicantRequestDefect, State>>();
            this.ChangeApplicantRequesDefectInternalState
                .Setup(tran => tran.Run(
                    It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                    It.IsAny<State>()
                 ))
                .Callback<ApplicantRequestDefect, State>((request, state) => { changedApplicantRequest = request; changedApplicantRequest.InternalState = state; })
                .Returns(TransactionResult.Succeeded(changedApplicantRequest));

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IBinaryBusinessTransaction<ApplicantRequestDefect, State>>())
                .Returns(this.ChangeApplicantRequesDefectInternalState.Object);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<ChangeApplicantRequesDefectInternalState>())
                .Returns(new MockedChangeApplicantRequesDefectInternalState(this.mockedCoreUnitOfWork));
            #endregion
            #region Mock<SaveApplicantRequestDefectAppointedSigners>
            this.SaveApplicantRequestDefectAppointedSigners = new Mock<IBinaryBusinessTransaction<ApplicantRequestDefect, IEnumerable<ApprovingSigner>>>();
            this.SaveApplicantRequestDefectAppointedSigners
                .Setup(tran => tran.Run(
                    It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                    It.IsAny<IEnumerable<ApprovingSigner>>()
                 ))
                .Callback<ApplicantRequestDefect, IEnumerable<ApprovingSigner>>((request, signers) =>
                {
                    changedApplicantRequest = request;
                    changedApplicantRequest.Signers = signers.SelectPerformers().Union(signers.SelectApprovers());
                    changedApplicantRequest.Signatory = signers.GetSignatory();
                })
                .Returns(TransactionResult.Succeeded(changedApplicantRequest));

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IBinaryBusinessTransaction<ApplicantRequestDefect, IEnumerable<ApprovingSigner>>>())
                .Returns(this.SaveApplicantRequestDefectAppointedSigners.Object);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<SaveApplicantRequestDefectAppointedSigners>())
                .Returns(new MockedSaveApplicantRequestDefectAppointedSigners(this.mockedCoreUnitOfWork));
            #endregion
            #region Mock<UpdateApplicantRequest>
            this.UpdateApplicantRequest = new Mock<IBusinessTransaction<IIdentifiedBase>>();
            this.UpdateApplicantRequest
                .Setup(tran => tran.Run(It.Is<IIdentifiedBase>(request => request is ApplicantRequestDefect )))
                .Callback<IIdentifiedBase>(request => { changedApplicantRequest = (ApplicantRequestDefect) request; })
                .Returns(TransactionResult.Succeeded(changedApplicantRequest));

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<IBusinessTransaction<IIdentifiedBase>>())
                .Returns(this.UpdateApplicantRequest.Object);

            this.mockedCoreUnitOfWork
                .Setup(unit => unit.Get<UpdateApplicantRequest>())
                .Returns(new MockedUpdateApplicantRequest(this.mockedCoreUnitOfWork));
            #endregion

            return this;
        }

        public UpdateApplicantRequestDefect Please()
        {
            return new UpdateApplicantRequestDefect(this.mockedCoreUnitOfWork.Object);
        }

        public (UpdateApplicantRequestDefect, TestService) PleaseWithTestService()
        {
            this.testService.UpdateApplicantRequest = this.UpdateApplicantRequest;
            this.testService.SaveApplicantRequestDefectAppointedSigners = this.SaveApplicantRequestDefectAppointedSigners;
            this.testService.ChangeApplicantRequesDefectInternalState = this.ChangeApplicantRequesDefectInternalState;

            return (this.Please(), this.testService);
        }
        #region Custom mocks
        private class MockedUpdateApplicantRequest : UpdateApplicantRequest
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;

            public MockedUpdateApplicantRequest(Mock<ICoreUnitOfWork> mockedUnitOfWork) : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override void OnConcreteTransactionSuccess(IIdentifiedBase t, TransactionResult result)
            {
                
            }
            protected override ValidationResult Validate(IIdentifiedBase t1)
            {
                return ValidationResult.Succeeded();
            }
            protected override TransactionResult PerformTransaction(IIdentifiedBase applicantRequest)
            {
                return this.mockedUnitOfWork
                    .Object
                    .Get<IBusinessTransaction<IIdentifiedBase>>()
                    .Run(applicantRequest);
            }
        }
        private class MockedSaveApplicantRequestDefectAppointedSigners : SaveApplicantRequestDefectAppointedSigners
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;
            public MockedSaveApplicantRequestDefectAppointedSigners(Mock<ICoreUnitOfWork> mockedUnitOfWork) : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override void OnConcreteTransactionSuccess(ApplicantRequestDefect applicantRequest, IEnumerable<ApprovingSigner> updatedSigners, TransactionResult result)
            {

            }
            protected override ValidationResult Validate(ApplicantRequestDefect applicantRequest, IEnumerable<ApprovingSigner> updatedSigners)
            {
                return ValidationResult.Succeeded();
            }
            protected override TransactionResult PerformTransaction(ApplicantRequestDefect applicantRequest, IEnumerable<ApprovingSigner> updatedSigners)
            {
                return this.mockedUnitOfWork
                    .Object
                    .Get<IBinaryBusinessTransaction<ApplicantRequestDefect, IEnumerable<ApprovingSigner>>>()
                    .Run(applicantRequest, updatedSigners);
            }
        }
        private class MockedChangeApplicantRequesDefectInternalState : ChangeApplicantRequesDefectInternalState
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;
            public MockedChangeApplicantRequesDefectInternalState(Mock<ICoreUnitOfWork> mockedUnitOfWork) : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }
            protected override ValidationResult Validate(ApplicantRequestDefect applicantRequest, State newState)
            {
                return ValidationResult.Succeeded();
            }
            protected override TransactionResult PerformTransaction(ApplicantRequestDefect applicantRequest, State newState)
            {
                return this.mockedUnitOfWork
                    .Object
                    .Get<IBinaryBusinessTransaction<ApplicantRequestDefect, State>>()
                    .Run(applicantRequest, newState);
            }
            protected override void OnConcreteTransactionSuccess(ApplicantRequestDefect applicantRequest, State newState, TransactionResult result)
            {

            }
        }
        private class MockedGetApplicantRequestDefectByGuid : GetApplicantRequestDefectByGuid
        {
            private readonly Mock<ICoreUnitOfWork> mockedUnitOfWork;

            public MockedGetApplicantRequestDefectByGuid(Mock<ICoreUnitOfWork> mockedUnitOfWork) : base(mockedUnitOfWork.Object)
            {
                this.mockedUnitOfWork = mockedUnitOfWork;
            }

            protected override ApplicantRequestDefect InternalDo(Guid guid)
            {
                var ops = mockedUnitOfWork.Object.Get<IDataAcquisition<ApplicantRequestDefect, Guid>>();
                return ops.Do(guid);
            }
        }
        #endregion
    }
}
