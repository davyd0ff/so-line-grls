using Core.BL.Tests.GRLS;
using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.LimitedPrice;
using Moq;
using System;
using System.Linq;


namespace Core.BL.Tests.Helpers.GRLS.ApplicantRequests
{
    public class GrlsLPApplicantRequestUsualBuilder
    {
        private readonly string _code = StatementTypeList.ApplicantRequestUsual;
        private ApplicantRequestLimitedPrice applicantRequest = null;
        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork = null;

        public GrlsLPApplicantRequestUsualBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;

            var requestId = ApplicantRequestsIdGeneator.Next();
            var documentId = DocumentIdGenerator.Next();
            var requestGuid = Guid.NewGuid();
            var documentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if (documentType == null)
            {
                documentType = new DocumentType
                {
                    Id = 95,
                    Code = this._code,
                    FlowId = 3,
                    Flow = new DocumentFlow
                    {
                        Id = 3,
                        Module = Enums.ModuleEnum.grls,
                        Code = "flow_lom_price"
                    }
                };
            }


            this.applicantRequest = new ApplicantRequestLimitedPrice
            {
                Id = requestId,
                DocumentId = documentId,
                RoutingGuid = requestGuid,
                DocumentType = documentType,
            };
        }

        public GrlsLPApplicantRequestUsualBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.applicantRequest.IncomingPackage = incoming;

            return this;
        }

        public ApplicantRequestLimitedPrice Please()
        {
            return this.applicantRequest;
        }
    }
}
