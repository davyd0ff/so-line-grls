using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Repositories.Abstract;
using Core.Repositories.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace Core.Repositories.Tests.DocumentState.ApplicantRequests.Grls.ClinicalStudyApplicantRequests.MZ
{
    [TestClass]
    public class InternalStateTransitionRepositoryTest
    {
        private IStateTransitionRepositiory InternalStateTransitionRepository;
        private string TestingEntityCode = "applicant_request_cs";

        public InternalStateTransitionRepositoryTest()
        {
            Loader.DocumentTypes();
            this.InternalStateTransitionRepository = UnitOfWork.Instance
                                                               .GetStateRepository<IStateTransitionRepositiory>(true);
        }


        [TestMethod]
        [DataRow(InnerStateCode.project, InnerStateCode.formated)]
        [DataRow(InnerStateCode.project, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.formated, InnerStateCode.transferred)]
        [DataRow(InnerStateCode.formated, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.transferred, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.transferred, InnerStateCode.handled_applicant)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.response_received)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.no_responded)]
        [DataRow(InnerStateCode.no_responded, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.response_received, InnerStateCode.canceled)]
        [DataRow(InnerStateCode.response_received, InnerStateCode.responded)]
        [DataRow(InnerStateCode.responded, InnerStateCode.canceled)]
        public void TestGetByType_HasInnerStates_IsNotReverse(string fromStateCode, string toStateCode)
        {
            var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == this.TestingEntityCode);

            var transitions = InternalStateTransitionRepository
                .GetByType(documentType.Id)
                .Where(transition => transition.Reverse == false);

            Assert.IsTrue(transitions.Count() == 14);
            Assert.IsNotNull(transitions.First(transition => transition.FromState.Code == fromStateCode
                                                          && transition.ToState.Code == toStateCode));
        }



        [TestMethod]
        [DataRow(InnerStateCode.formated, InnerStateCode.project)]
        [DataRow(InnerStateCode.transferred, InnerStateCode.formated)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.transferred)]
        [DataRow(InnerStateCode.no_responded, InnerStateCode.handled_applicant)]
        public void TestGetByType_HasInnerStates_IsReverse(string fromStateCode, string toStateCode)
        {
            var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == this.TestingEntityCode);

            var transitions = InternalStateTransitionRepository
                .GetByType(documentType.Id)
                .Where(transition => transition.Reverse == true);

            Assert.IsTrue(transitions.Count() == 4);
            Assert.IsNotNull(transitions.First(transition => transition.FromState.Code == fromStateCode
                                                          && transition.ToState.Code == toStateCode));
        }
    }
}
