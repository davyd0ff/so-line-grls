using Core.BL.Tests.GRLS;
using Core.DataAcquisition;
using Core.DataAcquisition.Abstract;
using Core.Entity.Models;
using Core.Infrastructure.Context.Abstract;
using Core.Models.CommunicationModels;
using Core.Models.Documents.Abstract;
using Moq;
using System;
using Core.BusinessTransactions.ApplicantRequests.grls;
using Core.DataAcquisition.eec;
using System.Collections.Generic;
using Core.BMCP.Models;
using Core.Infrastructure;
using Core.BusinessTransactions;
using Core.Models.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using EntityBase = Core.Models.Common.Abstract.EntityBase;
using System.Linq;
using Core.Helpers;
using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.Repositories.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Models.Common;
using Core.Repositories;
using Core.BL.Tests.Models.Common;


internal partial class Create
{
    public CreateMedicamentRegistrationApplicantRequestBuilder CreateMedicamentRegistrationApplicantRequest =>
        new CreateMedicamentRegistrationApplicantRequestBuilder(this.mockedUnitOfWork);
}


namespace Core.BL.Tests.Helpers.BusinessTransactions
{

    // TODO нужно пробросить загрузку из БД списка DocumentTypes

    public class CreateMedicamentRegistrationApplicantRequestBuilder
    {
        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
        private TestService testService;

        public CreateMedicamentRegistrationApplicantRequestBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.testService = new TestService();

            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;


