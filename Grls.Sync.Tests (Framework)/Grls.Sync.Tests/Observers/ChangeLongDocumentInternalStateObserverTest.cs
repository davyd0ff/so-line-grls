using Core.BusinessTransactions.ChangeDocumentInternalStateTransactions;
using Core.BusinessTransactions;
using Grls.Sync.Tests.Helpers.GRLS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.BusinessTransactions.ChangeDocumentExternalStateTransactions;
using Grls.Sync.Tests.Helpers.Models.Common;
using Grls.Sync.Tests.Helpers.Models;

namespace Grls.Sync.Tests.Observers
{
    [TestClass]
    public class ChangeLongDocumentInternalStateObserverTest
    {
        private readonly Create Create;

        public ChangeLongDocumentInternalStateObserverTest()
        {
            this.Create = new Create();
        }

        [TestMethod]
        public void Test_ChangeLongDocumentInternalStateObserver_RunTrigger_InternalState()
        {
            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered);

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(statement)
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, testService) =
                           Create.ChangeLongDocumentInternalStateObserver
                                 .WithDocument(applicantRequest)
                                 .WithInternalStateTransitions(
                                        Create.StateTransition
                                              .ForDocumentType(applicantRequest.DocumentType)
                                              .From(InternalStates.Signed)
                                              .To(InternalStates.Sending)
                                              .HasPermissions(Actions.InternalStateChange)
                                              .WithTriggers(
                                                    Create.Trigger
                                                          .ForDocument(statement)
                                                          .ToInternalState(InternalStates.RequestFormed)
                                               )
                                  )
                                 .PleaseWithTestService();



            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);



            testService.ICoreUnitOfWork.Verify(u => u.Get<ChangeGrlsApplicantRequestInternalState>(), Times.Never());
            testService.ICoreUnitOfWork.Verify(u => u.Get<ChangeDocumentInternalState>(), Times.Once());
        }

        [TestMethod]
        public void Test_ChangeLongDocumentInternalStateObserver_RunTrigger_ExternalState()
        {
            var statement = Create.StatementMR.WithInternalState(InternalStates.Entered);

            var applicantRequest = Create.GrlsMrApplicantRequestMZ
                                         .ToStatement(statement)
                                         .WithInternalState(InternalStates.Signed)
                                         .Please();

            var (observer, testService) =
                           Create.ChangeLongDocumentInternalStateObserver
                                 .WithDocument(applicantRequest)
                                 .WithInternalStateTransitions(
                                        Create.StateTransition
                                              .ForDocumentType(applicantRequest.DocumentType)
                                              .From(InternalStates.Signed)
                                              .To(InternalStates.Sending)
                                              .HasPermissions(Actions.InternalStateChange)
                                              .WithTriggers(
                                                    Create.Trigger
                                                          .ForDocument(applicantRequest)
                                                          .ToOuterState(OuterStates.SendApplicant)
                                               )
                                  )
                                 .PleaseWithTestService();



            observer.Execute(testService.ICoreUnitOfWork.Object, testService.IncomingParams);



            testService.ICoreUnitOfWork.Verify(u => u.Get<ChangeLongDocumentExternalState>(), Times.Once());
            testService.ICoreUnitOfWork.Verify(u => u.Get<ChangeGrlsApplicantRequestExternalState>(), Times.Once());
        }

    }
}
