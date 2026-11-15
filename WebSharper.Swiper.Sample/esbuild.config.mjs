import { build } from 'esbuild'
import { readFileSync, writeFileSync, copyFileSync } from 'fs'

const name = 'WebSharper.Swiper.Sample'

// The WebSharper-generated module imports the npm `swiper` package and its
// `swiper/modules` entry points (plus the WebSharper runtime); bundle it all
// into a single IIFE for the static page.
await build({
  entryPoints: [`./build/${name}.js`],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: `../dist/Scripts/${name}.min.js`,
  globalName: 'wsbundle'
})

copyFileSync('./site.css', '../dist/site.css')
// Swiper ships its stylesheet with the npm package; the page links it directly.
copyFileSync('./node_modules/swiper/swiper-bundle.css', '../dist/swiper-bundle.css')

// Fold the WebSharper head (linked resources) into the page.
const head = readFileSync(`./build/${name}.head.html`, 'utf8')
const html = readFileSync('./index.html', 'utf8').replace('<!-- ws-head -->', head)
writeFileSync('../dist/index.html', html)

console.log('Bundled:', name)