using Core.BL.Tests.GRLS.ApplicantRequests;
using Core.BL.Tests.Helpers.Extensions;
using Core.BL.Tests.Helpers.GRLS.ApplicantRequests;
using Core.BusinessTransactions;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.Common;
using Core.Models.Common.Documents.ApplicantRequest;
using Core.Models.Common.Enums.States;
using Core.Models.Common.Enums.Types;
using Grls.Security.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


internal partial class Create
{
    public GrlsMPApplicantRequestDefectBuilder GrlsMPApplicantRequestDefect =>
        new GrlsMPApplicantRequestDefectBuilder(mockedUnitOfWork);
}


namespace Core.BL.Tests.Helpers.GRLS.ApplicantRequests
{
    public class GrlsMPApplicantRequestDefectBuilder
    {
        private ApplicantRequestDefect applicantRequest;
        public GrlsMPApplicantRequestDefectBuilder(Mock<ICoreUnitOfWork> unitOfWork)
        {
            this.applicantRequest = new ApplicantRequestDefect
            {
                Id = ApplicantRequestsIdGeneator.Next(),
            };
        }

        #region Модельки для JSON
        public class ApplicantRequestModel
        {
            public ApplicantRequestModel()
            {
                this.Signers = new List<ApprovingSignerModel>();
            }

            [JsonProperty("documentId")]
            public long DocumentId { get; set; }

            [JsonProperty("guid")]
            public Guid RoutingGuid { get; set; }

            [JsonProperty("statement")]
            public StatementModel Statement { get; set; }

            [JsonProperty("incoming")]
            public IncomingModel Incoming { get; set; }

            [JsonProperty("outgoingNumber")]
            public string OutgoingNumber { get; set; }

            [JsonProperty("outgoingDate")]
            public string OutgoingDate { get; set; }

            [JsonProperty("transferingDate")]
            public string TransferingDate { get; set; }

            [JsonProperty("organization")]
            public OrganizationModel Organization { get; set; }

            [JsonProperty("request")]
            public string Request { get; set; }

            [JsonProperty("enclosure")]
            public string Enclosure { get; set; }

            [JsonProperty("messageId")]
            public int? MessageId { get; set; }

            [JsonProperty("dossierId")]
            public int DossierId { get; set; }

            [JsonProperty("application")]
            public ApplicationModel Application { get; set; }

            [JsonProperty("tradeName")]
            public string TradeName { get; set; }

            [JsonProperty("inn")]
            public string INN { get; set; }

            [JsonProperty("requestType")]
            public DocumentTypeModel RequestType { get; set; }

            [JsonProperty("documentType")]
            public DocumentTypeModel DocumentType { get; set; }

            [JsonProperty("innerState")]
            public StateModel InnerState { get; set; }

            [JsonProperty("outerState")]
            public StateModel OuterState { get; set; }

            [JsonProperty("author")]
            public AuthorModel Author { get; set; }

            [JsonProperty("signers")]
            public IEnumerable<ApprovingSignerModel> Signers { get; set; }

            [JsonProperty("signatory")]
            public SignatoryModel Signatory { get; set; }

            [JsonProperty("firstSeenDate")]
            public string FirstSeenDate { get; set; }

            [JsonProperty("handledFactDate")]
            public string HandledFactDate { get; set; }
        }
        public class StatementModel
        {
            [JsonProperty("number")]
            public int Number { get; set; }

            [JsonProperty("guid")]
            public Guid Guid { get; set; }

            [JsonProperty("documentType")]
            public DocumentTypeModel DocumentType { get; set; }
        }
        public class IncomingModel
        {
            [JsonProperty("number")]
            public int Number { get; set; }

            [JsonProperty("guid")]
            public Guid RoutingGuid { get; set; }
        }

        public class OrganizationModel
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }

