﻿using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Infrastructure.Managers;
using Core.Infrastructure.NinjectModules;
using Core.Models;
using Moq;
using Ninject;
using Core.Infrastructure.Context;



namespace Core.BL.Tests.Helpers
{
    internal static class UnitOfWork
    {
        private static readonly ICoreUnitOfWork _unitOfWork;
        static UnitOfWork()
        {
            var kernel = KernelManager.Instance;
            kernel.Load(new CommonBindings());
            kernel.Load(new EECBindings());
            kernel.Load(new BMCPBindings());

            _unitOfWork = new CoreUnitOfWork(new Mock<IDbContext>().Object, new Mock<OldUser>().Object);
        }

        public static ICoreUnitOfWork Instance => _unitOfWork;
    }
}
