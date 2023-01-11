using Core.BL.Tests.Helpers.IDGenerator;
using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.LimitedPrice;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.Helpers.GRLS.Statements
{
    public class StatementRegLimPriceBuilder
    {
        private readonly string _code = StatementTypeList.RegisterLimPrice;

        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

        private IncomingPackageBase _incoming;
        private LimitedPriceStatement _statement;

        public StatementRegLimPriceBuilder(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;

            var documentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if(documentType == null)
            {
                documentType = new DocumentType
                {
                    Id = 4,
                    Code = this._code,
                    FlowId = 3,
                    Flow = new DocumentFlow
                    {
                        Id = 3,
                        Module = Enums.ModuleEnum.grls,
                        Code = "flow_lom_price"
                    }
                };
                DocumentTypes.DocumentTypeList.Add(documentType);
            }



            var statementId = StatementIdGenerator.Next();
            var incomingNumber = IncomingNumberGenerator.Next();


            this._statement = new LimitedPriceStatement
            {
                Id = statementId,
                DocumentType = documentType,
            };

            this._incoming = new IncomingPackageBase
            {
                ParentDocument = this._statement,
                Document = this._statement,
                Number = incomingNumber,
            };
        }

        public LimitedPriceStatement Please()
        {
            return this._statement;
        }

        public static implicit operator IncomingPackageBase(StatementRegLimPriceBuilder builder)
        {
            return builder._incoming;
        }
        public static implicit operator StatementBase(StatementRegLimPriceBuilder builder)
        {
            return builder._statement;
        }
        public static implicit operator LimitedPriceStatement(StatementRegLimPriceBuilder builder)
        {
            return builder._statement;
        }

    }
}
