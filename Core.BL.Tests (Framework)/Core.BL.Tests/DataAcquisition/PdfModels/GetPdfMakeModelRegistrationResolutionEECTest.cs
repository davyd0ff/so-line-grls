using Core.BL.Tests.GRLS;
using Core.DataAcquisition.PdfModels;
using Core.Infrastructure;
using Core.Infrastructure.Context.Abstract;
using Core.Models.Common.Abstract;
using Core.Models.Infrastructure;
using Core.Models.ReportServicePdfMake.RegistrationResolution;
using Core.Repositories.Abstract;
using EEC.Models;
using EEC.Models.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Tests.DataAcquisition.PdfModels
{
    [TestClass]
    public class GetPdfMakeModelRegistrationResolutionEECTest
    {
        private readonly Create Create;
        public GetPdfMakeModelRegistrationResolutionEECTest()
        {
            Create = new Create();
        }

        //[TestMethod]
        //public void Test_TempTest()
        //{
        //    var unitOfWork = new Mock<ICoreUnitOfWork>().Object;
        //    var acquisitionOperation = new GetPdfMakeModelRegistrationResolutionEEC(unitOfWork);


        //    Assert.IsNotNull(acquisitionOperation);
        //    Assert.IsTrue(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_change_st_reject_pay
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement_2
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_alignment_statement
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_deny_eec_mp_change_statement
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_alignment_change_reject
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_pr_req
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasCertificateOwnerFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_dec_req
        //        }
        //    ));
        //}


        //[TestMethod]
        //public void Test_TempTest_GetHasRegNumberFlag()
        //{
        //    var unitOfWork = new Mock<ICoreUnitOfWork>().Object;
        //    var acquisitionOperation = new GetPdfMakeModelRegistrationResolutionEEC(unitOfWork);


        //    Assert.IsNotNull(acquisitionOperation);
        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_change_st_reject_pay
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement_2
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_alignment_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_deny_eec_mp_change_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_alignment_change_reject
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_pr_req
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_dec_req
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = true
        //                    }
        //                }
        //            }
        //        }
        //    ));


        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_change_st_reject_pay
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_change_statement_2
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_eec_mp_alignment_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_deny_eec_mp_change_statement
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsTrue(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_alignment_change_reject
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_pr_req
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //    Assert.IsFalse(acquisitionOperation.GetHasRegNumberFlag(
        //        new MedicamentRegistrationResolutionEECModel
        //        {
        //            ResolutionKindCode = ResolutionKindList.resolution_eec_mp_allow_ru_dec_req
        //        },
        //        new RegistrationStatement
        //        {
        //            StatementDetails = new RegStatement
        //            {
        //                Countries = new JsonCountry[]
        //                {
        //                    new JsonCountry
        //                    {
        //                        CountryCode = EECCountries.Russia3,
        //                        IsReferent = false
        //                    }
        //                }
        //            }
        //        }
        //    ));
        //}



        //[TestMethod]
        //public void Test_GetPdfMakeModelRegistrationResolutionEEC_HasCertificateOwner()
        //{
        //    Assert.IsFalse(
        //        Create.GetPdfMakeModelRegistrationResolutionEEC
        //              .WithMrEecResolutionModel(That.HasKind(ResolutionKindList.resolution_eec_mp_change_st_reject_pay))
        //              .PleaseDoWith(StatementTypeList.ResolutionEecMP)
        //              .As<MedicamentRegistrationResolutionEECJsonModel>()
        //              .HasCertificatOwner);

        //    Assert.IsFalse(
        //        Create.GetPdfMakeModelRegistrationResolutionEEC
        //              .WithMrEecResolutionModel(That.HasKind(ResolutionKindList.resolution_eec_mp_eec_mp_change_statement))
        //              .PleaseDoWith(StatementTypeList.ResolutionEecMP)
        //              .As<MedicamentRegistrationResolutionEECJsonModel>()
        //              .HasCertificatOwner);

        //    Assert.IsFalse(
        //        Create.GetPdfMakeModelRegistrationResolutionEEC
        //              .WithMrEecResolutionModel(That.HasKind(ResolutionKindList.resolution_eec_mp_eec_mp_alignment_statement))
        //              .PleaseDoWith(StatementTypeList.ResolutionEecMP)
        //              .As<MedicamentRegistrationResolutionEECJsonModel>()
        //              .HasCertificatOwner);

        //    Assert.IsFalse(
        //        Create.GetPdfMakeModelRegistrationResolutionEEC
        //              .WithMrEecResolutionModel(That.HasKind(ResolutionKindList.resolution_deny_eec_mp_change_statement))
        //              .PleaseDoWith(StatementTypeList.ResolutionEecMP)
        //              .As<MedicamentRegistrationResolutionEECJsonModel>()
        //              .HasCertificatOwner);
        //}


        //[TestMethod]
        //public void Test_GetPdfMakeModelRegistrationResolutionEEC()
        //{
        //    var dataAcquisition = Create.GetPdfMakeModelRegistrationResolutionEEC
        //                                //.WithMrEecResolutionModel(
        //                                //    //MedicamentRegistrationResolutionEECModel
        //                                // )
        //                                .Please();


        //    var model = dataAcquisition.Do(new Core.Models.CommunicationModels.Filters.GetPdfMakeModelFilter
        //    {
        //        DocumentGuid = Guid.NewGuid(),
        //        DocumentTypeCode = StatementTypeList.ResolutionEecMP
        //    }) as MedicamentRegistrationResolutionEECJsonModel;


        //    Assert.IsNotNull(model);
        //    Assert.IsTrue(model.HasRegNumber);
        //    Assert.IsTrue(model.HasCertificatOwner);
        //}


        private static class That
        {
            public static MedicamentRegistrationResolutionEECModelBuilder HasKind(string ResolutionKindCode)
            {
                return new MedicamentRegistrationResolutionEECModelBuilder().HasKind(ResolutionKindCode);
            }
        }
    }




    public class MedicamentRegistrationResolutionEECModelBuilder
    {
        private MedicamentRegistrationResolutionEECModel model;

        public MedicamentRegistrationResolutionEECModelBuilder()
        {
            this.model = new MedicamentRegistrationResolutionEECModel();
        }

        public MedicamentRegistrationResolutionEECModelBuilder HasKind(string resolutionKindCode)
        {
            this.model.ResolutionKindCode = resolutionKindCode;

            return this;
        }

        public static implicit operator MedicamentRegistrationResolutionEECModel(MedicamentRegistrationResolutionEECModelBuilder builder)
        {
            return builder.model;
        }
    }
}


//namespace Core.BL.Tests.GRLS
//{
//    public partial class Create
//    {
//        public GetPdfMakeModelRegistrationResolutionEECBuilder GetPdfMakeModelRegistrationResolutionEEC =>
//            new GetPdfMakeModelRegistrationResolutionEECBuilder(mockedUnitOfWork);
//    }


//    public class GetPdfMakeModelRegistrationResolutionEECBuilder
//    {
//        private Mock<ICoreUnitOfWork> _unitOfWork;

//        public GetPdfMakeModelRegistrationResolutionEECBuilder(Mock<ICoreUnitOfWork> unitOfWork)
//        {
//            this._unitOfWork = unitOfWork;


//            //var MockRoutableRepository = new Mock<IRoutableRepository>();
//            //MockRoutableRepository.Setup(r => r.FindByGuid(It.IsAny<Guid>()))
//            //                      .Returns();


//            //this._unitOfWork.Setup(u => u.Get<IRoutableRepository>())
//        }

//        public GetPdfMakeModelRegistrationResolutionEECBuilder WithMrEecResolutionModel(MedicamentRegistrationResolutionEECModel model)
//        {
//            var MockReportRepository = new Mock<IReportRepository>();
//            MockReportRepository.Setup(r => r.GetReportModelByGuid<MedicamentRegistrationResolutionEECModel>(It.IsAny<Guid>()))
//                                .Returns(model);

//            this._unitOfWork
//                .Setup(u => u.Get<IReportRepository>(
//                                It.Is<string>(documentTypeCode => documentTypeCode == StatementTypeList.ResolutionEecMP)))
//                .Returns(MockReportRepository.Object);

//            return this;
//        }


//        public GetPdfMakeModelRegistrationResolutionEEC Please()
//        {
//            return new GetPdfMakeModelRegistrationResolutionEEC(this._unitOfWork.Object);
//        }

//        public IPdfMakeModelJson PleaseDoWith(string resolutionKind)
//        {
//            return this.Please().Do(new Core.Models.CommunicationModels.Filters.GetPdfMakeModelFilter
//            {
//                DocumentGuid = Guid.NewGuid(),
//                DocumentTypeCode = resolutionKind
//            });
//        }
//    }


//    internal static class IPdfMakeModelJsonExtension
//    {
//        public static T As<T>(this IPdfMakeModelJson model)
//        {
//            return (T)model;
//        }
//    }
//}