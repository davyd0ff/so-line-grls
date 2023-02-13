using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.Enums.States;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;


namespace Core.BL.Tests.BusinessTransactions.dbo
{
    [TestClass]
    public class SaveApplicantRequestDefectAppointedSignersTest
    {
        private readonly Create Create;

        public SaveApplicantRequestDefectAppointedSignersTest()
        {
            Create = new Create();
        }

        /// <summary>
        /// Попытка сохранить null вместо списка согласующих
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_NullableSigners()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, null);

            
            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить пустой список согласующих
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_EmptySigners()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer, 
                Create.ApprovingSigners.Approver, 
                Create.ApprovingSigners.Signatory
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()), 
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без исполнителя
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_WithoutPerformer()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Approver,
                Create.ApprovingSigners.Signatory,
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без указания исполнителя
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_PerformerIsEmpty()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer,
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без согласующего
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_WithoutApprover()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer,
                Create.ApprovingSigners.Signatory,
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без указания согласующего
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_ApproverIsEmpty()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver,
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без подписанта
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_WithoutSignatory()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer, 
                Create.ApprovingSigners.Approver,
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить список без подписанта
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_SignatoryIsEmpty()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory,
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить визирование без визы автора
        /// </summary>
        /// <result>
        ///     Автор должен завизировать документ
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_AuthodDidNotApproveRequest()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .AuthorDidNotApprove()
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId().Approved(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }


        /// <summary>
        /// Performer установил отметку
        /// </summary>
        /// <result>
        ///     Уведомление Approver
        /// </result>
        [TestMethod]
        public void Test_PerformerSetApprove_MailSenderApplicantRequestDefectWasApproved_WasCalled()
        {
            var performer = Create.ApprovingSigners.Performer.WithId();
            var approver = Create.ApprovingSigners.Approver.WithId();
            var signatory = Create.ApprovingSigners.Signatory.WithId();

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(performer)
                .WithApprover(approver)
                .WithSignatory(signatory)
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer.Approved(), 
                approver,
                signatory,
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectWasApproved.Verify(
                repo => repo.Run(It.IsAny<IMailSenderModel>()),
                Times.Once());
        }


        /// <summary>
        /// Approver установил отметку
        /// </summary>
        /// <result>
        ///     Уведомление Signatory
        /// </result>
        [TestMethod]
        public void Test_ApproverSetApprove_MailSenderApplicantRequestDefectWasApproved_WasCalled()
        {
            var performer = Create.ApprovingSigners.Performer.WithId();
            var approver = Create.ApprovingSigners.Approver.WithId();
            var signatory = Create.ApprovingSigners.Signatory.WithId();

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(performer.Approved())
                .WithApprover(approver)
                .WithSignatory(signatory)
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer.Approved(),
                approver.Approved(),
                signatory,
            });


            testService.IsTrue(result.IsSuccess);
            testService.MailSenderApplicantRequestDefectWasApproved.Verify(
                repo => repo.Run(It.IsAny<IMailSenderModel>()),
                Times.Once());
        }


        #region Эксперт не имеет полномочие ApprovingAdministrator
        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Проект]
        /// </summary>
        /// <result>
        ///     Может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_ExpertCanChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsSuccessful_ExpertCannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Может ставить отметки согласовано (но только если текущий пользователь == signer)
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInIndorseStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInIndorseStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }



        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На подписи] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInSigningStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signing)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInSigningStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signing)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Подписан] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInSignedStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signed)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInSignedStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signed)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Подписан] (эксперт является подписантом)
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        /// </summary>

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На выдачу] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInSendingStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.sending)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInSendingStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.sending)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На выдачу] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Выдан заявителю] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInSendApplicantStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.send_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInSendApplicantStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.send_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Выдан заявителю] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Обработан заявителем] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInHandledApplicantStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.handled_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInHandledApplicantStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.handled_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Обработан заявителем] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Ответ зарегистрирован] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInResponseReceivedStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.response_received)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInResponseReceivedStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.response_received)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Ответ зарегистрирован] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не меет ApprovingAdministrator и запрос в состоянии [Ответ получен] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInAnsweredStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.answered)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInAnsweredStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.answered)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не меет ApprovingAdministrator и запрос в состоянии [Ответ получен] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Ответ не получен] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInNoAnswerStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.no_answer)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInNoAnswerStage()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.no_answer)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Ответ не получен] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Аннулирован] (эксперт не является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInCanceledState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.canceled)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInCanceledState()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.canceled)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Аннулирован] (эксперт является подписантом)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>

        #endregion


        #region Эксперт имеет полномочие ApprovingAdministrator
        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Проект]
        /// </summary>
        /// <result>
        ///     Может редактировать блок подписантов (менять подписантов)
        ///     Может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInProjectStage_CanChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.project)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInProjectStage_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.project)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInProjectStageSetApprovedPerformerAndApprover_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.project)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId().Approved(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [На визировании]
        /// </summary>
        /// <result>
        ///     Может редактировать блок подписантов (менять подписантов)
        ///     Может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInIndorseStage_CanChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInIndorseStage_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInIndorseStageSetApprovedPerformerAndApprover_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId().Approved(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [На подписи]
        /// </summary>
        /// <result>
        ///     Может редактировать блок подписантов (менять подписантов)
        ///     Может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSigningStage_CanChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signing)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSigningStage_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signing)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSigningStageSetApprovedPerformerAndApprover_CanSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signing)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId().Approved(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Подписан]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSignedStage_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signed)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSignedStage_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.signed)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [На выдачу]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSendingStage_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.sending)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSendingStage_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.sending)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Выдан заявителю]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSendApplicantStage_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.send_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInSendApplicantStage_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.send_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Обработан заявителем]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInHandledApplicantStage_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.handled_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInHandledApplicantStage_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.handled_applicant)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Ответ зарегистрирован]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInResponseReceivedStage_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.response_received)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInResponseReceivedStage_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.response_received)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Ответ получен]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInAnsweredState_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.answered)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInAnsweredState_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.answered)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Ответ не получен]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInNoAnswerState_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.no_answer)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInNoAnswerState_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.no_answer)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт имеет ApprovingAdministrator и запрос в состоянии [Аннулирован]
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInCanceledState_CannotChangeSignersList()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.canceled)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertHasApprovingAdministratorPermission_RequestIsInCanceledState_CannotSetApproved()
        {
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.canceled)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId().Approved(),
                Create.ApprovingSigners.Approver.WithId(),
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsFalse(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        #endregion
    }
}
