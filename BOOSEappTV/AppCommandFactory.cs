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
            switch (commandType.ToLower().Trim())
            {
                case "int":
                    return new AppInt();

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

                default:
                    // Let BOOSE handle anything we don't override
                    return base.MakeCommand(commandType);
            }
        }
    }
}
