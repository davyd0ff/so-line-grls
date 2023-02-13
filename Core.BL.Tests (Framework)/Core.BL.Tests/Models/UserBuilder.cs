using Core.BL.Tests.Models;
using Core.Enums;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Grls.Security;
using grlsWebSecurity;
using Moq;
using System.Linq;


internal partial class Create
{
    public UserBuilder User => new UserBuilder(mockedUnitOfWork);
}

namespace Core.BL.Tests.Models
{
    public class UserBuilder
    {
        private static int UserCount = 2; // т.к. 1 - это будет id у app (есть пользователь который представляет систему)
        private OldUser _user;
        private CoreUnitOfWorkUser _coreUnitOfWorkUser;
        private Mock<ICoreUnitOfWork> _unitOfWork;


        public UserBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            UserCount += 1;

            this._user = new OldUser();
            this._user.Id = UserCount;
            this._user.Fio = $"TestUser_{this._user.Id}";
            this._user.Actual = true;
            this._user.Email = $"test@localhost.com";

            this._coreUnitOfWorkUser = CoreUnitOfWorkUser.Create(UserCount);
            this._coreUnitOfWorkUser.Username = $"TestUser_{this._user.Id}";
            this._coreUnitOfWorkUser.Fio = $"TestUser_{this._user.Id}";
            this._coreUnitOfWorkUser.Email = $"test@localhost.com";
        }

        public OldUser PleaseOldUser()
        {
            return this._user;
        }

        public CoreUnitOfWorkUser Please()
        {
            return this._coreUnitOfWorkUser;
        }

        public static implicit operator OldUser(UserBuilder builder)
        {
            return builder.PleaseOldUser();
        }

        public static implicit operator CoreUnitOfWorkUser(UserBuilder builder)
        {
            return builder.Please();
        }


        public UserBuilder WithPermissions(params Enums.ActionsEnum[] permissions)
        {
            return this.WithPermissions(permissions.Select(p => (int)p).ToArray());
        }

        public UserBuilder WithPermissions(params int[] permissions)
        {

            var mockedActiveSession = new Mock<IActiveSession>();
            mockedActiveSession
                .Setup(session => session.Authenticated)
                .Returns(true);

            var mockedRequestPermission = new Mock<IRequestPermissions>();
            mockedRequestPermission
                .Setup(s => s.HasAny(
                    It.Is<int[]>(paramPermissions => paramPermissions.Any(pp => permissions.Contains(pp)))
                 ))
                .Returns(true);
            mockedRequestPermission
                .Setup(s => s.Has(
                    It.Is<int>(paramPermissions => permissions.Contains(paramPermissions))
                 ))
                .Returns(true);

            this._unitOfWork
                .Setup(u => u.UserPermissions)
                .Returns(mockedRequestPermission.Object);

            var mockedRequestSecuruty = new Mock<IRequestSecurity>();
            mockedRequestSecuruty
                .Setup(security => security.Session)
                .Returns(mockedActiveSession.Object);
            mockedRequestSecuruty
                .Setup(security => security.Permissions)
                .Returns(_unitOfWork.Object?.UserPermissions);

            var mockedRequestSecurityProvider = new Mock<IRequestSecurityProvider>();
            mockedRequestSecurityProvider
                .Setup(provider => provider.Current)
                .Returns(mockedRequestSecuruty.Object);

            CurrentUser.Configure(mockedRequestSecurityProvider.Object);

            return this;
        }

        public UserBuilder NoActual()
        {
            this._user.Actual = false;

            return this;
        }
    }
}
