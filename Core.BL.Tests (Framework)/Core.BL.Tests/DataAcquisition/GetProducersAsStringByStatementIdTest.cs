using Core.BL.Tests.GRLS;
using Core.BL.Tests.GRLS.Producers;
using Core.DataAcquisition;
using Core.Infrastructure.Context.Abstract;
using Core.Repositories.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Core.BL.Tests.DataAcquisition
{
    [TestClass]
    public class GetProducersAsStringByStatementIdTest
    {
        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasNoProducers_ItsEmpty()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers()
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.IsNotNull(result);
            Assert.Equals(result, "");
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasNoProducers_ItsNull()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(null)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(""));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasManyProducersLP_ButNotOneNeeded()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerLP.SomeStage, ProducerLP.SomeStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(""));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasManyProducersFS_ButNotOneNeeded()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerFS.SomeStage, ProducerFS.SomeStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(""));
        }


        #region В каких случаях выводится не пустая строка
        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducerLP_AllStage()
        {
            var getProducersAsStringByStatementId =
                Create.GetProducersAsStringByStatementId
                      .WithProducers(ProducerLP.AllStage)
                      .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.EqualTo(""));
            Assert.That(result, Does.Not.Contain(";\n"));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducerLP_QualityControl()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerLP.QualityControlStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.EqualTo(""));
            Assert.That(result, Does.Not.Contain(";\n"));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducerFS_QualityControl()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerFS.QualityControlStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.EqualTo(""));
            Assert.That(result, Does.Not.Contain(";\n"));
        }
        #endregion

        #region Для нескольких нужных стадий возвращается 2 объединенные строки
        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducersLP_AllAndQualityControl()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerLP.QualityControlStage, ProducerLP.AllStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain(";\n"));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducersFS_AllAndQualityControl()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(ProducerFS.QualityControlStage, ProducerFS.AllStage)
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain(";\n"));
        }
        #endregion

        #region Проверяем формат выводимого сообщения для различного сочитания атрибутов у Producer
        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasNeededProducer_OrgNamesAreSame()
        {
            var getProducersAsStringByStatementId =
                Create.GetProducersAsStringByStatementId
                      .WithProducers(
                            ProducerLP.AllStage
                                      .WithRuOrgName("Test")
                                      .WithEnOrgName("Test")
                                      .WithRuCountryName("РФ")
                                      .WithOrgAddress("Some address")
                       )
                      .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("Test, РФ\nSome address"));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasNeededProducer_OrgNamesAreDifferent()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(
                    ProducerFS.AllStage
                              .WithRuOrgName("Тест")
                              .WithEnOrgName("Test")
                              .WithRuCountryName("РФ")
                              .WithOrgAddress("Some address"))
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("Тест, РФ\nTest, Some address"));
        }

        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasNeededProducer_WithEmptyAddress()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(
                    ProducerFS.AllStage
                              .WithRuOrgName("Тест")
                              .WithEnOrgName("Test")
                              .WithRuCountryName("РФ")
                              .WithOrgAddress(""))
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("Тест, РФ\nTest"));
        }

        [TestMethod]
        public void Test_GetProducerAsStringByStatementId_HasNeededProducer_WithSameOrgNamesAndEmptyAddress()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(
                    ProducerFS.AllStage
                              .WithRuOrgName("Тест")
                              .WithEnOrgName("Тест")
                              .WithRuCountryName("РФ")
                              .WithOrgAddress(""))
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("Тест, РФ"));
        }

        [TestMethod]
        public void Test_GetProducerAsStringByStatementId_HasNeededProducer_WithEmptyEnglishOrgNameAndEmptyAddress()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(
                    ProducerFS.AllStage
                              .WithRuOrgName("Тест")
                              .WithRuCountryName("РФ")
                              .WithEnOrgName("")
                              .WithOrgAddress(""))
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo("Тест, РФ"));
        }

        #endregion

        #region Final test
        [TestMethod]
        public void Test_GetProducersAsStringByStatementId_HasProducersFS_AllAndQualityControlWithAttributes()
        {
            var getProducersAsStringByStatementId = Create.GetProducersAsStringByStatementId
                .WithProducers(
                    ProducerFS.QualityControlStage
                              .WithRuOrgName("Тест")
                              .WithEnOrgName("TestQuality")
                              .WithRuCountryName("РФ")
                              .WithOrgAddress("Some quality address"),

                    ProducerFS.AllStage
                              .WithRuOrgName("Тест")
                              .WithEnOrgName("TestAll")
                              .WithRuCountryName("РФ")
                              .WithOrgAddress("Some all address")
                 )
                .Please();

            var result = getProducersAsStringByStatementId.Do(It.IsAny<int>());

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "Тест, РФ\nTestAll, Some all address;\nТест, РФ\nTestQuality, Some quality address");
        }
        #endregion
    }

    public class GetProducersAsStringByStatementIdBuilder
    {
        private Mock<ICoreUnitOfWork> _unitOfWork;

        public GetProducersAsStringByStatementIdBuilder()
        {
            this._unitOfWork = new Mock<ICoreUnitOfWork>();

            this._unitOfWork.Setup(unit => unit.Get<IMedicamentRegistrationStatementRepository>(It.IsAny<string>()))
                            .Returns(GRLS.Mocks.MedicamentRegistrationStatementRepository);
        }

        public GetProducersAsStringByStatementIdBuilder WithProducers(params Core.Models.Medicaments.Producer[] producers)
        {
            var mockProducerRepo = new Mock<IRegistrationProducerRepository>();
            mockProducerRepo
                .Setup(r => r.FindByStatementId(It.IsAny<int>()))
                .Returns(producers);


            this._unitOfWork.Setup(unit => unit.Get<IRegistrationProducerRepository>(It.IsAny<string>()))
                            .Returns(mockProducerRepo.Object);


            return this;
        }

        public GetProducersAsStringByStatementId Please()
        {
            return new GetProducersAsStringByStatementId(this._unitOfWork.Object);
        }
    }


    public static class ProducerLP
    {
        public static ProducerBuilder AllStage =>
            Create.Producer
                  .WithStage("mnf_all");

        public static ProducerBuilder QualityControlStage =>
            Create.Producer
                  .WithStage("mnf_quality_control");

        public static ProducerBuilder SomeStage =>
            Create.Producer
                  .WithStage("some_stage");
    }

    public static class ProducerFS
    {
        public static ProducerBuilder AllStage =>
            Create.Producer
                  .WithStage("all");

        public static ProducerBuilder QualityControlStage =>
            Create.Producer
                  .WithStage("quality_control");

        public static ProducerBuilder SomeStage =>
            Create.Producer
                  .WithStage("some_stage");
    }
}