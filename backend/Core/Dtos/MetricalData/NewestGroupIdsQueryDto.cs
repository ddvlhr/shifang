using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.MetricalData
{
    public class NewestGroupIdsQueryDto
    {
        public List<string> Machines { get; set; }
        public bool IsMachine { get; set; }
    }
}
