using System.Collections.Generic;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* The 40-slot order of the redesigned set (PM 2026-10-13 ~18:40, PRODUCT.md
       "Sıra planı"). T Tanıtım, N Nefes, K Kolay, O Orta, Z Zor, ÇZ Çok zor;
       the new thing in brackets.
         1T 2K 3T(çivi) 4K 5T(makas) 6O 7T(mühür) 8K 9T(değnek) 10Z
         11N 12T(zincir) 13O 14Z 15T(saat) 16O 17Z 18N 19Z 20T(bomba)
         21K 22Z 23T(çekiç) 24O 25Z 26N 27O 28Z 29N 30ÇZ
         31N 32Z 33O 34ÇZ 35N 36Z 37O 38Z 39N 40ÇZ */
    public static class FactoryPlan
    {
        static readonly (Genre g, string thing)[] Order =
        {
            (Genre.Tanitim, null), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Nail), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Scissors),
            (Genre.Orta, null), (Genre.Tanitim, SlotSpec.Seal), (Genre.Kolay, null), (Genre.Tanitim, SlotSpec.Wand), (Genre.Zor, null),
            (Genre.Nefes, null), (Genre.Tanitim, SlotSpec.Chain), (Genre.Orta, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Clock),
            (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Bomb),
            (Genre.Kolay, null), (Genre.Zor, null), (Genre.Tanitim, SlotSpec.Hammer), (Genre.Orta, null), (Genre.Zor, null),
            (Genre.Nefes, null), (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.CokZor, null),
            (Genre.Nefes, null), (Genre.Zor, null), (Genre.Orta, null), (Genre.CokZor, null), (Genre.Nefes, null),
            (Genre.Zor, null), (Genre.Orta, null), (Genre.Zor, null), (Genre.Nefes, null), (Genre.CokZor, null),
        };

        public const int Slots = 40;

        public static SlotSpec Slot(int slot) => SlotSpec.For(slot, Order[slot - 1].g, Order[slot - 1].thing);

        public static List<SlotSpec> All()
        {
            var list = new List<SlotSpec>(Slots);
            for (int i = 1; i <= Slots; i++) list.Add(Slot(i));
            return list;
        }
    }
}
