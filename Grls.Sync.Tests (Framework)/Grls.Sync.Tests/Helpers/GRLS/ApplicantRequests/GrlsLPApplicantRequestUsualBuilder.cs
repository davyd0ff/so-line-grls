using Core.Enums;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Repositories.Abstract;
using Moq;
using System;
using System.Linq;
using Core.Models.Documents.LimitedPrice;
using Core.Entity.Models;


namespace Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests
{
    public class GrlsLPApplicantRequestUsualBuilder : GrlsApplicantRequestBaseBuilder
    {
        private readonly string _code = StatementTypeList.ApplicantRequestUsual;
        private ApplicantRequestLimitedPrice ApplicantRequest { get; set; }

        public GrlsLPApplicantRequestUsualBuilder(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork)
        {
            this.DocumentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code == this._code);
            if (this.DocumentType == null)
            {
                this.DocumentType = new DocumentType
                {
                    Id = 95,
                    Code = this._code,
                    FlowId = 3,
                    Flow = new DocumentFlow
                    {
                        Id = 3,
                        Module = ModuleEnum.grls,
                        Code = "flow_lom_price"
                    },
                };

                DocumentTypes.DocumentTypeList.Add(this.DocumentType);
            }

            this.ApplicantRequest = new ApplicantRequestLimitedPrice
            {
                Id = this.Id,
                DocumentId = this.DocumentId,
                DocumentType = this.DocumentType,
                RoutingGuid = this.RoutingGuid,
            };

            this.Document = new Document
            {
                Id = this.DocumentId,
                DocumentType = DocumentType,
                RoutingGuid = this.RoutingGuid,
            };


            var mockIdentifiedRepository = new Mock<IIdentifiedRepository>();
            mockIdentifiedRepository
                .Setup(repo => repo.FindById(It.Is<int>(id => id.Equals(this.Id))))
                .Returns(this.ApplicantRequest);

            unitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(
                    It.Is<string>(p => p.Equals(typeof(ApplicantRequestLimitedPrice).Name))))
                .Returns(mockIdentifiedRepository.Object);

            var mockRoutableRepository = new Mock<IRoutableRepository>();
            mockRoutableRepository
                .Setup(repo => repo.FindByGuid(It.Is<Guid>(p => p.Equals(this.RoutingGuid))))
                .Returns(this.ApplicantRequest);
            unitOfWork
                .Setup(u => u.Get<IRoutableRepository>(
                    It.Is<string>(p => p.Equals(typeof(ApplicantRequestLimitedPrice).Name)
                                    || p.Equals(typeof(ApplicantRequestBase).Name)
                                    || p.Equals(this._code))))
                .Returns(mockRoutableRepository.Object);

            unitOfWork
                .Setup(u => u.GetDocumentTypeByTypeCode(
                    It.Is<string>(p => p.Equals(this.DocumentType.Code))))
                .Returns(typeof(ApplicantRequestLimitedPrice));


            var mockDocumentIdentifiedLongRepository = new Mock<IIdentifiedLongRepository<Document>>();
            mockDocumentIdentifiedLongRepository
                    .Setup(r => r.GetById(It.Is<long>(p => p.Equals(this.DocumentId))))
                    .Returns(this.Document);
            unitOfWork
                .Setup(u => u.Get<IIdentifiedLongRepository<Document>>())
                .Returns(mockDocumentIdentifiedLongRepository.Object);
        }

        public GrlsLPApplicantRequestUsualBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.ApplicantRequest.IncomingPackage = incoming;

            return this;
        }

        public GrlsLPApplicantRequestUsualBuilder WithInternalState(StateBase state)
        {
            this.ApplicantRequest.InternalState = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsLPApplicantRequestUsualBuilder WithQRCode()
        {
            return this;
        }

        public ApplicantRequestLimitedPrice Please()
        {
            return ApplicantRequest;
        }

    }
}
