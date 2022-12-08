using Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.Models
{
    public static class Actions
    {
        public static ActionBuilder InternalStateChange => new ActionBuilder("InternalStateChange");
    }


    public class ActionBuilder
    {
        private static Dictionary<string, int> Hash = new Dictionary<string, int>();
        private static int ActionCounter = 0;

        private int _actionId;
        private string _actionCode;

        public ActionBuilder(string actionCode)
        {
            this._actionCode = actionCode;

            if (Hash.ContainsKey(actionCode))
            {
                this._actionId = Hash[actionCode];
            }
            else
            {
                ActionCounter += 1;
                this._actionId = ActionCounter;

                Hash.Add(actionCode, this._actionId);
            }
        }

        public static implicit operator int(ActionBuilder builder)
        {
            return builder._actionId;
        }

        public static implicit operator StateTransitionPermission(ActionBuilder builder)
        {
            return new StateTransitionPermission
            {
                ActionId = builder._actionId,
                ActionCode = builder._actionCode,
            };
        }

    }
}
