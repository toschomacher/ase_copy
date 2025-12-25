using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>else</c> command associated with a preceding <c>if</c> statement.
    /// </summary>
    /// <remarks>
    /// This command is responsible for controlling execution flow when an
    /// <c>if</c> condition evaluates to <c>true</c>. In such cases, the
    /// <c>else</c> block is skipped by advancing the program counter
    /// to the matching <c>end if</c> instruction.
    /// </remarks>
    public class AppElse : Command
    {
        /// <summary>
        /// Gets or sets the program line number corresponding to the matching
        /// <c>end if</c> command.
        /// </summary>
        /// <remarks>
        /// This value must be set by the parser during the compile phase.
        /// </remarks>
        public int EndIfLine { get; set; } = -1;

        /// <summary>
        /// Gets or sets the <see cref="AppIf"/> command that this
        /// <c>else</c> statement is associated with.
        /// </summary>
        /// <remarks>
        /// This reference is established by the parser and is required
        /// to determine whether the <c>else</c> block should execute.
        /// </remarks>
        public AppIf MatchingIf { get; set; }

        /// <summary>
        /// Associates this command with the current stored program.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">Unused parameter string.</param>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        /// <summary>
        /// Performs compile-time processing for the <c>else</c> command.
        /// </summary>
        /// <remarks>
        /// All linking between <c>if</c>, <c>else</c>, and <c>end if</c>
        /// commands is handled by the parser.
        /// </remarks>
        public override void Compile()
        {
            // linking happens in parser
        }

        /// <summary>
        /// Executes the <c>else</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// If the associated <c>if</c> condition evaluated to <c>true</c>,
        /// execution jumps to the matching <c>end if</c> command.
        /// Otherwise, execution continues sequentially into the <c>else</c> block.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when no matching <c>if</c> or <c>end if</c> command is defined.
        /// </exception>
        public override void Execute()
        {
            if (MatchingIf == null)
                throw new StoredProgramException("else without matching if");

            // If the IF was true, skip ELSE body
            if (MatchingIf.LastConditionTrue)
            {
                if (EndIfLine < 0)
                    throw new StoredProgramException("else missing matching end if");

                Program.PC = EndIfLine; // IMPORTANT: -1 due to PC++ after Execute
            }
            // else (IF was false): do nothing → execute else body in sequence
        }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// This command does not accept parameters.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