            [JsonProperty("inn")]
            public string INN { get; set; }
        }
        public class ApplicationModel
        {
            [JsonProperty("content")]
            public string Content { get; set; }
        }

        public class DocumentTypeModel
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }
        }

        public class StateModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }
        public class AuthorModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("approved")]
            public bool Approved { get; set; }

            [JsonProperty("signerType")]
            public SignerTypeModel SignerType { get; set; }
        }
        public class ApprovingSignerModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("approved")]
            public bool Approved { get; set; }

            [JsonProperty("position")]
            public string Position { get; set; }

            [JsonProperty("signerType")]
            public SignerTypeModel SignerType { get; set; }
        }
        public class SignatoryModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("position")]
            public string Position { get; set; }
        }

        public class SignerTypeModel
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }
        }
        #endregion


        public GrlsMPApplicantRequestDefectBuilder FromJSONFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                return FromJSON(json);
            }
        }

        public GrlsMPApplicantRequestDefectBuilder InnerState(DocumentInternalStateEnum innerState)
        {
            this.applicantRequest.InternalState = new State
            {
                Id = (int)innerState,
                Code = innerState.ToString(),
            };

            return this;
        }

        public GrlsMPApplicantRequestDefectBuilder WithAuthor(OldUser author)
        {
            this.applicantRequest.CreatedBy = author;
            this.applicantRequest.CreatedById = author.Id;

            return this;
        }

        private GrlsMPApplicantRequestDefectBuilder WithSigner(ApprovingSigner signer)
        {
            if (applicantRequest.Signers == null)
                applicantRequest.Signers = new List<ApprovingSigner>();

            this.applicantRequest.Signers.Concat(new[] { signer });

            return this;
        }

        public GrlsMPApplicantRequestDefectBuilder WithPerformer(ApprovingSigner performer)
        {
            if (applicantRequest.Signers != null && applicantRequest.Signers.Count() > 0)
            {
                var signer = applicantRequest.Signers.GetPerformer();
                signer.Id = performer.Id;
                signer.Approved = performer.Approved;
            }

            return this.WithSigner(performer);
        }

        public GrlsMPApplicantRequestDefectBuilder WithApprover(ApprovingSigner approver)
        {
            if (applicantRequest.Signers != null && applicantRequest.Signers.Count() > 0)
            {
                var signer = applicantRequest.Signers.GetApprover();
                signer.Id = approver.Id;
                signer.Approved = approver.Approved;
            }

            return this.WithSigner(approver);
        }

        public GrlsMPApplicantRequestDefectBuilder WithSignatory(ApprovingSigner signatory)
        {
            this.applicantRequest.Signatory = signatory;
            return this;
        }

        public GrlsMPApplicantRequestDefectBuilder AuthorDidNotApprove()
        {
            this.applicantRequest.IsApprovedByAuthor = false;
            return this;
        }

        public GrlsMPApplicantRequestDefectBuilder AuthorSetApproved()
        {
            this.applicantRequest.IsApprovedByAuthor = true;

            return this;
        }
        public GrlsMPApplicantRequestDefectBuilder PerformerSetApproved()
        {
            applicantRequest.Signers.SetPerformerApprivedValue(true);

            return this;
        }

        public GrlsMPApplicantRequestDefectBuilder FromJSON(string json)
        {
            var applicantRequestFromJson = JsonConvert.DeserializeObject<ApplicantRequestModel>(json);
            this.applicantRequest.DocumentId = applicantRequestFromJson.DocumentId;
            this.applicantRequest.RoutingGuid = applicantRequestFromJson.RoutingGuid;
            this.applicantRequest.RequestDescription = applicantRequestFromJson.Request;
            this.applicantRequest.RequestEnclosures = applicantRequestFromJson.Enclosure;
            this.applicantRequest.IsApprovedByAuthor = applicantRequestFromJson.Author.Approved;
            this.applicantRequest.CreatedById = applicantRequestFromJson.Author.Id;


            this.applicantRequest.TradeName = new Core.Models.Medicaments.TradeDescription
            {
                Name = new Core.Models.Name
                {
                    Russian = applicantRequestFromJson.TradeName,
                    English = applicantRequestFromJson.INN,
                }
            };

            if (!String.IsNullOrWhiteSpace(applicantRequestFromJson.HandledFactDate))
                this.applicantRequest.DtHandedFact = DateTime.Parse(applicantRequestFromJson.HandledFactDate, new CultureInfo("ru-RU"));


            this.applicantRequest.OutgoingRequisites = new Core.Models.OutgoingRequisites
            {
                OutgoingNumber = applicantRequestFromJson.OutgoingNumber,
                OutgoingDate = DateTime.Parse(applicantRequestFromJson.OutgoingDate, new CultureInfo("ru-RU")),
            };

            this.applicantRequest.IncomingPackage = new Core.Models.Documents.Abstract.IncomingPackageBase
            {
                Number = applicantRequestFromJson.Incoming.Number,
                RoutingGuid = applicantRequestFromJson.Incoming.RoutingGuid,
                Document = new Core.Models.Documents.Abstract.StatementBase
                {
                    Id = applicantRequestFromJson.Statement.Number,
                    DocumentType = new DocumentType
                    {
                        Id = applicantRequestFromJson.Statement.DocumentType.Id,
                        Code = applicantRequestFromJson.Statement.DocumentType.Code,
                    }
                }
            };

            this.applicantRequest.DocumentType = new DocumentType
            {
                Id = applicantRequestFromJson.DocumentType.Id,
                Code = applicantRequestFromJson.DocumentType.Code,
            };
            this.applicantRequest.RequestType = new BMCP.Models.ApplicantRequestType
            {
                Id = applicantRequestFromJson.RequestType.Id,
                Code = applicantRequestFromJson.RequestType.Code,
            };

            if (applicantRequestFromJson.Author.Id > 0)
                this.applicantRequest.CreatedBy = new Core.Models.OldUser
                {
                    Id = applicantRequestFromJson.Author.Id,
                };

            this.applicantRequest.Signatory = new Signer
            {
                Id = applicantRequestFromJson.Signatory.Id,
                SignerType = new PersonType
                {
                    Id = (int)SignerTypeEnum.request_sign,
                    Code = SignerTypeEnum.request_sign.ToString(),
                }
            };

            this.applicantRequest.Signers = applicantRequestFromJson.Signers
                .Select(signerModel => new ApprovingSigner
                {
                    Id = signerModel.Id,
                    SignerType = new PersonType
                    {
                        Id = signerModel.SignerType.Id,
                        Code = signerModel.SignerType.Code,
                    }
                })
                .ToList();


            if (applicantRequestFromJson.InnerState != null)
                this.applicantRequest.InternalState = new State
                {
                    Id = applicantRequestFromJson.InnerState.Id,
                    Code = applicantRequestFromJson.InnerState.Code,
                };

            if (applicantRequestFromJson.OuterState != null)
                this.applicantRequest.State = new State
                {
                    Id = applicantRequestFromJson.OuterState.Id,
                    Code = applicantRequestFromJson.OuterState.Code,
                };


            return this;
        }

        public ApplicantRequestDefect Please()
        {
            return this.applicantRequest;
        }

        public static implicit operator ApplicantRequestDefect(GrlsMPApplicantRequestDefectBuilder builder)
        {
            return builder.Please();
        }
    }
}
