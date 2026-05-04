namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of a shift assignment.
    /// </summary>
    public enum ShiftAssignmentStatus
    {
        /// <summary>
        /// Assignment is active and waiting for worker check-in.
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Worker has checked in for the assigned shift.
        /// </summary>
        CheckedIn = 20,

        /// <summary>
        /// Worker has checked out and assignment is closed.
        /// </summary>
        CheckedOut = 30,
    }
}
