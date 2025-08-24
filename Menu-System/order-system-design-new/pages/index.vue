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
          <div class="about_us" @click="changeLang()">
            <p class="truncate mb-0">
              {{this.tname}}
            </p>
          </div>
        </div>
      </div>
    </div>
    <div class="container-fluid items">
      <div class="d-flex mb-2">
        <div class="relative w-100">
          <input v-model="search" class="search_input" type="text" name="" :placeholder="$t('SEARCH')">
          <a href="#" class="search_icon"><i class="fas fa-search" /></a>
        </div>
      </div>
      <div class="d-flex flex-column category-items">
        <nav
          v-dragscroll="true"
          class="navbar navbar-expand align-center px-0 dragscroll overflow-hidden"
        >
          <div
            v-for="(category,i) of categories"
            :key="i"
            class="nav-item nav-link"
            :class="{'active':selectedCategory===category.categoryId}"
            @click="goToCategory(category)"
          >
            {{ getTranslationKey(category.categoryName, category.categoryNameEnglish) }}
          </div>
        </nav>
        <!--        <nav-->
        <!--          v-if="categories.length>0&&selectedCategory===categories[0].categoryId"-->
        <!--          v-dragscroll="true"-->
        <!--          class="navbar navbar-expand  align-center px-0 dragscroll overflow-hidden"-->
        <!--        >-->
        <!--          <div-->
        <!--            v-for="(category,i) of categories"-->
        <!--            :key="i"-->
        <!--            class="nav-item nav-link"-->
        <!--            :class="{'active':selectedCategory===category.categoryId}"-->
        <!--            @click="goToCategory(category)"-->
        <!--          >-->
        <!--            {{ getTranslationKey(category.categoryName, category.categoryNameEnglish) }}-->
        <!--          </div>-->
        <!--        </nav>-->
        <div class="scroll-arrow" />
      </div>
      <div
        v-for="(category,categoryIndex) of items"
        :id="'category'+category.categoryId"
        :key="category.id"
        style="scroll-margin-top: 50px;"
      >
        <div v-if="filteredList(category.listItemInCategories).length>0" class="collection-title d-flex justify-start">
          {{ getTranslationKey(category.categoryName, category.categoryNameEnglish) }}
        </div>
        <div class="row">
          <div
            v-for="(item,itemIndex) of filteredList(category.listItemInCategories)"
            :key="item.id"
            class="panel-wrapper col-sm-12 col-md-6 p-0"
            :class="{'last-item':(itemIndex=== category.listItemInCategories.length - 1)&&categoryIndex===items.length-1,'opacity-5':!item.isInService}"
          >
            <div
              class="product-panel d-flex flex-row justify-content-between px-3 py-2"
              @click="item.isInService?goToItem(item):''"
            >
              <div class="product-info d-flex flex-column flex-grow justify-content-between">
                <div class="title overflow-hidden mb-1">
                  {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                </div>
                <div class="description truncate">
                  {{ getTranslationKey(item.itemDescription, item.itemDescriptionEnglish) }}
                </div>
                <div class="d-flex justify-content-start mb-2">
                  <div v-if="item.isInService" class="product-price">
                    <span v-if="item.price>0">
                      {{ toCurrencyFormat(item.price) }}
                    </span>
                    <span v-else>{{ $t('FREE') }}</span>
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
import { dragscroll } from 'vue-dragscroll'

export default {
  components: {
    CoolLightBox
  },
  directives: {
    dragscroll
  },
  data () {
    return {
      logoImag:'',
      bgImg:'',
       tname:'',
        tnameEnglish:'',
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
      loadingSendOrder: false,
      lang: 'ar',
      isLangChange: false
    }
  },
  beforeMount () {
       this.bgImg="https://www.wmadaat.com/upload/white/white1.jpg"
      this.logoImag="https://www.wmadaat.com/upload/white/white1.jpg"

    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber

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
  mounted () {
    // this.changeLang()
  },
  methods: {
    filteredList (item) {
      if (this.search.length > 0) {
        return item.filter((post) => {
          return post.itemName.toLowerCase().includes(this.search.toLowerCase())
        })
      }
      return item
    },
    openImage (image) {
      this.selectedImage = [image]
      this.imageIndex = 0
    },
    getAllCategory () {

      axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetInfoTenant?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.languageBot}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res1) => {

debugger
        this.logoImag=res1.data.result.logoImag
           this.bgImg=res1.data.result.bgImag

           this.tname=res1.data.result.name
           this.tnameEnglish=res1.data.result.nameEnglish

     axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuItem2?TenantID=${this.tenantID}&MenuType=${this.menuType}&LanguageBotId=${this.languageBot}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        this.categories = res.data.result
        this.items = res.data.result
        this.$store.dispatch('item/setSelectedItems', this.items)
        this.$forceUpdate()
      })


      })

 
    },
    async goToItem (item) {
      await this.$store.dispatch('item/setSelectedItem', item)

      await this.$router.push({
        path: '/product-info',
        query: {
          ...this.$route.query,
          itemId: item.id
        }
      })
    },
    goToCategory (category) {
      this.selectedCategory = category.categoryId
      document.querySelector(`#category${category.categoryId}`).scrollIntoView({
        behavior: 'smooth',
        inline: 'nearest'
      })
    },
    async openCart () {
      await this.$router.push({
        path: '/cart',
        query: {
          ...this.$route.query
        }
      })
    },
    changeLang () {
      this.lang = this.$route.query.lang === 'ar' ? 'en' : 'ar'
      this.$router.push({
        path: this.$router.currentRoute.path,
        query: {
          ...this.$route.query,
          lang: this.lang
        }
      })
    }
  }
}
</script>
<style scoped>

</style>
