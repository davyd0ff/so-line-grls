using Core.Entity.Models;
using Core.Helpers;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System;
using System.Linq;


namespace Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests
{
    public abstract class GrlsApplicantRequestBaseBuilder
    {
        protected DocumentType DocumentType { get; set; }
        protected int Id { get; private set; }
        protected long DocumentId { get; private set; }

        protected Guid RoutingGuid { get; private set; }
        
        protected Document Document { get; set; }


        protected GrlsApplicantRequestBaseBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this.DocumentId = DocumentIdGenerator.Next();
            this.Id = ApplicantRequestsIdGeneator.Next();
            this.RoutingGuid = Guid.NewGuid();           
            
            if(DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code == this.DocumentType.Code) == null)
            {
                DocumentTypes.DocumentTypeList.Add(this.DocumentType);
            }
        }
    }
}
