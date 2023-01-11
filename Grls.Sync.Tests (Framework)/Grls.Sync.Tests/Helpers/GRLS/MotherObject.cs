using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.BusinessTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Models.BusinessTransactions;
using Core.Models.CommunicationModels.State;
using Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests;
using Grls.Sync.Tests.Helpers.GRLS.Statements;
using Grls.Sync.Tests.Helpers.Models;
using Grls.Sync.Tests.Helpers.Models.Common;
using grlsSync.Observers;
using Moq;
using Core.Models.Documents;
using Core.Repositories;
using Core.BusinessTransactions.Abstract;
using Core.Models.Common;
using Core.Helpers;
using Core.Enums;
using Grls.Common.Abstract;
using Core.Models.CommunicationModels;
using System.Linq;

namespace Grls.Sync.Tests.Helpers.GRLS
{
    public partial class Create
    {
        private Mock<ICoreUnitOfWork> unitOfWork = new Mock<ICoreUnitOfWork>();
        private Mock<IDbContext> dbContext = new Mock<IDbContext>();

        public Create()
        {
            if(DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code == "add_materials_received") == null)
            {
                DocumentTypes.DocumentTypeList.Add(new DocumentType
                {
                    Code = "add_materials_received",
                    Id = 154,
                    FlowId = 1,
                    Flow = new DocumentFlow
                    {
                        Id = 1,
                        Module = ModuleEnum.grls,
                        Code = "flow_reg"
                    }
                });
            }


            this.unitOfWork.Setup(u => u.User)
                .Returns(this.User);
        }

        public UserBuilder User => new UserBuilder(this.unitOfWork);
        #region ApplicantRequests
        public RequestAnswerBaseLongBuilder RequestAnswerBaseLong => new RequestAnswerBaseLongBuilder(unitOfWork);
        public GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ => new GrlsMrApplicantRequestMZBuilder(unitOfWork);
        public GrlsMrApplicantRequestFGBUBuilder GrlsMrApplicantRequestFGBU => new GrlsMrApplicantRequestFGBUBuilder(unitOfWork);
        public GrlsMrApplicantRequestInspectBuilder GrlsMrApplicantRequestInspect => new GrlsMrApplicantRequestInspectBuilder(unitOfWork);

        public GrlsLPApplicantRequestUsualBuilder GrlsLPApplicantRequestUsual => new GrlsLPApplicantRequestUsualBuilder(unitOfWork);
        #endregion

        #region Statements
        public StatementMRBuilder StatementMR => new StatementMRBuilder(this.unitOfWork);
        public StatementLPRegLimPriceBuidler StatementLPRegLimPrice => StatementLPRegLimPriceBuidler(this.unitOfWork);
        #endregion


        public StateTransitionBuilder StateTransition => new StateTransitionBuilder();

        public TriggerBuilder Trigger => new TriggerBuilder(this.unitOfWork);

        #region Observers
        public ChangeLongDocumentInternalStateObserverBuilder ChangeLongDocumentInternalStateObserver =>
            new ChangeLongDocumentInternalStateObserverBuilder(this.unitOfWork);

