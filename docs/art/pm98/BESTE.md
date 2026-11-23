# PM_98: kendi bestelerimiz (bölüm müziği A–D), nota planı

Araç: `Tools/make_level98.py`. Hepsi 4/4, 40 ölçü: A (16 ölçü: 8 ölçülük motif iki kez) + B (16: cevap, davul daha yoğun, tef) + A′ (8). Döngü yuvarlak çizilir: son ölçünün kuyrukları başa eklenir, dikiş yok.

## A: Marimba Pop

- 136 BPM, ton C majör; davul kalıbı `four`; motif marimba (+ bir oktav üstte glock), akor harp (2. ve 4. vuruşun arkası), bas bizim sentezimiz (sekizlik).
- Davul (16’lık adımlar, x vurgu, o hafif): kick `x...x...x...x...`, snare `....x.......x...`, clap `....x.......x...`, hat `x.o.x.o.x.o.x.o.`, ohat `..x...x...x...x.`, shaker `oxooxoooxooooxoo`, tamb `................`; B’de tef her arka vuruşta, 2 ölçüde bir 4. vuruş arkasında ek bas davul; her 8 ölçünün son vuruşunda trampet geçişi.
- Akorlar: A I – V – vi – IV, B IV – V – iii – vi (ölçü başına bir akor).
- Motif (vuruş, derece, uzunluk; derece 0 = ton kökü, 7 = bir oktav üstü): `[(0, 4, 0.5), (0.5, 4, 0.5), (1, 5, 0.5), (1.5, 4, 0.5), (2, 2, 1), (3.5, 0, 0.5), (4, 2, 0.5), (4.5, 4, 1.5), (6.5, 5, 0.5), (7, 4, 1), (8, 4, 0.5), (8.5, 5, 0.5), (9, 7, 1), (10.5, 5, 0.5), (11, 4, 1), (12, 2, 0.5), (12.5, 1, 0.5), (13, 2, 2), (16, 4, 0.5), (16.5, 4, 0.5), (17, 5, 0.5), (17.5, 4, 0.5), (18, 2, 1), (19.5, 0, 0.5), (20, 2, 0.5), (20.5, 4, 1.5), (22.5, 5, 0.5), (23, 7, 1), (24, 6, 0.5), (24.5, 5, 0.5), (25, 4, 0.5), (25.5, 2, 0.5), (26, 1, 1), (27.5, 2, 0.5), (28, 0, 2)]`
- Cevap (B): `[(0, 7, 1), (1, 9, 0.5), (1.5, 7, 0.5), (2, 5, 1), (3, 4, 1), (4, 5, 0.5), (4.5, 4, 0.5), (5, 2, 1), (6, 4, 2), (8, 7, 1), (9, 9, 0.5), (9.5, 11, 0.5), (10, 9, 1), (11, 7, 1), (12, 5, 0.5), (12.5, 4, 0.5), (13, 5, 1), (14, 4, 2)]`
- Ölçüm: {'start': 0.02505668934240363, 'length': 70.58823129251701, 'bpm': 136, 'lufs': np.float64(-18.0), 'perc': 0.8, 'name': 'Marimba Pop', 'centroid': 1852, 'beat_bpm': 136.0, 'beat_jitter_ms': 0.0}

## B: Ksilofon Arcade

