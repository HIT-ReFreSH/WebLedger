using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitReFreSH.WebLedger.Models;

public record ViewQueryResult(List<Entry> Raw, List<(string, decimal)> ByCategory, List<(string, decimal)> ByTime)
{
}