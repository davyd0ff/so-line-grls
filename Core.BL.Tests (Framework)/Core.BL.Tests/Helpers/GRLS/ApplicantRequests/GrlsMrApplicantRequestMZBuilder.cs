using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.Enums;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;
using System;
using System.Linq;


internal partial class Create
{
    public GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ =>
        new GrlsMrApplicantRequestMZBuilder(mockedUnitOfWork);
}

namespace Core.BL.Tests.GRLS.ApplicantRequests
{
    public class GrlsMrApplicantRequestMZBuilder
    {
        private readonly string _code = StatementTypeList.ApplicantRequestMinistryOfHealth;
        private MedicamentRegistrationApplicantRequest request = null;

        public GrlsMrApplicantRequestMZBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var requestId = ApplicantRequestsIdGeneator.Next();
            var documentId = DocumentIdGenerator.Next();
            var requestGuid = Guid.NewGuid();
            var documentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if (documentType == null)
            {
                documentType = new DocumentType
                {
                    Id = 94,
                    Code = this._code,
                    Flow = new DocumentFlow
                    {
                        Id = 1,
                        Module = Enums.ModuleEnum.grls,
                        Code = "flow_reg"
                    }
                };
            }


            this.request = new MedicamentRegistrationApplicantRequest
            {
                Id = requestId,
                DocumentId = documentId,
                DocumentType = documentType,
                RoutingGuid = requestGuid,
            };


            var mockedIdentifiedRepository = new Mock<IIdentifiedRepository>();
            mockedIdentifiedRepository.Setup(r => r.FindById(It.Is<int>(p => p.Equals(requestId))))
                                      .Returns(this.request);
            var mockedIRoutableRepository = new Mock<IRoutableRepository>();
            mockedIRoutableRepository.Setup(r => r.FindByGuid(It.Is<Guid>(p => p.Equals(requestGuid))))
                                     .Returns(this.request);


            unitOfWork.Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
                      .Returns(typeof(MedicamentRegistrationApplicantRequest));

            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                      .Returns(mockedIdentifiedRepository.Object);

            unitOfWork.Setup(u => u.Get<IRoutableRepository>(It.Is<string>(p => p.Equals(this._code))))
                      .Returns(mockedIRoutableRepository.Object);


            DocumentTypes.DocumentTypeList.Add(documentType);
        }

        public GrlsMrApplicantRequestMZBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.request.IncomingPackage = incoming;

            return this;
        }

        public GrlsMrApplicantRequestMZBuilder WithInternalState(StateBase state)
        {
            this.request.InternalState = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsMrApplicantRequestMZBuilder WithOuterState(StateBase state)
        {
            this.request.State = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsMrApplicantRequestMZBuilder WithQRCode()
        {
            return this;
        }

        public MedicamentRegistrationApplicantRequest Please()
        {
            return request;
        }

        public static implicit operator MedicamentRegistrationApplicantRequest(GrlsMrApplicantRequestMZBuilder builder)
        {
            return builder.Please();
        }
    }
}
