using System;
using System.Collections.Generic;
using System.Text;

namespace AlgoTest
{
    class Stock
    {
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public DataRecord[] Data { get; set; }
    }

    class DataRecord
    {
        public string Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
    }
}
