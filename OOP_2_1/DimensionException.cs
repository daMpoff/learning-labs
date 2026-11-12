using System;
using System.Runtime.Serialization;

namespace OOP_2_1
{
    [Serializable]
    public class DimensionException : Exception
    {
        private Cargo cargo;
        public Cargo Cargo
        {
            get
            {
                return cargo;
            }
        }
        public DimensionException()
            : base()
        {

        }
        public DimensionException(string message)
            : base(message)
        {

        }
        public DimensionException(string message, Cargo cargo)
            : base(message)
        {
            this.cargo = cargo;
        }
        public DimensionException(string message, Exception inner)
            : base(message, inner)
        {

        }
        public DimensionException(SerializationInfo info, StreamingContext context)
             : base(info, context)
        {

        }
    }
}
