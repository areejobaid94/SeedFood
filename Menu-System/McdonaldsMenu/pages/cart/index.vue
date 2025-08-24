<template>



  <div>


<div class="d-flex justify-content-between  relative header-container">


      <div class="cardmainmnue">

          <b-button v-b-toggle.sidebar-1 class="btn-secondary-card"><i class="fas fa-align-justify"></i></b-button>
          <b-sidebar id="sidebar-1" title="" >

          <div class="app-langague d-flex justify-content-end" @click="changeLang">
             <img class="lang-icon mr-1 ml-1" :src="require('assets/images/red-icon.png')" > <span>{{ lang === 'ar' ? 'English' : 'العربية' }}</span>
          </div>
          <div  @click="changeLang()"></div>


          <div class="">



             <div v-for="(menuItem) of MenuList"
                            :key="menuItem.id"
                            :id="'menuItem'+menuItem.id"
                             @click="menuItem.isInService ?changeMenu(menuItem):goisInService()">

                            >

                       <div class="menu-btn">
                        <button type="button" class="btn"> <img class="mr-1"  :src="menuItem.imageUri" > {{ getTranslationKey(menuItem.menuName, menuItem.menuNameEnglish) }} </button>
                    </div>

             </div>

          </div>
          </b-sidebar>
      </div>

         <div> <img  :src="lang === 'ar' ?require('assets/images/MenuCategorylogoAr.png'):require('assets/images/MenuCategorylogo.jpg')" > </div>
        <div class="shopping-cart" @click="$store.state.item.cart.length>0 ?openCart():''">
          <span class="count">{{$store.state.item.cart.length}}
            </span><button type="button" class="btn btn-warning" style="width: max-content;">
              <i class="fa fa-shopping-cart">{{ $t('MyOrder') }}</i></button>
 </div>


     </div>

 <div class="bodycontainer pt-0" >
   <div class="d-flex justify-content-between">
      <h1>{{ $t('CART_ITEMS') }}</h1>
    <div class="back-btn-txt cursor-pointer " @click="back()"> <i class="fas fa-angle-left arrow-icon"></i> {{ getTranslationKey("رجوع","Back") }}</div>
  </div>
    <div class="row">
      <div
        v-for="(item,i) of cartItems"
        :key="i"
        class="panel-wrapper col-sm-12 col-md-12"
        :class="{'last-item':i===cartItems.length-1}"
      >
         <div class="card product-panel">



          <div class="row m-0 p-2">

            <div class=" col-3 col-md-2  pr-0 pl-1">
               <div class="img-block"><img class="img-fluid" :src="item.imageUri">
              </div>
              </div>

              <div class="col-6 col-md-7  d-flex align-items-center ">

                <div class="product-info d-flex flex-column flex-grow justify-content-between ">


                  <div class="title overflow-hidden mb-1"  style=" font-family: Helvetica,Arial,sans-serif;font-size: 16px; font-weight: 600;text-align: right;" v-if="lang === 'ar'">
                    {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                  </div>
                  <div class="title overflow-hidden mb-1" style="font-family: Helvetica,Arial,sans-serif;font-size: 16px; font-weight: 600; text-align: left;" v-else >
                    {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                  </div>

 <div  v-if="!item.itemDescription === 'Condiments' || !item.itemDescription === 'Crispy' || !item.itemDescription === 'Deserts' ">

                  <div class="description truncate" style="text-align: right;" v-if="lang === 'ar'">
                    {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
                  </div>
                  <div class="description truncate"  style="text-align: left;" v-else>
                    {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
                  </div>
 </div>
                  <div :id="'item'+i">
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
                            class="mb-1" style="color: #a0a0a0;font-family: Helvetica,Arial,sans-serif;font-size: 12px; font-weight: 500; list-style: none;"
                          >

                          {{
                              getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                            }}
                          </li>
                        </div>
                        <div v-for="(extra,i) of item.createExtraOrderDetailsModels" :key="extra.id+'e'+i" class="mt-1 d-flex" style="white-space: initial;">
                        <span class="mr-1" style="line-height: 17px; color:#a0a0a0">• </span> <li  class="mb-1" style="color: #a0a0a0;font-family: Helvetica,Arial,sans-serif;font-size: 12px; font-weight: 500; list-style: none">
                          {{ getTranslationKey(extra.name, extra.nameEnglish) }} ({{extra.quantity}})

                          </li>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="flex-between mb-2 mt-1">
                    <div class="d-flex flex-row qty-button-holder-1">
                      <button type="button" class="qty-button minus-button small-qty-btn" @click="minQty(item,i)" ></button>
                      <span class="px-3">{{ item.quantity }}</span>
                      <button type="button" class="qty-button plus-button small-qty-btn" @click="plusQty(item,i)" ></button>
                    </div>
                  </div>
                </div>

              </div>

              <div class="col-3 col-md-3  d-flex align-items-center justify-content-end">
               <div class="product-price border-left pl-2">
                  <span v-if="item.total>0">{{ toCurrencyFormat(item.total) }}</span>

                  <span v-else>{{ $t('FREE') }}</span>
                </div>
              </div>
          </div>

        </div>
      </div>
    </div>

 <h1>{{ $t('ChooseCondiments') }} </h1>
 <div class="btn-blue cursor-pointer" @click="GoToCondiments()">{{ $t('Condiments') }} </div>
<div class="btn-blue cursor-pointer" @click="GoToCrispy()">{{ $t('Crispy') }} </div>
<div class="btn-blue cursor-pointer" @click="GoToDeserts()">{{ $t('Deserts') }} </div>
 <h1> {{ $t('OrderSummary') }}</h1>
 <div class="sub-total row m-0 mb-2">
    <div class="col-6">
      <h1 class="pb-1">{{ $t('Total') }}:</h1>
    </div>
    <div class="col-6 text-right d-flex justify-content-end align-items-center">
      <div class="cart-icon" @click="submitOrder()">
        <span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span>
      </div>
    </div>
  <div class="col-12 text-center mt-3">
    <div class="btn-checkout cursor-pointer" @click="submitOrder()">{{ $t('CHECK_OUT') }} </div>


  </div>

  </div>




    <div
      v-if=" $store.state.item.cart.length>0 && !(loadingSendOrder)"
      class="container d-flex flex-row justify-content-center  cart-btn"

    >

     <!-- <div class="back-btn cursor-pointer" @click="back()">
        <i class="fas fa-arrow-left fa-stack-1x fa-inverse" style="color: black;border-radius: 28px;"></i>
      </div>
     -->
      <!-- <div class="cart-icon" @click="submitOrder()">
        <span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span>
      </div>  -->
      <!-- <div class="btn-checkout" @click="submitOrder()">{{ $t('CHECK_OUT') }}</div> -->

      <!-- <div class="Condiments-btn cursor-pointer" @click="GoToCondiments()">
        {{ $t('Condiments') }}
      </div>        -->
    </div>
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
      lang: '',
      languageBot: '1',
      menuType: 0,
      phone: '',
      loadingSendOrder: false
    }
  },
  watch: {
    cartItems () {
      this.getTotal()
    }
  },

  beforeMount () {
     this.lang = this.$route.query.lang
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber
    this.goTop()
     this.GetMenu();
    if (localStorage.getItem('cart') && localStorage.getItem('cart').length > 0) {
      const data = JSON.parse(localStorage.getItem('cart'))
      this.$store.dispatch('item/setCartData', data)
      console.log(data)
    }
  },

  mounted () {
    debugger
    this.cartItems = JSON.parse(JSON.stringify(this.$store.state.item.cart))
    debugger
  },
  methods: {
    GetMenu() {

      if(JSON.parse(localStorage.getItem('MenuList'))!=null){

           this.MenuList = JSON.parse(localStorage.getItem('MenuList'))

     }else{
                axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenu?TenantID=${this.tenantID}&menu=${this.menuType}`, {
                      headers: {
                                'Content-Type': 'application/json',
                                'Access-Control-Allow-Origin': '*'
                               } }).then((res1) => {

                              this.MenuList = res1.data.result
                          })

       }
    },

    plusQty (item,index) {

      if(item.quantity < 9){
      // item.quantity += 1
      this.updateTotal(item, item.quantity + 1,index)
      }
    },
    minQty (item,index) {
      if (item.quantity === 1) {
        this.confirmAlert(item)
      } else {
        this.updateTotal(item, item.quantity - 1,index)
      }
    },
    confirmAlert (item) {
      console.log(this.cartItems)

     debugger
      const r = confirm('هل تريد حذف هذا الصنف ؟')
      if (r === true) {
        debugger
        const index = this.cartItems.findIndex(cartItem => cartItem.itemId === item.itemId)
        this.cartItems.splice(index, 1)
        if (this.cartItems.length === 0) {
          this.$store.dispatch('item/setCartData', [])

          this.$router.push(`/?TenantID=${this.tenantID}&Menu=${this.menuType}&ContactId=${this.contactId}&LanguageBot=${this.languageBot}&PhoneNumber=${this.phone}&lang=${this.lang}`)
        }else{

          this.$store.dispatch('item/setCartData', [])
          this.$store.dispatch('item/setCartData', this.cartItems)

          this.cartItems
        }
      }
    },
    changeLang() {

      this.lang = this.$route.query.lang === 'ar' ? 'en' : 'ar'
      this.$router.push({
        path: this.$router.currentRoute.path,
        query: {
          ...this.$route.query,
          lang: this.lang
        }
      })
      this.lang0 = this.$route.query.lang === 'ar' ? 'en' : 'ar'
      this.closeNav();
    },
     async back() {

         this.$store.state.item.cart=[]

          this.cartItems.forEach((value, index) => {
                this.$store.dispatch('item/addItemToCard', value)
            });
      await this.$router.back()
    },
      async changeMenu(menuItem) {
    localStorage.setItem('menuItem', JSON.stringify(menuItem));
    if(JSON.parse(localStorage.getItem('menuItem'))!=null){
      this.SelectedMenu = JSON.parse(localStorage.getItem('menuItem'))
     }

      await this.$router.push({
        path: '/',
        query: {
          ...this.$route.query
        //   itemId: item.id
        }
      })

 },
   async openCart() {
      await this.$router.push({
        path: '/cart',
        query: {
          ...this.$route.query
        }
      })


    },
    async GoToCondiments() {
          await this.$router.push({
                 path: '/Condiments',
                 query: {
                     ...this.$route.query
                     // SubcategoryId: SubCategory.id
                      }
                  })
    },

       async GoToCrispy() {
          await this.$router.push({
                 path: '/Crispy',
                 query: {
                     ...this.$route.query
                     // SubcategoryId: SubCategory.id
                      }
                  })
    },
     async GoToDeserts () {
          await this.$router.push({
                 path: '/Deserts',
                 query: {
                     ...this.$route.query
                     // SubcategoryId: SubCategory.id
                      }
                  })
    },
    submitOrder () {
      debugger
this.loadingSendOrder = true

       var stringg1='الرجاء الانتظار قليلا'

          if(this.lang==='en'){
           stringg1='Please wait a bit'
          }
          if(this.lang==='ar'){
              stringg1='الرجاء الانتظار قليلا'
          }



      this.$swal.fire(stringg1)
      this.$swal.showLoading()

      const request = {
        tenantId: this.tenantID,
        customerId: this.contactId,
        createOrderDetailsModels: this.cartItems,
        total: this.getTotal()
      }
      debugger
      this.$axios.post(`${this.$axios.defaults.baseURL}/api/MenuSystem/CreateOrder`, request, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        // this.$axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/SendToBot/?CustomerId=${this.contactId}`, {
        //   headers: {
        //     'Content-Type': 'application/json',
        //     'Access-Control-Allow-Origin': '*'
        //   }
        // }).then((res) => {

          localStorage.removeItem('categorySelected');
           localStorage.removeItem('logoImag');

          const phoneNumber = 'https://wa.me/' + this.phone

          let string1 = 'الطلب لم يكتمل بعد<br>'
          let string2 = 'يرجى العودة الى الواتس اب '

          if (this.lang === 'en') {
            string1 = 'The Order is not completed  yet<br>'
            string2 = 'Please go back to WhatsApp '
          }
          if (this.lang === 'ar') {
            string1 = 'الطلب لم يكتمل بعد<br>'
            string2 = 'يرجى العودة الى  الواتس اب '
          }


          this.cartItems = []
          this.$store.dispatch('item/setCartData', [])

          this.$swal.fire(
            string1,
            string2,
            'success'
          ).then(function () {
            window.location = phoneNumber
          })
          this.$store.dispatch('item/setCartData', [])

          this.selectedItems = []
        })
      // })
    },
    getTotal () {
      let total = 0
      // eslint-disable-next-line array-callback-return
      this.cartItems.map((item) => {
        total += item.total
      })
      return total
    },
    updateTotal (item, qty,indexx) {
      debugger
      const index = indexx
      console.log(item)
      const total = (this.cartItems[index].total / this.cartItems[index].quantity) * qty

      this.cartItems[index].quantity = qty

      this.cartItems[index].total = total
      debugger
    },
     goTop() {
debugger
      //  var id='item'+JSON.parse(localStorage.getItem('itemSelectedID'))
        var itemSelectedID = document.getElementById('__layout');
        if(itemSelectedID!=null){
               // var topPos = itemSelectedID.offsetTop;
                document.body.scrollTop = 0;
                this.$forceUpdate()
        }
       this.$forceUpdate()
     }
  }
}
</script>

<style scoped>
.specification-details {
  color: #546e7a
}
</style>
