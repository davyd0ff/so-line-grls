using Core.BL.Tests.GRLS;
using Core.BL.Tests.Helpers;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.Entity.Models.core;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.Enums.States;
using Core.Models.CommunicationModels.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions.States
{
    [TestClass]
    public class ChangeApplicantRequesDefectInternalStateTest
    {
        private readonly Create Create;

        public ChangeApplicantRequesDefectInternalStateTest()
        {
            this.Create = new Create();
            Loader.DocumentTypes();
        }


        /// <summary>
        /// Изменение состояние с [Проект] на [На визировании]
        /// - автор проставил отметку
        /// </summary>
        /// <result>
        ///     Транзакция выполнится успешно
        ///     - вызыв транзакции ChangeApplicantRequestInternalState
        ///     - вызов MailSenderApplicantRequestDefectUpdateState (уведомление по почте)
        /// </result>
        [TestMethod]
        public void Test_FromProjectToIndorse_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();
            //                                       .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
            //                                       .WithRequest(Create.GrlsMPApplicantRequestDefect.WithInternalState(InternalStates.Project))
            //                                       .ToInternalState(InternalStates.Indorse)
            //                                       .WithInternalStateTransition()


            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.indorse,
                Code = DocumentInternalStateEnum.indorse.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.indorse)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_FromProjectToIndorse_MailSenderApplicantRequestDefectUpdateState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();
            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.indorse,
                Code = DocumentInternalStateEnum.indorse.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.Is<IMailSenderModel>(model =>
                        model.DocumentId == applicantRequest.DocumentId)),
                    Times.Once()
                );
        }

        /// <summary>
        /// Изменение состояние с [Проект] на [На визировании]
        /// - без отметки автора
        /// </summary>
        /// <result>
        ///     Транзакция не выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState не произойдет
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_AutorDidNotSetApproved_FromProjectToIndorse_ChangeApplicantRequestInternalState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.indorse,
                Code = DocumentInternalStateEnum.indorse.ToString(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.IsAny<ChangeStateRequest>()),
                    Times.Never()
                );
        }
        [TestMethod]
        public void Test_TransactionIsFailure_AutorDidNotSetApproved_FromProjectToIndorse_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.indorse,
                Code = DocumentInternalStateEnum.indorse.ToString(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояние на [Проект]
        /// - все отметки были сняты
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_UncheckedAllApproved_ToProject_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .InnerState(DocumentInternalStateEnum.indorse)
                                         .AuthorSetApproved()
                                         .PerformerUnsetApproved()
                                         .ApproverUnsetApproved()
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.project,
                Code = DocumentInternalStateEnum.project.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.project)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_UncheckedAllApproved_ToProject_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .InnerState(DocumentInternalStateEnum.indorse)
                                         .AuthorSetApproved()
                                         .PerformerUnsetApproved()
                                         .ApproverUnsetApproved()
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.project,
                Code = DocumentInternalStateEnum.project.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }


        /// <summary>
        /// Изменение состояние на [Проект]
        /// - НЕ все отметки были сняты
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailue_OneNotUncheckedApproved_ToProject_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .InnerState(DocumentInternalStateEnum.indorse)
                                         .AuthorSetApproved()
                                         .PerformerSetApproved()
                                         .ApproverUnsetApproved()
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.project,
                Code = DocumentInternalStateEnum.project.ToString(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.IsAny<ChangeStateRequest>()),
                    Times.Never()
                );
        }


        /// <summary>
        /// Изменение состояния с [Indorse] на [Signing]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromIndorseToSigning_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.indorse)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.signing,
                Code = DocumentInternalStateEnum.signing.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.signing)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromIndorseToSigning_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.indorse)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.signing,
                Code = DocumentInternalStateEnum.signing.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Signing] на [Signed]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSigningToSigned_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.signing)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.signed,
                Code = DocumentInternalStateEnum.signed.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.signed)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSigningToSigned_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.signing)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.signed,
                Code = DocumentInternalStateEnum.signed.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Signed] на [Sending]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSignedToSending_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.signed)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.sending,
                Code = DocumentInternalStateEnum.sending.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.sending)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSignedToSending_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.signed)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.sending,
                Code = DocumentInternalStateEnum.sending.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Sending] на [Send_Applicant]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSendingToSendApplicant_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.sending)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.send_applicant,
                Code = DocumentInternalStateEnum.send_applicant.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.send_applicant)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSendingToSendApplicant_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.sending)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.send_applicant,
                Code = DocumentInternalStateEnum.send_applicant.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Send_Applicant] на [Handled_Applicant]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSendApplicantToHandledApplicant_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.send_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.handled_applicant,
                Code = DocumentInternalStateEnum.handled_applicant.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.handled_applicant)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromSendApplicantToHandledApplicant_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.send_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.handled_applicant,
                Code = DocumentInternalStateEnum.handled_applicant.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Handled_Applicant] на [Response_Received]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromHandledApplicantToResponseReceived_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.handled_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.response_received,
                Code = DocumentInternalStateEnum.response_received.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.response_received)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromHandledApplicantToResponseReceived_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.handled_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.response_received,
                Code = DocumentInternalStateEnum.response_received.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Handled_Applicant] на [NoAnswer]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromHandledApplicantToNoAnwer_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.handled_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.no_answer,
                Code = DocumentInternalStateEnum.no_answer.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.no_answer)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromHandledApplicantToNoAnwer_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.handled_applicant)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.no_answer,
                Code = DocumentInternalStateEnum.no_answer.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }

        /// <summary>
        /// Изменение состояния с [Response_Received] на [Answered]
        /// </summary>
        /// <result>
        ///     Транзакция выполнится
        ///     - вызыв транзакции ChangeApplicantRequestInternalState 
        ///     - вызов MailSenderApplicantRequestDefectUpdateState не произойдет
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromResponseReceivedToAnswered_ChangeApplicantRequestInternalState_WasCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.response_received)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.answered,
                Code = DocumentInternalStateEnum.answered.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.ChangeApplicantRequestInternalState
                .Verify(
                    service => service.Run(It.Is<ChangeStateRequest>(changeStateRequest =>
                        changeStateRequest.DocumentId == applicantRequest.Id
                        && changeStateRequest.DocumentType.Id == applicantRequest.DocumentType.Id
                        && changeStateRequest.NewStateId == (int)DocumentInternalStateEnum.answered)),
                    Times.Once()
                );
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_FromResponseReceivedToAnswered_MailSenderApplicantRequestDefectUpdateState_WasNotCalled()
        {
            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                                         .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Project.json")
                                         .AuthorSetApproved()
                                         .InnerState(DocumentInternalStateEnum.response_received)
                                         .Please();

            var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
                                                   .PleaseWithTestService();

            var result = transaction.Run(applicantRequest, new InternalState
            {
                Id = (int)DocumentInternalStateEnum.answered,
                Code = DocumentInternalStateEnum.answered.ToString(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectUpdateState
                .Verify(
                    service => service.Run(It.IsAny<IMailSenderModel>()),
                    Times.Never()
                );
        }
    }
}
