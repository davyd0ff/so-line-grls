using Core.BL.Tests.DataAcquisition;
using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.BL.Tests.GRLS.Producers;
using Core.BL.Tests.GRLS.Statements;
using Core.BL.Tests.Models.Common;
using Core.BL.Tests.Models;
using Core.Infrastructure.Context.Abstract;
using Moq;
using Core.BL.Tests.Helpers;

namespace Core.BL.Tests.GRLS
{
    public partial class Create
    {
        private Mock<ICoreUnitOfWork> unitOfWork = new Mock<ICoreUnitOfWork>();
        

        public Create()
        {
            unitOfWork.Setup(u => u.GetTypeByTypeCode(It.IsAny<string>()))
                           .Returns((string s) => UnitOfWork.Instance.GetTypeByTypeCode(s));

            unitOfWork.Setup(u => u.GetTypeLongByTypeCode(It.IsAny<string>()))
                           .Returns((string s) => UnitOfWork.Instance.GetTypeLongByTypeCode(s));
        }

        //public static GetProducersAsStringByStatementIdBuilder GetProducersAsStringByStatementId =>
        //    new GetProducersAsStringByStatementIdBuilder();


        public GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ => 
            new GrlsMrApplicantRequestMZBuilder(unitOfWork);

        public GrlsMrApplicantRequestFGBUBuilder GrlsMrApplicantRequestFGBU => 
            new GrlsMrApplicantRequestFGBUBuilder(unitOfWork);

        public GrlsMrApplicantRequestInspectBuilder GrlsMrApplicantRequestInspect => 
            new GrlsMrApplicantRequestInspectBuilder(unitOfWork);

        public UserBuilder User => new UserBuilder(unitOfWork);

        public StatementMRBuilder StatementMR => new StatementMRBuilder(unitOfWork);


        public StateTransitionBuilder StateTransition => new StateTransitionBuilder();
        public ProducerBuilder Producer => new ProducerBuilder();

        public TriggerBuilder Trigger = new TriggerBuilder();
    }
}
