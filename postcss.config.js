// https://sucipto.id/2019/05/01/tailwind-vue-js-postcss-configuration/

const tailwindcss = require('tailwindcss')
const autoprefixer = require('autoprefixer')
const purgecss = require('@fullhuman/postcss-purgecss')

class TailwindExtractor {
  static extract (content) {
    return content.match(/[A-Za-z0-9-_:\/]+/g) || []
  }
}

module.exports = {
  plugins: [
    tailwindcss('./tailwind.js'),
    autoprefixer({
      add: true,
      grid: true
    }),
    purgecss({
      content: [
        './src/**/*.html',
        './src/**/*.vue',
        './src/**/*.js',
        './public/**/*.html'
      ],
      extractors: [
        {
          extractor: TailwindExtractor,
          extensions: ['html', 'vue', 'js']
        }
      ]
    })
  ]
}