import Vue from 'vue'
import { polyfill } from 'seamless-scroll-polyfill'

Vue.mixin({
  beforeCreate () {
    polyfill()
  },
  methods: {
    getTotal () {
      let total = 0
      // eslint-disable-next-line array-callback-return
      this.$store.state.item.cart.map((item) => {
        total += item.total
      })
      return total
    },
    toCurrencyFormat (price) {
      if (price) {
        return price.toLocaleString('en-JO', {
          style: 'currency',
          currency: 'JOD'
        })
      }
      return 0
    },
    getTranslationKey (arabic, english) {
      if (this.$route.query.lang === 'en') {
        return english
      }
      return arabic
    }
  }
})
