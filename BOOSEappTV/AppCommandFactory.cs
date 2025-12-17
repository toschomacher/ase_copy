using BOOSE;
using BOOSEappTV;

public class AppCommandFactory : CommandFactory
{
    public override ICommand MakeCommand(string commandType)
    {
        // 🔑 ASSIGNMENT DETECTION
        if (commandType.Contains("="))
        {
            return new AppAssign();
        }

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

            case "write":
                return new AppWrite();

            case "pencolour":
                return new AppPenColour();

            default:
                return base.MakeCommand(commandType);
        }
    }

}
