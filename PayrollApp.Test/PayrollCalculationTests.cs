using PayrollApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollApp.Test
{
    public class PayrollCalculationTests
    {
        [Fact]
        public void RaviSharma_CalculationMatchesBrief()
        {
            var gross = PayrollCalculator.CalculateGrossPay(30000, 26, 24);
            var pf = PayrollCalculator.CalculatePF(30000);
            var net = PayrollCalculator.CalculateNetPay(gross, pf);

            Assert.Equal(27692.31m, gross);
            Assert.Equal(3600.00m, pf);
            Assert.Equal(23892.31m, net);
        }

        // Test 2 — full attendance
        [Fact]
        public void FullAttendance_GrossPayEqualsBasicSalary()
        {
            var gross = PayrollCalculator.CalculateGrossPay(25000, 26, 26);

            Assert.Equal(25000.00m, gross);
        }

        // Test 3 — zero days present
        [Fact]
        public void ZeroDaysPresent_GrossPayIsZero()
        {
            var gross = PayrollCalculator.CalculateGrossPay(26000, 26, 0);

            Assert.Equal(0m, gross);
        }

        // Test 4 — divide by zero protection
        [Fact]
        public void ZeroWorkingDays_ReturnsZero()
        {
            var gross = PayrollCalculator.CalculateGrossPay(26000, 0, 0);

            Assert.Equal(0m, gross);
        }

        // Test 5 — PF always 12% of basic
        [Fact]
        public void PF_IsAlwaysTwelvePercentOfBasic()
        {
            var pf = PayrollCalculator.CalculatePF(28000);

            Assert.Equal(3360.00m, pf);
        }
    }
}
