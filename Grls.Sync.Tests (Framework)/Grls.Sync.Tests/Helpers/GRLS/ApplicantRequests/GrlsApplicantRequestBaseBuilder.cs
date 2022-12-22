using Core.Entity.Models;
using Core.Helpers;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests
{
    public abstract class GrlsApplicantRequestBaseBuilder
    {
        protected DocumentType DocumentType { get; set; }
        protected int Id { get; private set; }
        protected long DocumentId { get; private set; }

        protected Guid RoutingGuid { get; private set; }

        protected MedicamentRegistrationApplicantRequest ApplicantRequest { get; set; }
        protected Document Document { get; set; }


        protected GrlsApplicantRequestBaseBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this.DocumentId = DocumentIdGenerator.Next();
            this.Id = ApplicantRequestsIdGeneator.Next();
            this.RoutingGuid = Guid.NewGuid();           
            
            
            DocumentTypes.DocumentTypeList.Add(this.DocumentType);
        }


    }
}
