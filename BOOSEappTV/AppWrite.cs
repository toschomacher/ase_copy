using System;
using BOOSE;

namespace BOOSEappTV
{
    internal class AppWrite : Evaluation
    {
        private AppCanvas canvas;
        private string message;

        public AppWrite() : base() {}

        // store canvas locally and pass it to base if needed by the framework
        public AppWrite(AppCanvas c, string message) : base()
        {
            this.canvas = c;
            this.message = message;
        }

        public override void CheckParameters(string[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new EvaluationException("AppWrite requires exactly one parameter: the message to write.");
            }
        }

        public override void Execute()
        {
            base.Execute();
            this.canvas = AppCanvas.GetCanvas();
            // read runtime parameters from base if available (most frameworks set Parameters)
            if (Parameters != null && Parameters.Length > 0)
                message = Parameters[0];

            if (string.IsNullOrEmpty(message))
                throw new EvaluationException("No message provided to write.");

            var token = message.Trim();

            if (token.Equals("circle", StringComparison.OrdinalIgnoreCase))
            {
                // canvas may be typed to BOOSE.Canvas; cast to our concrete AppCanvas to read last circle radius
                if (canvas is BOOSEappTV.AppCanvas appCanvas)
                {
                    if (appCanvas.HasLastCircle)
                    {
                        AppConsole.WriteLine(appCanvas.LastCircleRadius.ToString());
                        // optionally also draw text on canvas:
                        // appCanvas.WriteText(appCanvas.LastCircleRadius.ToString());
                        return;
                    }
                    else
                    {
                        AppConsole.WriteLine("No previously drawn circle found to write about.");
                        //throw new EvaluationException("No previously drawn circle found to write about.");
                    }
                }
                else
                {
                    throw new EvaluationException("Canvas implementation does not support querying last circle radius.");
                }
            } else if (token.Equals("rect", StringComparison.OrdinalIgnoreCase))
            {
                // canvas may be typed to BOOSE.Canvas; cast to our concrete AppCanvas to read last circle radius
                if (canvas is BOOSEappTV.AppCanvas appCanvas)
                {
                    if (appCanvas.HasLastRect)
                    {
                        AppConsole.WriteLine(appCanvas.LastRectParameters);
                        // optionally also draw text on canvas:
                        // appCanvas.WriteText(appCanvas.LastCircleRadius.ToString());
                        return;
                    }
                    else
                    {
                        AppConsole.WriteLine("No previously drawn rectangle found to write about.");
                        //throw new EvaluationException("No previously drawn circle found to write about.");
                    }
                }
                else
                {
                    throw new EvaluationException("Canvas implementation does not support querying last rectangle sides.");
                }
            }

                // default: print the literal message
                AppConsole.WriteLine(message);
        }
    }
}
