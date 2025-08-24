
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

<template>
  <div style="position: absolute; z-index: 99999999999999999999;">
    <b-button v-b-toggle.sidebar-1><i class="fas fa-align-justify"></i></b-button>
    <b-sidebar id="sidebar-1" title="Menu" shadow style="background-color: #ffc423;">
      <div class="px-3 py-2">
                   <div
                        v-for="(menuItem) of MenuList"
                        :key="menuItem.id"
                        :id="'menuItem'+menuItem.id" >
          
                              <div  class="row" style=" padding: 2%;box-shadow: 0 2px 2px #d69c00;">         
                                  <button type="button" class="btn btn-warning" style="color: #fff; font-size: 35px; font-family: 'Flama Condensed Medium',sans-serif; width: 100%; height: 65px;"> {{ getTranslationKey(menuItem.menuName, menuItem.menuNameEnglish) }} </button>
                             </div>          

                               <div
                                   v-for="(Categorys) of menuItem.getCategorysModels"
                                   :key="Categorys.id"
                                    @click="menuItem.isInService ?goToCategory(menuItem,Categorys):goisInService()"                                   
                                   :id="'menuItem'+Categorys.id" >
          
                                        <div  class="row" style="border-bottom: 1px solid #ffd256; padding: 2%; font-family: 'Flama Condensed Medium',sans-serif;">         
                                            <button type="button" class="btn btn-warning" style="width: 100%; height: 65px;">{{ getTranslationKey(Categorys.name, Categorys.nameEnglish) }}</button>
                                        </div>          
                                 </div>                             
                     </div>
      </div>
    </b-sidebar>
  </div>
</template>

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


      <div style="height: 17px; top: 100%; position: absolute; width: 100%; background-color: transparent; background-image: linear-gradient(180deg, #FFDD02 0%, #FFA201 100%);"> </div>


    </div>




<div class="container mt-4 mb-5">

  <div class="row mt-2 maninmenurow" >
   
    <div id="scrolling_div" class="col-4 col-sm-2 col-md-4 mt-3"
    
     v-for="(menuItem2,menuItemIndex) of MenuList"
            :key="menuItem2.id"
            :id="'menuItem2'+menuItem2.id"
            
           @click="menuItem2.isInService ?goToMenuItem(menuItem2):goisInService()">
      <div class="card card-block">
     
    <img :src="menuItem2.imageUri" alt="Photo of sunset">
    <button type="button" class="btn btn-warning" style="width: 100%; height:58px;">{{ getTranslationKey(menuItem2.menuName, menuItem2.menuNameEnglish) }}</button>

  </div>
    </div>
    
    
  </div>
  
</div>



    


        <div class="container d-flex flex-row checkout-btn cart-btn" >
        <div class="cart-icon" v-if=" $store.state.item.cart.length>0"><span class="cart-count px-3">{{ toCurrencyFormat(getTotal()) }}</span></div>
        <div class="row" style="width: 100%;" >
        <div class="col-6" @click="GoToMenu()" ><button type="button" class="btn btn-warning" style="width: 100%;"><i class="fas fa-hamburger">Menu</i></button></div>
        <div class="col-6"  @click="$store.state.item.cart.length>0 ?openCart():''" ><span class="count">{{$store.state.item.cart.length}}</span><button type="button" class="btn btn-warning" style="width: 100%;"><i class="fa fa-shopping-cart">Cart</i></button></div>
        
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
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.phone = this.$route.query.PhoneNumber      
    this.getTenantInfo();
    this.GetMenu();
      

  },
  updated() {
    if (this.$store.getters.dynamicComponent) {
      console.log('This component has been mounted');
      //localStorage.removeItem('categorySelected');
      //localStorage.removeItem('logoImag');
    }
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

    getTenantInfo() {
      //if(JSON.parse(localStorage.getItem('logoImag'))!=null){
     if(false){
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

                axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenu?TenantID=${this.tenantID}&menu=${this.menuType}`, {
                      headers: {
                                'Content-Type': 'application/json',
                                'Access-Control-Allow-Origin': '*'
                               } }).then((res1) => {  

                              this.MenuList = res1.data.result
                               localStorage.setItem('MenuList', JSON.stringify(this.MenuList));
                          })

     
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
                this.goToCategory(this.categories[0], true)
          }
          this.$forceUpdate()
        })


      
    },
      async goisInService() {
       var stringg1='نأسف '

          if(this.lang==='en'){
           stringg1='Sorry'        
          }
          if(this.lang==='ar'){
              stringg1='نأسف '      
          }

  var stringg2='انتهى وقت القائمة'

          if(this.lang==='en'){
           stringg2='List time is over'        
          }
          if(this.lang==='ar'){
              stringg2='انتهى وقت القائمة'      
          }

// var stringg1='dsfds';// $t('Sorry') 
 //var stringg2='dsfsd';//$t('SorryBody') 
      this.$swal.fire( {
         title: stringg1,
         text: stringg2 ,
         imageUrl: 'https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/26151-mcdonalds-social-distancing.gif',
         imageWidth: 400,
         imageHeight: 200,
        imageAlt: 'Custom image'})
      
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
    async GoToMenu() {

      await this.$router.push({
        path: '/',
        query: {
          ...this.$route.query
        }
      })
    },    
    goToCategory(menuItem,category) {
       this.goToMenuItem(menuItem,category)

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

        axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=${subCat.categoryId}&SubCategoryId=${subCat.subcategoryId}&PageSize=${this.pageSize}&PageNumber=0&Search=null&IsSort=${this.SortValue}`, {
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
},
  onChangeD( event){
    debugger
}

}


</script>