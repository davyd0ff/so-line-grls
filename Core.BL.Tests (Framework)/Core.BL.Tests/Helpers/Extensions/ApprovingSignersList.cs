using Core.Models.Common.Enums.Types;
using Core.Models.Common;
using System.Collections.Generic;


namespace Core.BL.Tests.Helpers.Extensions
{
    internal static class ApprovingSignersListExtension 
    {
        public static void SetPerformerApprivedValue(this IEnumerable<ApprovingSigner> signers, bool approved)
        {
            foreach (var signer in signers)
                if (signer.SignerType.Id == (int)SignerTypeEnum.performer)
                    signer.Approved = approved;
        }

        public static void SetApproverApprivedValue(this IEnumerable<ApprovingSigner> signers, bool approved)
        {
            foreach (var signer in signers)
                if (signer.SignerType.Id == (int)SignerTypeEnum.approver_2)
                    signer.Approved = approved;
        }
    }
}
