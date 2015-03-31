using System;
using System.Runtime.Serialization; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudrocket.Interfaces
{
    interface IOrleansExceptions
    {
    }
    //  GKliot: You just need to define the proper constructors, like this:

    [Serializable]
    public class OrleansException : ApplicationException
    {
        public OrleansException() : base("Unexpected error.") { }

        public OrleansException(string message) : base(message) { }

        public OrleansException(string message, Exception innerException) : base(message, innerException) { }

        protected OrleansException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }


}
