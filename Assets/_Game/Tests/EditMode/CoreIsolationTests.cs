using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* The headless solver and Monte Carlo runs depend on Core never touching
       the engine. noEngineReferences enforces it at compile time; this test
       guards against someone flipping that flag. */
    public class CoreIsolationTests
    {
        [Test]
        public void CoreDoesNotReferenceUnity()
        {
            var core = Assembly.Load("ReverseSolver.Core");
            var unityRefs = core.GetReferencedAssemblies()
                .Select(a => a.Name)
                .Where(n => n.StartsWith("UnityEngine") || n.StartsWith("UnityEditor") || n.StartsWith("Unity."))
                .ToArray();
            Assert.That(unityRefs, Is.Empty);
        }
    }
}
