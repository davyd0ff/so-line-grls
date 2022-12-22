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

        //public static GetProducersAsStringByStatementIdBuilder GetProducersAsStringByStatementId =>
        //    new GetProducersAsStringByStatementIdBuilder();

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

        public UserBuilder User => new UserBuilder(mockedUnitOfWork);

        public StatementMRBuilder StatementMR => new StatementMRBuilder(mockedUnitOfWork);


        public StateTransitionBuilder StateTransition => new StateTransitionBuilder();

        public StateTransitionBuilder OuterStateTransition => new StateTransitionBuilder();

        public ProducerBuilder Producer => new ProducerBuilder();

        public TriggerBuilder Trigger = new TriggerBuilder();

        public ChangeGrlsApplicantRequestInternalStateBuilder ChangeGrlsApplicantRequestInternalState =>
            new ChangeGrlsApplicantRequestInternalStateBuilder(mockedUnitOfWork);
    }
}
