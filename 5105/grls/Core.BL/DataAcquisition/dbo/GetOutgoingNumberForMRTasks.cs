using Core.DataAcquisition.Abstract;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Documents;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.DataAcquisition.dbo
{
    public class GetOutgoingNumberForMRTasks : DataAcquisitionOperationBase<string, RegistrationResolution, DocumentTemplate>
    {
        public GetOutgoingNumberForMRTasks(ICoreUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected override string InternalDo(RegistrationResolution resolution, DocumentTemplate taskTemplate)
        {
            if(resolution.IncomingPackage.Document.DocumentType.Code == StatementTypeList.DefectRegStatement)
            {
                //int count = new GetDocumentCount<ResolutionBaseLong>(UnitOfWork)
                //    .Do(resolution.IncomingPackage.Number, taskTemplate.ResolutionTypeId);

                string count = "";
                var regex = new Regex(@".+(\/\d+)$");
                var matches = regex.Matches(resolution.OutgoingRequisites.OutgoingNumber);
                if(matches.Count > 0)
                {
                    count = matches[0].Groups[1].Value;
                }

                return $"25-5-{resolution.IncomingPackage?.Number}/Д/ЗД/{taskTemplate.PostFix}{count}";
            }

            return String.Format("{0}-З", resolution.OutgoingRequisites.OutgoingNumber);
        }
    }
}
