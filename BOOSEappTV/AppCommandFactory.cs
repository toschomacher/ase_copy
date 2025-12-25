using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Provides a command factory for the BOOSEappTV interpreter.
    /// </summary>
    /// <remarks>
    /// This factory is responsible solely for creating command objects
    /// based on the supplied command type. It does not perform any
    /// parsing logic or decide whether a statement represents a
    /// variable reassignment; such decisions are handled by
    /// <see cref="AppParser"/>.
    /// </remarks>
    public class AppCommandFactory : CommandFactory
    {
        /// <summary>
        /// Creates a command instance corresponding to the specified command type.
        /// </summary>
        /// <param name="commandType">
        /// The textual command identifier as parsed from the source program.
        /// </param>
        /// <returns>
        /// An <see cref="ICommand"/> instance corresponding to the command type.
        /// </returns>
        /// <remarks>
        /// If the command type is not recognised by this factory, creation
        /// is delegated to the base BOOSE <see cref="CommandFactory"/>.
        /// </remarks>
        public override ICommand MakeCommand(string commandType)
        {
            // strip BOM if the first token has it
            commandType = commandType.Trim().Trim('\uFEFF');

            switch (commandType.ToLower().Trim())
            {
                case "int":
                    return new AppInt();

                case "real":
                    return new AppReal();

                case "boolean":
                    return new AppBoolean();

                case "array":
                    return new AppArray();

                case "poke":
                    return new AppPoke();

                case "peek":
                    return new AppPeek();

                case "circle":
                    return new AppCircle();

                case "rect":
                    return new AppRect();

                case "moveto":
                    return new AppMoveTo();

                case "drawto":
                    return new AppDrawTo();

                case "pencolour":
                    return new AppPenColour();

                case "write":
                    return new AppWrite();

                case "star":
                    return new AppStar();

                case "method":
                    return new AppMethod();

                case "end":
                    return new AppEndMethod();

                case "call":
                    return new AppCall();

                default:
                    // Let BOOSE handle anything else
                    return base.MakeCommand(commandType);
            }
        }
    }
}
