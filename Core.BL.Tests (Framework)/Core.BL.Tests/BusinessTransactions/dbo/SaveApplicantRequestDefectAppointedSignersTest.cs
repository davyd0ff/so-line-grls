using Core.BusinessTransactions;
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

        #region Процесс согласования (Последовательность галочек)
        /// <summary>
        /// Approver пытается поставить галочку без галочки Performer
        /// </summary>
        [TestMethod]
        public void Test_TransactionIsFailure_RequestHasIndorseState_ApproverSetApproveBeforePerformer()
        {
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId();
            var approver = Create.ApprovingSigners.Approver.WithId().WithUser(currentUser);
            var signatory = Create.ApprovingSigners.Signatory.WithId();

            var applicantRequest = Create.GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(performer)
                .WithApprover(approver)
                .WithSignatory(signatory)
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                .WithAuthenticatedUser(currentUser)
                .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer, 
                approver.Approved(), 
                signatory
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }
        #endregion


        /// <summary>
        /// Попытка сохранить пустой список согласующих (project)
        /// </summary>
        /// <result>
        ///   - IDocumentSignersRepository не будет вызван, т.к. пустой запрос ничем не отличается от изначальных данных
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSucess_EmptySigners_RequestHasProjectState()
        {
            var currentUser = Create.User;

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(currentUser)
                .InnerState(DocumentInternalStateEnum.project)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer, 
                Create.ApprovingSigners.Approver, 
                Create.ApprovingSigners.Signatory
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()), 
                Times.Never());
        }

        /// <summary>
        /// Попытка сохранить частично заполненный список согласующих (project)
        /// </summary>
        /// <result>
        ///   - IDocumentSignersRepository будет вызван
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSucess_SignersWithOnlyPerformer_RequestHasProjectState()
        {
            var currentUser = Create.User;
            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(currentUser)
                .InnerState(DocumentInternalStateEnum.project)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                Create.ApprovingSigners.Performer.WithId(),
                Create.ApprovingSigners.Approver,
                Create.ApprovingSigners.Signatory
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(It.IsAny<long>(), It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Once());
        }

        /// <summary>
        /// Попытка сохранить список без исполнителя (Performer is null)
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
        /// Попытка сохранить список без указания исполнителя (performer is empty)
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
        /// Попытка сохранить список без согласующего (Approver, null)
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
        /// Попытка сохранить список без указания согласующего (Approver, empty)
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
        /// Попытка сохранить список без подписанта (null)
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
        /// Попытка сохранить список без подписанта (empty)
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
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId().WithUser(currentUser);
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
                                                   .WithAuthenticatedUser(currentUser)
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
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId();
            var approver = Create.ApprovingSigners.Approver.WithId().WithUser(currentUser);
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
                                                   .WithAuthenticatedUser(currentUser)
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
        /// эксперт не имеет ApprovingAdministrator (но является автором) и запрос в состоянии [Проект]
        /// </summary>
        /// <result>
        ///     Может редактировать блок подписантов (менять подписантов)
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_ExpertIsAuthorAndCanChangeSignersList()
        {
            var currentUser = Create.User;

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(currentUser)
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
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

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator (но является автором) и запрос в состоянии [Проект]
        /// </summary>
        /// <result>
        ///     Не может ставить отметку согласовано напротив Автора
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_ExpertCannotSetApproved()
        {
            var currentUser = Create.User;

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(currentUser)
                .AuthorSetApproved()
                .WithPerformer(Create.ApprovingSigners.Performer.WithId())
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
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
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт не является approvingSigner)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Может ставить отметки согласовано (но только если текущий пользователь == signer)
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotChangeSignersList_RequestIsInIndorseStage()
        {
            var currentUser = Create.User;

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
                                                   .WithAuthenticatedUser(currentUser)
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

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт не является approvingSigner)
        /// </summary>
        /// <result>
        ///     Не может ставить отметки согласовано
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertCannotSetApproved_RequestIsInIndorseStage()
        {
            var currentUser = Create.User;

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
                                                   .WithAuthenticatedUser(currentUser)  
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
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт является performer)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов) (Approver)
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertIsPerformerAndCannotChangeApprover_RequestIsInIndorseStage()
        {
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId().WithUser(currentUser);
            var signatory = Create.ApprovingSigners.Signatory.WithId();

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(performer)
                .WithApprover(Create.ApprovingSigners.Approver.WithId())
                .WithSignatory(signatory)
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer,
                Create.ApprovingSigners.Approver.WithId(),
                signatory,
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }

        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт является performer)
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов) (Signatory)
        /// </result>
        [TestMethod]
        public void Test_TransactionIsFailure_ExpertIsPerformerAndCannotChangeSignatory_RequestIsInIndorseStage()
        {
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId().WithUser(currentUser);
            var approver = Create.ApprovingSigners.Approver.WithId();

            var applicantRequest = Create
                .GrlsMPApplicantRequestDefect
                .FromJSONFile("./BusinessTransactions/dbo/ApplicantRequests/json/ApplicantRequestDefect_Empty.json")
                .WithAuthor(Create.User)
                .AuthorSetApproved()
                .WithPerformer(performer)
                .WithApprover(approver)
                .WithSignatory(Create.ApprovingSigners.Signatory.WithId())
                .InnerState(DocumentInternalStateEnum.indorse)
                .Please();

            var (transaction, testService) = Create.SaveApplicantRequestDefectAppointedSigners
                                                   .WithAuthenticatedUser(currentUser)
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer,
                approver,
                Create.ApprovingSigners.Signatory.WithId(),
            });


            testService.IsTrue(result.IsFail);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(
                    It.IsAny<long>(),
                    It.IsAny<IEnumerable<ApprovingSigner>>()),
                Times.Never());
        }


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На визировании] (эксперт является performer)
        /// </summary>
        /// <result>
        ///     может ставить отметку согласовано напротив Performer
        /// </result>
        [TestMethod]
        public void Test_TransactionIsSuccessful_ExpertIsPerformerAndCanSetApproved_RequestIsInIndorseStage()
        {
            var currentUser = Create.User;
            var performer = Create.ApprovingSigners.Performer.WithId().WithUser(currentUser);
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
                                                   .WithAuthenticatedUser(currentUser)
                                                   .PleaseWithTestService();


            var result = transaction.Run(applicantRequest, new ApprovingSigner[]
            {
                performer.Approved(),
                approver,
                signatory,
            });


            testService.IsTrue(result.IsSuccess);
            testService.IDocumentSignersRepository.Verify(
                repo => repo.SaveSigners(
                    It.IsAny<long>(), 
                    It.Is<IEnumerable<ApprovingSigner>>(signers => signers.GetPerformer().Approved == true)),
                Times.Once());
        }


        /// <summary>
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [На подписи] (эксперт не является approvingSigner)
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
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Подписан] (эксперт не является approvingSigner)
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
        /// </summary>
        /// <result>
        ///     Не может редактировать блок подписантов (менять подписантов)
        ///     Не может ставить отметки согласовано
        /// </result>
        //[TestMethod]
        //public void Test_


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
        /// эксперт не имеет ApprovingAdministrator и запрос в состоянии [Ответ не получен] (эксперт является одним из подписантов)
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
                                                   .WithAuthenticatedUser(Create.User.WithPermissions(Enums.ActionsEnum.ApproovingAdministrator))
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
