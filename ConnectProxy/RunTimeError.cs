using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy
{
    class RunTimeError
    {
        public string Errordescription
        {
            get ;
            set ;
        }
        public bool IsError => (Errordescription !=null) && (this.Errordescription.Length != 0);

    }
}
