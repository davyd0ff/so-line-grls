using Core.BusinessTransactions;
using Core.BusinessTransactions.Abstract;
using Core.Infrastructure.Context.Abstract;
using Core.Models.CommunicationModels;
using Core.Repositories.Abstract;
using Grls.Common.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grls.Sync.Tests
{
    public class TestService
    {
        public Mock<ICoreUnitOfWork> ICoreUnitOfWork { get; set; }
        public Mock<IDocumentStateRepository> IDocumentStateRepository { get; set; }
        public Mock<IBinaryBusinessTransaction<ChangeStateInfo, bool>> ChangeLongDocumentInternalState { get; set; }
        public Mock<IDocumentRepository> IDocumentRepository { get; set; }
        public Mock<IBusinessTransaction<IIdentifiedBase>> UpdateApplicantRequest { get; set; }
        public Mock<IBusinessTransaction<AddMaterialsReceivedCreateParams>> CreateAddMaterialsReceived { get; set; }


        public object[] IncomingParams { get; set; }
    }
}
