using System.Linq;
using System.Reflection;

namespace ALARm.Core.Report
{
    public class DigName
    {
        public string Name { get; set; }
        public int Value { get; set; } = -1;

        public static explicit operator int(DigName dig)
        {
            return dig.Value;
        }
        public static explicit operator string(DigName dig)
        {
            return dig.Name;
        }

        public static explicit operator DigName(string name)
        {

            var properties = typeof(DigressionName).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
            foreach (var property in properties) {
                var dig = (DigName)property.GetValue(null);
               if (dig.Name.Equals(name))
                {
                    return dig;
                }
            }
            return DigressionName.Undefined;

        }

        public static explicit operator DigName(int v)
        {
            var properties = typeof(DigressionName).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
            foreach (var property in properties)
            {
                var dig = (DigName)property.GetValue(null);
                if (dig.Value == v)
                {
                    return dig;
                }
            }
            return DigressionName.Undefined;
        }
        public static bool operator ==(DigName d1, DigName d2)
        {
            return d1.Name.Equals(d2.Name);
        }

        public static bool operator !=(DigName d1, DigName d2)
        {
            return !d1.Name.Equals(d2.Name);
        }
        public bool Equals(DigName other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Name.Equals(other.Name);
                   
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DigName);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
        
    }

}

