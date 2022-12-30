using Core.Models.Common;
using Core.Repositories.Abstract;
using System.Linq;


namespace Core.BL.Tests.Helpers
{
    internal static class Loader
    {
        public static void DocumentTypes()
        {
            if (Core.Helpers.DocumentTypes.DocumentTypeList.Count == 0)
            {
                using (UnitOfWork.Instance)
                {
                    var repo = UnitOfWork.Instance.Get<IIdentifiedRepository>(typeof(DocumentType).Name);
                    Core.Helpers.DocumentTypes.DocumentTypeList.AddRange(repo.GetAll().Select(e => (DocumentType)e).ToArray());
                }
            }
        }
    }
}
