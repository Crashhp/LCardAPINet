﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCardE440
{
    class Program
    {
        static void Main(string[] args)
        {
            E440 card = new E440(10, new TimeSpan(0, 2, 0));
            card.ReadData();
        }
    }
}
