using Core.BL.Tests.Helpers.IDGenerator;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.Models.Common
{
    public class TriggerBuilder
    {
        private DocumentType _documentType;
        private StateBase _state;
        private bool _reverse = false;
        private StateTransitionTriggerType _typeTrigger;

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


            return trigger;
        }


        public static implicit operator StateTransitionTrigger(TriggerBuilder builder)
        {
            return builder.Please();
        }
    }
}
