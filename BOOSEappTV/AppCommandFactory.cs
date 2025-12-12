using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEappTV
{
    public class AppCommandFactory : CommandFactory
    {
        /// <inheritdoc/>
        public override ICommand MakeCommand(string commandType)
        {
            AppConsole.WriteLine("My AppCommandFactory method called");
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
                default:
                    try
                    {
                        return base.MakeCommand(commandType); // Call base (CommandFactory) for unknown commands
                    }
                    catch (BOOSE.BOOSEException)
                    {
                        // throw new ArgumentException($"Unknown command type: {commandType}");
                        AppConsole.WriteLine($"Unknown command type: {commandType}");
                        return null;
                    }
            }
        }
    }
}

