using Core.Models.Common;
using Core.Models.Common.Abstract;
using System.Linq;


namespace Core.BL.Tests.Models.Common
{
    public class StateTransitionBuilder
    {
        private StateTransition transition;
        private static int StateTransitionCount = 0;

        public StateTransitionBuilder()
        {
            StateTransitionCount += 1;

            transition = new StateTransition();
            transition.Id = StateTransitionCount;
        }

        public StateTransitionBuilder From(StateBase state)
        {
            transition.FromStateId = state.Id;
            transition.FromState = state;

            return this;
        }

        public StateTransitionBuilder To(StateBase state)
        {
            transition.ToStateId = state.Id;
            transition.ToState = state;

            return this;
        }

        public StateTransitionBuilder ForDocumentType(DocumentType documentType)
        {
            transition.DocumentTypeId = documentType.Id;
            transition.DocumentTypeCode = documentType.Code;

            return this;
        }


        public StateTransitionBuilder WithoutTriggers()
        {
            return this;
        }

        public StateTransitionBuilder WithTriggers(params StateTransitionTrigger[] trigers)
        {
            trigers.ToList().ForEach(t => t.StateTransitionId = this.transition.Id);
            transition.Triggers = trigers;

            return this;
        }

        public StateTransitionBuilder HasPermissions(params StateTransitionPermission[] permissions)
        {
            permissions.ToList().ForEach(p =>
            {
                p.StateTransitionId = this.transition.Id;
                p.FromState = this.transition.FromState;
                p.FromStateId = this.transition.FromStateId;
                p.ToState = this.transition.ToState;
                p.ToStateId = this.transition.ToStateId;
                p.DocumentTypeId = this.transition.DocumentTypeId;
                p.DocumentTypeCode = this.transition.DocumentTypeCode;
            });
            transition.Actions = permissions;


            return this;
        }

        public StateTransition Please()
        {
            return transition;
        }

        public static implicit operator StateTransition(StateTransitionBuilder builder)
        {
            return builder.transition;
        }

    }
}
