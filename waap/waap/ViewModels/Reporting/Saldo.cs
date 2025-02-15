using System.ComponentModel.DataAnnotations;

namespace waap.ViewModels.Reporting
{
    public class SaldoViewModel
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Final value must be greater than 0.")]
        public decimal TotalPaidValue { get; set; }
        public decimal TotalSalesValue { get; set; }
        public decimal TotalNonPaidValue { get; set; }

    }
}
