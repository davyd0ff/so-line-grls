using Core.Enums;
using Core.Infrastructure;
using Core.Models.Documents.Abstract;
using Core.Models.Infrastructure;

namespace Core.Models.Documents
{
    /// <summary>
    /// Документ по шаблону
    /// </summary>
    public class RegistrationResolution : TemplatedDocument, IMakeOutNumber
    {
        public int ModuleID { get; set; }

        public int ResolutionTypeId { get; set; }

        public string ResolutionTypeCode { get; set; }

        /// <summary>
        /// Аббревиатура в бд abr
        /// </summary>
        public string Abbreviation { get; set; }

        public virtual string MakeOutNumber(int count = 0, bool isFs = false, string postfix = "")
        {
            var parentType = IncomingPackage?.Document?.DocumentType;
            string flow = string.Empty;
            string suFix = string.Empty;

            switch (IncomingPackage.Flow.Id)
            {
                case (int)DocumentFlowEnum.ПроведениеКлиническихИсследований:
                    flow = "-2"; break;
                case (int)DocumentFlowEnum.РегистрацияЛекарственныхПрепаратовИФармСубстанций:
                    flow = (parentType.Code == StatementTypeList.RegistrationStatement ||
                            parentType.Code == StatementTypeList.EmergencyRegistrationStatement
                    ) ? "-5" : "-6";
                    suFix = ResolutionTypeId == 10000 ? "/ВЗ"
                        : parentType.Code == StatementTypeList.RegistrationStatement ? "/Р"
                        : parentType.Code == StatementTypeList.EmergencyRegistrationStatement ? "/ЧС"
                        : parentType.Code == StatementTypeList.ChangeRegistrationStatement ? "/ИД" : "/П";

                    if (parentType.Code == StatementTypeList.DefectRegStatement)
                    {
                        flow = "-5";
                        suFix = "/Д/Р";

                        switch (ResolutionKindCode)
                        {
                            case ResolutionKindList.resolution_mr_em_q_riskexp_req:
                            case ResolutionKindList.resolution_mr_em_q_riskexp_req_force:
                                postfix = "ЭКфПР";
                                break;

                            case ResolutionKindList.resolution_mr_em_etexp_req:
                            case ResolutionKindList.resolution_mr_em_etexp_req_force:
                                postfix = "ЭТ";
                                break;

                            case ResolutionKindList.resolution_mr_allow_force_req:
                            case ResolutionKindList.resolution_mr_allow_req:
                            case ResolutionKindList.resolution_mr_emergency_em_allow_req:
                                postfix = "ЛП";
                                break;

                            case ResolutionKindList.resolution_mr_emergency_reject_fail_req:
                            case ResolutionKindList.resolution_mr_emergency_reject_fail_force__req:
                                postfix = "О";
                                break;

                            default:
                                postfix = "";
                                break;
                        }
                        
                    }

                    break;
                case (int)DocumentFlowEnum.РегистрацияПредельныхОтпускныхЦен:
                    flow = "-7"; break;
                case (int)DocumentFlowEnum.РегистрацияЛПЕЭК:
                    flow = "-13"; break;
            }

            return $"25{flow}-{IncomingPackage?.Number}{suFix}/{postfix}{(count > 0 ? $"/{count}" : string.Empty)}";
        }
    }
}
