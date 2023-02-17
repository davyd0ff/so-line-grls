namespace Core.BL.Tests.Helpers.IDGenerator
{
    internal static class UserIdGenerator
    {
        // т.к. 1 - это будет id у app (есть пользователь который представляет систему)
        private static int InitId = 2;

        public static int Next()
        {
            return InitId++;
        }
    }
}
