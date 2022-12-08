using Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.Models.Common
{
    public class StateTransitionPermissionBuilder
    {
        private StateTransitionPermission _permission;
        private static int PermissionCount = 0;

        public StateTransitionPermissionBuilder()
        {
            PermissionCount += 1;

            this._permission = new StateTransitionPermission();
            this._permission.Id = PermissionCount;
        }

        public StateTransitionPermission Please()
        {
            return this._permission;
        }

        public static implicit operator StateTransitionPermission(StateTransitionPermissionBuilder builder)
        {
            return builder.Please();
        }

        public StateTransitionPermissionBuilder WithActions(params string[] actions)
        {
            throw new NotImplementedException();
            return this;
        }
    }
}
