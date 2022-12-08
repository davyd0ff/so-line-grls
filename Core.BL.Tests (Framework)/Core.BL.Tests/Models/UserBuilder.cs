using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Grls.Security;
using Grls.Security.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.Models
{
    public class UserBuilder
    {
        private static int UserCount = 2; // т.к. 1 - это будет id у app (есть пользователь который представляет систему)
        private OldUser _user;
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
        }

        public OldUser Please()
        {
            return this._user;
        }

        public static implicit operator OldUser(UserBuilder builder)
        {
            return builder.Please();
        }


        public UserBuilder WithPermissions(params int[] permissions)
        {
            var MockRequestPermissions = new Mock<IRequestPermissions>();
            MockRequestPermissions
                .Setup(s => s.HasAny(
                    It.Is<int[]>(paramPermissions => paramPermissions.Any(pp => permissions.Contains(pp)))
                 ))
                .Returns(true);

            //var IRequestPermissionsMock = new Mock<IRequestPermissions>

            this._unitOfWork
                .Setup(u => u.UserPermissions)
                .Returns(MockRequestPermissions.Object);

            return this;
        }

        public UserBuilder NoActual()
        {
            this._user.Actual = false;

            return this;
        }
    }
}
