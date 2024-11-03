using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HitRefresh.WebLedger;

[Serializable]
public class ViewUndefinedException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public ViewUndefinedException()
    {
    }

    public ViewUndefinedException(string message) : base(message)
    {
    }

    public ViewUndefinedException(string message, Exception inner) : base(message, inner)
    {
    }

}