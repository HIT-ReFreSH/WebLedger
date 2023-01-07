using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitReFreSH.WebLedger.Models;

public record ViewQueryResult(List<Entry> Raw, 
    Dictionary<string, decimal> ByCategory,
    Dictionary<string, decimal> ByTime)
{
}