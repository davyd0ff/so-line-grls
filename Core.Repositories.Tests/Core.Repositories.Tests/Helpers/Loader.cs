using Core.Models.Common;
using Core.Repositories.Abstract;
using System.Linq;


namespace Core.Repositories.Tests.Helpers
{
    internal static class Loader
    {

        public static void DocumentTypes()
        {
            if(Core.Helpers.DocumentTypes.DocumentTypeList.Count == 0)
            {
                var unit = new Core.Infrastructure.Context.CoreUnitOfWork(new Core.Models.OldUser { Id = 0 }, false);
                using (unit)
                {
                    var repo = unit.Get<IIdentifiedRepository>(typeof(DocumentType).Name);
                    Core.Helpers.DocumentTypes.DocumentTypeList.AddRange(repo.GetAll().Select(e => (DocumentType) e).ToArray());
                }
            }
        }
    }
}
