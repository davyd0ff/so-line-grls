using Core.Infrastructure;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using System.Collections.Generic;
using System.Linq;


namespace Grls.Sync.Tests.Helpers.Models.Common
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

        public static InternalStateBuilder SendApplicant =>
            new InternalStateBuilder(InnerStateCode.send_applicant);

        public static InternalStateBuilder Transferred =>
            new InternalStateBuilder(InnerStateCode.transferred);

        public static InternalStateBuilder RequestFormed =>
            new InternalStateBuilder(InnerStateCode.request_formed);

        public static InternalStateBuilder Formated =>
            new InternalStateBuilder(InnerStateCode.formated);

        public static InternalStateBuilder HandledApplicant =>
            new InternalStateBuilder(InnerStateCode.handled_applicant);

        public static InternalStateBuilder ResponseReceived =>
            new InternalStateBuilder(InnerStateCode.response_received);
    }

    public class InternalStateBuilder
    {
        private static int InternalStateCount = 0;
        private static Dictionary<string, InternalState> Hash = new Dictionary<string, InternalState>();
        private InternalState _state;
        //private Mock<ICoreUnitOfWork> _mockedCoreUnitOfWork;

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

        public static implicit operator StateBase(InternalStateBuilder builder)
        {
            return builder.Please();
        }
        public static implicit operator InternalState(InternalStateBuilder builder)
        {
            return builder.Please();
        }
        public static implicit operator int(InternalStateBuilder builder)
        {
            return builder.Please().Id;
        }
        public static implicit operator string(InternalStateBuilder bulder)
        {
            return bulder.Please().Code;
        }
        //public static implicit operator IThesaurusBase(InternalStateBuilder builder)
        //{
        //    return builder.Please() as IThesaurusBase;
        //}
    }
}
