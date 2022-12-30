using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.DataAcquisition.Abstract;
using Core.Entity.Models;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common.CommunicationModels;
using Core.Models.Documents.Abstract;
using Core.Repositories;
using Core.Repositories.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests
{
    public class TestService
    {
        public void IsTrue(bool expression)
        {
            Assert.IsTrue(expression);
        }
        public Mock<ICoreUnitOfWork> ICoreUnitOfWork { get; set; }

        public Mock<IDocumentRepository> IDocumentRepository { get; set; }
        public Mock<IDocumentStateRepository> IDocumentStateRepository { get; set; }
        public Mock<IQrCodeOldRepository> IQrCodeOldRepository { get; set; }
        public Mock<IIdentifiedRepository> MedicamentRegistrationApplicantRequestRepository { get; set; }

        public Mock<IBinaryBusinessTransaction<Document, long?>> InsertDocumentOperation { get; set; }

        public Mock<IBinaryBusinessTransaction<ChangeStateInfo, bool>> ChangeGrlsApplicantRequestInternalState { get; set; }
        public Mock<IDataAcquisition<string, ApplicantRequestBase>> GetNewOutgoingNumberOfApplicantRequest { get; set; }
        //public void Verify_That_IDocumentStateRepository_SetState_WasCalledOnceWith(long documentId, int stateId, int? any)
        //{
        //    this.IDocumentStateRepositoryMock.Verify(repo => repo.SetState(documentId, stateId, any), Times.Once());
        //}


        //public void Verify_That_ICoreUnitOfWork_OnTransactionSuccess_WasCalledOnceWith(Func<TransactionParams> func)
        //{
        //    this.ICoreUnitOfWorkMock.Verify(unit => unit.OnTransactionSuccess(func.Invoke()), Times.Once());
        //}
        //public void Verify_That_ICoreUnitOfWork_OnTransactionSuccess_WasCalledOnceWith(TransactionParams transactionParams)
        //{
        //    this.ICoreUnitOfWorkMock.Verify(unit => unit.OnTransactionSuccess(transactionParams), Times.Once());
        //}

        //public void Verify_That_ICoreUnitOfWork_OnTransactionSuccess_WasCalledOnceWith(Func<Expression<Func<TransactionParams, bool>>, TransactionParams> func)
        //{
        //    this.ICoreUnitOfWorkMock.Verify(unit => unit.OnTransactionSuccess(func.), Times.Once());
        //}

        //public TestService<T> Verify => 
        //    CurrentMock.Verify()

        //public TestService<T> Verify => 
        //    currentMock.Verify()

    }


    //public static class TestServiceExtensions_DocumentStateRepository {

    //    public static TestService<IDocumentStateRepository> IDocumentStateRepository<T>(this TestService<T> service)
    //    {
    //        TestService<IDocumentStateRepository> newService = new TestService<IDocumentStateRepository>();
    //        service.CurrentMock = service.GetMock<IDocumentStateRepository>();

    //        return newService;
    //    }

    //    public static TestService<IDocumentStateRepository> SetState(this TestService<IDocumentStateRepository> service)
    //    {
    //        service.VerifyAction = (repo) => repo.SetState(service.SomeAction);

    //        return service;
    //    }

    //    public static void WasCalledOnceWith(this TestService<IDocumentStateRepository> service)
    //    {
    //        service.CurrentAction
    //    }
    //}

}


//Verify.That.DocumentStateRepository.SetState.WasCalledWith(documentId, stateId, It.IsAny<int?>()).Once

//Mock<T>.Verify(
//                repo => repo.SetState(documentId, canceledStateId, It.IsAny<int?>()),
//                Times.Once()
//            );