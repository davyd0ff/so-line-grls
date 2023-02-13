using Core.BL.Tests.GRLS;
using Core.BL.Tests.Helpers;
using Core.BL.Tests.Models;
using Core.BL.Tests.Models.Common;
using Core.Models.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.BusinessTransactions.States
{
    [TestClass]
    public class ChangeApplicantRequesDefectInternalStateTest
    {
        private readonly Create Create;

        public ChangeApplicantRequesDefectInternalStateTest()
        {
            this.Create = new Create();
            Loader.DocumentTypes();
        }

        [TestMethod]
        public void Test_FromProjectToIndorse_ChangeApplicantRequestInternalState_WasCalled()
        {


            //var (transaction, testService) = Create.ChangeApplicantRequesDefectInternalState
            //                                       .WithUser(Create.User.WithPermissions(Actions.InternalStateChange))
            //                                       .WithRequest(Create.GrlsMPApplicantRequestDefect.WithInternalState(InternalStates.Project))
            //                                       .ToInternalState(InternalStates.Indorse)
            //                                       .WithInternalStateTransition()
            //                                       .PleaseWithTestService();
                                                   
            //                                       .

        }

    }
}
