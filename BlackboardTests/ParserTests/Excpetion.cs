using System;
using System.Runtime.Serialization;

namespace BlackboardTests.ParserTests {
    [Serializable]
    internal class Excpetion: Exception {
        public Excpetion() {
        }

        public Excpetion(string message) : base(message) {
        }

        public Excpetion(string message, Exception innerException) : base(message, innerException) {
        }

        protected Excpetion(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}