using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System.Collections.Generic;

namespace Grls.Sync.Tests.Helpers.Models.Common
{
    public class TriggerBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;
        private DocumentType _documentType;
        private StateBase _state;
        private bool _reverse = false;
        private StateTransitionTriggerType _typeTrigger;

        public TriggerBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public TriggerBuilder ForDocument(MedicamentRegistrationStatement statement)
        {
            this._documentType = statement.DocumentType;

            return this;
        }

        public TriggerBuilder ForDocument(MedicamentRegistrationApplicantRequest applicantRequest)
        {
            this._documentType = applicantRequest.DocumentType;

            return this;
        }

        public TriggerBuilder ToInternalState(InternalState state)
        {
            this._state = state;
            this._typeTrigger = StateTransitionTriggerType.InternalStateChange;

            return this;
        }

        public TriggerBuilder ToOuterState(State state)
        {
            this._state = state;
            this._typeTrigger = StateTransitionTriggerType.ExtenralStateChange;

            return this;
        }

        public TriggerBuilder IsReverse()
        {
            this._reverse = true;

            return this;
        }

        public StateTransitionTrigger Please()
        {
            var trigger = new StateTransitionTrigger();
            trigger.Id = TriggerIdGenerator.Next();
            trigger.DocumentTypeCode = this._documentType.Code;
            trigger.DocumentTypeId = this._documentType.Id;
            trigger.ToDocumentStateId = this._state.Id;
            trigger.Reverse = this._reverse;
            trigger.TargetDocumentTypeCode = this._documentType.Code;
            trigger.TargetDocumentTypeId = this._documentType.Id;
            trigger.TargetStateCode = this._state.Code;
            trigger.TargetStateId = this._state.Id;
            trigger.ToState = this._state;
            trigger.Type = this._typeTrigger;

            // Мок проверки, что для данного типа документа есть состояния
            //     вот почему не работает ... у меня 2 триггера ... которые вызываются последовательно друг за другом 
            //     в результате Mock<ICoreUnitOfWork> имеет логику последнего триггера 
            //     
            //     значит мне надо как-то выносить на уровень выше .... или использовать 1 триггер в тестах
            //     а при необходимости подменять... проблема в том что не удастся, т.к. там тоже массив и он затрет 
            //     предыдущий мок
            //
            //     либо где-то подгрузить фикстуру на самом высоком уровне... причем эта фикстура должна быть связана
            //     с данными которые создает Builder
            //
            //     Итог - нужны фикстуры, на это надо потратить время ... 
            //
            var mockDocumentInternalStatusDocumentTypeRepository = new Mock<IDocumentStatusDocumentTypeRepository>();
            mockDocumentInternalStatusDocumentTypeRepository
                .Setup(r => r.GetStatesByTypeId(It.Is<int>(p => p.Equals(this._documentType.Id))))
                .Returns(new List<DocumentStatusDocumentType> {
                        new DocumentStatusDocumentType
                        {
                            DocumentType = this._documentType,
                            State = this._state,
                        }
                    }
                );

            this._unitOfWork
                .Setup(u => u.Get<IDocumentStatusDocumentTypeRepository>(
                    It.Is<string>(p => p.Equals(typeof(DocumentInternalStateDocumentType).Name))))
                .Returns(mockDocumentInternalStatusDocumentTypeRepository.Object);



            var mockDocumentExternalStatusDocumentTypeRepository = new Mock<IDocumentStatusDocumentTypeRepository>();
            mockDocumentExternalStatusDocumentTypeRepository
                .Setup(r => r.GetStatesByTypeId(It.Is<int>(p => p.Equals(this._documentType.Id))))
                .Returns(new List<DocumentStatusDocumentType> {
                        new DocumentStatusDocumentType
                        {
                            DocumentType = this._documentType,
                            State = this._state,
                        }
                    }
                );

            this._unitOfWork
                .Setup(u => u.Get<IDocumentStatusDocumentTypeRepository>(
                    It.Is<string>(p => p.Equals(typeof(DocumentStatusDocumentType).Name))))
                .Returns(mockDocumentExternalStatusDocumentTypeRepository.Object);

            return trigger;
        }


        public static implicit operator StateTransitionTrigger(TriggerBuilder builder)
        {
            return builder.Please();
        }
    }
}
