module.exports = {
  root: true,
  env: {
    node: true
  },
  'extends': [
    'plugin:vue/essential',
    'eslint:recommended',
    '@vue/typescript'
  ],
  rules: {
    /* TODO: get rid of console.log() in sources 'no-console': process.env.NODE_ENV === 'production' ? 'error' : 'off',*/
    'no-console': 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'css.lint.emptyRules': 'ignore',
    'scss.lint.emptyRules': 'ignore',
    'less.lint.emptyRules': 'ignore'
  },
  parserOptions: {
    parser: '@typescript-eslint/parser'
  }
}
