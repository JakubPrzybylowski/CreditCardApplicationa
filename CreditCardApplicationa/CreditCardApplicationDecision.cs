using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public enum CreditCardApplicationDecision
    {
        Unknow,
        AutoAccepted,
        AutoDeclined,
        ReferredToHuman,
        RefferedToHumanFraudRisk
    }
}
