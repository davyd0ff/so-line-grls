using Core.Entity.Models;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Common.Abstract;
using Core.Models.Documents;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories.Abstract;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grls.Sync.Tests.Helpers.GRLS.Statements
{
    public class StatementMRBuilder
    {
        private readonly string _code = StatementTypeList.RegistrationStatement;

        private Mock<ICoreUnitOfWork> _unitOfWork;
        private IncomingPackageBase _incoming;
        private MedicamentRegistrationStatement _statement;

        private Mock<IRegistrationResolutionRepository> mockedRegistrationResolutionRepository;

        public StatementMRBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var statementDocumentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if (statementDocumentType == null)
            {
                statementDocumentType = new DocumentType
                {
                    Id = 1,
                    Code = this._code,
                    Flow = new DocumentFlow
                    {
                        Id = 1,
                        Code = "flow_reg"
                    }
                };
                DocumentTypes.DocumentTypeList.Add(statementDocumentType);
            }

            var statamenentId = StatementIdGenerator.Next();
            var incomingNumber = IncomingNumberGenerator.Next();


            this._unitOfWork = unitOfWork;

            this._statement = new MedicamentRegistrationStatement();
            this._statement.Id = statamenentId;
            this._statement.DocumentType = statementDocumentType;

            this._incoming = new IncomingPackageBase();
            this._incoming.ParentDocument = this._statement;
            this._incoming.Document = this._statement;
            this._incoming.Number = incomingNumber;



            this.mockedRegistrationResolutionRepository = new Mock<IRegistrationResolutionRepository>();
            this.mockedRegistrationResolutionRepository
                .Setup(repo => repo.GetByIncoming(It.Is<long>(number => number.Equals(this._incoming.Number))))
                .Returns(new List<RegistrationResolution>());


            this._unitOfWork
                .Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
                .Returns(typeof(MedicamentRegistrationStatement));
            this._unitOfWork
                .Setup(u => u.Get<IRegistrationResolutionRepository>())
                .Returns(mockedRegistrationResolutionRepository.Object);

            this.MockIncomingRepositories();
        }

        private void MockIncomingRepositories()
        {
            var mockSearchRepositoryForIncomingNumber = new Mock<ISearchRepository<IncomingNumber>>();
            mockSearchRepositoryForIncomingNumber
                .Setup(r => r.Find(It.IsAny<IFilterBase>()))
                .Returns(new List<IncomingNumber>());

            
            this._unitOfWork
                .Setup(u => u.Get(typeof(ISearchRepository<IncomingNumber>)))
                .Returns(mockSearchRepositoryForIncomingNumber.Object);
        }


        public StatementMRBuilder WithInternalState(InternalState internalState)
        {
            this._statement.State = State.FromBase(internalState);
            //this._incoming.State = State.FromBase(internalState); //???

            var MockInternalStateBindingRepository = new Mock<IInternalStateBindingRepository>();
            MockInternalStateBindingRepository
                .Setup(r => r.GetDocumentInternalState(
                    It.Is<int>(entityId => entityId.Equals(this._statement.Id)),
                    It.Is<int>(typeId => typeId.Equals(this._statement.DocumentType.Id))
                 ))
                .Returns(new DocumentToInternalStateBinding { InternalState = internalState });

            this._unitOfWork
                .Setup(u => u.Get<IInternalStateBindingRepository>())
                .Returns(MockInternalStateBindingRepository.Object);


            return this;
        }

        public StatementMRBuilder WithFinalResolution(RegistrationResolution finalResolution)
        {
            this.mockedRegistrationResolutionRepository
                .Setup(repo => repo.GetByIncoming(It.Is<long>(number => number.Equals(this._incoming.Number))))
                .Returns(new List<RegistrationResolution> { finalResolution });

            this._unitOfWork
                .Setup(u => u.Get<IRegistrationResolutionRepository>())
                .Returns(mockedRegistrationResolutionRepository.Object);

            return this;
        }

        public static implicit operator IncomingPackageBase(StatementMRBuilder builder)
        {
            return builder._incoming;
        }

        public static implicit operator StatementBase(StatementMRBuilder builder)
        {
            return builder._statement;
        }

        public static implicit operator MedicamentRegistrationStatement(StatementMRBuilder builder)
        {
            return builder._statement;
        }
    }
}
