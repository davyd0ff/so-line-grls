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

            var MockIdentifiedRepository = new Mock<IIdentifiedRepository>();
            MockIdentifiedRepository.Setup(r => r.FindById(It.IsAny<int>()))
                                    .Returns(this.ApplicantRequest);

            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(
                            It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                      .Returns(MockIdentifiedRepository.Object);


            var mockRoutableRepository = new Mock<IRoutableRepository>();
            mockRoutableRepository
                .Setup(r => r.FindByGuid(It.Is<Guid>(p => p.Equals(this.RoutingGuid))))
                .Returns(this.ApplicantRequest);

            unitOfWork
                .Setup(u => u.Get<IRoutableRepository>(
                    It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                .Returns(mockRoutableRepository.Object);

            unitOfWork
                .Setup(u => u.GetDocumentTypeByTypeCode(
                    It.Is<string>(p => p.Equals(this.DocumentType.Code))))
                .Returns(typeof(MedicamentRegistrationApplicantRequest));
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
