
<template>

    <div> 
  
        <div class="d-flex  relative header-container">
        
        
            <img  :src="require('assets/images/MenuCategorylogo.jpg')" >
        



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



        </div>



        <div class="bodycontainer">
            <div class="row m-0">
                <div class="col-6 pt-2">
                    <div class="menucategoryheader">
                    {{ getTranslationKey(SelectedMenu.menuName, SelectedMenu.menuNameEnglish) }}
                    </div>
                </div>
                <div class="col-6 text-right change-icon pt-3">
               <!-- <img class=""  :src="require('assets/images/change.png')" > Change -->
                </div>
            </div>

            <div class="container">

            <div class="row">
            
                <div id="scrolling_div" class="col-6 col-sm-6 col-md-4 mt-3"
                v-for="(category) of SelectedMenu.getCategorysModels"
                        :key="category.id"
                     
                    @click="goToSubCategoryItems(category)">
                <div class="card card-block">
                <img :src="category.logoImag"  class="imgcard" alt="Photo of sunset">
                <button type="button" class="btn buttonCard">{{ getTranslationKey(category.name, category.nameEnglish) }}</button>

            </div>

            </div>
                
                
            </div>
            <br>

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
      SubCategoryList: [],
      SelectedMenu: [],
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
    this.categoryId = this.$route.query.categoryId     
    this.getTenantInfo();
    this.GetMenu();
      debugger
      if(JSON.parse(localStorage.getItem('menuItem'))!=null){

      this.SelectedMenu = JSON.parse(localStorage.getItem('menuItem'))

      if(this.categoryId==null){
        this.goToCategory(this.SelectedMenu.getCategorysModels[0])
      }else{
        this.SelectedMenu.getCategorysModels.forEach(cat => {
            if(cat.id==this.categoryId){
 
             this.goToCategory(cat)

            }       
        })

      }
 

      //this.MenuList.getCategorysModels

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
    async goToSubCategoryItems(SubCategory) {

        localStorage.setItem('SubCategory', JSON.stringify(SubCategory));
        localStorage.setItem('SubCategoryList', JSON.stringify(this.SubCategoryList));
      
          await this.$router.push({
                 path: '/SubCategory',
                 query: {
                     ...this.$route.query,
                      SubcategoryId: SubCategory.id
                      }
                  })      
       

       

    },
    goToCategory(category) {
        this.selectedCategory=category.id
        this.SubCategoryList=category.getSubCategorysModels
        debugger
        var x=  document.getElementById("sidebar-1") ;


        if(x!=null){
          document.getElementById("sidebar-1").style.visibility = "hidden";

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
 OpenNav() {

    document.getElementById("sidebar-1").style.visibility = "visible";
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


 async changeMenu(menuItem) {
 localStorage.setItem('menuItem', JSON.stringify(menuItem));
  this.getTenantInfo();
    this.GetMenu(); 
    if(JSON.parse(localStorage.getItem('menuItem'))!=null){
      this.SelectedMenu = JSON.parse(localStorage.getItem('menuItem'))
     }



}
,
 


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