namespace TwoWheels.Functions.Domains.Entities
{
    public class RentalPlan
    {
        public int Days { get; set; }
        public decimal DailyRate { get; set; }
        public decimal EarlyReturnPenaltyPercentage { get; set; }
        public decimal LateReturnDailyFee { get; set; } = 50.00m;

        //Não foi definido a porcentagem de multa para os restantes dos dias, somente para os de 7 e 15 dias
        //Por isso, utilizei o valor de 40% para as demais opções
        public static readonly RentalPlan[] AvailablePlans =
        [
            new() { Days = 7, DailyRate = 30.00m, EarlyReturnPenaltyPercentage = 0.20m },
            new() { Days = 15, DailyRate = 28.00m, EarlyReturnPenaltyPercentage = 0.40m },
            new() { Days = 30, DailyRate = 22.00m, EarlyReturnPenaltyPercentage = 0.40m },
            new() { Days = 45, DailyRate = 20.00m, EarlyReturnPenaltyPercentage = 0.40m },
            new() { Days = 50, DailyRate = 18.00m, EarlyReturnPenaltyPercentage = 0.40m }
        ];
    }
}
