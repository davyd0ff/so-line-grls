using Core.BL.Tests.Helpers.IDGenerator;
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
        private OldUser _user;
        private CoreUnitOfWorkUser _coreUnitOfWorkUser;
        private Mock<ICoreUnitOfWork> _unitOfWork;
        private Mock<IActiveSession> _mockedSession;
        private Mock<IRequestSecurity> _mockedRequestSecurity;


        public UserBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            this._user = new OldUser();
            this._user.Id = UserIdGenerator.Next();
            this._user.Fio = $"TestUser_{this._user.Id}";
            this._user.Actual = true;
            this._user.Email = $"test@localhost.com";

            this._coreUnitOfWorkUser = CoreUnitOfWorkUser.Create(this._user.Id);
            this._coreUnitOfWorkUser.Username = $"TestUser_{this._user.Id}";
            this._coreUnitOfWorkUser.Fio = $"TestUser_{this._user.Id}";
            this._coreUnitOfWorkUser.Email = $"test@localhost.com";

            this._mockedSession = new Mock<IActiveSession>();
            this._mockedSession
                .Setup(session => session.Authenticated)
                .Returns(true);
            this._mockedSession
                .Setup(session => session.UserId)
                .Returns(this._user.Id);

            this._mockedRequestSecurity = new Mock<IRequestSecurity>();
            this._mockedRequestSecurity
                .Setup(security => security.Session)
                .Returns(this._mockedSession.Object);
        }

        public OldUser PleaseOldUser()
        {
            return this._user;
        }

        public CoreUnitOfWorkUser Please()
        {
            var mockedRequestSecurityProvider = new Mock<IRequestSecurityProvider>();
            mockedRequestSecurityProvider
                .Setup(provider => provider.Current)
                .Returns(this._mockedRequestSecurity.Object);

            CurrentUser.Configure(mockedRequestSecurityProvider.Object);

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


            this._mockedRequestSecurity
                .Setup(security => security.Permissions)
                .Returns(_unitOfWork.Object?.UserPermissions);

            return this;
        }

        public UserBuilder NoActual()
        {
            this._user.Actual = false;

            return this;
        }
    }
}
