using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEappTV;

namespace BOOSEtests
{
    /// <summary>
    /// Minimal fake canvas implementation for testing.
    /// Drawing behavior is not required for variable tests.
    /// </summary>
    public class FakeCanvas : ICanvas
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }
        public object PenColour { get; set; } = null!;

        public void Circle(int radius) { }
        public void Circle(int radius, bool filled) { }
        public void Clear() { }
        public void DrawTo(int x, int y) { }
        public void MoveTo(int x, int y) { }
        public void Rect(int width, int height) { }
        public void Rect(int width, int height, bool filled) { }
        public void Reset() { }
        public void Set(int width, int height) { }
        public void SetColour(int red, int green, int blue) { }
        public void SetPenColour(int r, int g, int b) { }
        public void Tri(int width, int height) { }
        public void WriteText(string text) { }
        public object getBitmap() { return null; }
    }

    [TestClass]
    public class StoredProgramVariableTests
    {
        private AppStoredProgram program = null!;
        private Parser parser = null!;
        private CommandFactory factory = null!;

        private FakeCanvas canvas = null!;

        [TestInitialize]
        public void Setup()
        {
            canvas = new FakeCanvas();
            program = new AppStoredProgram(canvas);
            factory = new AppCommandFactory();
            parser = new Parser(factory, program);
        }


        [TestMethod]
        public void Parser_Executes_FullProgram_WithWhitespaceSensitiveInput()
        {
            var canvas = new FakeCanvas();
            var program = new AppStoredProgram(canvas);
            var factory = new AppCommandFactory();
            var parser = new Parser(factory, program);

            string commands =
                            @"int radius = 50
                    int width
                    width = 2*radius
                    int height = 100
                    int colour = 255
                    pen colour,0,0
                    moveto 100,100
                    circle radius
                    pen 0,colour,0
                    rect width,height";

            parser.ParseProgram(commands);
            program.Run();

            // Test variable values
            Assert.AreEqual(50, program.GetVariable("radius").Value);
            Assert.AreEqual(100, program.GetVariable("width").Value); // 2*radius
            Assert.AreEqual(100, program.GetVariable("height").Value);
            Assert.AreEqual(255, program.GetVariable("colour").Value);
        }



    }
}
