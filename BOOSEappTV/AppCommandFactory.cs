using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Command factory for BOOSEappTV.
    /// Creates command objects but does NOT decide variable reassignment.
    /// That logic belongs in AppParser.
    /// </summary>
    public class AppCommandFactory : CommandFactory
    {
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
