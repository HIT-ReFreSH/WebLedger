using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitReFreSH.WebLedger.Models;

public record ViewTemplate(string Name,string Categories,bool? IsIncome)
{
}