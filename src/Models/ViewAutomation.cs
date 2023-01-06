using HitReFreSH.WebLedger.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitReFreSH.WebLedger.Models;

public record ViewAutomation( LedgerViewAutomationType Type,  string TemplateName)
{
}