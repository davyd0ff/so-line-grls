using Core.Infrastructure;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Common.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests.Helpers.Models.Common
{
    public static class OuterStates
    {
        public static OuterStateBuilder SendApplicant =>
            new OuterStateBuilder(OuterStateCode.send_applicant);
    }

    public class OuterStateBuilder
    {
        private static int OuterStateCount = 0;
        private static Dictionary<string, State> Hash = new Dictionary<string, State>();
        private State _state;

        public OuterStateBuilder(string stateCode)
        {
            if (Hash.Keys.Contains(stateCode))
            {
                this._state = Hash[stateCode];
            }
            else
            {
                OuterStateCount += 1;

                this._state = new State();
                this._state.Id = OuterStateCount;
                this._state.Code = stateCode;

                Hash.Add(stateCode, this._state);
            }
        }

        public State Please()
        {
            return this._state;
        }

        public static implicit operator StateBase(OuterStateBuilder builder)
        {
            return builder.Please();
        }
        public static implicit operator State(OuterStateBuilder builder)
        {
            return builder.Please();
        }
    }
}
