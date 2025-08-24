
<template>
  <div > 
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

          </div>
        </div>
      </div>
    </div>


    <div id="scrolling_div" class="container-fluid items">
      <div class="d-flex mb-2">
        <div class="relative w-100">
          <input v-model="search" class="search_input" type="text" name="" :placeholder="$t('SEARCH')"  @keyup.enter="searchForCustomerClicked" >
          <a href="#" @click="searchForCustomerClicked" class="search_icon" > <i class="fas fa-search"></i> </a>
        </div>
      </div>




                                                                                               
    

         
          

     <div class="back-btn cursor-pointer" @click="goTop()" style="position: fixed; right: 0px; left: 23px; bottom: 0;">
        <i class="fas fa-arrow-up" style="color: red;"></i>
      </div> 

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
      </div>

  <!--    <CoolLightBox                 -->
  <!--      :items="selectedImage"      -->
  <!--      :index="imageIndex"         -->
  <!--      @close="imageIndex = null"  -->
  <!--    />                            -->

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
      selectSubName:'',
      counSubOn:1,
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






    if(JSON.parse(localStorage.getItem('menuType')) != this.menuType){

       localStorage.removeItem('categorySelected');
       localStorage.removeItem('logoImag');
       this.cartItems = []
       this.$store.dispatch('item/setCartData', [])
       this.selectedItems = []

    }

    localStorage.setItem('menuType', JSON.stringify(this.menuType)); 
    
    

    if(JSON.parse(localStorage.getItem('tenantID')) != this.tenantID){

       localStorage.removeItem('categorySelected');
       localStorage.removeItem('logoImag');
       this.cartItems = []
       this.$store.dispatch('item/setCartData', [])
       this.selectedItems = []

    }

    localStorage.setItem('tenantID', JSON.stringify(this.tenantID));   
   

      

     if(JSON.parse(localStorage.getItem('logoImag'))!=null){

      this.logoImag = JSON.parse(localStorage.getItem('logoImag'))
      this.bgImg = JSON.parse(localStorage.getItem('bgImg'))
      this.tname = JSON.parse(localStorage.getItem('tname'))
      this.tnameEnglish = JSON.parse(localStorage.getItem('tnameEnglish'))


     }else
     {
                axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetInfoTenant?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.languageBot}`, {
                      headers: {
                                'Content-Type': 'application/json',
                                'Access-Control-Allow-Origin': '*'
                               } }).then((res1) => {

                              localStorage.setItem('logoImag', JSON.stringify(res1.data.result.logoImag));
                              localStorage.setItem('bgImg', JSON.stringify(res1.data.result.bgImag));
                              localStorage.setItem('tname', JSON.stringify(res1.data.result.name));
                              localStorage.setItem('tnameEnglish', JSON.stringify(res1.data.result.nameEnglish));
                              
                              this.logoImag = res1.data.result.logoImag
                              this.bgImg = res1.data.result.bgImag
                              this.tname = res1.data.result.name
                              this.tnameEnglish = res1.data.result.nameEnglish
                          })
     }



debugger

   if(this.tenantID != "34" ){


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
               
               
               

   }else{

    if (localStorage.getItem('cart') && localStorage.getItem('cart').length > 0) {
      const data = JSON.parse(localStorage.getItem('cart'))
      this.$store.dispatch('item/setCartData', data)
    }
    
    
    if (localStorage.getItem('categorySelected') == null) {
      this.getAllCategory()
    } else {

      debugger

       
          this.categories = JSON.parse(localStorage.getItem('AllcategorySelected'))
          this.selectedCategory = JSON.parse(localStorage.getItem('selectedCategory'))
          this.selectedCategorySub = JSON.parse(localStorage.getItem('selectedCategorySub'))
          this.subCategorySelected= JSON.parse(localStorage.getItem('subCategorySelected'))
          this.subCategorySelected.subCategorysInItemModels=[]
          
          this.items = []
          this.categorySelected = JSON.parse(localStorage.getItem('categorySelected'))
          this.categorySelected.listItemInCategories = JSON.parse(localStorage.getItem('categorySelected_listItemInCategories'))
          this.items = JSON.parse(localStorage.getItem('items'))
          this.$forceUpdate()
   
            

      


    }

   }

  },
  updated() {
    if (this.$store.getters.dynamicComponent) {
      console.log('This component has been mounted');
      //localStorage.removeItem('categorySelected');
      //localStorage.removeItem('logoImag');
    }
  },
  created() {
    setTimeout(function(){localStorage.removeItem('categorySelected');}, 3600 *2 * 1000); 

    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber
     this.isback =this.$route.query.itemId 


debugger
          if(this.isback==null){

            this.isback=false
          }

          if(this.isback==-1){

            this.isback=true
          }

     if (performance.navigation.type == 1 && this.tenantID == "34" && !this.isback  ) {///performance.navigation.type == 1 && this.tenantID == "34"   && !this.isback

      

          console.info( "This page is reloaded" );
       localStorage.removeItem('categorySelected');
       localStorage.removeItem('logoImag');
       this.cartItems = []
       this.$store.dispatch('item/setCartData', [])
       this.selectedItems = []
         
            } else {
            console.info( "This page is not reloaded");
        }
        this.back()
  },
  
  mounted() {
    this.openNav()
    window.addEventListener('touchend', this.loadMore);
    window.addEventListener('wheel', this.loadMore);

  },
  methods: {
      
    filteredList(item) {
      
      if (this.search.length > 0) {

        if (this.tenantID === "34" ) {

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

        // axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=3&PageSize=20&PageNumber=0&LanguageBotId=${this.languageBot}`, {
        axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuItem?TenantId=${this.tenantID}&MenuType=${this.menuType}&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0`, {
           headers: {
             'Content-Type': 'application/json',
             'Access-Control-Allow-Origin': '*'
           }
         }).then((res) => {

          this.categories = res.data.result
          this.items = res.data.result
          
           localStorage.removeItem('AllcategorySelected');
           localStorage.setItem('AllcategorySelected', JSON.stringify(this.categories));

          if (this.categories[0] && this.categories[0].subCategorysInItemModels.length > 0) {
                this.goToCategory(this.categories[0], true)
          }
          this.$forceUpdate()
        })


      
    },
    async goToItem(item) {
      await this.$store.dispatch('item/setSelectedItem', item)

      await this.$router.push({
        path: '/product-info',
        query: {
          ...this.$route.query
        //   itemId: item.id
        }
      })
    },
    goToCategory(category, selectSub = false) {
        
      this.search=''
      this.page = 0
      
       if ( category.subCategorysInItemModels!=null ) {
            selectSub= true
          }else{
            selectSub= false
          }

      

      this.selectedCategory = category.categoryId
      this.selectedCategorySub = category

           localStorage.removeItem('selectedCategory');
           localStorage.removeItem('selectedCategorySub');
           localStorage.setItem('selectedCategory', JSON.stringify(this.selectedCategory));
           localStorage.setItem('selectedCategorySub', JSON.stringify(this.selectedCategorySub));


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
      this.search=''
      this.hideShowMore = true
      this.subCategorySelected = subCat
        localStorage.removeItem('categorySelected_listItemInCategories');

      localStorage.removeItem('subCategorySelected');
      localStorage.setItem('subCategorySelected', JSON.stringify(this.subCategorySelected));
     // the.selectSubName=subCat.
      if (this.tenantID === "34") {

        axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=${subCat.categoryId}&SubCategoryId=${subCat.subcategoryId}&PageSize=${this.pageSize}&PageNumber=0`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {
      
          this.items = []
          this.categorySelected = category
          this.categorySelected.listItemInCategories=[];
          this.categorySelected.listItemInCategories = res.data.result.listItemInCategories
          this.items.push(res.data.result.listItemInCategories)

          localStorage.removeItem('categorySelected');
          localStorage.removeItem('categorySelected_listItemInCategories');
          localStorage.removeItem('items');

         localStorage.setItem('categorySelected', JSON.stringify(this.categorySelected));
         localStorage.setItem('categorySelected_listItemInCategories', JSON.stringify(this.categorySelected.listItemInCategories));
         localStorage.setItem('items', JSON.stringify(this.items));

           this.$forceUpdate()
          
        })
       
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

      if(this.search.length == 0){


    if(document.body.scrollTop+900 + document.body.offsetHeight >= document.body.scrollHeight)
      {
          
      let {scrollTop, clientHeight, scrollHeight} = e.target;
      if (!this.loading && scrollTop + clientHeight >= scrollHeight * 4 / 5) {


        this.loading = true;

        setTimeout(async () => {
          this.page++;
          let data = await axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuSubCategorys?TenantId=34&MenuType=${this.menuType}&CategoriesID=${this.categorySelected.categoryId}&SubCategoryId=${this.subCategorySelected.subcategoryId}&PageSize=${this.pageSize}&PageNumber=${this.page}`, {
            headers: {
              'Content-Type': 'application/json',
              'Access-Control-Allow-Origin': '*'
            }
          })
         this.loading = false;
          this.items = []
          
          let subItems = data.data.result.listItemInCategories ? data.data.result.listItemInCategories : []
          
         this.categorySelected.listItemInCategories = this.categorySelected.listItemInCategories.concat(subItems)


         // if(subItems[0].itemCategoryId==this.categorySelected.listItemInCategories[0].itemCategoryId  && subItems[0].itemSubCategoryId ==this.categorySelected.listItemInCategories[0].itemSubCategoryId)
        //  {
         //  this.categorySelected.listItemInCategories = this.categorySelected.listItemInCategories.concat(subItems)
         // }else{

         //   this.categorySelected.listItemInCategories=[]
         //   this.categorySelected.listItemInCategories =subItems
        // }
          

          this.items.push(data.data.result.listItemInCategories)

           localStorage.removeItem('categorySelected');
           localStorage.removeItem('categorySelected_listItemInCategories');
           localStorage.removeItem('items');



         localStorage.setItem('categorySelected', JSON.stringify(this.categorySelected));
         localStorage.setItem('categorySelected_listItemInCategories', JSON.stringify(this.categorySelected.listItemInCategories));
         localStorage.setItem('items', JSON.stringify(this.items));

           this.sortedArray()
          this.$forceUpdate()
          
          
        }, 1000);
      
       }
      
      }


        
      }

    },
    searchForCustomerClicked(){
        
       if (this.tenantID === "34") {

           this.hideShowMore = true
           this.pageSize=20
       
         axios.get(`https://infofoodservices.azurewebsites.net/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0&Search=${this.search.toLowerCase()}`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {
       
           
          let subItems = res.data.result.listItemInCategories ? res.data.result.listItemInCategories : []
          if(res.data.result.listItemInCategories==null){
              this.categorySelected.listItemInCategories = [] // this.categorySelected.listItemInCategories.concat(subItems)
              this.items = []
              this.items=[]
          }else{
               this.categorySelected.listItemInCategories = res.data.result.listItemInCategories // this.categorySelected.listItemInCategories.concat(subItems)
               this.items = []
               this.items=res.data.result.listItemInCategories
          }

          this.$forceUpdate()

        })


       }

      
    }
    ,
 openNav() {
   debugger
 // document.getElementById("myNav").style.height = "100%";

       var id='item'+JSON.parse(localStorage.getItem('itemSelectedID'))

        var itemSelectedID = document.getElementById(id);

        if(itemSelectedID!=null){
                var topPos = itemSelectedID.offsetTop;           
                document.body.scrollTop = topPos;
                this.$forceUpdate()
        }
       this.$forceUpdate()
},

 closeNav() {
   debugger
   document.getElementById("myNav").style.height = "0%";
}
,
    async back() {
        
        
      await this.$router.push({
        path: '/',
        query: {
          ...this.$route.query
        }
      })
    },
     goTop() {

      //  var id='item'+JSON.parse(localStorage.getItem('itemSelectedID'))
        var itemSelectedID = document.getElementById('__layout');
        if(itemSelectedID!=null){
               // var topPos = itemSelectedID.offsetTop;           
                document.body.scrollTop = 0;
                this.$forceUpdate()
        }
       this.$forceUpdate()
     },   
 test1() {
     
     debugger
      var x=document.body.offsetTop;
        console.info( x);

         var y= document.getElementById("scrolling_div").offsetHeight;
         console.info( y);


      if(document.body.scrollTop + document.body.offsetHeight == document.body.scrollHeight)
      {

        console.info("yessssssssssssssss");
      }
    }
  },
  
  sortedArray(){
    return this.categorySelected.listItemInCategories .sort((a, b) => a.itemName - b.itemName );
}
}


</script>