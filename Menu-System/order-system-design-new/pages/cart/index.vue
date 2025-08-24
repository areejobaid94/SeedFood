<template>
  <div class="container-fluid">
    <div class="d-flex justify-content-center p-2">
      <h3>{{ $t('CART_ITEMS') }}</h3>
    </div>
    <div class="row">
      <div
        v-for="(item,i) of cartItems"
        :key="i"
        class="panel-wrapper col-sm-12 col-md-6 p-0"
        :class="{'last-item':i===cartItems.length-1}"
      >
        <div
          class="product-panel d-flex flex-column justify-content-between px-3 py-2"
        >
          <div class="mb-2 d-flex flex-row w-100 justify-content-between">
            <div class="product-info d-flex flex-column flex-grow justify-content-between">
              <div
                class="title overflow-hidden mb-1"
              >
                {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
              </div>
              <div class="description truncate">
                {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
              </div>
              <div :id="'item'+i" class="mb-3">
                <div class="specification-details">
                  <div class="d-flex flex-column align-items-start">
                    <div
                      v-for="(specification,i) of item.itemSpecifications"
                      :key="specification.id+'s'+i"
                      class="d-flex flex-column align-items-start"
                    >
                      <li
                        v-for="(choice,j) of specification.specificationChoices"
                        :key="choice.id+'c'+'-'+j+'-'+i"
                        class="mb-1"
                      >
                        {{
                          getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                        }}
                      </li>
                    </div>
                    <div v-for="(extra,i) of item.createExtraOrderDetailsModels" :key="extra.id+'e'+i" class="mt-1">
                      {{ getTranslationKey(extra.name, extra.nameEnglish) }}
                    </div>
                  </div>
                </div>
              </div>
              <div class="flex-between mb-2 mt-1">
                <div class="d-flex flex-row">
                  <button type="button" class="qty-button minus-button small-qty-btn" @click="minQty(item)" />
                  <span class="px-3">{{ item.quantity }}</span>
                  <button type="button" class="qty-button plus-button small-qty-btn" @click="plusQty(item)" />
                </div>
              </div>
            </div>
            <div class="flex-column position-relative ">
              <div class="product-image cover round plx-4">
                <img
                  :src="item.imageUri"
                >
              </div>
              <div class="d-flex flex-between mb-2">
                <div class="product-price col-md-5">
                  <span v-if="item.total>0">{{ toCurrencyFormat(item.total) }}</span>

                  <span v-else>{{ $t('FREE') }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div
      v-if=" $store.state.item.cart.length>0"
      class="container d-flex flex-row justify-content-center checkout-btn cart-btn"
      @click="submitOrder()"
    >
      <div class="cart-icon">
        <span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span>
      </div>
      <div>{{ $t('CHECK_OUT') }}</div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Index',
  data () {
    return {
      cartItems: [],
      tenantID: '',
      contactId: '',
      languageBot: '1',
      menuType: 0,
      phone: ''
    }
  },
  watch: {
    cartItems () {
      this.getTotal()
    }
  },
  beforeMount () {
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber
    if (localStorage.getItem('cart') && localStorage.getItem('cart').length > 0) {
      const data = JSON.parse(localStorage.getItem('cart'))
      this.$store.dispatch('item/setCartData', data)
      console.log(data)
    }
  },
  mounted () {
    this.cartItems = JSON.parse(JSON.stringify(this.$store.state.item.cart))
  },
  methods: {
    plusQty (item) {
      // item.quantity += 1
      this.updateTotal(item, item.quantity + 1)
    },
    minQty (item) {
      if (item.quantity === 1) {
        this.confirmAlert(item)
      } else {
        this.updateTotal(item, item.quantity - 1)
      }
    },
    confirmAlert (item) {
      console.log(this.cartItems)

      const r = confirm('هل تريد حذف هذا الصنف ؟')
      if (r === true) {
        const index = this.cartItems.findIndex(cartItem => cartItem.id === item.id)
        this.cartItems.splice(index, 1)
        if (this.cartItems.length === 0) {
          this.$store.dispatch('item/setCartData', [])

          this.$router.push(`/?TenantID=${this.tenantID}&Menu=${this.menuType}&ContactId=${this.contactId}&LanguageBot=${this.languageBot}&PhoneNumber=${this.phone}`)
        }
      }
    },
    submitOrder () {

           var stringg1='الرجاء الانتظار قليلا'

          if(this.lang==='en'){

           stringg1='Please wait a bit'
          
          }
          if(this.lang==='ar'){

              stringg1='الرجاء الانتظار قليلا'
        
          }



      this.$swal.fire(stringg1)
      this.$swal.showLoading()

      debugger
      const request = {
        tenantId: this.tenantID,
        customerId: this.contactId,
        createOrderDetailsModels: this.cartItems,
        total: this.getTotal()
      }
      this.$axios.post(`${this.$axios.defaults.baseURL}/api/MenuSystem/CreateOrder`, request, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        this.$axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/SendToBot/?CustomerId=${this.contactId}`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {
          const phoneNumber = 'https://wa.me/' + this.phone

          let string1 = 'الطلب لم يكتمل بعد<br>'
          let string2 = 'يرجى العودة الى واتسب '

          if (this.lang === 'EN') {
            string1 = 'The Order is not completed  yet<br>'
            string2 = 'Please go back to WhatsApp '
          }
          if (this.lang === 'AR') {
            string1 = 'الطلب لم يكتمل بعد<br>'
            string2 = 'يرجى العودة الى واتسب '
          }

          this.$swal.fire(
            string1,
            string2,
            'success'
          ).then(function () {
            window.location = phoneNumber
          })

          this.$store.dispatch('item/setCartData', [])
          this.loadingSendOrder = false
          this.selectedItems = []
        })
      })
    },
    getTotal () {
      let total = 0
      // eslint-disable-next-line array-callback-return
      this.cartItems.map((item) => {
        total += item.total
      })
      return total
    },
    updateTotal (item, qty) {
      const index = this.cartItems.findIndex(cartItem => cartItem.id === item.id)
      console.log(item)
      const total = (item.total / item.quantity) * qty

      item.quantity = qty

      this.cartItems[index].total = total
    }
  }
}
</script>

<style scoped>
.specification-details {
  color: #546e7a
}
</style>
