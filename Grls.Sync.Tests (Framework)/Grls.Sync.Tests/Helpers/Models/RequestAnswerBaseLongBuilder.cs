using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Documents;
using Core.Models.Documents.ClinicalStudies;
using Core.Models.Documents.LimitedPrice;
using Core.Models.Documents.MedicamentRegistration;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.Models
{
    public class RequestAnswerBaseLongBuilder
    {
        private Mock<ICoreUnitOfWork> _mockedUnitOfWork;
        private RequestAnswerBaseLong _answer;

        public RequestAnswerBaseLongBuilder(Mock<ICoreUnitOfWork> mockedUnitOfWork)
        {
            this._mockedUnitOfWork = mockedUnitOfWork;

            this._answer = new RequestAnswerBaseLong
            {
                Id = DocumentIdGenerator.Next(),
                RoutingGuid = Guid.NewGuid(),
                DocumentNumber = "ТЕСТ",
            };
        }


        public RequestAnswerBaseLongBuilder WithRequest(MedicamentRegistrationApplicantRequest applicantRequest)
        {
            var documentTypeId = 146;
            this._answer.DocumentType = new Core.Models.Common.DocumentType
            {
                Code = StatementTypeList.GrlsMrRequestAnswer,
                Id = documentTypeId,
                FlowId = applicantRequest.DocumentType.FlowId,
                Flow = applicantRequest.DocumentType.Flow,
            };
            this._answer.DocumentTypeId = documentTypeId;
            this._answer.IncomingPackage = applicantRequest.IncomingPackage;

            return this;
        }

        public RequestAnswerBaseLongBuilder WithRequest(ClinicalStudyApplicantRequest applicantRequest)
        {
            var documentTypeId = 148;
            this._answer.DocumentType = new Core.Models.Common.DocumentType
            {
                Code = StatementTypeList.GrlsCsRequestAnswer,
                Id = documentTypeId,
                FlowId = applicantRequest.DocumentType.FlowId,
                Flow = applicantRequest.DocumentType.Flow,
            };
            this._answer.DocumentTypeId = documentTypeId;
            this._answer.IncomingPackage = applicantRequest.IncomingPackage;

            return this;
        }

        public RequestAnswerBaseLongBuilder WithRequest(ApplicantRequestLimitedPrice applicantRequest)
        {
            var documentTypeId = 147;
            this._answer.DocumentType = new Core.Models.Common.DocumentType
            {
                Code = StatementTypeList.GrlsLpRequestAnswer,
                Id = documentTypeId,
                FlowId = applicantRequest.DocumentType.FlowId,
                Flow = applicantRequest.DocumentType.Flow,
            };
            this._answer.DocumentTypeId = documentTypeId;
            this._answer.IncomingPackage = applicantRequest.IncomingPackage;

            return this;
        }



        public RequestAnswerBaseLong Please()
        {
            return this._answer;
        }
    }
}
