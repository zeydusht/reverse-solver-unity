using System.IO;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_116: &renk=canli the pieces in the picture's own colours (no eased contrast, gradient or highlight; the bevel,
       the kit's rim, the shadow and the badges stay); &renk=yavas the game as it is; both: the pieces' look comes over the
       whole pull back instead of its last fifth. No flag: PM_115's. */
    public class Pm116Tests
    {
        [Test]
        public void TheFlagsAndWhatTheyChange()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            string shader = File.ReadAllText("Assets/_Game/Presentation/Shaders/Piece.shader");
            StringAssert.Contains("Dive75.Color116 = renk == \"yavas\" ? 'y' : renk == \"0\" || renk == \"simdiki\" ? '0' : 'c';", root);   // canli the default
            StringAssert.Contains("Shader.SetGlobalFloat(\"_PieceVivid\", Dive75.Color116 == 'c' ? 1 : 0);", root);
            // the teal chain's light rim, only with the picture's own colours
            string chain = File.ReadAllText("Assets/_Game/Presentation/Board/ChainView.cs");
            StringAssert.Contains("bool rim = colour == \"turkuaz\" && Dive75.Color116 == 'c';", chain);
            Assert.That(File.Exists("Assets/_Game/Presentation/Resources/Shaders/ChainRim.shader"));
            StringAssert.Contains("public static char Color116 = '0';", d);
            StringAssert.Contains("_look = Dive75.Color116 != '0' ? sm(back) : sm((back - Dive75.LinesEnd) / Mathf.Max(.05f, 1 - Dive75.LinesEnd));", d);
            // canli: the eased contrast, the gradient and the highlight only without it; the bevel and the kit's rim as before
            StringAssert.Contains("float3 imgFill = img;", shader);
            StringAssert.Contains("if (_PieceVivid < 0.5)", shader);
            StringAssert.Contains("float3 pf = fill + bev * 0.16 * look;", shader);
            Assert.That(shader.IndexOf("if (_PieceVivid < 0.5)"), Is.LessThan(shader.IndexOf("img = lerp(img, float3(0.5, 0.5, 0.5), 0.12);")));
        }
    }
}
