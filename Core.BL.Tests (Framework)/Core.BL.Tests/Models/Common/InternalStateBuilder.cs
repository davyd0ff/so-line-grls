using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.Enums.States;


namespace Core.BL.Tests.Models.Common
{
    public static class InternalStates
    {
        public static InternalStateBuilder Project =>
            new InternalStateBuilder(DocumentInternalStateEnum.project);

        public static InternalStateBuilder Canceled =>
            new InternalStateBuilder(DocumentInternalStateEnum.canceled);

        public static InternalStateBuilder Entered =>
            new InternalStateBuilder(DocumentInternalStateEnum.entered);

        public static InternalStateBuilder Signed =>
            new InternalStateBuilder(DocumentInternalStateEnum.signed);

        public static InternalStateBuilder Signing =>
            new InternalStateBuilder(DocumentInternalStateEnum.signing);

        public static InternalStateBuilder Sending =>
            new InternalStateBuilder(DocumentInternalStateEnum.sending);

        public static InternalStateBuilder RequestFormed =>
            new InternalStateBuilder(DocumentInternalStateEnum.request_formed);

        public static InternalStateBuilder Indorse =>
            new InternalStateBuilder(DocumentInternalStateEnum.indorse);
    }

    public class InternalStateBuilder
    {
        private InternalState _state;

        public InternalStateBuilder(DocumentInternalStateEnum state)
        {
            this._state = new InternalState();
            this._state.Id = (int)state;
            this._state.Code = state.ToString();
        }

        //public InternalStateBuilder(string stateCode)
        //{
        //    if (Hash.Keys.Contains(stateCode))
        //    {
        //        this._state = Hash[stateCode];
        //    }
        //    else
        //    {
        //        InternalStateCount += 1;

        //        this._state = new InternalState();
        //        this._state.Id = InternalStateCount;
        //        this._state.Code = stateCode;

        //        Hash.Add(stateCode, this._state);
        //    }
        //}

        public InternalState Please()
        {
            return this._state;
        }

        public static implicit operator int(InternalStateBuilder builder)
        {
            return builder.Please().Id;
        }
        public static implicit operator StateBase(InternalStateBuilder builder)
        {
            return builder.Please();
        }
        public static implicit operator InternalState(InternalStateBuilder builder)
        {
            return builder.Please();
        }
    }
}
