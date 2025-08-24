
<template>

    <div >

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
                             @click="menuItem.isInService ?changeMenu(menuItem):closeNav()">

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
            </span><button type="button" class="btn btn-warning" style="width: 100%;">
              <i class="fa fa-shopping-cart">{{ $t('MyOrder') }}</i></button>
 </div>


     </div>

      <div class="relative full-background">

          <div class="bodycontainer pt-0">


                    <div class="row m-0">

                      <div class="back-btn-txt cursor-pointer" @click="back()">  <i class="fas fa-angle-left arrow-icon"></i> {{ getTranslationKey("رجوع","Back") }}</div>

                      <div class="col-12 p-0 d-flex justify-content-center">
                      <div class="card card-details-img ">
                        <div class="text-center"><img class="img-fluid" :src="this.subCategorySelected.bgImag"></div>
                          <div class="text-center">{{ getTranslationKey(subCategorySelected.name,subCategorySelected.nameEnglish) }}  </div>
                      </div>

                      </div>


                        </div>


 <h1>{{ $t('ChooseItem') }} </h1>

                        <div id="scrolling_div" class="Item">
                            <div class="row m-0">
                               <div class="product-block"

                                 v-for="(items) of items"
                                 :key="items.id"
                                 :id="'items'+items.id"
                                @click="items.isInService ?goToItem(items):''"
                               >

                                   <div class="row m-0">
                                      <div class="col-3 col-md-2">  <div class="img-block"><img class="img-fluid" :src="items.imageUri" ></div>  </div>
                                      <div class="col-5 col-md-6 d-flex align-items-center">

                                        <div class="border-right">
                                        <div v-if="lang0 === 'ar'" class="title ARClass" style="word-wrap: break-word; font-size: 18px;"> {{ getTranslationKey(items.itemName, items.itemNameEnglish) }} </div>
                                            <div v-else  class="title  ENClass itemName" style="font-weight: bold;word-wrap: break-word; font-size: 18px;"> {{ getTranslationKey(items.itemName, items.itemNameEnglish) }} </div>
                                          <div v-if="lang0 === 'ar'" class="description ARClass" style="word-wrap: break-word; font-size: 15px;"> {{ getTranslationKey(items.itemDescription, items.itemDescriptionEnglish) }} </div>                             <div v-else  class="description ENClass " style="word-wrap: break-word; font-size: 15px;"> {{ getTranslationKey(items.itemDescription, items.itemDescriptionEnglish) }} </div>


                                        </div>



                           </div>
     <div class="col-3 col-md-3 d-flex align-items-center">


        <div v-if="lang0 === 'ar'"  class="ARClass">
            <div class="product-Oldprice" v-if="items.oldPrice != null && items.oldPrice != 0 && items.oldPrice != items.price " > {{ toCurrencyFormat(items.oldPrice) }} </div>
            <div class="product-price" v-if="items.price>0" > {{ toCurrencyFormat(items.price) }} </div>
            <div v-if="!items.isInService">
                <div   > {{ $t('UNAVAILABLE_ITEM') }} </div>
            </div>

        </div>
        <div v-else class="ENClass">
            <div class="product-Oldprice" v-if="items.oldPrice != null && items.oldPrice != 0 && items.oldPrice != items.price " > {{ toCurrencyFormat(items.oldPrice) }} </div>
            <div class="product-price" v-if="items.price>0" > {{ toCurrencyFormat(items.price) }} </div>
            <div v-if="!items.isInService">
                <div   > {{ $t('UNAVAILABLE_ITEM') }} </div>
            </div>
        </div>


     </div>
     <div class="col-1 col-md-1 d-flex align-items-center justify-content-between">
           <i v-if="lang0 === 'en'" class="fas fa-angle-right arrow-icon"></i>
                  <i v-else class="fas fa-angle-left arrow-icon"></i>

      </div>



