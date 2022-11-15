using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuYang.WebService
{
    public class Rule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Standard { get; set; }
        public string Upper { get; set; }
        public string Lower { get; set; }

        /// <summary>
        /// 是否包含等于上下限值
        /// </summary>
        public bool Equal { get; set; } = true;
        public string Points { get; set; }
        public string Deduction { get; set; }
    }
}