using BOOSE;

namespace BOOSEappTV
{
    public class AppCommandFactory : CommandFactory
    {
        public override ICommand MakeCommand(string commandType)
        {
            switch (commandType.ToLower().Trim())
            {
                case "star":
                    return new AppStar();
                case "circle":
                    return new AppCircle();
                case "rect":
                    return new AppRect();
                case "moveto":
                    return new AppMoveTo();
                case "pencolour":
                    return new AppPenColour();
                case "write":
                    return new AppWrite();
                case "drawto":
                    return new AppDrawTo();
                case "int":
                    return new AppInt();

                default:
                    try { return base.MakeCommand(commandType); }
                    catch { return null; }
            }
        }
    }
}
