using Core.BL.Tests.Helpers.IDGenerator;
using Core.BL.Tests.Models.Common;
using Core.DataAcquisition;
using Core.DataAcquisition.Abstract;
using Core.Infrastructure.Context.Abstract;
using Core.Models;
using Core.Models.Common;
using Core.Models.Common.Enums.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

internal partial class Create
{
    private ApprovingSignersContainer _instanceApprovingSignersContainer = null;

    internal ApprovingSignersContainer ApprovingSigners
    {
        get
        {
            if(_instanceApprovingSignersContainer == null)
            {
                _instanceApprovingSignersContainer = new ApprovingSignersContainer(this.mockedUnitOfWork);
            }

            return _instanceApprovingSignersContainer;
        }
    } 
        
}

namespace Core.BL.Tests.Models.Common
{
    internal class ApprovingSignersContainer
    {
        private Mock<ICoreUnitOfWork> mockedUnitOfWork;
        private List<ApprovingSigner> signers;

        public ApprovingSignersContainer(Mock<ICoreUnitOfWork> mockedUnitOfWork)
        {
            this.mockedUnitOfWork = mockedUnitOfWork;
            this.signers = new List<ApprovingSigner>();
        }

        public ApprovingSignerBuilder Performer => new ApprovingSignerBuilder(SignerTypeEnum.performer, OnSignerCreated);
        public ApprovingSignerBuilder Approver => new ApprovingSignerBuilder(SignerTypeEnum.approver_2, OnSignerCreated);
        public ApprovingSignerBuilder Signatory => new ApprovingSignerBuilder(SignerTypeEnum.request_sign, OnSignerCreated);



        private void OnSignerCreated(ApprovingSigner signer)
        {
            if (signer.Id <= 0)
                return;

            if (signers.FirstOrDefault(s => s.Id == signer.Id) != null)
                return;

            this.signers.Add(signer);

            //ApprovingSigner findedSigner = null;
            //var mockedDataAcquisitionOperation = new Mock<IDataAcquisition<Signer, int>>();
            //mockedDataAcquisitionOperation
            //    .Setup(ops => ops.Do(It.IsAny<int>()))
            //    .Callback<int>(id => { findedSigner = signers.FirstOrDefault(s => s.Id == id); })
            //    .Returns(findedSigner);


            //this.mockedUnitOfWork
            //    .Setup(unit => unit.Get<IDataAcquisition<Signer, int>>())
            //    .Returns(mockedDataAcquisitionOperation.Object);

            this.mockedUnitOfWork
                .Setup(unit => unit.Get<GetEntityById<Signer>>())
                .Returns(new MockedGetEntityById(this.mockedUnitOfWork.Object, signers));
        }

        private class MockedGetEntityById : GetEntityById<Signer>
        {
            private readonly IEnumerable<ApprovingSigner> _signers;

            public MockedGetEntityById(ICoreUnitOfWork unitOfWork, IEnumerable<ApprovingSigner> signers) 
                : base(unitOfWork, false) { 
                
                _signers = signers;
            }

            protected override Signer InternalDo(int id)
            {
                var signer = _signers.FirstOrDefault(s => s.Id == id);
                return signer;

                //return this.UnitOfWork
                //    .Get<IDataAcquisition<Signer, int>>()
                //    .Do(id);
            }
        }

    }

    internal class ApprovingSignerBuilder
    {
        private ApprovingSigner signer;
        protected event Action<ApprovingSigner> SignerWasCreatedEvent;
        
        public ApprovingSignerBuilder(SignerTypeEnum signerType, Action<ApprovingSigner> onSignerWasCreated)
        {
            this.signer = new ApprovingSigner
            {
                Id = 0,
                Position = "signer position",
                Approved = false,
                Name = "test signer",
                SignerType = new PersonType
                {
                    Id = (int)signerType,
                    Code = signerType.ToString(),
                },
                UserId = UserIdGenerator.Next(),
            };
            this.SignerWasCreatedEvent += onSignerWasCreated;
        }

        public ApprovingSignerBuilder WithUser(OldUser user)
        {
            this.signer.UserId = user.Id;

            return this;
        }

        //public ApprovingSignerBuilder WithUser(CoreUnitOfWorkUser user)
        //{
        //    this.signer.UserId = user.Id;

        //    return this;
        //}

        public ApprovingSignerBuilder WithId(int id)
        {
            this.signer.Id = id;
            return this; 
        }

        public ApprovingSignerBuilder WithUserId(int id)
        {
            this.signer.UserId = id;
            return this;
        }

        public ApprovingSignerBuilder WithId()
        {
            this.signer.Id = SignerIdGenerator.Next();
            return this;
        }

        public ApprovingSignerBuilder Approved()
        {
            return SetApprovedValue(true);
        }

        public ApprovingSignerBuilder NotApproved()
        {
            return SetApprovedValue(false);
        }

        public ApprovingSignerBuilder SetApprovedValue(bool approved)
        {
            this.signer.Approved = approved;

            return this;
        }

        public ApprovingSigner Please()
        {
            SignerWasCreatedEvent(this.signer);
            return this.signer;
        }


        public static implicit operator ApprovingSigner(ApprovingSignerBuilder builder)
        {
            return builder.Please();
        }
    }
}
