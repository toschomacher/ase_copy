namespace BOOSEappTV
{
    /// <summary>
    /// Represents errors that occur during runtime evaluation of expressions.
    /// </summary>
    /// <remarks>
    /// This exception is thrown when an expression cannot be evaluated at runtime,
    /// for example due to invalid syntax, unknown variables, or unsupported operations.
    /// It is primarily used by evaluation-based commands such as <c>write</c>.
    /// </remarks>
    [Serializable]
    internal class EvaluationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EvaluationException"/> class.
        /// </summary>
        public EvaluationException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EvaluationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EvaluationException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EvaluationException"/> class
        /// with a specified error message and a reference to the inner exception
        /// that caused this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// </param>
        public EvaluationException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
