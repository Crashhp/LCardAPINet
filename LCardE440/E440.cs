using LCardE440Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCardE440
{
    class E440
    {
        private E440Bridge q;

        public E440()
        {
            q = new E440Bridge();
            var dll = q.DllVersion();
            var w = q.CreateInstance();
        }
    }
}