- 138 BPM, ton F majör; davul kalıbı `break`; motif xylo (+ bir oktav üstte glock), akor marimba (2. ve 4. vuruşun arkası), bas bizim sentezimiz (sekizlik).
- Davul (16’lık adımlar, x vurgu, o hafif): kick `x.....x...x.....`, snare `....x.......x..o`, clap `....x.......x...`, hat `xoxoxoxoxoxoxoxo`, ohat `..............x.`, shaker `o.o.o.o.o.o.o.o.`, tamb `....x.......x...`; B’de tef her arka vuruşta, 2 ölçüde bir 4. vuruş arkasında ek bas davul; her 8 ölçünün son vuruşunda trampet geçişi.
- Akorlar: A vi – IV – I – V, B ii – V – I – vi (ölçü başına bir akor).
- Motif (vuruş, derece, uzunluk; derece 0 = ton kökü, 7 = bir oktav üstü): `[(0, 0, 0.5), (0.5, 2, 0.5), (1, 4, 0.5), (1.5, 7, 1), (3, 6, 0.5), (3.5, 4, 0.5), (4, 5, 1), (5, 4, 0.5), (5.5, 2, 0.5), (6, 4, 2), (8, 0, 0.5), (8.5, 2, 0.5), (9, 4, 0.5), (9.5, 7, 1), (11, 9, 0.5), (11.5, 7, 0.5), (12, 8, 1), (13, 7, 0.5), (13.5, 4, 0.5), (14, 7, 2), (16, 0, 0.5), (16.5, 2, 0.5), (17, 4, 0.5), (17.5, 7, 1), (19, 6, 0.5), (19.5, 4, 0.5), (20, 5, 1), (21, 4, 0.5), (21.5, 2, 0.5), (22, 1, 2), (24, 2, 0.5), (24.5, 4, 0.5), (25, 1, 0.5), (25.5, 2, 0.5), (26, -1, 1), (27, 0, 3)]`
- Cevap (B): `[(0, 4, 0.5), (0.75, 4, 0.25), (1, 5, 0.5), (2, 4, 0.5), (2.5, 2, 0.5), (3, 0, 1), (4, 1, 0.5), (4.75, 1, 0.25), (5, 2, 0.5), (6, 4, 2), (8, 4, 0.5), (8.75, 4, 0.25), (9, 5, 0.5), (10, 7, 0.5), (10.5, 5, 0.5), (11, 4, 1), (12, 2, 0.5), (12.5, 1, 0.5), (13, 0, 3)]`
- Ölçüm: {'start': 0.02505668934240363, 'length': 69.56521541950113, 'bpm': 138, 'lufs': np.float64(-18.0), 'perc': 0.78, 'name': 'Ksilofon Arcade', 'centroid': 2468, 'beat_bpm': 136.0, 'beat_jitter_ms': 10.3}

## C: Kalimba Zıplama

- 132 BPM, ton G majör; davul kalıbı `four`; motif kalimba (+ bir oktav üstte xylo), akor harp (2. ve 4. vuruşun arkası), bas bizim sentezimiz (sekizlik).
- Davul (16’lık adımlar, x vurgu, o hafif): kick `x...x...x...x...`, snare `....x.......x...`, clap `....x.......x...`, hat `x.o.x.o.x.o.x.o.`, ohat `..x...x...x...x.`, shaker `oxooxoooxooooxoo`, tamb `................`; B’de tef her arka vuruşta, 2 ölçüde bir 4. vuruş arkasında ek bas davul; her 8 ölçünün son vuruşunda trampet geçişi.
- Akorlar: A I – iii – IV – V, B vi – IV – I – V (ölçü başına bir akor).
- Motif (vuruş, derece, uzunluk; derece 0 = ton kökü, 7 = bir oktav üstü): `[(0, 4, 0.75), (0.75, 4, 0.75), (1.5, 5, 0.5), (2, 7, 1), (3, 5, 1), (4, 4, 0.75), (4.75, 2, 0.75), (5.5, 1, 0.5), (6, 2, 2), (8, 4, 0.75), (8.75, 4, 0.75), (9.5, 5, 0.5), (10, 7, 1), (11, 9, 1), (12, 7, 0.75), (12.75, 5, 0.75), (13.5, 4, 0.5), (14, 4, 2), (16, 4, 0.75), (16.75, 4, 0.75), (17.5, 5, 0.5), (18, 7, 1), (19, 5, 1), (20, 4, 0.75), (20.75, 2, 0.75), (21.5, 1, 0.5), (22, 2, 2), (24, 5, 0.75), (24.75, 4, 0.75), (25.5, 2, 0.5), (26, 1, 1), (27, -1, 1), (28, 0, 3)]`
- Cevap (B): `[(0, 9, 0.5), (0.5, 7, 0.5), (1, 9, 0.5), (1.5, 11, 0.5), (2, 9, 2), (4, 7, 0.5), (4.5, 5, 0.5), (5, 7, 0.5), (5.5, 9, 0.5), (6, 7, 2), (8, 5, 0.5), (8.5, 4, 0.5), (9, 5, 0.5), (9.5, 7, 0.5), (10, 5, 1), (11, 4, 1), (12, 2, 1), (13, 4, 1), (14, 2, 2)]`
- Ölçüm: {'start': 0.02505668934240363, 'length': 72.72725623582767, 'bpm': 132, 'lufs': np.float64(-18.0), 'perc': 0.81, 'name': 'Kalimba Zıplama', 'centroid': 1970, 'beat_bpm': 129.2, 'beat_jitter_ms': 11.5}

