using Core.Entity.Models;
using Core.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.Models.Common
{
    public class StateTransitionTriggerBuilder
    {
        private StateTransitionTrigger _trigger;
        private static int StateTransitionTriggerCount = 0;

        public StateTransitionTriggerBuilder()
        {
            StateTransitionTriggerCount += 1;

            this._trigger = new StateTransitionTrigger();
            this._trigger.Id = StateTransitionTriggerCount;
        }

        public StateTransitionTrigger Please()
        {
            return this._trigger;
        }

        public static implicit operator StateTransitionTrigger(StateTransitionTriggerBuilder builder)
        {
            return builder.Please();
        }

        public StateTransitionTriggerBuilder InnerStateChangeType()
        {
            this._trigger.Type = StateTransitionTriggerType.InternalStateChange;
            return this;
        }
        public StateTransitionTriggerBuilder OuterStateChangeType()
        {
            this._trigger.Type = StateTransitionTriggerType.ExtenralStateChange;
            return this;
        }

        public StateTransitionTriggerBuilder WithNewDocumentState(string stateCode)
        {
            this._trigger.TargetStateCode = stateCode;
            return this;
        }

        public StateTransitionTriggerBuilder WithTargetDocument(DocumentType documentType)
        {
            this._trigger.TargetDocumentTypeCode = documentType.Code;
            this._trigger.TargetDocumentTypeId = documentType.Id;

            return this;
        }
    }
}
