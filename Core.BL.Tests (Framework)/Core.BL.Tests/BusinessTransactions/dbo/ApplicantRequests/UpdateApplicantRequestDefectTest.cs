using Core.BL.Tests.Helpers.DeepClone;
using Core.BL.Tests.Helpers.Extensions;
using Core.BL.Tests.Models;
using Core.BusinessTransactions;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.Documents.ApplicantRequest;
using Core.Models.Common.Enums.States;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;


namespace Core.BL.Tests.BusinessTransactions.dbo.ApplicantRequests
{
    [TestClass]
    public class UpdateApplicantRequestDefectTest
    {
        private readonly Create Create;

        public UpdateApplicantRequestDefectTest()
        {
            Create = new Create();
        }


        /// <summary>
        /// Запрос имеет состояние project 
        /// - идет исправление
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена
        /// </result>
        [TestMethod]
        public void Test_UpdateApplicantRequest_WasCalled()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();


            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.RequestDescription = "Test";
            updatedApplicantRequest.IsApprovedByAuthor = true;
            var result = transaction.Run(updatedApplicantRequest);


            testService.UpdateApplicantRequest.Verify(tran => tran.Run(It.Is<IIdentifiedBase>(entity => entity.Id == applicantRequest.Id)), Times.Once());
        }


        /// <summary>
        /// Запрос имеет состояние project 
        /// - переводится в состояние canceled
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена только с вызовом ChangeApplicantRequesDefectInternalState с состоянием Canceled
        /// </result>
        [TestMethod]
        public void Test_UpdateApplicantRequest_WasNotCalled_BecauseApplicantRequestToCanceledState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.InternalState = new State
            {
                Id = (int)DocumentInternalStateEnum.canceled,
                Code = DocumentInternalStateEnum.canceled.ToString(),
            };
            var result = transaction.Run(updatedApplicantRequest);