## D: Piyano Koşu

- 136 BPM, ton D majör; davul kalıbı `break`; motif glock (+ bir oktav üstte marimba), akor piano (2. ve 4. vuruşun arkası), bas bizim sentezimiz (sekizlik).
- Davul (16’lık adımlar, x vurgu, o hafif): kick `x.....x...x.....`, snare `....x.......x..o`, clap `....x.......x...`, hat `xoxoxoxoxoxoxoxo`, ohat `..............x.`, shaker `o.o.o.o.o.o.o.o.`, tamb `....x.......x...`; B’de tef her arka vuruşta, 2 ölçüde bir 4. vuruş arkasında ek bas davul; her 8 ölçünün son vuruşunda trampet geçişi.
- Akorlar: A I – vi – ii – V, B IV – I – IV – V (ölçü başına bir akor).
- Motif (vuruş, derece, uzunluk; derece 0 = ton kökü, 7 = bir oktav üstü): `[(0, 0, 0.5), (0.5, 4, 0.5), (1, 7, 0.5), (1.5, 4, 0.5), (2, 5, 0.5), (2.5, 4, 0.5), (3, 2, 1), (4, 1, 0.5), (4.5, 2, 0.5), (5, 4, 0.5), (5.5, 2, 0.5), (6, 1, 2), (8, -1, 0.5), (8.5, 1, 0.5), (9, 4, 0.5), (9.5, 1, 0.5), (10, 3, 0.5), (10.5, 2, 0.5), (11, 1, 1), (12, 0, 0.5), (12.5, 1, 0.5), (13, 2, 0.5), (13.5, 4, 0.5), (14, 4, 2), (16, 0, 0.5), (16.5, 4, 0.5), (17, 7, 0.5), (17.5, 4, 0.5), (18, 5, 0.5), (18.5, 4, 0.5), (19, 2, 1), (20, 1, 0.5), (20.5, 2, 0.5), (21, 4, 0.5), (21.5, 5, 0.5), (22, 7, 2), (24, 6, 0.5), (24.5, 4, 0.5), (25, 2, 0.5), (25.5, 1, 0.5), (26, 4, 1), (27, 1, 1), (28, 0, 3)]`
- Cevap (B): `[(0, 7, 1.5), (1.5, 6, 0.5), (2, 7, 0.5), (2.5, 9, 0.5), (3, 7, 1), (4, 5, 1.5), (5.5, 4, 0.5), (6, 2, 2), (8, 3, 1.5), (9.5, 4, 0.5), (10, 5, 0.5), (10.5, 7, 0.5), (11, 5, 1), (12, 4, 1), (13, 2, 1), (14, 4, 2)]`
- Ölçüm: {'start': 0.02505668934240363, 'length': 70.58823129251701, 'bpm': 136, 'lufs': np.float64(-18.0), 'perc': 0.79, 'name': 'Piyano Koşu', 'centroid': 2651, 'beat_bpm': 136.0, 'beat_jitter_ms': 0.0}

