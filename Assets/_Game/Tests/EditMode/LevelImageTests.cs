using NUnit.Framework;
using ReverseSolver.Editing;

namespace ReverseSolver.Core.Tests
{
    /* G1 completion (PRODUCT.md, night of 2026-10-13, A7): crop window, crop
       stored as a look-only field, seam readability score. */
    public class LevelImageTests
    {
        static void Win((float x, float y, float w, float h) a, float x, float y, float w, float h, string what)
        {
            Assert.That(a.x, Is.EqualTo(x).Within(1e-5), what + " x");
            Assert.That(a.y, Is.EqualTo(y).Within(1e-5), what + " y");
            Assert.That(a.w, Is.EqualTo(w).Within(1e-5), what + " w");
            Assert.That(a.h, Is.EqualTo(h).Within(1e-5), what + " h");
        }

        [Test]
        public void CoverCropIsCentred()
        {
            // 1000 x 1000 picture on a 5 x 6 board: keep full height, trim the sides to 5/6
            Win(ImageCrop.None.Window(1000, 1000, 5, 6), (1 - 5f / 6) / 2, 0, 5f / 6, 1, "square on 5x6");
            // 2000 x 1000 picture on a 6 x 6 board: width trimmed to half
            Win(ImageCrop.None.Window(2000, 1000, 6, 6), .25f, 0, .5f, 1, "wide on 6x6");
            // 1000 x 2000 picture on a 6 x 6 board: height trimmed to half
            Win(ImageCrop.None.Window(1000, 2000, 6, 6), 0, .25f, 1, .5f, "tall on 6x6");
        }

        [Test]
        public void ZoomAndPanMoveTheWindowInsideThePicture()
        {
            // square picture, square board: zoom 2 = the middle quarter
            Win(new ImageCrop(2, 0, 0).Window(1000, 1000, 4, 4), .25f, .25f, .5f, .5f, "zoom 2");
            // pan fully right and down: window against the right and the bottom (v = 0) edges
            Win(new ImageCrop(2, 1, 1).Window(1000, 1000, 4, 4), .5f, 0, .5f, .5f, "zoom 2, right-down");
            Win(new ImageCrop(2, -1, -1).Window(1000, 1000, 4, 4), 0, .5f, .5f, .5f, "zoom 2, left-up");
            // without zoom the trimmed sides are still reachable by panning
            Win(new ImageCrop(1, -1, 0).Window(2000, 1000, 6, 6), 0, 0, .5f, 1, "wide, pan left");
        }

        [Test]
        public void CropValuesAreClamped()
        {
            var c = new ImageCrop(9, 3, -3);
            Assert.That((c.Zoom, c.PanX, c.PanY), Is.EqualTo((ImageCrop.MaxZoom, 1f, -1f)));
            Assert.That(new ImageCrop(.2f, 0, 0).Zoom, Is.EqualTo(1f));
            Assert.That(new ImageCrop(1, 0, 0).IsNone, Is.True);
        }

        [Test]
        public void CropIsStoredAndKeepsTheVersion()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(5, 6);
            d.Image = "D01.jpg";
            store.Save(d);
            d.Crop = new ImageCrop(1.5f, -.25f, .5f);
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(1), "the crop is look only");
            var again = DesignStore.Parse(store.ToJson()).Find(d.Id);
            Assert.That(again.Crop, Is.EqualTo(new ImageCrop(1.5f, -.25f, .5f)));
            StringAssert.Contains("\"imageCrop\":{\"zoom\":1.5,\"x\":-0.25,\"y\":0.5}", store.ToJson());
            d.Crop = ImageCrop.None;
            store.Save(d);
            StringAssert.DoesNotContain("imageCrop", store.ToJson(), "no field for the default crop");
            Assert.That(DesignStore.Parse(store.ToJson()).Find(d.Id).Crop.IsNone, Is.True);
        }

        static float[] Fill(int w, int h, System.Func<int, int, float> f)
        {
            var a = new float[w * h];
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) a[y * w + x] = f(x, y);
            return a;
        }

        [Test]
        public void SeamReadabilityFollowsTheLightnessAroundSeams()
        {
            // eased like the shader: l + (0.5 - l) * 0.12
            float white = SeamReadability.Score(Fill(100, 120, (x, y) => 1f), 100, 120, 5, 6);
            float black = SeamReadability.Score(Fill(100, 120, (x, y) => 0f), 100, 120, 5, 6);
            float grey = SeamReadability.Score(Fill(100, 120, (x, y) => .5f), 100, 120, 5, 6);
            Assert.That(white, Is.EqualTo(1 - .94f).Within(1e-4), "white picture: the white rim disappears");
            Assert.That(black, Is.EqualTo(1 - .06f).Within(1e-4));
            Assert.That(grey, Is.EqualTo(.5f).Within(1e-4));
            // only the band around inner seams counts: light cell middles with dark seams read well
            float darkSeams = SeamReadability.Score(Fill(100, 120, (x, y) =>
            {
                float fx = x / 20f, fy = y / 20f;
                bool seam = System.Math.Abs(fx - System.Math.Round(fx)) < .1 || System.Math.Abs(fy - System.Math.Round(fy)) < .1;
                return seam ? 0f : 1f;
            }), 100, 120, 5, 6);
            Assert.That(darkSeams, Is.GreaterThan(.9f));
        }
    }
}
