using Core.BL.Tests.Helpers;
using Core.BL.Tests.Helpers.IDGenerator;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.ClinicalStudies;
using Core.Repositories;
using Core.Repositories.Abstract;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Core.BL.Tests.GRLS.Statements
{
    public class ChangeResolutionStatementCSBuilder
    {
        private readonly string _code = StatementTypeList.ChangePermissionStatement;

        private Mock<ICoreUnitOfWork> _unitOfWork;
        private IncomingPackageBase _incoming;
        private ClinicalStudyChangeResolutionStatement _statement;

        private Mock<IRegistrationResolutionRepository> mockedRegistrationResolutionRepository;

        //private class MyMockRegistrationResolutionRepository : RegistrationResolutionRepository
        //{
        //    private IEnumerable<RegistrationResolution> _registrationResolutions = new List<RegistrationResolution>();

        //    public MyMockRegistrationResolutionRepository(ICoreUnitOfWork unitOfWork, IDbContext context)
        //        : base(unitOfWork, context) { }

        //    public MyMockRegistrationResolutionRepository()
        //        : this(new Mock<ICoreUnitOfWork>().Object, new Mock<IDbContext>().Object) { }

        //    public MyMockRegistrationResolutionRepository(IEnumerable<RegistrationResolution> resolutions) : this()
        //    {
        //        _registrationResolutions = resolutions;
        //    }

        //    public MyMockRegistrationResolutionRepository(RegistrationResolution resolution) : this()

        //    {
        //        _registrationResolutions = new List<RegistrationResolution> { resolution };
        //    }

        //    public override IEnumerable<RegistrationResolution> GetByIncoming(long number)
        //    {
        //        return new List<RegistrationResolution>();
        //    }
        //}

        public ChangeResolutionStatementCSBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var documentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if(documentType == null)
            {
                documentType = new DocumentType
                {
                    Id = 21,
                    Code = this._code,
                    Flow = new DocumentFlow
                    {
                        Id = 2,
                        Module = Enums.ModuleEnum.grls,
                        Code = "flow_cs"
                    }
                };

                DocumentTypes.DocumentTypeList.Add(documentType);
            }
            

            var statamenentId = StatementIdGenerator.Next();
            var incomingNumber = IncomingNumberGenerator.Next();


            this._unitOfWork = unitOfWork;

            this._statement = new ClinicalStudyChangeResolutionStatement();
            this._statement.Id = statamenentId;
            this._statement.DocumentType = documentType;

            this._incoming = new IncomingPackageBase();
            this._incoming.ParentDocument = this._statement;
            this._incoming.Document = this._statement;
            this._incoming.Number = incomingNumber;


            // TODO возможна ошибка, если вызвать еще метод WithFinalResolution()
            // TODO SOLUTION: тогда нужно быдет выносить Mock<RegistrationResolutionRepository> как атрибут класса 
            //                для Incoming и Statement вызывать отдельные методы Please(), которые бы вызывали метод "регистрации"
            //                НО скорее всего если вызвать Please() дважды, то будет ошибка, т.к. опять таки будет дважды 
            //                регистрироваться замоканная зависимость

            //this._unitOfWork.Setup(u => u.Get<RegistrationResolutionRepository>())
            //                .Returns(new MyMockRegistrationResolutionRepository());

            this.mockedRegistrationResolutionRepository = new Mock<IRegistrationResolutionRepository>();
            this.mockedRegistrationResolutionRepository
                .Setup(repo => repo.GetByIncoming(It.Is<long>(number => number.Equals(this._incoming.Number))))
                .Returns(new List<RegistrationResolution>());


            this._unitOfWork
                .Setup(u => u.GetDocumentTypeByTypeCode(It.Is<string>(p => p.Equals(this._code))))
                .Returns(typeof(ClinicalStudyChangeResolutionStatement));
            this._unitOfWork
                .Setup(u => u.Get<IRegistrationResolutionRepository>())
                .Returns(mockedRegistrationResolutionRepository.Object);
        }


        public ChangeResolutionStatementCSBuilder WithParentStatement(ClinicalStudyPermission parentStatement)
        {
            this._statement.ParentDocument = parentStatement;

            return this;
        }

        public ChangeResolutionStatementCSBuilder WithInternalState(InternalState internalState)
        {
            //this._statement.State = State.FromBase(internalState);
            ////this._incoming.State = State.FromBase(internalState); //???

            //var MockInternalStateBindingRepository = new Mock<IInternalStateBindingRepository>();
            //MockInternalStateBindingRepository
            //    .Setup(r => r.GetDocumentInternalState(
            //        It.Is<int>(entityId => entityId.Equals(this._statement.Id)),
            //        It.Is<int>(typeId => typeId.Equals(this._statement.DocumentType.Id))
            //     ))
            //    .Returns(new DocumentToInternalStateBinding { InternalState = internalState });

            //this._unitOfWork
            //    .Setup(u => u.Get<IInternalStateBindingRepository>())
            //    .Returns(MockInternalStateBindingRepository.Object);


            return this;
        }

        public ChangeResolutionStatementCSBuilder WithFinalResolution(RegistrationResolution finalResolution)
        {
            //this._unitOfWork.Setup(u => u.Get<RegistrationResolutionRepository>())
            //                .Returns(new MyMockRegistrationResolutionRepository(finalResolution));
            this.mockedRegistrationResolutionRepository
                .Setup(repo => repo.GetByIncoming(It.Is<long>(number => number.Equals(this._incoming.Number))))
                .Returns(new List<RegistrationResolution> { finalResolution });

            this._unitOfWork
                .Setup(u => u.Get<IRegistrationResolutionRepository>())
                .Returns(mockedRegistrationResolutionRepository.Object);

            return this;
        }

        public static implicit operator IncomingPackageBase(ChangeResolutionStatementCSBuilder builder)
        {
            return builder._incoming;
        }

        public static implicit operator StatementBase(ChangeResolutionStatementCSBuilder builder)
        {
            return builder._statement;
        }

        public static implicit operator ClinicalStudyChangeResolutionStatement(ChangeResolutionStatementCSBuilder builder)
        {
            return builder._statement;
        }
    }
}
