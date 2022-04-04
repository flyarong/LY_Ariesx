using System;
using System.Runtime.Serialization;

namespace Poukoute {

    /// <summary>
    /// The exception that is thrown when a instance is null;
    /// </summary>
    public class PONullException : Exception {
        /// <summary>
        /// Initializes a new instance of the PONullException class.
        /// </summary>
        public PONullException() : base() { }

        /// <summary>
        /// Initializes a new instance of the PONullException class with its message.
        /// </summary>
        public PONullException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the PONullException class with its message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public PONullException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the PONullException class with the specified
        /// serialization and context information.
        /// </summary>
        protected PONullException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// The exception that is thrown when a instance is not initialized;
    /// </summary>
    public class POUninitializedException : Exception {
        /// <summary>
        /// Initializes a new instance of the POUninitializedException class.
        /// </summary>
        public POUninitializedException() : base() { }

        /// <summary>
        /// Initializes a new instance of the POUninitializedException class with its message.
        /// </summary>
        public POUninitializedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the POUninitializedException class with its message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public POUninitializedException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the POUninitializedException class with the specified
        /// serialization and context information.
        /// </summary>
        protected POUninitializedException(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }
    }

    public class PONetException : Exception {
        /// <summary>
        /// Initializes a new instance of the PONetException class.
        /// </summary>
        public PONetException() : base() { }

        /// <summary>
        /// Initializes a new instance of the PONetException class with its message.
        /// </summary>
        public PONetException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the PONetException class with its message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public PONetException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the PONetException class with the specified
        /// serialization and context information.
        /// </summary>
        protected PONetException(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }
    }
}