            this.GetNewOutgoingNumberOfApplicantRequest();
            this.GetDocumentBaseByNumber();
            this.GetDocumentBaseByGuid();
            this.GetDocumentByCode();
            this.InsertDocumentOperation();
            this.CheckFinalResolution(false);
            this.GetApplicantRequestTypesByStatementType();
            this.CreateIdentifiedRepository_MedicamentRegistrationApplicantRequest();
            this.IDocumentRepository();
            this.IQrCodeOldRepository();
            this.ChangeGrlsApplicantRequestInternalState();
            this.IThesaurusRepository_InternalState();
            this.CustomEventRepository();
        }
        private void ChangeGrlsApplicantRequestInternalState()
        {
            var mockedTransaction = new Mock<IBinaryBusinessTransaction<ChangeStateInfo, bool>>();
            mockedTransaction
                .Setup(tran => tran.Run(It.IsAny<ChangeStateInfo>(), It.IsAny<bool>()))
                .Returns(TransactionResult.Succeeded());

            mockedCoreUnitOfWork
                .Setup(u => u.Get<IBinaryBusinessTransaction<ChangeStateInfo, bool>>())
                .Returns(mockedTransaction.Object);

            mockedCoreUnitOfWork
                .Setup(u => u.Get<ChangeGrlsApplicantRequestInternalState>())
                .Returns(new Mock_ChangeGrlsApplicantRequestInternalState(this.mockedCoreUnitOfWork));

            this.testService.ChangeGrlsApplicantRequestInternalState = mockedTransaction;
        }
        private void CheckFinalResolution(bool hasFinalResolution = false)
        {
            var mockedIDataAcquisition = new Mock<IDataAcquisition<bool, long, long, string>>();
            mockedIDataAcquisition
                .Setup(ops => ops.Do(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .Returns(hasFinalResolution);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDataAcquisition<bool, long, long, string>>())
                .Returns(mockedIDataAcquisition.Object);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<CheckFinalResolution>())
                .Returns(new Mock_CheckFinalResolution(this.mockedCoreUnitOfWork));
        }
        private void GetNewOutgoingNumberOfApplicantRequest()
        {
            var mockedIDataAcquisition = new Mock<IDataAcquisition<string, ApplicantRequestBase>>();
            mockedIDataAcquisition
                .Setup(ops => ops.Do(It.IsAny<MedicamentRegistrationApplicantRequest>()))
                .Returns("TEST_OUTGOING_NUMBER");

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDataAcquisition<string, ApplicantRequestBase>>())
                .Returns(mockedIDataAcquisition.Object);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<GetNewOutgoingNumberOfApplicantRequest>())
                .Returns(new Mock_GetNewOutgoingNumberOfApplicantRequest(this.mockedCoreUnitOfWork));


            this.testService.GetNewOutgoingNumberOfApplicantRequest = mockedIDataAcquisition;
        }
        private void GetDocumentBaseByNumber(Document document = null)
        {
            var mockedIDataAcquisition = new Mock<IDataAcquisition<Document, string, int>>();
            mockedIDataAcquisition
                .Setup(ops => ops.Do(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(document);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDataAcquisition<Document, string, int>>())
                .Returns(mockedIDataAcquisition.Object);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<GetDocumentBaseByNumber>())
                .Returns(new Mock_GetDocumentBaseByNumber(this.mockedCoreUnitOfWork));
        }
        private void GetDocumentByCode(EntityBase document = null)
        {
            var mockedIDataAcquisition = new Mock<IDataAcquisition<EntityBase, CommonDocumentRequest>>();
            mockedIDataAcquisition
                .Setup(ops => ops.Do(It.IsAny<CommonDocumentRequest>()))
                .Returns(document);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDataAcquisition<EntityBase, CommonDocumentRequest>>())
                .Returns(mockedIDataAcquisition.Object);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<GetDocumentByCode>())
                .Returns(new Mock_GetDocumentByCode(this.mockedCoreUnitOfWork));
        }
        private void InsertDocumentOperation()
        {
            var mockedIBinaryBusinessTransaction = new Mock<IBinaryBusinessTransaction<Document, long?>>();
            mockedIBinaryBusinessTransaction
                .Setup(tran => tran.Run(It.IsAny<Document>(), It.IsAny<long?>()))
                .Returns(TransactionResult.Succeeded(DocumentIdGenerator.Next()));

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IBinaryBusinessTransaction<Document, long?>>())
                .Returns(mockedIBinaryBusinessTransaction.Object);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<InsertDocumentOperation<Document>>())
                .Returns(new Mock_InsertDocumentOperation_Document(this.mockedCoreUnitOfWork));

            this.testService.InsertDocumentOperation = mockedIBinaryBusinessTransaction;
        }
        private void GetDocumentBaseByGuid()
        {
            var mock = new Mock<IDataAcquisition<Document, Guid>>();
            mock.Setup(ops => ops.Do(It.IsAny<Guid>()))
                .Returns(new Document
                {
                    Id = DocumentIdGenerator.Next(),
                    RoutingGuid = Guid.NewGuid(),
                });

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDataAcquisition<Document,Guid>>())
                .Returns(mock.Object);


            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<GetDocumentBaseByGuid<Document>>())
                .Returns(new Mock_GetDocumentBaseByGuid<Document>(this.mockedCoreUnitOfWork));
        }
        private void GetApplicantRequestTypesByStatementType()
        {
            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<GetApplicantRequestTypesByStatementType>())
                .Returns(new Mock_GetApplicantRequestTypesByStatementType(this.mockedCoreUnitOfWork));
        }
        private void CreateIdentifiedRepository_MedicamentRegistrationApplicantRequest()
        {
            var mockedMedicamentRegistrationApplicantRequest = new Mock<IIdentifiedRepository>();
            mockedMedicamentRegistrationApplicantRequest
                .Setup(repo => repo.Add(It.IsAny<MedicamentRegistrationApplicantRequest>()))
                .Returns(ApplicantRequestsIdGeneator.Next());

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(
                    It.Is<string>(code => code.Equals(typeof(MedicamentRegistrationApplicantRequest).Name))))
                .Returns(mockedMedicamentRegistrationApplicantRequest.Object);
            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(
                    It.Is<string>(code => code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)
                                       || code.Equals(StatementTypeList.ApplicantRequestFgbu)
                                       || code.Equals(StatementTypeList.ApplicantRequestInspection))))
                .Returns(mockedMedicamentRegistrationApplicantRequest.Object);


            this.testService.MedicamentRegistrationApplicantRequestRepository = mockedMedicamentRegistrationApplicantRequest;
        }
        private void IDocumentRepository()
        {
            var mockedIDocumentRepository = new Mock<IDocumentRepository>();
            mockedIDocumentRepository
                .Setup(repo => repo.SaveQrCode(It.IsAny<long>(), It.IsAny<BarCodeQr>()));

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IDocumentRepository>())
                .Returns(mockedIDocumentRepository.Object);

            this.testService.IDocumentRepository = mockedIDocumentRepository;
        }
        private void IQrCodeOldRepository()
        {
            var mockedIQrCodeOldRepository = new Mock<IQrCodeOldRepository>();
            mockedIQrCodeOldRepository
                .Setup(u => u.UpdateQrCode(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.IsAny<long>(),
                    It.IsAny<int?>(),
                    It.IsAny<DateTime?>()));

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IQrCodeOldRepository>())
                .Returns(mockedIQrCodeOldRepository.Object);

            this.testService.IQrCodeOldRepository = mockedIQrCodeOldRepository;
        }
        private void IThesaurusRepository_InternalState()
        {
            var mockedRepository = new Mock<IThesaurusRepository>();
            mockedRepository
                .Setup(repo => repo.FindByCode(It.Is<string>(code => code.Equals(InnerStateCode.project))))
                .Returns(InternalStates.Project.Please());

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IThesaurusRepository>(typeof(InternalState).Name))
                .Returns(mockedRepository.Object);
        }
        private void CustomEventRepository()
        {
            var mockRepository = new Mock<IIdentifiedRepository>();
            mockRepository.Setup(repo => repo.Add(It.IsAny<CustomEvent>()))
                          .Returns(1);

            this.mockedCoreUnitOfWork
                .Setup(u => u.Get<IIdentifiedRepository>(It.Is<string>(code => code.Equals(typeof(CustomEvent).Name))))
                .Returns(mockRepository.Object);

            this.testService.CustomEventRepository = mockRepository;
        }


        public CreateMedicamentRegistrationApplicantRequestBuilder HasParentDocument()
        {
            var document = new Document
            {
                Id = DocumentIdGenerator.Next(),
                RoutingGuid = Guid.NewGuid(),
            };

            GetDocumentBaseByNumber(document);
            GetDocumentByCode(new DocumentBase
            {
                RoutingGuid = document.RoutingGuid,
            });

            return this;
        }
        public CreateMedicamentRegistrationApplicantRequestBuilder WithFinalResolution()
        {
            this.CheckFinalResolution(true);

            return this;
        }


        public CreateMedicamentRegistrationApplicantRequest Please()
        {
            return new CreateMedicamentRegistrationApplicantRequest(mockedCoreUnitOfWork.Object);
        }
        public (CreateMedicamentRegistrationApplicantRequest, TestService) PleaseWithTestService()
        {
            testService.ICoreUnitOfWork = this.mockedCoreUnitOfWork;

            return (this.Please(), testService);
        }

        #region CheckFinalResolution
        internal sealed class Mock_CheckFinalResolution : CheckFinalResolution
        {
            private readonly Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
            public Mock_CheckFinalResolution(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
                : base(mockedCoreUnitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            }

            protected override bool InternalDo(long incoming, long id, string code)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IDataAcquisition<bool, long, long, string>>()
                           .Do(incoming, id, code);
            }
        }
        #endregion
        #region GetApplicantRequestTypesByStatementType
        internal sealed class Mock_GetApplicantRequestTypesByStatementType : GetApplicantRequestTypesByStatementType
        {
            private readonly Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
            public Mock_GetApplicantRequestTypesByStatementType(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
                : base(mockedCoreUnitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            }

            protected override IEnumerable<ApplicantRequestType> InternalDo(int typeId)
            {
                return StatementTypeList.RequestsOfGRLS
                                        .Select(code => new ApplicantRequestType
                                        {
                                            RequestDocumentType = DocumentTypes.DocumentTypeList
                                                                               .First(dt => dt.Code.Equals(code))
                                        })
                                        .ToList();
            }
        }
        #endregion
        #region GetNewOutgoingNumberOfApplicantRequest
        internal sealed class Mock_GetNewOutgoingNumberOfApplicantRequest : GetNewOutgoingNumberOfApplicantRequest
        {
            private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

            public Mock_GetNewOutgoingNumberOfApplicantRequest(Mock<ICoreUnitOfWork> unitOfWork)
                : base(unitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = unitOfWork;
            }


            protected override string InternalDo(ApplicantRequestBase applicantRequest)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IDataAcquisition<string, ApplicantRequestBase>>()
                           .Do(applicantRequest);
            }
        }
        #endregion
        #region GetDocumentByCode
        internal sealed class Mock_GetDocumentByCode : GetDocumentByCode
        {
            private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

            public Mock_GetDocumentByCode(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = unitOfWork;
            }

            protected override EntityBase InternalDo(CommonDocumentRequest t1)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IDataAcquisition<EntityBase, CommonDocumentRequest>>()
                           .Do(t1);
            }
        }
        #endregion
        #region GetDocumentBaseByNumber
        internal sealed class Mock_GetDocumentBaseByNumber : GetDocumentBaseByNumber
        {
            private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;
            public Mock_GetDocumentBaseByNumber(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = unitOfWork;
            }


            protected override Document InternalDo(string number, int documentTypeId)
            {
                return this.mockedCoreUnitOfWork
                    .Object
                    .Get<IDataAcquisition<Document, string, int>>()
                    .Do(number, documentTypeId);
            }
        }
        #endregion
        #region InsertDocumentOperation 
        internal sealed class Mock_InsertDocumentOperation_Document : InsertDocumentOperation<Document>
        {
            private readonly Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

            public Mock_InsertDocumentOperation_Document(Mock<ICoreUnitOfWork> unitOfWork) : base(unitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = unitOfWork;
            }


            protected override ValidationResult Validate(Document t, long? parentId)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(Document document, long? parentId)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IBinaryBusinessTransaction<Document, long?>>()
                           .Run(document, parentId);

                //return base.PerformTransaction(document, parentId);
            }
        }
        #endregion
        #region ChangeGrlsApplicantRequestInternalState
        internal sealed class Mock_ChangeGrlsApplicantRequestInternalState : ChangeGrlsApplicantRequestInternalState {
            private readonly Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

            //protected override IRoutableRepository ApplicantRequestRoutableRepository => throw new NotImplementedException();

            public Mock_ChangeGrlsApplicantRequestInternalState(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
                : base(mockedCoreUnitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            }

            protected override ValidationResult Validate(ChangeStateInfo changeStateInfo, bool withTriggers)
            {
                return ValidationResult.Succeeded();
            }

            protected override TransactionResult PerformTransaction(ChangeStateInfo info, bool withTriggers)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IBinaryBusinessTransaction<ChangeStateInfo, bool>>()
                           .Run(info, withTriggers);
            }
        }
        #endregion
        #region GetDocumentBaseByGuid
        internal sealed class Mock_GetDocumentBaseByGuid<TModel> : GetDocumentBaseByGuid<TModel>
            where TModel : Document
        {
            private readonly Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

            public Mock_GetDocumentBaseByGuid(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork) : base(mockedCoreUnitOfWork.Object)
            {
                this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;
            }

            protected override TModel InternalDo(Guid guid)
            {
                return this.mockedCoreUnitOfWork
                           .Object
                           .Get<IDataAcquisition<TModel, Guid>>()
                           .Do(guid);
            }
        }
        #endregion
    }
}
