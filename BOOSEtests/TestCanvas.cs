using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEappTV;
using BOOSE;
using System.Drawing;

/// <summary>
/// Contains unit tests for the BOOSEappTV application components, specifically validating command execution.
/// </summary>
namespace BOOSEtests
{
    /// <summary>
    /// A test class containing unit tests for core drawing commands and program execution within <see cref="AppCanvas"/>.
    /// </summary>
    [TestClass]
    public sealed class AppCommandsTests
    {
        /// <summary>
        /// Tests that the <see cref="AppCanvas.MoveTo(int, int)"/> command correctly updates the X and Y cursor positions.
        /// </summary>
        [TestMethod]
        public void Execute_MoveTo_UpdatesXandYpositions()
        {
            // Arrange
            int setX = 50;
            int setY = 60;

            // Initialise a test canvas instance
            AppCanvas testCanvas = new AppCanvas(748, 500);

            // Act
            testCanvas.MoveTo(setX, setY);
            int finalX = testCanvas.Xpos;
            int finalY = testCanvas.Ypos;

            // Assert
            Assert.AreEqual(setX, testCanvas.Xpos, 0, "X position not correct");
            Assert.AreEqual(setY, testCanvas.Ypos, 0, "Y position not correct");
        }

        /// <summary>
        /// Tests that the <see cref="AppCanvas.DrawTo(int, int)"/> command correctly updates the X and Y cursor positions after drawing a line.
        /// </summary>
        [TestMethod]
        public void Execute_DrawTo_UpdatesXandYpositions()
        {
            // Arrange
            int setX = 150;
            int setY = 160;

            // Initialise a test canvas instance
            AppCanvas testCanvas = new AppCanvas(748, 500);

            // Act
            testCanvas.DrawTo(setX, setY);
            int finalX = testCanvas.Xpos;
            int finalY = testCanvas.Ypos;

            // Assert
            Assert.AreEqual(setX, testCanvas.Xpos, 0, "X position not correct");
            Assert.AreEqual(setY, testCanvas.Ypos, 0, "Y position not correct");
        }

        /// <summary>
        /// Tests the execution of a multi-line program script, ensuring the final X and Y positions of the cursor are correct.
        /// This test validates the sequence of commands (moveto, circle, rect) processed by the parser and stored program.
        /// </summary>
        [TestMethod]
        public void Execute_MultilineProgram_CheckXYpositionsAtTheEnd()
        {
            // Arrange
            int expectedX = 140;
            int expectedY = 200;
            AppCanvas testCanvas = new AppCanvas(748, 500);
            CommandFactory commandFactory = new AppCommandFactory();
            StoredProgram storedProgram = new StoredProgram(testCanvas);
            IParser parser = new Parser(commandFactory, storedProgram);

            String commands = "moveto 100,100\ncircle 40\nmoveto 140,200\nrect 60,80";

            // Act
            parser.ParseProgram(commands);
            storedProgram.Run();

            // Assert
            Assert.AreEqual(expectedX, testCanvas.Xpos, 0, "X position not correct");
            Assert.AreEqual(expectedY, testCanvas.Ypos, 0, "Y position not correct");
        }
    }
}