        public ChangeGrlsApplicantRequestInternalStateObserverBuilder ChangeGrlsApplicantRequestInternalStateObserver =>
            new ChangeGrlsApplicantRequestInternalStateObserverBuilder(this.unitOfWork, this.dbContext);
        #endregion
    }

    internal sealed class Mock_Successed_ChangeLongDocumentInternalState : ChangeLongDocumentInternalState
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public Mock_Successed_ChangeLongDocumentInternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
        {
            this._unitOfWork = unitOfWork;
        }

        protected override ValidationResult Validate(ChangeStateInfo info, bool withTriggers)
        {
            return ValidationResult.Succeeded();
        }

        protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
        {
            return this._unitOfWork.Object.Get<IBinaryBusinessTransaction<ChangeStateInfo, bool>>().Run(info, withTriggers);

            //return TransactionResult.Succeeded();
        }
    }
    internal sealed class Mock_UpdateApplicantRequest : UpdateApplicantRequest
    {
        private Mock<ICoreUnitOfWork> _mockedCoreUnitOfWork;

        public Mock_UpdateApplicantRequest(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork) : base(mockedCoreUnitOfWork.Object)
        {
            _mockedCoreUnitOfWork = mockedCoreUnitOfWork;
        }

        protected override ValidationResult Validate(IIdentifiedBase applicantRequest)
        {
            return ValidationResult.Succeeded();
        }

        protected override TransactionResult PerformTransaction(IIdentifiedBase applicantRequest)
        {
            return this._mockedCoreUnitOfWork
                       .Object
                       .Get<IBusinessTransaction<IIdentifiedBase>>()
                       .Run(applicantRequest);
        }
    }
    #region Mock of CreateAddMaterialsReceived
    internal sealed class Mock_CreateAddMaterialsReceived : CreateAddMaterialsReceived
    {
        private Mock<ICoreUnitOfWork> _mockedCoreUnitOfWork;

        public Mock_CreateAddMaterialsReceived(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork) : base (mockedCoreUnitOfWork.Object)
        {
            _mockedCoreUnitOfWork = mockedCoreUnitOfWork;
        }

        protected override ValidationResult Validate(AddMaterialsReceivedCreateParams @params)
        {
            return ValidationResult.Succeeded();
        }

        protected override TransactionResult PerformTransaction(AddMaterialsReceivedCreateParams @params)
        {
            return this._mockedCoreUnitOfWork
                       .Object
                       .Get<IBusinessTransaction<AddMaterialsReceivedCreateParams>>()
                       .Run(@params);
        }
    }
    #endregion
    #region Mock of ChangeLongDocumentInternalStateObserver
    internal sealed class Mock_Successed_ChangeLongDocumentInternalStateObserver : ChangeLongDocumentInternalStateObserver
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public Mock_Successed_ChangeLongDocumentInternalStateObserver(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public void Execute(ICoreUnitOfWork unitOfWork, object[] incomingParams)
        {
            incomingParams = incomingParams;
        }
    }
    #endregion
    #region Mock of ChangeDocumentInternalState
    internal sealed class MockChangeDocumentInternalState : ChangeDocumentInternalState
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public MockChangeDocumentInternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
        {
            this._unitOfWork = unitOfWork;
        }

        protected override ValidationResult Validate(ChangeStateRequest request)
        {
            return ValidationResult.Succeeded();
        }
        protected override TransactionResult PerformTransaction(ChangeStateRequest request)
        {
            return TransactionResult.Succeeded();
        }
    }
    #endregion
    #region Mock of ChangeGrlsApplicantRequestInternalState
    internal sealed class MockChangeGrlsApplicantRequestInternalState : ChangeGrlsApplicantRequestInternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;
            public MockChangeGrlsApplicantRequestInternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo changeStateInfo, bool withTriggers)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return TransactionResult.Succeeded();
            }
        }
    #endregion
    #region Mock of ChangeLongDocumentExternalState
        internal sealed class Mock_Successed_ChangeLongDocumentExternalState : ChangeLongDocumentExternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;
            public Mock_Successed_ChangeLongDocumentExternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo info)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info)
            {
                return TransactionResult.Succeeded();
            }
        }
    #endregion
    #region Mock of ChangeGrlsApplicantRequestExternalState
        internal sealed class MockChangeGrlsApplicantRequestExternalState : ChangeGrlsApplicantRequestExternalState
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;

            public MockChangeGrlsApplicantRequestExternalState(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo changeExternalStateInfo)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo changeExternalStateInfo)
            {
                return TransactionResult.Succeeded();
            }
        }
    #endregion
    #region Mock of CreateRequestAnswer
        internal sealed class Mock_Successed_CreateRequestAnswer : CreateRequestAnswer
        {
            private Mock<ICoreUnitOfWork> _unitOfWork;

            public Mock_Successed_CreateRequestAnswer(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object) {
                this._unitOfWork = unitOfWork;
            }

            protected override ValidationResult Validate(long requestId, int documentTypeId)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(long requestId, int documentTypeId)
            {
                return TransactionResult.Succeeded();
            }
        }
    #endregion
}
