"""Offline regressions: python -m unittest discover -s Tools/ajan -p 'test_*.py'."""
import copy
import csv
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import dongu
from level_similarity import mask_similarity, occupancy_mask


class AgentTests(unittest.TestCase):
    def setUp(self):
        self.card = dongu.load_card(dongu.CARDS / 'p21_06.json')
        self.level = dongu.read(dongu.ROOT / 'Assets/_Game/Levels/redesign.json')['levels'][5]
        self.metrics = dict(adim_sayisi=20, ilk3_min_secenek=3, en_uzun_bogaz=1,
                            bogaz_sayisi=0, cig_goreli=.5, ilk_hamle_tuzak_orani=0,
                            tuzak_adim=1, bomba_tehdit_adimi=3)
        self.measured = dict(solved=True, steps=[dict(unk=[])], bots={
            'bot_bilincli': dict(won=True, bomb_exits=[dict(p=14, earliest=2)]),
            'bot_rastgele_bombasiz': dict(win_rate=.25, runs=1000), 'bot_bomba_unutan': dict(win_rate=0),
            'funsim': dict(win_rate=.70, runs=100, time_left=dongu.distribution([22.2] * 70),
                           loss_piece_share=dongu.distribution([.2] * 30))})
        self.player = dict(source='ZeydBot v1', calibrated=True, remaining=dict(n=30, median=.37),
                           loss_piece_share=dict(n=0), tail_time_share=None)

    def checks(self):
        return dongu.critique(self.card, self.level, self.measured, self.metrics,
                             dict(intro_max=.7), self.player)

    def test_cards_and_unknown_recipe_edges(self):
        dongu.load_card(dongu.CARDS / 'p21_07.json')
        schema = dongu.read(dongu.HERE / 'kart_sema.json')
        card = copy.deepcopy(self.card)
        card['tarif']['vEdge'] = []
        with self.assertRaises(ValueError):
            dongu.validate(card, schema, schema['definitions'])

    def test_reversed_ranges_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            card = copy.deepcopy(self.card)
            card['tarif']['bombs'][0]['earliest'] = [4, 2]
            path = Path(folder) / 'card.json'
            dongu.write(path, card)
            with self.assertRaises(ValueError):
                dongu.load_card(path)

    def test_rectangle_aspect_and_seals(self):
        levels = dongu.read(dongu.ROOT / 'Assets/_Game/Levels/redesign.json')['levels']
        square = levels[1]
        wide = dongu.read(dongu.ROOT / 'Assets/_Game/Levels/redesign.json')['levels'][3]
        self.assertEqual(mask_similarity(square, square), 1)
        self.assertLess(mask_similarity(square, wide), .9)
        sealed = copy.deepcopy(square)
        sealed['sealed'] = dict(U=[0])
        self.assertLess(mask_similarity(square, sealed), 1)
        self.assertTrue(all(0 <= x < 16 and 0 <= y < 16 for _, x, y in occupancy_mask(wide)))
        def rectangle(w, h):
            return dict(w=w, h=h, pieces=[[[x, y] for y in range(h) for x in range(w)]])
        self.assertLess(mask_similarity(rectangle(3, 3), rectangle(5, 5)), .9)
        self.assertLess(mask_similarity(rectangle(4, 6), rectangle(6, 4)), .9)

    def test_unsafe_historical_bomb_fails(self):
        checks = self.checks()
        for prefix in ('R09', 'R10'):
            self.assertTrue(any(c['rule'].startswith(prefix) and c['status'] == 'KALDI' for c in checks))

    def test_intro_random_range_and_forgetful_loss(self):
        self.measured['bots']['bot_rastgele_bombasiz']['win_rate'] = .11
        checks = self.checks()
        self.assertEqual(next(c for c in checks if c['rule'] == 'R12 bot_rastgele_bombasiz')['status'], 'KALDI')
        self.assertEqual(next(c for c in checks if c['rule'] == 'R12 bot_bomba_unutan')['status'], 'GEÇTİ')

    def test_no_winners_and_unknown_are_warnings(self):
        self.player['remaining']['median'] = None
        self.player['remaining']['n'] = 0
        self.measured['bots']['funsim']['time_left'] = dongu.distribution([])
        self.measured['steps'][0]['unk'] = [2]
        checks = self.checks()
        for prefix in ('R02', 'R14'):
            self.assertTrue(any(c['rule'].startswith(prefix) and c['status'] == 'UYARI' for c in checks))

    def test_revisions_use_concrete_fields_and_advance_seed_window(self):
        rows = [dict(checks=[dict(correction=dict(field='bombs[0].margin', value=0)),
                            dict(correction=dict(field='w,h', value=[3, 7]))])]
        revised = dongu.revise(self.card['tarif'], rows)
        self.assertEqual(revised['bombs'][0]['margin'], 0)
        self.assertEqual([revised['w'], revised['h']], [3, 7])
        self.assertEqual([revised['seed_from'], revised['seed_to']], [401, 800])
        self.assertEqual(self.card['tarif']['seed_from'], 1)

    def test_quantiles_interpolate_and_empty_stays_missing(self):
        self.assertEqual(dongu.distribution([0, 1, 2, 3]), dict(n=4, median=1.5, p25=.75, p75=2.25))
        self.assertIsNone(dongu.distribution([])['median'])

    def test_intro_boundaries_and_warning_never_rejects(self):
        self.level['bombs'] = {'14': 5}
        self.measured['bots']['bot_bilincli']['bomb_exits'] = [dict(p=14, earliest=3)]
        def state(prefix):
            return next(c['status'] for c in self.checks() if c['rule'].startswith(prefix))
        self.assertEqual(state('R10'), 'GEÇTİ')
        self.level['bombs']['14'] = 4
        self.assertEqual(state('R10'), 'KALDI')
        self.level['bombs']['14'] = 6
        self.assertEqual(state('R10'), 'KALDI')
        self.metrics['ilk_hamle_tuzak_orani'] = .01
        self.assertEqual(state('R07'), 'KALDI')
        self.measured['bots']['funsim']['win_rate'] = .64
        self.assertEqual(state('R19'), 'KALDI')
        self.measured['bots']['funsim']['win_rate'] = .65
        self.assertEqual(state('R19'), 'GEÇTİ')
        self.measured['bots']['funsim']['runs'] = 5
        self.assertEqual(state('R19'), 'UYARI')
        for rate, expected in ((.149, 'KALDI'), (.15, 'GEÇTİ'), (.50, 'GEÇTİ'), (.501, 'KALDI')):
            self.measured['bots']['bot_rastgele_bombasiz']['win_rate'] = rate
            self.assertEqual(state('R12 bot_rastgele'), expected)
        self.assertEqual(dongu.decision([dict(status='GEÇTİ'), dict(status='UYARI')]), 'GEÇTİ')
        self.assertEqual(dongu.decision([dict(status='UYARI'), dict(status='KALDI')]), 'RED')

    def test_remaining_falls_back_to_funsim_and_small_samples_warn(self):
        self.player['remaining'] = dict(n=2, median=.99)
        self.player['tail_time_share'] = dict(n=2, median=.99)
        checks = self.checks()
        zeyd = next(c for c in checks if c['rule'] == 'R14 ZeydBot örneği')
        remaining = next(c for c in checks if c['rule'] == 'R14 kalan süre')
        self.assertEqual(zeyd['status'], 'UYARI')
        self.assertEqual(remaining['status'], 'GEÇTİ')
        self.assertAlmostEqual(remaining['measured']['median'], 22.2 / self.level['timer'])
        self.assertEqual(remaining['measured']['source'], 'FunSim')
        self.assertEqual(next(c for c in checks if c['rule'].startswith('R16'))['status'], 'UYARI')
        self.measured['bots']['funsim']['time_left']['n'] = 19
        self.assertEqual(next(c for c in self.checks() if c['rule'] == 'R14 kalan süre')['status'], 'UYARI')
        self.measured['bots']['funsim']['time_left'].update(n=20, median=1)
        self.assertEqual(next(c for c in self.checks() if c['rule'] == 'R14 kalan süre')['status'], 'KALDI')

    def test_round_diversity_and_concrete_placement(self):
        checks = dongu.critique(self.card, self.level, self.measured, self.metrics,
                               dict(intro_max=.7, candidate_max=.90), self.player)
        diversity = next(c for c in checks if c['rule'].startswith('R20'))
        self.assertEqual(diversity['status'], 'KALDI')
        revised = dongu.revise(self.card['tarif'], [dict(checks=[diversity])])
        self.assertEqual(len(revised['shapes']), 3)
        self.assertNotEqual(revised['shapes'], self.card['tarif']['shapes'])
        text = dongu.placement_note(dict(field='bombs[0].earliest', value=[4, 5]), self.card['tarif'])
        self.assertIn('3. sıradan 4. sıraya', text)

    def test_conflicts_require_individual_passes(self):
        rows = [dict(checks=[dict(rule='R07 ilk tuzak', status='GEÇTİ'), dict(rule='R11 tehdit', status='KALDI')]),
                dict(checks=[dict(rule='R07 ilk tuzak', status='KALDI'), dict(rule='R11 tehdit', status='GEÇTİ')])]
        self.assertIn('R07 ve R11 aynı anda sağlanamadı', dongu.conflicts(rows)[0])
        rows.append(dict(checks=[dict(rule='R07 ilk tuzak', status='GEÇTİ'), dict(rule='R11 tehdit', status='GEÇTİ')]))
        self.assertNotIn('aynı anda sağlanamadı', '\n'.join(dongu.conflicts(rows)))

    def test_finalist_masks_must_be_distinct(self):
        row = dict(player=dict(remaining=dict(median=.375)), similarity=dict(maximum=.4))
        pairs = [(self.level, row), (copy.deepcopy(self.level), row)]
        self.assertEqual(len(dongu.select_finalists(self.card, pairs)), 1)

    def test_shapes_only_and_region_schema(self):
        with tempfile.TemporaryDirectory() as folder:
            card = copy.deepcopy(self.card)
            del card['tarif']['w'], card['tarif']['h']
            card['tarif']['bombs'][0]['region'] = 'left'
            path = Path(folder) / 'card.json'
            dongu.write(path, card)
            dongu.load_card(path)
            card['tarif']['bombs'][0]['region'] = 'outside'
            dongu.write(path, card)
            with self.assertRaises(ValueError):
                dongu.load_card(path)

    def test_calibration_all_40_same_model_path_and_type_fallback(self):
        reference = dongu.read(dongu.ROOT / 'Assets/_Game/Levels/redesign.json')['levels']
        reference = copy.deepcopy(reference)
        for level in reference:
            level.pop('genre', None)
            level.pop('zeydbot', None)
        reference[10]['zeydbot'] = dict(kind='zor')
        actual_read = dongu.read
        with tempfile.TemporaryDirectory() as folder:
            measurement = Path(folder) / 'measurement.json'
            dongu.write(measurement, dict(levels=[dict(id=l['id']) for l in reference]))
            def read(path):
                return dict(levels=reference) if Path(path).name == 'redesign.json' else actual_read(path)
            def model(level, measured):
                self.assertEqual(level['id'], measured['id'])
                share = .1 if reference.index(level) < 10 else .3
                return dict(source='ZeydBot v1', tail_time_share=dongu.distribution([share] * 30))
            with patch.object(dongu, 'HERE', Path(folder)), patch.object(dongu, 'read', side_effect=read), \
                    patch.object(dongu, 'player_model', side_effect=model) as model_call, patch('builtins.print'):
                dongu.main(['--kalibre', str(measurement)])
            result = actual_read(Path(folder) / 'kalibrasyon.json')
            self.assertEqual(model_call.call_count, 40)
            self.assertEqual(result['all']['n'], 40)
            self.assertEqual(result['all']['median'], .3)
            self.assertEqual(result['by_type']['tanitim']['median'], .1)
            self.assertEqual(result['by_type']['normal']['n'], 29)
            self.assertEqual(result['by_type']['zor']['n'], 1)
            self.assertEqual(len(result['fallback_ids']), 39)

    def test_r16_reports_calibration_threshold_as_warning(self):
        with tempfile.TemporaryDirectory() as folder:
            dongu.write(Path(folder) / 'kalibrasyon.json', dict(by_type=dict(tanitim=dict(median=.31))))
            dongu.write(Path(folder) / 'tip_varsayilan.json', dongu.read(dongu.HERE / 'tip_varsayilan.json'))
            with patch.object(dongu, 'HERE', Path(folder)):
                result = next(c for c in self.checks() if c['rule'].startswith('R16'))
            self.assertEqual(result['status'], 'UYARI')
            self.assertEqual(result['measured']['kalibrasyon'], .31)

    def test_similarity_includes_set40_and_other_candidates(self):
        sig = '1;1;1;1;1;1;1;1;1;1'
        other = copy.deepcopy(self.level)
        other['id'] = 'test_other'
        result = dongu.comparisons(self.level, sig, [(self.level, sig), (other, sig)])
        self.assertEqual(result['curve'][0]['id'], 'test_other')
        self.assertEqual(len(result['mask']), 3)

    def test_model_fallback_is_explicit(self):
        measured = dict(bots=dict(funsim=dict(win_rate=.5, time_left=dict(n=2, median=20, p25=10, p75=30),
                                             loss_piece_share=dict(n=1, median=.2, p25=.2, p75=.2))))
        with tempfile.TemporaryDirectory() as folder, patch.object(dongu, 'LOG', Path(folder)):
            result = dongu.player_model(self.level, measured)
        self.assertEqual(result['source'], 'FunSim')
        self.assertTrue(result['fallback_reason'])
        self.assertAlmostEqual(result['remaining']['median'], 20 / self.level['timer'])

    def test_v1_model_accepts_new_candidate_id_and_samples(self):
        import zeydbot_model as model
        with (Path(model.D) / 'yollar.csv').open(encoding='utf-8-sig', newline='') as stream:
            rows = [r for r in csv.DictReader(stream) if r['level_id'] == 'Z12']
        if not rows:
            self.skipTest('Frozen Z12 training paths not present')
        level = next(l for l in dongu.read(dongu.ROOT / 'Assets/_Game/Levels/zeydbot.json')['levels'] if l['id'] == 'Z12')
        level = copy.deepcopy(level)
        level['id'] = 'p21_test_a'
        for row in rows:
            row['level_id'] = level['id']
        with tempfile.TemporaryDirectory() as folder, patch.object(dongu, 'LOG', Path(folder)):
            with (Path(folder) / 'paths_p21_test_a.csv').open('w', newline='', encoding='utf-8') as stream:
                writer = csv.DictWriter(stream, fieldnames=list(rows[0]))
                writer.writeheader()
                writer.writerows(rows)
            result = dongu.player_model(level, {})
        self.assertEqual(result['source'], 'ZeydBot v1')
        self.assertIsNotNone(result['remaining']['median'])
        self.assertGreater(result['tail_time_share']['n'], 0)

    def test_optional_samples_preserve_frozen_model_output(self):
        import zeydbot_model as model
        paths = {'new_a': {'0': [dict(secenek='1', dogru='1', kalan_parca='2'),
                                   dict(secenek='1', dogru='1', kalan_parca='1')]}}
        feat = {'new_a': dict(sure_limiti=60)}
        think = ([7, 0, 0, 0], .1, .2)
        expected = model.play('new_a', feat, paths, (-1, 1), think)
        samples = []
        actual = model.play('new_a', feat, paths, (-1, 1), think, samples=samples)
        self.assertEqual(expected, actual)
        self.assertEqual(len(samples), model.SIMS)

    def test_three_round_limit_and_fallback_finalist(self):
        with tempfile.TemporaryDirectory() as folder:
            log, cards = Path(folder) / 'log', Path(folder) / 'cards'
            def fake_batch(method, *args):
                recipe = dongu.read(args[1])
                level = copy.deepcopy(self.level)
                level['id'] = recipe['id'] + '_a'
                dongu.write(args[3], dict(format=1, levels=[level]))
            def fake_evaluate(card, path, *_):
                return [dict(id=dongu.read(path)['levels'][0]['id'], decision='RED',
                             checks=[dict(rule='R14 kalan süre', status='KALDI',
                                          correction=dict(field='timer', value=50))])]
            with patch.object(dongu, 'LOG', log), patch.object(dongu, 'CARDS', cards), \
                    patch.object(dongu, 'batch', side_effect=fake_batch) as batch, \
                    patch.object(dongu, 'evaluate', side_effect=fake_evaluate), patch('builtins.print'):
                dongu.main(['--kart', str(dongu.ROOT / 'docs/bolum_kartlari/p21_06.json'), '--uret'])
            self.assertEqual(batch.call_count, 3)
            finalists = dongu.read(cards / 'p21_06_finalist.json')['levels']
            self.assertEqual(len(finalists), 1)
            self.assertEqual(finalists[0]['etiket'], 'aday')
            self.assertEqual(dongu.read(log / 'p21_06_tarif_tur3.json')['seed_from'], 801)
            self.assertIn('GEÇTİ yok', (cards / 'p21_06_turlar.md').read_text(encoding='utf-8'))

    def test_best_two_finalists_and_stop_after_success(self):
        with tempfile.TemporaryDirectory() as folder:
            log, cards = Path(folder) / 'log', Path(folder) / 'cards'
            def fake_batch(method, *args):
                levels = []
                for letter in 'abc':
                    level = copy.deepcopy(self.level)
                    level['id'] = 'p21_06_' + letter
                    w, h = dict(a=(6, 4), b=(3, 7), c=(4, 6))[letter]
                    level.update(w=w, h=h, pieces=[[[x, y] for y in range(h) for x in range(w)]], sealed={})
                    levels.append(level)
                dongu.write(args[3], dict(format=1, levels=levels))
            def fake_evaluate(*_):
                return [dict(id='p21_06_' + letter, decision='GEÇTİ', checks=[],
                             player=dict(remaining=dict(median=median)), similarity=dict(maximum=sim))
                        for letter, median, sim in [('a', .31, .2), ('b', .375, .9), ('c', .375, .4)]]
            with patch.object(dongu, 'LOG', log), patch.object(dongu, 'CARDS', cards), \
                    patch.object(dongu, 'batch', side_effect=fake_batch) as batch, \
                    patch.object(dongu, 'evaluate', side_effect=fake_evaluate), patch('builtins.print'):
                dongu.main(['--kart', str(dongu.ROOT / 'docs/bolum_kartlari/p21_06.json'), '--uret'])
            self.assertEqual(batch.call_count, 1)
            self.assertEqual([l['id'] for l in dongu.read(cards / 'p21_06_finalist.json')['levels']], ['p21_06_c', 'p21_06_b'])


if __name__ == '__main__':
    unittest.main()
