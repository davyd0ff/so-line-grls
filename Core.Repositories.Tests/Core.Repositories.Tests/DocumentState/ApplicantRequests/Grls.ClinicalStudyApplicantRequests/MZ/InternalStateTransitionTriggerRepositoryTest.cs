using Core.DataAcquisition;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Repositories.Abstract;
using Core.Repositories.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace Core.Repositories.Tests.DocumentState.ApplicantRequests.Grls.ClinicalStudyApplicantRequests
{
    [TestClass]
    public class InternalStateTransitionTriggerRepositoryTest
    {
        private IStateTransitionRepositiory InternalStateTransitionRepository;
        private IStateTransitionTriggerRepositiory InternalStateTransitionTriggerRepository;
        private readonly string TestingEntityCode = "applicant_request_cs";
        private readonly ICoreUnitOfWork unitOfWork;

        public InternalStateTransitionTriggerRepositoryTest()
        {
            this.unitOfWork = UnitOfWork.Instance;
            Loader.DocumentTypes();
            this.InternalStateTransitionRepository = unitOfWork.GetStateRepository<IStateTransitionRepositiory>(true);
            this.InternalStateTransitionTriggerRepository = unitOfWork.GetStateRepository<IStateTransitionTriggerRepositiory>(true);
        }

        [TestMethod]
        [DataRow(InnerStateCode.project, InnerStateCode.formated, StatementTypeList.ApplicantRequestCS, OuterStateCode.formated)]
        [DataRow(InnerStateCode.project, InnerStateCode.canceled, StatementTypeList.ApplicantRequestCS, OuterStateCode.canceled)]
        [DataRow(InnerStateCode.formated, InnerStateCode.transferred, StatementTypeList.ApplicantRequestCS, OuterStateCode.send_applicant)]
        [DataRow(InnerStateCode.formated, InnerStateCode.canceled, StatementTypeList.ApplicantRequestCS, OuterStateCode.canceled)]
        [DataRow(InnerStateCode.transferred, InnerStateCode.handled_applicant, StatementTypeList.ApplicantRequestCS, OuterStateCode.handled_applicant)]
        [DataRow(InnerStateCode.transferred, InnerStateCode.canceled, StatementTypeList.ApplicantRequestCS, OuterStateCode.canceled)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.no_responded, StatementTypeList.ApplicantRequestCS, OuterStateCode.no_answer)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.response_received, StatementTypeList.ApplicantRequestCS, OuterStateCode.response_received)]
        [DataRow(InnerStateCode.handled_applicant, InnerStateCode.canceled, StatementTypeList.ApplicantRequestCS, OuterStateCode.canceled)]
        [DataRow(InnerStateCode.response_received, InnerStateCode.responded, StatementTypeList.ApplicantRequestCS, OuterStateCode.answered)]
        [DataRow(InnerStateCode.response_received, InnerStateCode.canceled, StatementTypeList.ApplicantRequestCS, OuterStateCode.canceled)]
        public void Test_GetTransitionTriggers(string fromStateCode, string toStateCode, string documentTypeCode, string toNextStateCode)
        {
            var transitionId = InternalStateTransitionRepository
                .FindTransition(
                    DocumentTypes.DocumentTypeList.First(dt => dt.Code.Equals(TestingEntityCode)).Id,
                    GetThesaurusItemByCode<InternalState>.Do(unitOfWork, fromStateCode).Id,
                    GetThesaurusItemByCode<InternalState>.Do(unitOfWork, toStateCode).Id)
                .Id;       


            var trigger = this
                .InternalStateTransitionTriggerRepository.GetTransitionTriggers(transitionId)
                .First(t => t.TargetDocumentTypeCode.Equals(documentTypeCode) 
                         && t.TargetStateCode.Equals(toNextStateCode)
                         && t.Type == StateTransitionTriggerType.ExtenralStateChange);


            Assert.IsNotNull(trigger);
        }

    }
}
