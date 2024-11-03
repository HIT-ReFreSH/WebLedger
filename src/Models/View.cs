using HitRefresh.WebLedger.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitRefresh.WebLedger.Models;

public record View(string Name,DateTime StartTime ,DateTime EndTime ,string TemplateName)
{
}