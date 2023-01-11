using Core.Helpers;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common;
using Core.Models.Documents.Abstract;
using Core.Models.Documents.LimitedPrice;
using Grls.Sync.Tests.Helpers.IDGenerator;
using Moq;
using System.Linq;


namespace Grls.Sync.Tests.Helpers.GRLS.Statements
{
    public class StatementLPRegLimPriceBuidler
    {
        private readonly string _code = StatementTypeList.RegisterLimPrice;

        private Mock<ICoreUnitOfWork> mockedCoreUnitOfWork;

        private IncomingPackageBase _incoming;
        private LimitedPriceStatement _statement;


        public StatementLPRegLimPriceBuidler(Mock<ICoreUnitOfWork> mockedCoreUnitOfWork)
        {
            this.mockedCoreUnitOfWork = mockedCoreUnitOfWork;

            var documentType = DocumentTypes.DocumentTypeList.FirstOrDefault(dt => dt.Code.Equals(this._code));
            if (documentType == null)
            {
                documentType = new DocumentType
                {
                    Id = 4,
                    Code = this._code,
                    FlowId = 3,
                    Flow = new DocumentFlow
                    {
                        Id = 3,
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

        public static implicit operator IncomingPackageBase(StatementLPRegLimPriceBuidler builder)
        {
            return builder._incoming;
        }
        public static implicit operator StatementBase(StatementLPRegLimPriceBuidler builder)
        {
            return builder._statement;
        }
        public static implicit operator LimitedPriceStatement(StatementLPRegLimPriceBuidler builder)
        {
            return builder._statement;
        }
    }
}
