using System;
using System.Globalization;

namespace ALARm.Core
{
    public class ReportPeriod
    {
        public int Id { get; set; }
        public int PeriodYear { get; set;}
        public int PeriodMonth { get; set;}
        public int PeriodDay { get; set; }
        public string Period => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(PeriodMonth) + ' ' + PeriodYear.ToString();
        public string PeriodMonthMonth => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(PeriodMonth);
        public DateTime StartDate => new DateTime(PeriodYear, PeriodMonth, 1);
        public DateTime FinishDate => StartDate.AddMonths(1).AddDays(-1);
        public override string ToString()
        {
            return $"{(PeriodMonth < 10 ? "0":"")}{PeriodMonth}-{PeriodYear}";
        }

    }
}