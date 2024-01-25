const colors = require('tailwindcss/colors');
const plugin = require('tailwindcss/plugin');
const { createThemes } = require('tw-colors');
const svelte_ux = require('svelte-ux/plugins/tailwind.cjs')
const { iconsPlugin, getIconCollections } = require('@egoist/tailwindcss-icons');

module.exports = {
  content: ['./src/**/*.{html,svelte,ts}', './node_modules/svelte-ux/**/*.{svelte,js}'],
  variants: {
    extend: {},
  },
  plugins: [
    iconsPlugin({
      collections: getIconCollections(['mdi']),
    }),
    svelte_ux,
    createThemes({
      dark: {
        "primary": "#FF865B",
        "secondary": "#FD6F9C",
        "accent": colors.blue,
        // "neutral": "oklch(26% 0.019 237.69)",
        // "neutral-content": "oklch(70% 0.019 237.69)",
        "base": colors.slate,
        "base-content": "#fff",
        "info": "#89e0eb",
        "success": "#addfad",
        "warning": "#f1c891",
        "error": "#ffbbbd",
      },
    }),
  ],
};