</div>


                               </div>

                      </div>
                           <br>



                         </div>


                           <!--
                             <div class="container d-flex flex-row checkout-btn cart-btn" >
                             <div class="cart-icon" v-if=" $store.state.item.cart.length>0"><span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span></div>
                             <div class="row" style="width: 100%;" >
                             <div class="col-6" @click="GoToMenu()" ><button type="button" class="btn btn-warning" style="width: 100%;"><i class="fas fa-hamburger">Menu</i></button></div>
                             <div class="col-6"  @click="$store.state.item.cart.length>0 ?openCart():''" ><span class="count">{{$store.state.item.cart.length}}</span><button type="button" class="btn btn-warning" style="width: 100%;"><i class="fa fa-shopping-cart">Cart</i></button></div>

                             </div>



                             </div> -->


        </div>




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
      selectedSubCategory:0,
      SubCategoryList: [],
      CategoryList: [],
      MenuList: [],
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
      oldlinght:-1,
      SortValue:0,
        onChangeD(e) {
              this.SortValue=e.target.value;
              console.log(e.target.value);
              this.goToSubCategory(this.subCategorySelected, this.categorySelected)

          }
    }
  },

  beforeMount() {

    debugger

    this.lang0 = this.$route.query.lang === 'ar' ? 'ar' : 'en'
    this.lang = this.$route.query.lang === 'ar' ? 'ar' : 'en'
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber
    this.selectedSubCategory = this.$route.query.SubcategoryId
    this.getTenantInfo();
    this.GetMenu();
      debugger
      if(JSON.parse(localStorage.getItem('SubCategoryList'))!=null){
debugger
         this.SubCategoryList = JSON.parse(localStorage.getItem('SubCategoryList'))
         this.selectedSubCategory = JSON.parse(localStorage.getItem('SubCategory'))
            this.goToSubCategory(this.selectedSubCategory)
     }
        if(this.selectedSubCategory==null){
        this.goToSubCategory(this.SubCategoryList[0])
      }



  },
  updated() {
    if (this.$store.getters.dynamicComponent) {
      console.log('This component has been mounted');
      //localStorage.removeItem('categorySelected');
      //localStorage.removeItem('logoImag');
    }
  },

  mounted() {
   // this.openNav()
   // window.addEventListener('touchend', this.loadMore);
   // window.addEventListener('wheel', this.loadMore);

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

    getTenantInfo() {
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

    },

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

    getAllCategory() {

        // axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=3&PageSize=20&PageNumber=0&LanguageBotId=${this.languageBot}`, {
        axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuItem?TenantId=${this.tenantID}&MenuType=${this.menuType}&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0`, {
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
                this.goToCategory(this.categories[0])
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
    async goToMenuItem(menuItem,category) {
      localStorage.setItem('menuItem', JSON.stringify(menuItem));

        if(category==null){
          await this.$router.push({
                 path: '/MenuCat',
                 query: {
                     ...this.$route.query
                     //categoryId: -1
                      }
                  })

        }else{
          await this.$router.push({
                 path: '/MenuCat',
                 query: {
                     ...this.$route.query,
                     categoryId: category.id
                      }
                  })

        }
    },
    goToCategory(menuItem,category) {
         debugger
         this.goToMenuItem(menuItem,category)

    },
    goToSubCategory(subCat, category) {

      debugger
      this.subCategorySelected = subCat
      this.selectedSubCategory=subCat.id
        axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetAllItems?tenantID=${this.tenantID}&menuType=${this.menuType}&itemSubCategoryId=${subCat.id}`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {

          this.items =res.data.result
           this.$forceUpdate()

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
    loadMore(e) {

      if(this.search.length == 0){


    if(document.body.scrollTop+900 + document.body.offsetHeight >= document.body.scrollHeight)
      {

      let {scrollTop, clientHeight, scrollHeight} = e.target;
      if (!this.loading && scrollTop + clientHeight >= scrollHeight * 4 / 5) {


        this.loading = true;

        setTimeout(async () => {
          this.page++;
          let data = await axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=34&MenuType=${this.menuType}&CategoriesID=${this.categorySelected.categoryId}&SubCategoryId=${this.subCategorySelected.subcategoryId}&PageSize=${this.pageSize}&PageNumber=${this.page}&Search=null&IsSort=${this.SortValue}`, {
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

         axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0&Search=${this.search.toLowerCase()}&IsSort=${this.SortValue}`, {
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
 OpenNav() {

    document.getElementById("sidebar-1").style.visibility = "visible";
},
closeNav() { 
   $('.close.text-dark').trigger('click');
   }
,
    async back() {


      await this.$router.push({
        path: '/SubCategory',
        query: {
          ...this.$route.query
        }
      })
    },
        async GoToMenu() {

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
},
  onChangeD( event){
    debugger
}

}


</script>
