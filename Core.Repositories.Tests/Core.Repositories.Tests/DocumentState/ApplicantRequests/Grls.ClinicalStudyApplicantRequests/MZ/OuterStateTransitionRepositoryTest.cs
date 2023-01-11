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
    public class OuterStateTransitionRepositoryTest
    {
        private IStateTransitionRepositiory OuterStateTransitionRepository;
        private readonly string TestingEntityCode = "applicant_request_cs";
        
        public OuterStateTransitionRepositoryTest()
        {
            Loader.DocumentTypes();
            this.OuterStateTransitionRepository = UnitOfWork.Instance
                                                            .GetStateRepository<IStateTransitionRepositiory>(false);
        }


        [TestMethod]
        [DataRow(OuterStateCode.project, OuterStateCode.formated)]
        [DataRow(OuterStateCode.project, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.formated, OuterStateCode.send_applicant)]
        [DataRow(OuterStateCode.formated, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.send_applicant, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.send_applicant, OuterStateCode.handled_applicant)]
        [DataRow(OuterStateCode.handled_applicant, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.handled_applicant, OuterStateCode.response_received)]
        [DataRow(OuterStateCode.handled_applicant, OuterStateCode.no_answer)]
        [DataRow(OuterStateCode.no_answer, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.response_received, OuterStateCode.canceled)]
        [DataRow(OuterStateCode.response_received, OuterStateCode.answered)]
        [DataRow(OuterStateCode.answered, OuterStateCode.canceled)]        
        public void TestGetByType_HasOuterStates_IsNotReverse(string stateCodeFrom, string stateCodeTo) {
            var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == this.TestingEntityCode);

            var transitions = OuterStateTransitionRepository.GetByType(documentType.Id)
                                                            .Where(transition => transition.Reverse == false);

            Assert.IsTrue(transitions.Count() == 13);
            Assert.IsNotNull(transitions.First(transition => transition.FromState.Code == stateCodeFrom 
                                                          && transition.ToState.Code == stateCodeTo));
        }


        [TestMethod]
        [DataRow(OuterStateCode.formated, OuterStateCode.project)]
        [DataRow(OuterStateCode.send_applicant, OuterStateCode.formated)]
        [DataRow(OuterStateCode.handled_applicant, OuterStateCode.send_applicant)]
        [DataRow(OuterStateCode.response_received, OuterStateCode.handled_applicant)]
        [DataRow(OuterStateCode.no_answer, OuterStateCode.handled_applicant)]
        [DataRow(OuterStateCode.answered, OuterStateCode.response_received)]
        public void TestGetByType_HasOuterStates_IsReverse(string fromStateCode, string toStateCode)
        {
            var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == this.TestingEntityCode);

            var transitions = OuterStateTransitionRepository.GetByType(documentType.Id)
                                                            .Where(transition => transition.Reverse == true);

            Assert.IsTrue(transitions.Count() == 6);
            Assert.IsNotNull(transitions.First(transition => transition.FromState.Code == fromStateCode
                                                          && transition.ToState.Code == toStateCode));
        }


        [TestMethod]
        [DataRow(OuterStateCode.formated)]
        [DataRow(OuterStateCode.send_applicant)]
        [DataRow(OuterStateCode.handled_applicant)]
        [DataRow(OuterStateCode.response_received)]
        [DataRow(OuterStateCode.no_answer)]
        [DataRow(OuterStateCode.answered)]
        public void TestGetByType_HasNoAnyOuterTransitionsFromCanceledState(string toStateCode)
        {
            var documentType = DocumentTypes.DocumentTypeList.First(dt => dt.Code == this.TestingEntityCode);

            var transitions = OuterStateTransitionRepository.GetByType(documentType.Id)
                .Where(transition => transition.FromState.Code == OuterStateCode.canceled
                                  && transition.ToState.Code == toStateCode);

            Assert.IsTrue(transitions.Count() == 0);
        }

    }
}