            testService.UpdateApplicantRequest.Verify(tran => tran.Run(It.IsAny<IIdentifiedBase>()), Times.Never());
        }
        [TestMethod]
        public void Test_SaveApplicantRequestDefectAppointedSigners_WasNotCalled_BecauseApplicantRequestToCanceledState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.InternalState = new State
            {
                Id = (int)DocumentInternalStateEnum.canceled,
                Code = DocumentInternalStateEnum.canceled.ToString(),
            };
            var result = transaction.Run(updatedApplicantRequest);

            testService.SaveApplicantRequestDefectAppointedSigners.Verify(
                tran => tran.Run(It.IsAny<ApplicantRequestDefect>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_ChangeApplicantRequesDefectInternalState_WasCalled_WithCanceledState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.InternalState = new State
            {
                Id = (int)DocumentInternalStateEnum.canceled,
                Code = DocumentInternalStateEnum.canceled.ToString(),
            };
            var result = transaction.Run(updatedApplicantRequest);

            testService.ChangeApplicantRequesDefectInternalState.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)), 
                                 It.Is<InternalState>(state => state.Id == (int)DocumentInternalStateEnum.canceled)),
                Times.Once());
        }


        /// <summary>
        /// Запрос имеет состояние indorse, стоит отметка у Approver 
        /// - переводится в состояние canceled
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена только с вызовом ChangeApplicantRequesDefectInternalState с состоянием Canceled
        /// </result>
        [TestMethod]
        public void Test_ChangeApplicantRequesDefectInternalState_WasCalled_WithCanceledState_ApplicantRequestHasIndorseStateAndSetedApproveByApprover()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_IndorseWithApproved.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.InternalState = new State
            {
                Id = (int)DocumentInternalStateEnum.canceled,
                Code = DocumentInternalStateEnum.canceled.ToString(),
            };
            var result = transaction.Run(updatedApplicantRequest);

            testService.ChangeApplicantRequesDefectInternalState.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                                 It.Is<InternalState>(state => state.Id == (int)DocumentInternalStateEnum.canceled)),
                Times.Once());
        }


        /// <summary>
        /// Запрос имеет состояние indorse 
        /// - ставиится отметка у Approver 
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена 
        ///     - с вызовом ChangeApplicantRequesDefectInternalState с состоянием [Signing] (На подписи)
        /// </result>
        [TestMethod]
        public void Test_ChangeApplicantRequesDefectInternalState_WasCalled_WithSigningState_ApplicantRequestHasIndorseStateAndSetedApproveByApprover()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .InnerState(DocumentInternalStateEnum.indorse)
                .AuthorSetApproved()
                .PerformerSetApproved()
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.Signers.SetApproverApprivedValue(true);
            var result = transaction.Run(updatedApplicantRequest);

            testService.ChangeApplicantRequesDefectInternalState.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                                 It.Is<InternalState>(state => state.Id == (int)DocumentInternalStateEnum.signing)),
                Times.Once());
        }
        [TestMethod]
        public void Test_SaveApplicantRequestDefectAppointedSigners_WasCalled_ApplicantRequestHasIndorseStateAndSetedApproveByApprover()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .InnerState(DocumentInternalStateEnum.indorse)
                .AuthorSetApproved()
                .PerformerSetApproved()
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.Signers.SetApproverApprivedValue(true);
            var result = transaction.Run(updatedApplicantRequest);

            testService.SaveApplicantRequestDefectAppointedSigners.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                                 It.Is<IEnumerable<ApprovingSigner>>(signers => signers.GetApprover().Approved && signers.GetPerformer().Approved)),
                Times.Once());
        }


        /// <summary>
        /// Запрос имеет состояние project (например у пользователя права ApprovingAdministrator)
        /// - ставиится отметка у Approver 
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена 
        ///     - с вызовом SaveApplicantRequestDefectAppointedSigners c переданным списком Signers
        /// </result>
        [TestMethod]
        public void Test_SaveApplicantRequestDefectAppointedSigners_WasCalled_ApplicantRequestHasProjectStateAndSetedApproveByPerformer()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.Signers.SetApproverApprivedValue(true);
            var result = transaction.Run(updatedApplicantRequest);

            testService.SaveApplicantRequestDefectAppointedSigners.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                                 It.Is<IEnumerable<ApprovingSigner>>(signers => signers.GetApprover().Approved)),
                Times.Once());
        }

        /// <summary>
        /// Запрос имеет состояние project (например у пользователя права ApprovingAdministrator)
        /// - ставиится отметка у Approver 
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена 
        ///     - с вызовом ChangeApplicantRequesDefectInternalState с состоянием [Signing] (project)
        /// </result>
        [TestMethod]
        public void Test_ChangeApplicantRequesDefectInternalState_WasCalled_ApplicantRequestHasProjectStateAndSetedApproveByPerformer()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.Signers.SetApproverApprivedValue(true);
            var result = transaction.Run(updatedApplicantRequest);

            testService.ChangeApplicantRequesDefectInternalState.Verify(
                tran => tran.Run(It.Is<ApplicantRequestDefect>(request => request.RoutingGuid.Equals(applicantRequest.RoutingGuid)),
                                 It.Is<InternalState>(state => state.Id == (int) DocumentInternalStateEnum.project)),
                Times.Once());
        }

        /// <summary>
        /// Запрос имеет состояние indorse и пользователь не имеет ApprovingAdministrator
        /// - правится запрос
        /// </summary>
        /// <result>
        ///     Транзакция не должна быть выполнена 
        ///     - result.IsFail ("Редактирование запроса запрещено в состоянии на визировании")
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_WhenApplicantRequestIsIndorseThenEditingDeny()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();
            
            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .PleaseWithTestService();

            
            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.RequestDescription = "TEST";
            var result = transaction.Run(updatedApplicantRequest);


            testService.IsFalse(result.IsSuccess);
        }

        /// <summary>
        /// Запрос имеет состояние indorse и пользователь имеет ApprovingAdministrator
        /// - правится запрос
        /// </summary>
        /// <result>
        ///     Транзакция должна быть выполнена 
        ///     - result.IsSuccess
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_WhenUserHasPermission_AndRequestInIndorseState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_ProjectWithSigners.json")
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.UpdateApplicantRequestDefect
                                                   .ForRequest(applicantRequest)
                                                   .WithUser(Create.User.WithPermissions(Actions.ApprovingAdministrator))
                                                   .PleaseWithTestService();


            var updatedApplicantRequest = Cloner.DeepCopy(applicantRequest);
            updatedApplicantRequest.RequestDescription = "TEST";
            var result = transaction.Run(updatedApplicantRequest);


            testService.IsTrue(result.IsSuccess);
            testService.UpdateApplicantRequest.Verify(tran => tran.Run(It.IsAny<IIdentifiedBase>()), Times.Once());
        }

    }
}
