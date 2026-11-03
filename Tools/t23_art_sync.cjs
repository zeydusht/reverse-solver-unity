// Copy approved runtime assets verbatim; only Resources/TemplateData copies are managed here.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const root = path.resolve(__dirname, '..');
const resource = path.join(root, 'Assets/_Game/Presentation/Resources/Art');
const source = path.join(root, 'Assets/Art/Redesign_T23/NIHAI');
const template = path.join(root, 'Assets/WebGLTemplates/ReverseSolver/TemplateData');
const hash = file => crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
const metaTemplate = fs.readFileSync(path.join(resource, 't23-win-panel.png.meta'), 'utf8');
const before = fs.readdirSync(resource).filter(n => /^t23-.*\.png$/.test(n))
    .reduce((sum, n) => sum + fs.statSync(path.join(resource, n)).size, 0);
const sourceHashes = {};
function copy(folder, name, target) {
    const file = path.join(source, folder, name);
    sourceHashes[folder + '/' + name] = hash(file);
    fs.copyFileSync(file, target);
}
const families = fs.readdirSync(source, { withFileTypes: true }).filter(d =>
    d.isDirectory() && /^(0[7-9]|1[0-5])-/.test(d.name));
for (const dir of families) {
    const spec = JSON.parse(fs.readFileSync(path.join(source, dir.name, 'deneme.json')));
    copy(dir.name, 'character-scene.png', path.join(resource, `t23-intro-${spec.sceneKey}.png`));
}
copy('07-tanitim-civi', 'panel.png', path.join(resource, 't23-intro-panel.png'));
copy('18-profil-sorusu', 'panel.png', path.join(resource, 't23-profile-panel.png'));
for (const name of ['panel', 'name-input', 'button-primary', 'button-primary-pressed'])
    copy('17-isim', name + '.png', path.join(template, 't23-name-' + name + '.png'));
fs.copyFileSync(path.join(root, 'Assets/_Game/Presentation/Resources/Fonts/Nunito-Black.ttf'),
    path.join(template, 't23-nunito-black.ttf'));
for (const name of fs.readdirSync(template).filter(n => /^t23-/.test(n) && !n.endsWith('.meta'))) {
    const target = path.join(template, name + '.meta');
    if (!fs.existsSync(target)) fs.writeFileSync(target,
        `fileFormatVersion: 2\nguid: ${crypto.randomBytes(16).toString('hex')}\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);
}
// Deduplicate by actual SHA-256, including redundant ratings carried by old families.
const groups = new Map();
for (const name of fs.readdirSync(resource).filter(n => /^t23-.*\.png$/.test(n))) {
    const digest = hash(path.join(resource, name));
    if (!groups.has(digest)) groups.set(digest, []);
    groups.get(digest).push(name);
}
for (const names of groups.values()) {
    let key = names.find(n => /^t23-rating-/.test(n)) || names[0];
    if (names.some(n => n.includes('-button-'))) key = names[0].replace(/^t23-(win|loss-bomb|loss-time|pause)-/, 't23-');
    if (names.includes('t23-loss-bomb-panel.png')) key = 't23-loss-panel.png';
    if (!fs.existsSync(path.join(resource, key))) {
        fs.renameSync(path.join(resource, names[0]), path.join(resource, key));
        if (fs.existsSync(path.join(resource, names[0] + '.meta')))
            fs.renameSync(path.join(resource, names[0] + '.meta'), path.join(resource, key + '.meta'));
    }
    for (const name of names) if (name !== key) {
        for (const suffix of ['', '.meta']) {
            const target = path.resolve(resource, name + suffix);
            if (path.dirname(target) !== resource) throw new Error('Asset outside Resources/Art');
            if (fs.existsSync(target)) fs.unlinkSync(target);
        }
    }
}
for (const name of fs.readdirSync(resource).filter(n => /^t23-.*\.png$/.test(n))) {
    const file = path.join(resource, name + '.meta');
    let meta = fs.existsSync(file) ? fs.readFileSync(file, 'utf8') : metaTemplate
        .replace(/^guid: .*$/m, 'guid: ' + crypto.randomBytes(16).toString('hex'));
    const panel = name.endsWith('-panel.png');
    meta = meta.replace(/maxTextureSize: \d+/g, `maxTextureSize: ${panel ? 1024 : 512}`)
        .replace(/spriteBorder: \{[^}]+\}/, panel ? 'spriteBorder: {x: 72, y: 72, z: 72, w: 210}' : 'spriteBorder: {x: 0, y: 0, z: 0, w: 0}');
    meta = meta.replace(/(    buildTarget: WebGL\r?\n)([\s\S]*?)(?=  - serializedVersion:|  spriteSheet:)/,
        (_, head, body) => head + body.replace(/textureFormat: -?\d+/, 'textureFormat: 29')
            .replace(/overridden: \d+/, 'overridden: 1').replace(/compressionQuality: \d+/, 'compressionQuality: 50')
            .replace(/crunchedCompression: \d+/, 'crunchedCompression: 1'));
    fs.writeFileSync(file, meta);
}
for (const [name, digest] of Object.entries(sourceHashes))
    if (hash(path.join(source, name)) !== digest) throw new Error('Source modified: ' + name);
const files = fs.readdirSync(resource).filter(n => /^t23-.*\.png$/.test(n));
if (new Set(files.map(n => hash(path.join(resource, n)))).size !== files.length) throw new Error('Duplicate runtime PNG');
const after = files.reduce((sum, n) => sum + fs.statSync(path.join(resource, n)).size, 0);
const htmlBytes = fs.readdirSync(template).filter(n => /^t23-.*\.png$/.test(n))
    .reduce((sum, n) => sum + fs.statSync(path.join(template, n)).size, 0);
console.log(JSON.stringify({ runtimeFiles: files.length, beforeBytes: before, runtimeBytes: after, htmlBytes,
    totalBytes: after + htmlBytes, sourceHashes }, null, 2));
