using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public class FrequentFlyerNumberValidatorService : IFrequentFlyerNumberValidator
    {
        public IServiceInformation ServiceInformation => throw new NotImplementedException();

        public ValidationMode ValidationMode { 
            get => throw new NotImplementedException("For demo purposes"); 
            set => throw new NotImplementedException("For demo purposes"); }

        public event EventHandler ValidatorLookupPerformed;

        //

        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException();
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException();
        }
    }
}
