using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public interface Filter { 
        public string Name { get; set; }
        public object Value { get; set; }
    
    }

    public enum PU32Type
    {
        ByDistance = 0, ByDirection = 1, Comparative = 2
    }
    public class FloatFilter : Filter
    {
        public  float Value { get; set; }
        public string Name { get; set; }
        object Filter.Value { get => this.Value; set=> this.Value = float.Parse(((string)value).Replace(".", ",")); }
    }
    public class INTFilter : Filter
    {
        public int Value { get; set; }
        public string Name { get; set; }
        object Filter.Value { get => this.Value; set => this.Value = int.Parse(((string)value)); }
    }

 
    public class DoubleFilter : Filter
    {
        public double Value { get; set; }
        public string Name { get; set; }
        object Filter.Value { get => this.Value; set => this.Value = double.Parse(((string)value).Replace(".", ",")); }
    }

    public class TripTypeEntity { 
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class StringFilter : Filter
    {
        public string Value { get; set; }
        public string Name { get; set; }
        object Filter.Value { get => this.Value; set => this.Value = ((string)value).Replace(".", ","); }
    }

    public class PU32TypeFilter : Filter
    {
        public string Value { get; set; }
        public PU32Type PU32Type {get;set;}
        public string Name { get; set; }
        object Filter.Value
        {
            get => this.Value;
            set
            {
                if (value == null)
                    this.Value = null;
                else
                {

                    this.Value = (string)value;
                    if (this.Value == "по ПЧ")
                        PU32Type = PU32Type.ByDistance;
                    else
                    if (this.Value == "по направлению")
                        PU32Type = PU32Type.ByDirection;
                    else
                    if (this.Value == "cравнительная")
                        PU32Type = PU32Type.Comparative;
                }
            }
        }

    }
    public class TripTypeFilter : Filter
    {
        public string Value { get; set; }
        public TripType TripType { get; set; }
        public string Name { get; set; }
        object Filter.Value
        {
            get => this.Value; 
            set
            {
                if (value == null)
                    this.Value = null;
                else
                {
                    
                    this.Value = (string)value ;
                    if (this.Value == "контрольная")
                        TripType = TripType.Control;
                    else
                    if (this.Value == "рабочая")
                        TripType = TripType.Work;
                    else
                    if (this.Value == "дополнительная")
                        TripType = TripType.Additional;
                }
            }
        }
    }

    public class DateFilter : Filter
    {
        public string Name { get; set; }
        public DateTime DateValue { get; set; }
        public string Value { get; set; }
        object Filter.Value { get => this.Value; 
            set {
                if (value == null)
                    return;
                DateTime date;
                
                if (DateTime.TryParse((string)value, out date)) 
                {
                    this.Value = (string)value;
                    this.DateValue = date;
                } else
                {
                    throw new Exception("Пожайлуста укажите значение в формате даты 'ДД.ММ.ГГГГ'. Например: 01.01.2000");
                }
                
            } 
        }
    }
    public class PeriodFilter : Filter
    {
        public ReportPeriod PeriodValue { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        object Filter.Value { get => this.Value; 
            set {
                if (value == null)
                    return;
                var s = (value as string).Split('-');
                if (s.Count()<2)
                {
                    return;
                }
                int month, year = 0;
                if (!int.TryParse(s[0],out month) || !int.TryParse(s[1], out year))
                    return;
                PeriodValue = new ReportPeriod { PeriodMonth = month, PeriodYear = year };
                Value = PeriodValue.ToString();
            }
        }
        
    }




}
