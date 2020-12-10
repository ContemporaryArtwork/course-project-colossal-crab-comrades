using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ColossalGame.Models.Exceptions
{
    
    public class GameLogicExceptions
    {
        
    }

    [Serializable()]
    public class UnspawnedException : System.Exception
    {
        public UnspawnedException() : base() { }
        public UnspawnedException(string message) : base(message) { }
        public UnspawnedException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected UnspawnedException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
