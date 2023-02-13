using Core.Infrastructure.Context.Abstract;
using Moq;
using Core.BL.Tests.Helpers;
using Core.Models;
using Grls.Security;
using grlsWebSecurity;


internal partial class Create
{
    private Mock<ICoreUnitOfWork> mockedUnitOfWork = new Mock<ICoreUnitOfWork>();


    public Create()
    {
        // TODO Если нужно будет делить тесты на авторизованного пользователя и неавторизованного,
        //      то придется городить что-то типа конструктора принимающий authenticated
        //
        var mockedActiveSession = new Mock<IActiveSession>();
        mockedActiveSession
            .Setup(session => session.Authenticated)
            .Returns(true);

        var mockedRequestPermission = new Mock<IRequestPermissions>();

        mockedUnitOfWork.Setup(u => u.GetTypeByTypeCode(It.IsAny<string>()))
                        .Returns((string s) => UnitOfWork.Instance.GetTypeByTypeCode(s));

        mockedUnitOfWork.Setup(u => u.GetTypeLongByTypeCode(It.IsAny<string>()))
                        .Returns((string s) => UnitOfWork.Instance.GetTypeLongByTypeCode(s));

        mockedUnitOfWork.Setup(u => u.User)
                        .Returns(new Mock<CoreUnitOfWorkUser>().Object);

        mockedUnitOfWork.Setup(u => u.UserPermissions)
                        .Returns(mockedRequestPermission.Object);
        

        var mockedRequestSecuruty = new Mock<IRequestSecurity>();
        mockedRequestSecuruty
            .Setup(security => security.Session)
            .Returns(mockedActiveSession.Object);
        mockedRequestSecuruty
            .Setup(security => security.Permissions)
            .Returns(mockedUnitOfWork.Object?.UserPermissions);

        var mockedRequestSecurityProvider = new Mock<IRequestSecurityProvider>();
        mockedRequestSecurityProvider
            .Setup(provider => provider.Current)
            .Returns(mockedRequestSecuruty.Object);

        CurrentUser.Configure(mockedRequestSecurityProvider.Object);
        //.Returns(new Mock<OldUser>().Object);
    }
}
