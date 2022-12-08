using Core.Infrastructure.Context.Abstract;
using Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests;
using Grls.Sync.Tests.Helpers.GRLS.Statements;
using Grls.Sync.Tests.Helpers.Models;
using Grls.Sync.Tests.Helpers.Models.Common;
using Moq;

namespace Grls.Sync.Tests.Helpers.GRLS
{
    public partial class Create
    {
        private Mock<ICoreUnitOfWork> unitOfWork = new Mock<ICoreUnitOfWork>();

        public Create()
        {
            this.unitOfWork.Setup(u => u.User)
                .Returns(this.User);
        }

        public GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ => new GrlsMrApplicantRequestMZBuilder(unitOfWork);
        public GrlsMrApplicantRequestFGBUBuilder GrlsMrApplicantRequestFGBU => new GrlsMrApplicantRequestFGBUBuilder(unitOfWork);
        public GrlsMrApplicantRequestInspectBuilder GrlsMrApplicantRequestInspect => new GrlsMrApplicantRequestInspectBuilder(unitOfWork);
        public UserBuilder User => new UserBuilder(this.unitOfWork);
        public StatementMRBuilder StatementMR => new StatementMRBuilder(this.unitOfWork);


        public StateTransitionBuilder StateTransition => new StateTransitionBuilder();

        public TriggerBuilder Trigger => new TriggerBuilder(this.unitOfWork);
    }
}
