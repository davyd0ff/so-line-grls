using Core.Infrastructure.Context.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;

namespace Core.BL.Tests.GRLS
{
    public static class Mocks
    {
        public static ICoreUnitOfWork UnitOfWork
        {
            get
            {
                var mock = new Mock<ICoreUnitOfWork>();

                return mock.Object;
            }
        }

        public static IDbContext DbContext
        {
            get
            {
                var mock = new Mock<IDbContext>();
                return mock.Object;
            }
        }

        public static MedicamentRegistrationStatement MedicamentRegistrationStatement
        {
            get
            {
                return new MedicamentRegistrationStatement();
            }
        }


        public static IMedicamentRegistrationStatementRepository MedicamentRegistrationStatementRepository
        {
            get
            {
                var mock = new Mock<IMedicamentRegistrationStatementRepository>();

                mock.Setup(repo => repo.GetStatementLevelN(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Mocks.MedicamentRegistrationStatement);

                return mock.Object;
            }
        }
    }
}
