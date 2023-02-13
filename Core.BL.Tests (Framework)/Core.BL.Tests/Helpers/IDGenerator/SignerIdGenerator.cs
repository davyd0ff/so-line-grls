namespace Core.BL.Tests.Helpers.IDGenerator
{
    internal static class SignerIdGenerator
    {
        private static int InitId = 115000;

        public static int Next()
        {
            return InitId++;
        }
    }
}
