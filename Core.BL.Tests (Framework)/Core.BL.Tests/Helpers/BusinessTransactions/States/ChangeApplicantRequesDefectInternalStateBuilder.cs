using Core.BL.Tests.Helpers.BusinessTransactions.States;
using Core.Infrastructure.Context.Abstract;
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
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public ChangeApplicantRequesDefectInternalStateBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
    }
}
