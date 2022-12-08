using System.Collections.Generic;

namespace Core.Models
{
    public class PageViewModel<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}