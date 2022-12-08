using Core.BMCP.Models;
using Core.BusinessTransactions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.CommunicationModels.State;
using Core.Models.Documents.MedicamentRegistration;
using Core.Models.Enums.States;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions
{
    public class ChangeDocumentInternalStateTest
    {

        [Test]
        public void Test_GrlsMrApplicantRequestMz_ToCanceled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .WithInternalState(InnerStateCode.project)
                                         .Please();

            var transaction = Create.ChangeDocumentInternalState
                                    .WithDocument(applicantRequest)
                                    .WithNextInternalState(InnerStateCode.canceled)
                                    .WithUserHavingRights(/* какие права... перечислить... что значит right? Role или Action */)
                                    .Please();


            var result = transaction.Run(new ChangeStateRequest
            {
                DocumentId = applicantRequest.Id,
                DocumentType = applicantRequest.DocumentType,
                Level = Enums.RegistrationStatementLevelEnum.ЭкспертныйМЗ,
                NewStateId = 0,
            });


            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }


        [Test]
        public void Test_GrlsApplicantRequest()
        {
            //var incoming = Create.Incoming

            var grlsMrApplicantRequest = Create.GrlsMrApplicantRequestMZ
                                               .ToStatement(Create.GrlsMPStatementLP.WithInnerState(InnerStateCode.entered))
                                               .WithInternalState(InnerStateCode.project)
                                               .WithQRCode()
                                               .Please();

            var transaction = Create.ChangeDocumentInternalState
                                    .WithDocument(grlsMrApplicantRequest)
                                    .WithNextInternalState(InnerStateCode.indorse)
                                    .WithAllowedTransition(/* project => indorse */)
                                    .Please();

            //var transactionResult = transaction.Run();
        }

        [Test]
        public void Test_AnswerToGrlsMrApplicantRequestMZ()
        {

        }

        [Test]
        public void Test_AttemptChangeInternalStateWhenIncomingHasCanceledStatus()
        {
            var grlsMrApplicantRequest = Create.GrlsMrApplicantRequestMZ
                                               .ToStatement(Create.GrlsMPStatementLP.WithInnerState(InnerStateCode.canceled))
                                               .WithInternalState(InnerStateCode.project)
                                               .WithQRCode()
                                               .Please();

        }
    }

}

namespace Core.BL.Tests.GRLS { 
    public static partial class Create
    {
        public static GrlsMPStatementLPBuilder GrlsMPStatementLP { get { return new GrlsMPStatementLPBuilder(); } }
        public static GrlsMrApplicantRequestMZBuilder GrlsMrApplicantRequestMZ { get { return new GrlsMrApplicantRequestMZBuilder(); } }

        public static ChangeDocumentInternalStateBuilder ChangeDocumentInternalState { get { return new ChangeDocumentInternalStateBuilder(); } }
    }


    public class GrlsMPStatementLPBuilder
    {
        public GrlsMPStatementLPBuilder()
        {

        }

        public GrlsMPStatementLPBuilder WithInnerState(string state)
        {
            return this;
        }
    }

    public class GrlsMrApplicantRequestMZBuilder
    {
        MedicamentRegistrationApplicantRequest request = null;

        public GrlsMrApplicantRequestMZBuilder()
        {
            //var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == StatementTypeList.ApplicantRequestMinistryOfHealth);
            //var idRequestType = ApplicantRequestTypeEnum.МЗРФ_Поток_Регистрация;

            this.request = new MedicamentRegistrationApplicantRequest
            {
                Id = 777,
                DocumentId = 777940000,
                DocumentType = new Models.Common.DocumentType
                {
                    Id = 94,
                    Code = StatementTypeList.ApplicantRequestMinistryOfHealth,
                },
            };
        }

        public GrlsMrApplicantRequestMZBuilder ToStatement(object заглушка)
        {
            // Вместо заглушки надо получить PackageIncoming

            throw new NotImplementedException();
        }

        public GrlsMrApplicantRequestMZBuilder WithInternalState(string stateCode)
        {
            this.request.InternalState = new Models.Common.State
            {
                Code = stateCode,
                Id = 1
            };

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
    }



    public class ChangeDocumentInternalStateBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public ChangeDocumentInternalStateBuilder()
        {
            this._unitOfWork = new Mock<ICoreUnitOfWork>();

            //this._unitOfWork.Setup(unit => unit.Get<>())
            //    .Returns();

        }

        public ChangeDocumentInternalStateBuilder WithDocument<TDocument>(TDocument document)
        {


            return this;
        }

        public ChangeDocumentInternalStateBuilder WithNextInternalState(string stateCode)
        {
            return this;
        }

        public ChangeDocumentInternalStateBuilder WithAllowedTransition(/* project => indorse */)
        {
            return this;
        }

        public ChangeDocumentInternalStateBuilder WithUserHavingRights()
        {
            return this;
        }

        public ChangeDocumentInternalState Please()
        {
            return new ChangeDocumentInternalState(this._unitOfWork.Object);
        }
    }
}
