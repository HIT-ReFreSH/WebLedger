using System.Runtime.Serialization;

namespace HitRefresh.WebLedger;

[Serializable]
public class TypeUndefinedException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public TypeUndefinedException()
    {
    }

    public TypeUndefinedException(string message) : base(message)
    {
    }

    public TypeUndefinedException(string message, Exception inner) : base(message, inner)
    {
    }

    protected TypeUndefinedException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}