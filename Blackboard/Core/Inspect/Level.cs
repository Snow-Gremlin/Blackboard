namespace Blackboard.Core.Inspect {

    /// <summary>The level for messages being logged at.</summary>
    public enum Level {

        /// <summary>
        /// This level is for not important messages used to indicate some
        /// state has been reached or some value has been set.
        /// </summary>
        Info = 0,

        /// <summary>
        /// This level is for messages used to indicate some
        /// special state has been reached or some specific value has been set.
        /// These are something to bring attention to but not really important.
        /// </summary>
        Notice = 1,

        /// <summary>
        /// This level is for important messaged to indicate problems which
        /// should be fixed but the problem doesn't stop the current process.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// This level is for very important messages which must be fixed and
        /// caused the current process to stop such as an exception.
        /// </summary>
        Error = 3,
    }
}
