using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class AlertNote
    {
        public string Note { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public int FontStyle { get; set; }
        public bool NotMoving { get; set; } = false;
        public override string ToString()
        {
            return Note;
        }
    }
}
