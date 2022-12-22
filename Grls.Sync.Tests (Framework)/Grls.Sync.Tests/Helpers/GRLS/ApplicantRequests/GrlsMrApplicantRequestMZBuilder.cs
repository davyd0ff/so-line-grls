using Core.Entity.Models;
using Core.Enums;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;
using System;

namespace Grls.Sync.Tests.Helpers.GRLS.ApplicantRequests
{
    public class GrlsMrApplicantRequestMZBuilder : GrlsApplicantRequestBaseBuilder
    {
        private readonly string _code = StatementTypeList.ApplicantRequestMinistryOfHealth;
        //private MedicamentRegistrationApplicantRequest request = null;

        public GrlsMrApplicantRequestMZBuilder(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork)
        {
            this.DocumentType = new DocumentType
            {
                Id = 94,
                Code = this._code,
                FlowId = 1,
                Flow = new DocumentFlow
                {
                    Id = 1,
                    Module = ModuleEnum.grls,
                    Code = "flow_reg"
                },
            };

            this.ApplicantRequest = new MedicamentRegistrationApplicantRequest
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
            mockIdentifiedRepository.Setup(r => r.FindById(It.IsAny<int>()))
                                    .Returns(this.ApplicantRequest);
            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(
                            It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                      .Returns(mockIdentifiedRepository.Object);

            var mockRoutableRepository = new Mock<IRoutableRepository>();
            mockRoutableRepository
                .Setup(r => r.FindByGuid(It.Is<Guid>(p => p.Equals(this.RoutingGuid))))
                .Returns(this.ApplicantRequest);
            unitOfWork
                .Setup(u => u.Get<IRoutableRepository>(
                    It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name) 
                                    || p.Equals(typeof(ApplicantRequestBase).Name)
                                    || p.Equals(this._code))))
                .Returns(mockRoutableRepository.Object);

            unitOfWork
                .Setup(u => u.GetDocumentTypeByTypeCode(
                    It.Is<string>(p => p.Equals(this.DocumentType.Code))))
                .Returns(typeof(MedicamentRegistrationApplicantRequest));


            var mockDocumentIdentifiedLongRepository = new Mock<IIdentifiedLongRepository<Document>>();
            mockDocumentIdentifiedLongRepository
                    .Setup(r => r.GetById(It.Is<long>(p => p.Equals(this.DocumentId))))
                    .Returns(this.Document);
            unitOfWork
                .Setup(u => u.Get<IIdentifiedLongRepository<Document>>())
                .Returns(mockDocumentIdentifiedLongRepository.Object);
        }

        public GrlsMrApplicantRequestMZBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.ApplicantRequest.IncomingPackage = incoming;

            return this;
        }

        public GrlsMrApplicantRequestMZBuilder WithInternalState(StateBase state)
        {
            this.ApplicantRequest.InternalState = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsMrApplicantRequestMZBuilder WithQRCode()
        {
            return this;
        }

        public MedicamentRegistrationApplicantRequest Please()
        {
            return ApplicantRequest;
        }
    }
}
