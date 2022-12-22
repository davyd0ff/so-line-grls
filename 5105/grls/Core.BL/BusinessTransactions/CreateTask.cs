using Core.DataAcquisition;
using Core.DataAcquisition.dbo;
using Core.Entity.Models;
using Core.Enums;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.BusinessTransactions;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.CommunicationModels.ClinicalStudy;
using Core.Models.CommunicationModels.DepartmentTask;
using Core.Models.Documents;
using Core.Models.Documents.ClinicalStudies;
using Core.Repositories.Abstract;
using System;
using System.Linq;

namespace Core.BusinessTransactions
{
    public class CreateTask : CreateRelatedDocument<CreateTaskAndResolutionRequest>
    {

        public CreateTask(ICoreUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected override TransactionResult PerformTransaction(CreateTaskAndResolutionRequest request)
        {
            //Получаем шаблон задания
            GetDocumentTemplate getDocumentTemplate = new GetDocumentTemplate(UnitOfWork);
            DocumentTemplate taskTemplate = getDocumentTemplate.Do(request.Id);
            if (taskTemplate.ResolutionKindPairId == 0) return TransactionResult.Fail("Отсутствует привязанное к заданию решение.");
            //Получаем шаблон связанного с заданием решения
            DocumentTemplate resTemplate = getDocumentTemplate.Do(taskTemplate.ResolutionKindPairId);
            //Создаем решение
            CreateTaskAndResolutionRequest resolutionCreate = new CreateTaskAndResolutionRequest
            {
                Id = Convert.ToInt32(resTemplate.Id),
                IncomingNumber = request.IncomingNumber,
                DocumentType = taskTemplate.DocumentType
            };
            TransactionResult resolutionResult = new CreateResolution(UnitOfWork).Run(resolutionCreate);
            if (!resolutionResult.IsSuccess) return TransactionResult.Fail(resolutionResult.GetErrorText());
            var resolution = ((RegistrationResolution)resolutionResult.ResultingObject);

            //Создаем задание
            RegistrationTask task = new RegistrationTask
            {
                ResolutionId = resolution.Id,
                ResolutionKindId = taskTemplate.Id,
                DocumentType = taskTemplate.DocumentType,
                RoutingGuid = Guid.NewGuid(),
                IncomingPackage = incoming,
                OutgoingRequisites = new OutgoingRequisites { 
                    OutgoingNumber = UnitOfWork.Get<GetOutgoingNumberForMRTasks>()
                                               .Do(resolution, taskTemplate)
                }
            };
            Document doc = null;
            //для ЗВИРКИ поиск по GUID
            if (incoming?.DocumentType?.Id == (int)DocumentTypeEnum.changes_ecs)
            {
                var st = new GetEntityByFilter<ClinicalStudyChangeResolutionStatement, ClinicalStudyChangeResolutionStatementFilter>(UnitOfWork).Do(
                                new ClinicalStudyChangeResolutionStatementFilter()
                                {
                                    IncomingNumber = request.IncomingNumber
                                }).FirstOrDefault();
                doc = new GetDocumentBaseByGuid<Document>(UnitOfWork).Do(st.RoutingGuid);
            }
            else
            {
                doc = new GetDocumentBaseByNumber(UnitOfWork)
                  .Do(incoming.Document.Id.ToString(), incoming.Document.DocumentType.Id);
            }

            var createTask = new InsertDocumentOperation<Document>(UnitOfWork).Run(new Document
            {
                DocumentTypeId = task.DocumentType.Id,
                DocumentNumber = task.OutgoingRequisites.OutgoingNumber,
                RoutingGuid = task.RoutingGuid,
                DocumentType = task.DocumentType
            }, doc?.Id);
            task.DocumentId = (long)createTask.ResultingObject;
            var repository = UnitOfWork.CreateIdentifiedRepository(typeof(RegistrationTask));
            int id = repository.Add(task);

            var repo = UnitOfWork.GetStateRepository<IDocumentStateRepository>(false);
            var states = GetStatesByType.Do(UnitOfWork, false, task.DocumentType.Id);
            var state = states.FirstOrDefault(s => s.State.Code == "project");
            repo.SetState(task.DocumentId, state.State.Id);

            UnitOfWork.UpdateQrCode(task);

            var repoVT = UnitOfWork.CreateIdentifiedRepository(typeof(ResolutionVariableText));
            if (taskTemplate.ResolutionTemplateId == 29 || taskTemplate.ResolutionTemplateId == 36)
            {
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ArticleTimeNumber", Text = "20" });
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ExpTime", Text = "30" });
            }
            if (taskTemplate.ResolutionTemplateId == 34)
            {
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ArticleTimeNumber", Text = "30" });
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ExpTime", Text = "70" });
            }
            if (taskTemplate.ResolutionTemplateId == 32)
            {
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ArticleTimeNumber", Text = "со статьёй 34" });
                repoVT.Add(new ResolutionVariableText { DocumentId = task.DocumentId, Name = "ExpTime", Text = "60" });
            }
            var document = (RegistrationTask)repository.FindById(id);
            AddIncomingWorkflowEvent workflowEvent = new AddIncomingWorkflowEvent(UnitOfWork);

            IncomingWorkFlowModel wfevent = new IncomingWorkFlowModel
            {
                StatementTypeId = document.DocumentType.Id,
                StatementId = document.Id,
                FlowId = document.Flow.Id,
                IncomingNumber = document.IncomingPackage.Number,
                StateId = document.State.Id
            };
            workflowEvent.Run(wfevent);
            //Добавляем событие "Добавлено задание ФГБУ"
            var eventRepository = UnitOfWork.CreateIdentifiedRepository(typeof(CustomEvent));
            IIdentifiedBase customEvent = new CustomEvent
            {
                UserId = UnitOfWork.User.Id,
                EventType = new CustomEventType { Id = 250 }
            };
            eventRepository.Add(customEvent);
            return TransactionResult.Succeeded(document);
        }


        protected override ValidationResult Validate(CreateTaskAndResolutionRequest request)
        {
            if (request.Id == 0)
            {
                return ValidationResult.Fail("Идентификатор не указан");
            }

            return base.Validate(request);
        }
    }
}
