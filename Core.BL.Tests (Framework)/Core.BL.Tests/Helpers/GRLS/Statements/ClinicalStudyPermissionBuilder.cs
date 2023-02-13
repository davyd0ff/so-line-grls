using Core.BL.Tests.Helpers.GRLS.Statements;
using Core.BL.Tests.Helpers.IDGenerator;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.ClinicalStudies;
using Moq;


internal partial class Create
{
    public ClinicalStudyPermissionBuilder StatementPermissionCS =>
        new ClinicalStudyPermissionBuilder(mockedUnitOfWork);
}

namespace Core.BL.Tests.Helpers.GRLS.Statements
{
    public class ClinicalStudyPermissionBuilder
    {
        //private readonly string _code = StatementTypeList.
        private ClinicalStudyPermission _permission;
        private IncomingPackageBase _incoming;

        public ClinicalStudyPermissionBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            var statementDocumentType = new DocumentType
            {
                Id = 100500,
                //Code = this._code,
                Flow = new DocumentFlow
                {
                    Id = 2,
                    Module = Enums.ModuleEnum.grls,
                    Code = "flow_cs"
                }
            };

            var statamenentId = StatementIdGenerator.Next();
            var incomingNumber = IncomingNumberGenerator.Next();

            this._incoming = new IncomingPackageBase
            {
                Number = incomingNumber,
                
            };

            this._permission = new ClinicalStudyPermission
            {
                IncomingPackage = this._incoming,
            };

            
        }

        public ClinicalStudyPermissionBuilder ForStatement(ClinicalStudyStatement statement)
        {
            this._permission.ParentDocument = statement;

            return this;
        }

        public ClinicalStudyPermission Please()
        {
            return this._permission;
        }

        public static implicit operator ClinicalStudyPermission (ClinicalStudyPermissionBuilder builder)
        {
            return builder.Please();
        }
    }
}
