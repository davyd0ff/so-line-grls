using Core.Models.Common.Documents.ApplicantRequest;
using System.Linq;
using Core.Models.Common;

namespace Core.BL.Tests.Helpers.DeepClone
{
    internal static class Cloner
    {
        public static ApplicantRequestDefect DeepCopy(ApplicantRequestDefect applicantRequest)
        {
            var clone = ApplicantRequestDefect.From(applicantRequest);
            clone.Signers = applicantRequest.Signers
                .Select(signer => new Core.Models.Common.ApprovingSigner(signer, signer.Approved))
                .ToList();

            clone.Signatory = new Signer
            {
                Id = applicantRequest.Signatory.Id,
                ActualNow = applicantRequest.Signatory.ActualNow,
                Position = applicantRequest.Signatory?.Position ?? "",
                Code = applicantRequest.Signatory?.Code ?? "",
                Name = applicantRequest.Signatory?.Name ?? "",
                SignerType = new PersonType
                {
                    Id = applicantRequest.Signatory.SignerType.Id,
                    Code = applicantRequest.Signatory.SignerType.Code,
                },
                Departments = applicantRequest.Signatory?.Departments?.Select(department => new Department
                {
                    Id = department.Id,
                    Code = department.Code,
                    Name = department.Name,
                    ShortName = department.ShortName,
                    Prefix = department.Prefix,
                })?.ToList() ?? new System.Collections.Generic.List<Department>(),

            };

            return clone;
        }
    }
}
