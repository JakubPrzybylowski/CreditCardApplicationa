using Moq;
using System.Collections.Generic;
using Xunit;
using Moq.Protected;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {

        private Mock<IFrequentFlyerNumberValidator> mockValidator;
        private CreditCardApplicationEvaluator sut;

        public CreditCardApplicationEvaluatorShould()
        {
            mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
        
            sut = new CreditCardApplicationEvaluator(mockValidator.Object);
        }


        [Fact]
        public void AcceptHighIncomeApplications()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator =
            //    new Mock<IFrequentFlyerNumberValidator>();

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator =
            //    new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.DefaultValue = DefaultValue.Mock;

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);

        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator =
            //    new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(
            //    x => x.IsValid(It.Is<string>(number => number.StartsWith("y"))))
            //    .Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive))).Returns(true);
            //mockValidator.Setup(
            //    x => x.IsValid(It.IsIn("z","y","x"))).Returns(true);

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator =
        //        new Mock<IFrequentFlyerNumberValidator>();

        //    bool isValid = true;
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));
        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {

            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");
            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpiryString()
        {
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode);
            mockValidator.SetupAllProperties(); // s³u¿y do œledzenia zmien dla wszystkich w³asciwosci obiektu
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };

            sut.Evaluate(application);

            // mockValidator.Verify(X => X.IsValid(It.IsAny<string>()), "Frequent flyer should be validated");
            mockValidator.Verify(X => X.IsValid(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 100_000
            };
            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 99_000
            };
            sut.Evaluate(application);

            mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                Age = 30
            };
            sut.Evaluate(application);

            //mockValidator.VerifySet(x => x.ValidationMode = ValidationMode.Detailed);
            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);
           
            //mockValidator.VerifyNoOtherCalls(); -> ³atwo dziêki temu zweryfikowaæ czy jakieœ inne metody nie wywo³uj¹ siê oprócz tej sprawdzanej
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<System.Exception>();

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookupCount()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true)
                .Raises(x => x.ValidatorLookupPerformed += null, System.EventArgs.Empty);

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "x",
                Age = 25
            };
            sut.Evaluate(application);

            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, System.EventArgs.Empty);

            Assert.Equal(1, sut.ValidatorLookupCount);
        }

        [Fact]
        public void ReferInvaildFrequentFlyerApplications_ReturnValuesSequence()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                .Returns(false)
                .Returns(true);
            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                Age = 25
            };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);
            
            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);

        }

        [Fact]
        public void ReferInvaildFrequentFlyerApplications_MultipleCallsSequence()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                .Returns(false)
                .Returns(true);

            var frequentFlyerNumberPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumberPassed)));

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumberPassed);
        }

        [Fact]
        public void ReferFraudRisk()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            var mockFraudLookup = new Mock<FraudLookup>();
            //mockFraudLookup.Setup(x => x.IsFraudRisk(It.IsAny<CreditCardApplication>()))
            //    .Returns(true);
            mockFraudLookup.Protected()
                .Setup<bool>("CheckApplication", ItExpr.IsAny<CreditCardApplication>())
                .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object, mockFraudLookup.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.RefferedToHumanFraudRisk, decision);
        }

        [Fact]
        public void LinqToMocks()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
            //    .Returns(true);

            IFrequentFlyerNumberValidator mockValidator = Mock.Of<IFrequentFlyerNumberValidator>
                (
                    validator =>
                    validator.ServiceInformation.License.LicenseKey == "OK" &&
                    validator.IsValid(It.IsAny<string>()) == true
                );

            //var sut = new CreditCardApplicationEvaluator(mockValidator);
            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}
