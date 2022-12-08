using Core.BL.Tests.GRLS;
using Core.BusinessTransactions;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Documents;
using Core.Models.Documents.MedicamentRegistration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions
{
    public class ChangeLongDocumentIntrnalStateTest
    {
        //[Test]
        //public void Test_SomeDocumentWithoutTransition()
        //{

        //}

        //[Test]
        //public void Test_SomeDocumentWithoutPermissionToTransition()
        //{

        //}





        //[Test]
        //public void Test_SomeTestSomeDocument()
        //{
        //    var eecApplicantRequestFGBU = Create.EecApplicantRequestFGBU
        //                                        .WithInternalState(InnerStateCode.project)
        //                                        .WithFile()
        //                                        .WithQRCode()
        //                                        .Please();

        //    var transaction = Create.ChangeLongDocumentInternalState
        //                            .WithDocument(eecApplicantRequestFGBU)
        //                            .WithNextInternalState(InnerStateCode.canceled)
        //                            .WithAllowedTransition(/* project => canceled */)
        //                            .Please();

        //    //var transactionResult = transaction.Run();

        //    // Asserts
        //}

        //[Test]
        //public void Test_SomeTestEECFinalResolution()
        //{
        //    //var eecFinalResolution = Create.EecFinalResolution
        //    //                               .WithInternalState()
        //    //                               .Please();

        //    var transaction = Create.ChangeLongDocumentInternalState
        //                            //.WithDocument(eecFinalResolution)
        //                            .WithNextInternalState("somecode")
        //                            .Please();

        //    //var transactionResult = transaction.Run();

        //    // asserts
        //}
    }
}
namespace Core.BL.Tests.GRLS { 
    public static partial class Create
    {
        //public static GrlsApplicantRequestBuilder GrlsApplicantRequest { get { return new GrlsApplicantRequestBuilder(); } }
        public static EecApplicantRequestFGBUBuilder EecApplicantRequestFGBU { get { return new EecApplicantRequestFGBUBuilder(); } }
        public static EecFinalResolutionBuilder EecFinalResolution { get { return new EecFinalResolutionBuilder(); } }
        public static ChangeLongDocumentInternalStateBuilder ChangeLongDocumentInternalState { get { return new ChangeLongDocumentInternalStateBuilder(); } }
        //public static ChangeDocumentInternalStateBuilder ChangeDocumentInternalState { get { return new ChangeDocumentInternalStateBuilder(); } }
    }

    

    public class EecFinalResolutionBuilder
    {
        public EecFinalResolutionBuilder WithInternalState()
        {
            throw new NotImplementedException();

            return this;
        }

        //.Please();
    }

    public class ChangeLongDocumentInternalStateBuilder
    {
        public ChangeLongDocumentInternalStateBuilder WithDocument<TDocument>(TDocument document)
        {
            throw new NotImplementedException();

            return this;
        }

        public ChangeLongDocumentInternalStateBuilder WithNextInternalState(string nextInternalCode)
        {
            throw new NotImplementedException();

            return this;
        }

        public ChangeLongDocumentInternalStateBuilder WithAllowedTransition(/* transition */)
        {
            throw new NotImplementedException();

            return this;
        }

        public ChangeLongDocumentInternalState Please()
        {
            throw new NotImplementedException();
        }
    }

    public class EecApplicantRequestFGBUBuilder
    {
        public EecApplicantRequestFGBUBuilder WithInternalState(string innerStateCode)
        {
            throw new NotImplementedException();

            return this;
        }

        public EecApplicantRequestFGBUBuilder WithFile()
        {
            // замокать логику получения файла для ФГБУ
            throw new NotImplementedException();

            return this;
        }

        public EecApplicantRequestFGBUBuilder WithQRCode()
        {
            // замокать логику получения QRCode
            throw new NotImplementedException();

            return this;
        }

        public ApplicantRequestBaseLong Please()
        {
            throw new NotImplementedException();
        }
    }
}


