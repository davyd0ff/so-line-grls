using Core.BL.Tests.GRLS;
using Core.BL.Tests.Models.Common;
using Core.BusinessTransactions;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.CommunicationModels.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core.BL.Tests.BusinessTransactions
{

    [TestClass]
    public class ChangeDocumentInternalStateTest
    {

        [TestMethod]
        public void Test_GrlsMrApplicantRequestMz_ToCanceled()
        {
            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .WithInternalState(InternalStates.Project)
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
                                               //.ToStatement(Create.GrlsMPStatementLP.WithInnerState(InnerStateCode.entered))
                                               .WithInternalState(InternalStates.Project)
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
                                               //.ToStatement(Create.GrlsMPStatementLP.WithInnerState(InnerStateCode.canceled))
                                               .WithInternalState(InternalStates.Project)
                                               .WithQRCode()
                                               .Please();

        }
    }

}

namespace Core.BL.Tests.GRLS { 
    public static partial class Create
    {
        public static ChangeDocumentInternalStateBuilder ChangeDocumentInternalState { get { return new ChangeDocumentInternalStateBuilder(); } }
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
