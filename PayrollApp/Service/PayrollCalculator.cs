namespace PayrollApp.Service
{
    public static class PayrollCalculator
    {
        public static decimal CalculateGrossPay(decimal basicSalary, int totalWorkingDays, int daysPresent)
        {
            if (totalWorkingDays == 0) return 0;
            return Math.Round((basicSalary / totalWorkingDays) * daysPresent, 2);
        }

        public static decimal CalculatePF(decimal basicSalary)
        {
            return Math.Round(basicSalary * 0.12m, 2);
        }

        public static decimal CalculateNetPay(decimal grossPay, decimal pf, decimal professionalTax = 200m)
        {
            return grossPay - pf - professionalTax;
        }
    }
}
