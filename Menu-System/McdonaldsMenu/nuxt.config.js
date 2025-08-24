export default {
  // Disable server-side rendering: https://go.nuxtjs.dev/ssr-mode
  ssr: false,
  // Global page headers: https://go.nuxtjs.dev/config-head
  head: {
    htmlAttrs: {
      lang: 'ar',
      dir: 'rtl'
    },
    title: 'order system',
    meta: [
      {charset: 'utf-8'},
      {
        name: 'viewport',
        content: 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'
      },
      {
        hid: 'description',
        name: 'description',
        content: 'order system powered by info-seed'
      }
    ],
    link: [
      {
        rel: 'icon',
        type: 'image/x-icon',
        href: '/favicon.ico'
      },
      {
        rel: 'stylesheet',
        href: 'https://use.fontawesome.com/releases/v5.2.0/css/all.css'
      },
      {
        rel: 'stylesheet',
        href: 'https://cdn.jsdelivr.net/npm/bootstrap@4.3.1/dist/css/bootstrap.min.css'
      },
      

    ],
  
  
  
  
    script: [

      {src:  ('./js/jquery.js')},
      {src: 'https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js'},
      {src: 'https://cdn.jsdelivr.net/npm/bootstrap@5.1.1/dist/js/bootstrap.min.js'},
      {src: 'https://kit.fontawesome.com/a076d05399.js'}
    ]
  },

  // Global CSS: https://go.nuxtjs.dev/config-css
   css: [
    
  //   {
 
  //     // src:"./assets/style.css",
  //     // lang:'css'
  //     } 
    
  
   ],



  // Plugins to run before rendering page: https://go.nuxtjs.dev/config-plugins
  plugins: [
    {src: 'plugins/sweet-alert.js'},
    {
      src: '~/plugins/common.js'
    },
    {
      src: '~/plugins/i18n.js'
    }
  ],
  router: {
    middleware: ['i18n']
  },
  // Auto import components: https://go.nuxtjs.dev/config-components
  components: true,

  // Modules for dev and build (recommended): https://go.nuxtjs.dev/config-modules
  buildModules: [],

  // Modules: https://go.nuxtjs.dev/config-modules
  modules:
    [
      // https://go.nuxtjs.dev/axios
      '@nuxtjs/axios',
      // https://go.nuxtjs.dev/pwa
      '@nuxtjs/pwa',
      'bootstrap-vue/nuxt'
    ],
  // Axios module configuration: https://go.nuxtjs.dev/config-axios
  axios:
    {
      // baseURL: 'https://teaminboxapiprod.azurewebsites.net', // Used as fallback if no runtime config is provided
      // baseURL: 'https://teaminboxapiprod-stg.azurewebsites.net', // Used as fallback if no runtime config is provided
      baseURL: 'https://localhost:44301', // Used as fallback if no runtime config is provided,
     //baseURL: 'https://teaminboxapiprod-stg.azurewebsites.net', // Used as fallback if no runtime config is provided,
      useCache: true
    },

  // PWA module configuration: https://go.nuxtjs.dev/pwa
  pwa: {
    manifest: {
      lang: 'en'
    }
  },
  server: {
    host: '0.0.0.0'
  },
  // Build Configuration: https://go.nuxtjs.dev/config-build
  build: {},
  useEslint: false
}
