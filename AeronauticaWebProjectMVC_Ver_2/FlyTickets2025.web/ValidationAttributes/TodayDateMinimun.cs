using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.ValidationAttributes
{
    public class TodayDateMinimun : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dataInformada)
            {
                return dataInformada.Date >= DateTime.Today;
            }

            return true; // Not for null values, so we return true to indicate no validation error
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} não pode ser anterior à data de hoje.";
        }

    }
}
