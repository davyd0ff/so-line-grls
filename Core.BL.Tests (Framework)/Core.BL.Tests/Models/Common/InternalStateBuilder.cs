using Core.Infrastructure;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Core.BL.Tests.Models.Common
{
    public static class InternalStates
    {
        public static InternalStateBuilder Project =>
            new InternalStateBuilder(InnerStateCode.project);

        public static InternalStateBuilder Canceled =>
            new InternalStateBuilder(InnerStateCode.canceled);

        public static InternalStateBuilder Entered =>
            new InternalStateBuilder(InnerStateCode.entered);

        public static InternalStateBuilder Signed =>
            new InternalStateBuilder(InnerStateCode.signed);

        public static InternalStateBuilder Signing =>
            new InternalStateBuilder(InnerStateCode.signing);

        public static InternalStateBuilder Sending =>
            new InternalStateBuilder(InnerStateCode.sending);

        public static InternalStateBuilder RequestFormed =>
            new InternalStateBuilder(InnerStateCode.request_formed);
    }

    public class InternalStateBuilder
    {
        private static int InternalStateCount = 0;
        private static Dictionary<string, InternalState> Hash = new Dictionary<string, InternalState>();
        private InternalState _state;

        public InternalStateBuilder(string stateCode)
        {
            if (Hash.Keys.Contains(stateCode))
            {
                this._state = Hash[stateCode];
            }
            else
            {
                InternalStateCount += 1;

                this._state = new InternalState();
                this._state.Id = InternalStateCount;
                this._state.Code = stateCode;

                Hash.Add(stateCode, this._state);
            }
        }

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
