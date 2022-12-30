using Core.BL.Tests.Helpers.IDGenerator;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.MedicamentRegistration;
using Core.Repositories;
using Core.Repositories.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.GRLS.Statements
{
    public class StatementMRBuilder
    {
        private readonly string _code = StatementTypeList.RegistrationStatement;

        private Mock<ICoreUnitOfWork> _unitOfWork;
        private IncomingPackageBase _incoming;
        private MedicamentRegistrationStatement _statement;
        private class MyMockRegistrationResolutionRepository : RegistrationResolutionRepository
        {
            private IEnumerable<RegistrationResolution> _registrationResolutions = new List<RegistrationResolution>();

            public MyMockRegistrationResolutionRepository(ICoreUnitOfWork unitOfWork, IDbContext context)
                : base(unitOfWork, context) { }

            public MyMockRegistrationResolutionRepository()
                : this(new Mock<ICoreUnitOfWork>().Object, new Mock<IDbContext>().Object) { }

            public MyMockRegistrationResolutionRepository(IEnumerable<RegistrationResolution> resolutions) : this()
            {
                _registrationResolutions = resolutions;
            }

            public MyMockRegistrationResolutionRepository(RegistrationResolution resolution) : this()

            {
                _registrationResolutions = new List<RegistrationResolution> { resolution };
            }

            public override IEnumerable<RegistrationResolution> GetByIncoming(long number)
            {
                return new List<RegistrationResolution>();
            }
        }

        public StatementMRBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var statementDocumentType = new DocumentType
            {
                Id = 1,
                Code = this._code,
                Flow = new DocumentFlow
                {
                    Id = 1,
                    Module = Enums.ModuleEnum.grls,
                    Code = "flow_reg"
                }
            };

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


            // TODO возможна ошибка, если вызвать еще метод WithFinalResolution()
            // TODO SOLUTION: тогда нужно быдет выносить Mock<RegistrationResolutionRepository> как атрибут класса 
            //                для Incoming и Statement вызывать отдельные методы Please(), которые бы вызывали метод "регистрации"
            //                НО скорее всего если вызвать Please() дважды, то будет ошибка, т.к. опять таки будет дважды 
            //                регистрироваться замоканная зависимость

            this._unitOfWork.Setup(u => u.Get<RegistrationResolutionRepository>())
                            .Returns(new MyMockRegistrationResolutionRepository());


            unitOfWork.Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
                      .Returns(typeof(MedicamentRegistrationStatement));

            DocumentTypes.DocumentTypeList.Add(statementDocumentType);
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
            this._unitOfWork.Setup(u => u.Get<RegistrationResolutionRepository>())
                            .Returns(new MyMockRegistrationResolutionRepository(finalResolution));

            return this;
        }

        public MedicamentRegistrationStatement Please()
        {
            return this._statement;
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
