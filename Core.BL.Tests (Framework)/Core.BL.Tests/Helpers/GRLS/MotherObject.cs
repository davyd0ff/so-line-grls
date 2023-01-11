using Core.BL.Tests.DataAcquisition;
using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.BL.Tests.GRLS.Producers;
using Core.BL.Tests.GRLS.Statements;
using Core.BL.Tests.Models.Common;
using Core.BL.Tests.Models;
using Core.Infrastructure.Context.Abstract;
using Moq;
using Core.BL.Tests.Helpers;
using Core.Models;
using Core.BL.Tests.Helpers.GRLS.Statements;
using Core.BL.Tests.Helpers.BusinessTransactions;
using Core.BusinessTransactions.ApplicantRequests.grls;
using Core.DataAcquisition;
using Core.Models.Documents.Abstract;
using Core.DataAcquisition.Abstract;
using Grls.Common.Abstract;
using Core.Models.CommunicationModels;
using Core.Entity.Models;
using Core.BusinessTransactions;
using Core.BL.Tests.Helpers.GRLS.ApplicantRequests;

namespace Core.BL.Tests.GRLS
{
    public partial class Create
    {
        private Mock<ICoreUnitOfWork> mockedUnitOfWork = new Mock<ICoreUnitOfWork>();


        public Create()
        {
            mockedUnitOfWork.Setup(u => u.GetTypeByTypeCode(It.IsAny<string>()))
                            .Returns((string s) => UnitOfWork.Instance.GetTypeByTypeCode(s));

            mockedUnitOfWork.Setup(u => u.GetTypeLongByTypeCode(It.IsAny<string>()))
                            .Returns((string s) => UnitOfWork.Instance.GetTypeLongByTypeCode(s));

            mockedUnitOfWork.Setup(u => u.User)
                            .Returns(new Mock<OldUser>().Object);
        }

        public UserBuilder User => new UserBuilder(mockedUnitOfWork);
        #region ApplicantRequests
        public ClinicalStudyPermissionBuilder StatementPermissionCS =>
            new ClinicalStudyPermissionBuilder(mockedUnitOfWork);

        public ChangeResolutionStatementCSBuilder ChangeResolutionStatementCS =>
            new ChangeResolutionStatementCSBuilder(mockedUnitOfWork);

        public GrlsCSApplicantRequestFGBUBuilder GrlsCSApplicantRequestFGBU =>
            new GrlsCSApplicantRequestFGBUBuilder(mockedUnitOfWork);

        public GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ =>
            new GrlsMrApplicantRequestMZBuilder(mockedUnitOfWork);

        public GrlsMrApplicantRequestFGBUBuilder GrlsMrApplicantRequestFGBU =>
            new GrlsMrApplicantRequestFGBUBuilder(mockedUnitOfWork);

        public GrlsMrApplicantRequestInspectBuilder GrlsMrApplicantRequestInspect =>
            new GrlsMrApplicantRequestInspectBuilder(mockedUnitOfWork);

        public GrlsLPApplicantRequestUsualBuilder GrlsLPApplicantRequestUsual =>
            new GrlsLPApplicantRequestUsualBuilder(mockedUnitOfWork);
        #endregion
        #region Statements
        public StatementMRBuilder StatementMR => new StatementMRBuilder(mockedUnitOfWork);
        public StatementRegLimPriceBuilder StatementRegLimPrice => new StatementRegLimPriceBuilder(mockedUnitOfWork);
        #endregion

        public StateTransitionBuilder StateTransition => new StateTransitionBuilder();

        public StateTransitionBuilder OuterStateTransition => new StateTransitionBuilder();

        public ProducerBuilder Producer => new ProducerBuilder();

        public TriggerBuilder Trigger = new TriggerBuilder();

        #region Transactions
        public ChangeGrlsApplicantRequestInternalStateBuilder ChangeGrlsApplicantRequestInternalState =>
            new ChangeGrlsApplicantRequestInternalStateBuilder(mockedUnitOfWork);

        public ChangeGrlsApplicantRequestExternalStateBuilder ChangeGrlsApplicantRequestExternalState =>
            new ChangeGrlsApplicantRequestExternalStateBuilder(this.mockedUnitOfWork);

        public CreateMedicamentRegistrationApplicantRequestBuilder CreateMedicamentRegistrationApplicantRequest =>
            new CreateMedicamentRegistrationApplicantRequestBuilder(this.mockedUnitOfWork);

        public UpdateApplicantRequestBuilder UpdateApplicantRequest => 
            new UpdateApplicantRequestBuilder(this.mockedUnitOfWork);

        public UpdateLimitedPriceApplicantRequestBuilder UpdateLimitedPriceApplicantRequest =>
            new UpdateLimitedPriceApplicantRequestBuilder(this.mockedUnitOfWork);

        #endregion
    }
}
