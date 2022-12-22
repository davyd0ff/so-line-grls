using Core.Helpers;
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
using Core.Models.Documents.ClinicalStudies;

namespace Core.BL.Tests.GRLS.ApplicantRequests
{
    public class GrlsCSApplicantRequestFGBUBuilder
    {
        // Запрос ФГБУ о предоставлении дополнительных данных (ЗВИРКИ)
        private readonly string _code1 = StatementTypeList.ApplicantRequestCSCPFGBU;
        // Запрос ФГБУ о предоставлении дополнительных данных (КИ)
        private readonly string _code2 = StatementTypeList.ApplicantRequestCSFGBU;


        private MedicamentRegistrationApplicantRequest mrRequest = null;
        private ClinicalStudyApplicantRequest csRequest = null;

        public GrlsCSApplicantRequestFGBUBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var requestId = ApplicantRequestsIdGeneator.Next();
            var documentId = DocumentIdGenerator.Next();
            var documentType1 = new DocumentType
            {
                Id = 36,
                Code = this._code1,
                Flow = new DocumentFlow
                {
                    Id = 2,
                    Module = Enums.ModuleEnum.grls,
                    Code = "flow_cs"
                }
            };
            var documentType2 = new DocumentType
            {
                Id = 35,
                Code = this._code2,
                Flow = new DocumentFlow
                {
                    Id = 2,
                    Module = Enums.ModuleEnum.grls,
                    Code = "flow_cs"
                }
            };

            this.mrRequest = new MedicamentRegistrationApplicantRequest
            {
                Id = requestId,
                DocumentId = documentId,
                //DocumentType = documentType1,
            };
            this.csRequest = new ClinicalStudyApplicantRequest
            {
                Id = requestId,
                DocumentId = documentId,
                //DocumentType = documentType2,
            };



            DocumentTypes.DocumentTypeList.Add(documentType1);
            DocumentTypes.DocumentTypeList.Add(documentType2);


            var MockMRIdentifiedRepository = new Mock<IIdentifiedRepository>();
            MockMRIdentifiedRepository.Setup(r => r.FindById(It.IsAny<int>()))
                                      .Returns(this.mrRequest);
            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                      .Returns(MockMRIdentifiedRepository.Object);



            var MockCSIdentifiedRepository = new Mock<IIdentifiedRepository>();
            MockCSIdentifiedRepository.Setup(r => r.FindById(It.IsAny<int>()))
                                      .Returns(this.csRequest);
            unitOfWork.Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(p => p.Equals(typeof(ClinicalStudyApplicantRequest).Name))))
                      .Returns(MockCSIdentifiedRepository.Object);



            // TODO как быть? должен же быть еще и ClinicalStudyApplicantRequest
            //unitOfWork.Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
            //          .Returns(typeof(MedicamentRegistrationApplicantRequest));


        }

        public GrlsCSApplicantRequestFGBUBuilder ToStatement(IncomingPackageBase incoming)
        {
            this.mrRequest.IncomingPackage = incoming;
            this.csRequest.IncomingPackage = incoming;

            return this;
        }

        public GrlsCSApplicantRequestFGBUBuilder WithInternalState(StateBase state)
        {
            this.mrRequest.InternalState = Core.Models.Common.State.FromBase(state);
            this.csRequest.InternalState = Core.Models.Common.State.FromBase(state);

            return this;
        }

        public GrlsCSApplicantRequestFGBUBuilder WithQRCode()
        {
            return this;
        }

        public T Please<T>()
            where T : ApplicantRequestBase
        {
            // костыль: чтобы работали старые транзакции
            if(typeof(T) == typeof(MedicamentRegistrationApplicantRequest))
                return mrRequest as T;
            
            if(typeof(T) == typeof(ClinicalStudyApplicantRequest))
                return csRequest as T;
           

            throw new InvalidOperationException();
        }
    }
}
