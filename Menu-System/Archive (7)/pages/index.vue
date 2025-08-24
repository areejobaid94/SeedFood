<template>
  <div>
    <div class="d-flex mb-2 relative full-background">
      <div class="app-langague d-flex justify-content-end" @click="changeLang">
        <span>{{ lang === 'ar' ? 'En' : 'ar' }}</span>
      </div>
      <div class="background-container w-100">
        <img
          class="bg-img"
          :src="this.bgImg"
        >
      </div>

      <div class="d-flex absolute restaurant-info">
        <div class="logo flex-column">
          <img
            :src="this.logoImag"
          >
        </div>
        <div class="restaurant-name py-1">
          <!--        <div class="" style=" color: aquamarine; ">           -->
          <!--         {{this.tname}}                                        -->
          <!--        </div>                                                  -->
          <div class="about_us" @click="changeLang()">

          </div>
        </div>
      </div>
    </div>
    <div class="container-fluid items">
      <div class="d-flex mb-2">
        <div class="relative w-100">
          <input v-model="search" class="search_input" type="text" name="" :placeholder="$t('SEARCH')" >
          <a href="#" class="search_icon"><i class="fas fa-search"/></a>
        </div>
      </div>

         <!--       \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\    -->
      <div class="d-flex flex-column category-items" style="  lang === 'ar' ?    direction: rtl; text-align: right;  : direction: ltr; text-align: left;">


        <nav
          v-dragscroll="true"
          class="navbar navbar-expand align-center px-0 dragscroll overflow-hidden" style="font-weight: bold;"
        >
          <div
            v-for="(category,i) of categories"
            :key="i"
            class="nav-item nav-link"
            :class="{'active':selectedCategory===category.categoryId}"
            @click="goToCategory(category,true)"
          >
            {{ getTranslationKey(category.categoryName, category.categoryNameEnglish) }}
          </div>
        </nav>

        <div>

          <nav

            v-dragscroll="true"
            class="navbar navbar-expand  align-center px-0 dragscroll overflow-hidden"
          >
            <div
              v-for="(subcategory,ii) of this.selectedCategorySub.subCategorysInItemModels"
              :key="ii"
              class="nav-item nav-link"
              :class="{'active':subCategorySelected.subcategoryId===subcategory.subcategoryId}"
              @click="goToSubCategory(subcategory,selectedCategorySub)"
            >
              {{ getTranslationKey(subcategory.categoryName, subcategory.categoryNameEnglish) }}
            </div>
          </nav>

        </div>





        <div class="scroll-arrow"/>
      </div>





   <!--       \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\    -->


      <div
        v-if="!categorySelected"
        v-for="(category,categoryIndex) of items"
        :id="'category'+category.categoryId"
        :key="category.id"
        style="scroll-margin-top: 70px;"
      >
        <div v-if="filteredList(category.listItemInCategories).length>0" class="collection-title d-flex justify-start">
          {{ getTranslationKey(category.categoryName, category.categoryNameEnglish) }}
        </div>
        <div class="row">
          <div
            v-for="(item,itemIndex) of filteredList(category.listItemInCategories)"
            :key="item.id"
            class="panel-wrapper col-sm-12 col-md-6 p-0"
            :class="{'last-item':(itemIndex=== category.listItemInCategories.length - 1)&&categoryIndex===items.length-1,'opacity-5':!item.isInService }"
          >
            <div
              class="product-panel d-flex flex-row justify-content-between px-3 py-2"
              @click="item.isInService ?goToItem(item):''"
            > <!--    &&item.qty>-11  -->
              <div class="product-info d-flex flex-column flex-grow justify-content-between">
                <div class="title overflow-hidden mb-1">
                  {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                </div>
                <div class="description truncate" style="direction: rtl; text-align: right; " v-if="lang0 === 'ar'">
                  {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
                </div>
                <div class="description truncate" style="direction: ltr; text-align: left;" v-else>
                  {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
                </div>

              <div class="d-flex justify-content-start mb-2">
                <div v-if="item.isInService " class="product-price"> <!--    && item.qty>-11 -->
                  <span v-if="item.oldPrice != null" style="text-decoration: line-through;">
                      {{ toCurrencyFormat(item.oldPrice) }}
                    </span>
                </div>               
              </div>  

                <div class="d-flex justify-content-start mb-2">
                  <div v-if="item.isInService " class="product-price"> <!--    && item.qty>-11 -->
                    <span v-if="item.price>0">
                      {{ toCurrencyFormat(item.price) }}
                    </span>
                    <span v-else>
                 <!--    {{ $t('FREE') }} -->
                    </span>
                  </div>
                  <div v-else class="product-price">
                    <span>{{ $t('UNAVAILABLE_ITEM') }}</span>
                  </div>
              
                </div>



              </div>
              <div class="product-image cover flex-column position-relative round plx-4">
                <img
                  :src="item.imageUri"
                >
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row" v-if="categorySelected&&categorySelected.listItemInCategories.length>0">
        <div
          v-for="(item,itemIndex) of filteredList(categorySelected.listItemInCategories)"
          :key="item.id"
          class="panel-wrapper col-sm-12 col-md-6 p-0"
        >
          <div
            class="product-panel d-flex flex-row justify-content-between px-3 py-2"
            @click="item.isInService ?goToItem(item):''"
          > <!--    &&item.qty>-11  -->
            <div class="product-info d-flex flex-column flex-grow justify-content-between">
              <div class="title overflow-hidden mb-1">
                {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
              </div>
              <div class="description truncate" style="direction: rtl; text-align: right; " v-if="lang0 === 'ar'">
                {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
              </div>
              <div class="description truncate" style="direction: ltr; text-align: left;" v-else>
                {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
              </div>

              <div class="d-flex justify-content-start mb-2">
                <div v-if="item.isInService " class="product-price" style="text-decoration: line-through;"> <!--    && item.qty>-11 -->
                  <span v-if="item.oldPrice != null">
                      {{ toCurrencyFormat(item.oldPrice) }}
                    </span>
                </div>               
              </div>                

              <div class="d-flex justify-content-start mb-2">
                <div v-if="item.isInService " class="product-price"> <!--    && item.qty>-11 -->
                  <span v-if="item.price>0">
                      {{ toCurrencyFormat(item.price) }}
                    </span>
                  <span v-else>
                 <!--    {{ $t('FREE') }} -->
                    </span>
                </div>
                <div v-else class="product-price">
                  <span>{{ $t('UNAVAILABLE_ITEM') }}</span>
                </div>
                
              </div>

            


            </div>
            <div class="product-image cover flex-column position-relative round plx-4">
              <img
                :src="item.imageUri"
              >
            </div>
          </div>
        </div>
        <transition name="fade">
          <p v-if="loading" class="loading">Loading</p>
        </transition>
      </div>
      <div class="d-flex justify-content-center">
        <div
          v-if=" $store.state.item.cart.length>0"
          class="container d-flex flex-row checkout-btn cart-btn"
          @click="openCart()"
        >
          <div class="cart-icon">
            <span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span>
          </div>
          <div>{{ $t('VIEW_BASKET') }}</div>
        </div>
      </div>

      <CoolLightBox
        :items="selectedImage"
        :index="imageIndex"
        @close="imageIndex = null"
      />
    </div>
  </div>
</template>

<script>

import axios from 'axios'
import CoolLightBox from 'vue-cool-lightbox'
import 'vue-cool-lightbox/dist/vue-cool-lightbox.min.css'
import {dragscroll} from 'vue-dragscroll'

export default {
  components: {
    CoolLightBox,
  },
  directives: {
    dragscroll
  },
  data() {
    return {
      logoImag: '',
      bgImg: '',
      tname: '',
      tnameEnglish: '',
      items: [],
      selectedItems: [],
      selectedItemsAddOn: [],
      search: '',
      tenantID: '',
      languageBot: '1',
      contactId: '',
      menuType: 0,
      total: 0,
      phone: '',
      selectedImage: [],
      imageIndex: null,
      categories: [],
      selectedCategory: -1,
      selectedCategorySub: [],
      categorySelected: null,
      subCategorySelected: null,
      loadingSendOrder: false,
      lang: 'ar',
      lang0: 'ar',
      isLangChange: false,
      subCategory: [],
      page: 0,
      hideShowMore: false,
      loading: false,
      pageSize: 20,
      IsSearch:false,
      oldlinght:-1
    }
  },
  beforeMount() {
    debugger
    this.lang0 = this.$route.query.lang === 'ar' ? 'ar' : 'en'

    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber

    axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetInfoTenant?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.languageBot}`, {
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*'
      }
    }).then((res1) => {
      this.logoImag = res1.data.result.logoImag
      this.bgImg = res1.data.result.bgImag

      this.tname = res1.data.result.name
      this.tnameEnglish = res1.data.result.nameEnglish
    })

    if (localStorage.getItem('cart') && localStorage.getItem('cart').length > 0) {
      const data = JSON.parse(localStorage.getItem('cart'))
      this.$store.dispatch('item/setCartData', data)
    }
    if (this.$store.state.item.items.length === 0) {
      this.getAllCategory()
    } else {
      this.items = this.$store.state.item.items
      this.categories = this.$store.state.item.items
    }
  },
  mounted() {
    window.addEventListener('touchend', this.loadMore);
    window.addEventListener('wheel', this.loadMore);
  },
  methods: {
    filteredList(item) {
      
 
       if (this.search.length==0){
         this.oldlinght=-1

       }
      if (this.search.length > 0) {


       if (this.tenantID === "34" && this.search.length >this.oldlinght) {
        


        axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0&Search=${this.search.toLowerCase()}`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {
       
          this.oldlinght=this.search.length
          //this.items = res.data.result.listItemInCategories
          this.items = []

        //  this.categorySelected = category
         // this.categorySelected.categoryId=res.data.result.categoryId
          //this.categorySelected.listItemInCategories = res.data.result.listItemInCategories
        //  this.subCategorySelected=res.data.result
          //this.items.push(res.data.result.listItemInCategories)
           debugger


      this.selectedCategory = res.data.result.categoryId
      this.selectedCategorySub = res.data.result
      this.selectedCategorySub.listItemInCategories=res.data.result.listItemInCategories
      this.subCategorySelected=res.data.result
      this.categorySelected = category
      this.categorySelected.categoryId=res.data.result.categoryId

       this.goToSubCategory(this.selectedCategory.subCategorysInItemModels[0], this.selectedCategory)
         
         return this.selectedCategorySub.listItemInCategories
        })

       }else{

        return item.filter((post) => {
          return post.itemName.toLowerCase().includes(this.search.toLowerCase())
        })
       }
        
      }
      return item
    },
    openImage(image) {
      this.selectedImage = [image]
      this.imageIndex = 0
    },

    getAllCategory() {

      axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetInfoTenant?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.languageBot}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res1) => {
        this.logoImag = res1.data.result.logoImag
        this.bgImg = res1.data.result.bgImag

        this.tname = res1.data.result.name
        this.tnameEnglish = res1.data.result.nameEnglish


        var stringg1='الرجاء الانتظار قليلا'

          if(this.lang==='en'){
           stringg1='Please wait a bit'        
          }
          if(this.lang==='ar'){
              stringg1='الرجاء الانتظار قليلا'      
          }

      this.$swal.fire(stringg1)
      this.$swal.showLoading()  



        // axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=3&PageSize=20&PageNumber=0&LanguageBotId=${this.languageBot}`, {
        axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuItem?TenantId=${this.tenantID}&MenuType=${this.menuType}&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {

           this.$swal.close();


          this.categories = res.data.result
          this.items = res.data.result
          //this.$store.dispatch('item/setSelectedItems', this.items)
          if (this.categories[0] && this.categories[0].subCategorysInItemModels.length > 0) {
            this.goToCategory(this.categories[0], true)
          }
          this.$forceUpdate()
        })


      })
    },
    async goToItem(item) {
      await this.$store.dispatch('item/setSelectedItem', item)

      await this.$router.push({
        path: '/product-info',
        query: {
          ...this.$route.query,
          itemId: item.id
        }
      })
    },
    goToCategory(category, selectSub = false) {
      debugger
       if ( category.subCategorysInItemModels!=null ) {
            selectSub= true
          }else{
            selectSub= false
          }

      this.page = 0

      this.selectedCategory = category.categoryId
      this.selectedCategorySub = category
      if (selectSub) {
        this.goToSubCategory(category.subCategorysInItemModels[0], category)
      } else {
        if (document.querySelector(`#category${category.categoryId}`)) {
          document.querySelector(`#category${category.categoryId}`).scrollIntoView({
            behavior: 'smooth',
            inline: 'nearest'
          })
        }
      }

    },
    goToSubCategory(subCat, category) {
      
      this.hideShowMore = true
      this.subCategorySelected = subCat
      if (this.tenantID === "34") {

        var stringg1='الرجاء الانتظار قليلا'

          if(this.lang==='en'){
           stringg1='Please wait a bit'        
          }
          if(this.lang==='ar'){
              stringg1='الرجاء الانتظار قليلا'      
          }



      this.$swal.fire(stringg1)
      this.$swal.showLoading()          


        axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=${subCat.categoryId}&SubCategoryId=${subCat.subcategoryId}&PageSize=${this.pageSize}&PageNumber=0`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {
       
          this.$swal.close();

          this.items = []
          this.categorySelected = category
          this.categorySelected.listItemInCategories = res.data.result.listItemInCategories
          this.items.push(res.data.result.listItemInCategories)
        })
        // /api/MenuSystem/GetMenuSubCategorys?TenantId=29&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=3&PageSize=20&PageNumber=0
      } else {
        this.selectedCategory = subCat.categoryId

        this.items = []

        this.items.push(subCat)
      }
    },
    async openCart() {
      await this.$router.push({
        path: '/cart',
        query: {
          ...this.$route.query
        }
      })
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
    },
    loadMore(e) {
      let {scrollTop, clientHeight, scrollHeight} = e.target;
      if (!this.loading && scrollTop + clientHeight >= scrollHeight * 4 / 5) {

        this.loading = true;



        setTimeout(async () => {
          this.page++;
          let data = await axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=34&MenuType=${this.menuType}&CategoriesID=${this.categorySelected.categoryId}&SubCategoryId=${this.subCategorySelected.subcategoryId}&PageSize=${this.pageSize}&PageNumber=${this.page}`, {
            headers: {
              'Content-Type': 'application/json',
              'Access-Control-Allow-Origin': '*'
            }
          })
   
          this.items = []
          let subItems = data.data.result.listItemInCategories ? data.data.result.listItemInCategories : []
          this.categorySelected.listItemInCategories = this.categorySelected.listItemInCategories.concat(subItems)

          this.items.push(data.data.result.listItemInCategories)

          this.$forceUpdate()
          this.loading = false;
        }, 1000);
      }
    },
    searchForCustomerClicked(){

      debugger
    }
  },
}
</script>
