﻿using Core.Helpers;
using Core.Infrastructure.Context.Abstract;
using Core.Infrastructure;
using Core.Models.Common.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.GRLS.ApplicantRequests
{
    public  class GrlsMrApplicantRequestFGBUBuilder
    {
        private readonly string _code = StatementTypeList.ApplicantRequestFgbu;
        private MedicamentRegistrationApplicantRequest request = null;

        public GrlsMrApplicantRequestFGBUBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var requestId = ApplicantRequestsIdGeneator.Next();
            var documentId = DocumentIdGenerator.Next();
            var documentType = new DocumentType
            {
                Id = 10,
                Code = this._code,
                Flow = new DocumentFlow
                {
                    Id = 1,
                    Module = Enums.ModuleEnum.grls,
                    Code = "flow_reg"
                }
            };

            this.request = new MedicamentRegistrationApplicantRequest
            {
                Id = requestId,
                DocumentId = documentId,
                DocumentType = documentType,
            };

            DocumentTypes.DocumentTypeList.Add(documentType);


            var MockIdentifiedRepository = new Mock<IIdentifiedRepository>();
            MockIdentifiedRepository.Setup(r => r.FindById(It.IsAny<int>()))
                                    .Returns(this.request);


            unitOfWork.Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
                      .Returns(typeof(MedicamentRegistrationApplicantRequest));

            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                      .Returns(MockIdentifiedRepository.Object);
        }

        public GrlsMrApplicantRequestFGBUBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.request.IncomingPackage = incoming;

            return this;
        }

        public GrlsMrApplicantRequestFGBUBuilder WithInternalState(StateBase state)
        {
            this.request.InternalState = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsMrApplicantRequestFGBUBuilder WithQRCode()
        {
            return this;
        }

        public MedicamentRegistrationApplicantRequest Please()
        {
            return request;
        }
    }
}
