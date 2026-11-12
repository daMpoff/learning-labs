using System;
using System.Runtime.Serialization;

namespace OOP_2_1
{
    [Serializable]
    public class WeightException : Exception
    {
        private Cargo cargo;
        public Cargo Cargo
        {
            get
            {
                return cargo;
            }
        }
        public WeightException()
        {

        }
        public WeightException(string message, Cargo cargo)
            : base(message)
        {
            this.cargo = cargo;
        }
        public WeightException(string message, Exception inner)
            : base(message, inner)
        {

        }
        public WeightException(SerializationInfo info, StreamingContext context)
             : base(info, context)
        {

        }
    }
}
