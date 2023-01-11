using Core.Infrastructure.Context;
using Core.Infrastructure.Context.Abstract;
using Core.Infrastructure.Managers;
using Core.Infrastructure.NinjectModules;
using Core.Models;
using Moq;
using Ninject;


namespace Core.Repositories.Tests
{
    internal static class UnitOfWork
    {
        private static readonly ICoreUnitOfWork _unitOfWork;

        static UnitOfWork()
        {
            var kernel = KernelManager.Instance;
            kernel.Load(new CommonBindings());
            kernel.Load(new IdentifiedRepositories());
            kernel.Load(new ThesaurusRepositories());

            //_unitOfWork = new CoreUnitOfWork(new Mock<IDbContext>().Object, new Mock<OldUser>().Object);
            _unitOfWork = new CoreUnitOfWork("grls", new Mock<OldUser>().Object);
        }

        public static ICoreUnitOfWork Instance => _unitOfWork;
    }
}
