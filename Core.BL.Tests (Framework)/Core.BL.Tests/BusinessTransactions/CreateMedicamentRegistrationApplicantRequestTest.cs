using Core.BL.Tests.GRLS;
using Core.BL.Tests.Helpers;
using Core.BL.Tests.Helpers.IDGenerator;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.Entity.Models;
using Core.Helpers;
using Core.Infrastructure;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions
{
    [TestClass]
    public class CreateMedicamentRegistrationApplicantRequestTest
    {
        private readonly Create Create;

        public CreateMedicamentRegistrationApplicantRequestTest()
        {
            this.Create = new Create();
            Loader.DocumentTypes();
        }


        [TestMethod]
        public void CreateMRApplicantRequestMZ_GetNewOutgoingNumberOfApplicantRequest_WasCalled()
        {
            var (transaction, testService) = 
                Create.CreateMedicamentRegistrationApplicantRequest
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.GetNewOutgoingNumberOfApplicantRequest.Verify(ops => ops.Do(It.IsAny<MedicamentRegistrationApplicantRequest>()), Times.Once());
        }


        [TestMethod]
        public void CreateMRApplicantRequestMZ_HasParentDocument_InsertDocumentOperation_WasCalled()
        {
            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered).Please();
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .HasParentDocument()
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = statement,
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.InsertDocumentOperation.Verify(
                tran => tran.Run(
                    It.Is<Document>(d => d.DocumentTypeId == DocumentTypes.DocumentTypeList
                                                                          .First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth))
                                                                          .Id),
                    It.Is<long?>(parentId => parentId == statement.DocumentId)), 
                Times.Once());
        }

        [TestMethod]
        public void CreateMRApplicantRequestMZ_HasParentDocument_IdentityRepository_Add_WasCalled()
        {
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .HasParentDocument()
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.MedicamentRegistrationApplicantRequestRepository.Verify(
                ops => ops.Add(It.IsAny<MedicamentRegistrationApplicantRequest>()),
                Times.Once());
        }


        [TestMethod]
        public void CreateMRApplicantRequestMZ_HasNoParentDocument_InsertDocumentOperation_WasCalledTwice()
        {
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.InsertDocumentOperation.Verify(
                ops => ops.Run(It.IsAny<Document>(), It.IsAny<long?>()),
                Times.Exactly(2));
        }


        [TestMethod]
        public void CreateMRApplicantRequestMZ_IDocumentRepository_SaveQrCode_WasCalled()
        {
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.IDocumentRepository.Verify(
                repo => repo.SaveQrCode(It.IsAny<long>(), It.IsAny<BarCodeQr>()),
                Times.Once());
        }

        [TestMethod]
        public void CreateMRApplicantRequestMZ_IQrCodeOldRepository_UpdateQrCode_WasCalled()
        {
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.IQrCodeOldRepository.Verify(
                repo => repo.UpdateQrCode(
                    It.IsAny<byte[]>(), 
                    It.IsAny<int>(), 
                    It.IsAny<long>(), 
                    It.IsAny<int?>(), 
                    It.IsAny<DateTime?>()),
                Times.Once());
        }

        [TestMethod]
        public void CreateMRApplicantRequestMZ_ChangeGrlsApplicantRequestInternalState_WasCalled()
        {
            var (transaction, testService) =
                Create.CreateMedicamentRegistrationApplicantRequest
                      .PleaseWithTestService();

            var result = transaction.Run(new MedicamentRegistrationApplicantRequest
            {
                DocumentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(StatementTypeList.ApplicantRequestMinistryOfHealth)),
                IncomingPackage = new IncomingPackageBase
                {
                    Number = IncomingNumberGenerator.Next(),
                    Flow = new DocumentFlow { Id = 1 },
                    Document = Create.StatementMR.WithInternalState(InternalStates.Entered)
                }
            });


            //testService.IsTrue(result.IsSuccess);
            testService.ChangeGrlsApplicantRequestInternalState
                       .Verify(
                            tran => tran.Run(It.Is<ChangeStateInfo>(info => info.StateId == InternalStates.Project), It.IsAny<bool>()),
                            Times.Once()
                       );
        }
    }
}
