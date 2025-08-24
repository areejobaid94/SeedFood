<template>

    <div id="main-content">

      <div id="flat-app" class="flat-app flex-col font-mirza" style="min-height: 795px;">

        <div id="main" class="container-fluid max740 flex-col flex-grow p-0 padBottom" >

          <div class="home" style="height: 100%">





       
         <div class="row banner-container" >




              <div class="x-search-input flex-row p-3 align-items-center" style="width: 100%;">

               
                
                   
                     <div class="input-container flex-grow flex-row">
                        <i class="fas fa-search icon-search"></i>
                        <input 
                         v-if="this.Language==='AR'"
                         v-model="search"
                         type="text"
                         class="form-control border-0 border-bottom"
                         name="search"
                         placeholder="ابحث في القائمة">

                        <input 
                         v-if="this.Language==='EN'"
                         v-model="search"
                         type="text"
                         class="form-control border-0 border-bottom"
                         name="search"
                         placeholder="Search for menu items">
                     </div>



                  

              </div>


              <div  id="categories-menu" class="container-fluid showArrow">

                 <div class="navigation-bar flex-row align-center"> 


                      <nav class="navbar navbar-expand align-center px-0 dragscroll" style="">
          
                      
                          <div  class="navbar-nav"  style="" >
                                   
                               <div   v-for="category in categories" >
	                                <a class="nav-item nav-link mrx-4" :href="'#category'+category.id" > {{ category.name }} </a>
                               </div>
                          </div>
                     </nav>

                     <div class="scroll-arrow">
                     </div>
                 
                 </div>

              </div>

         </div>



         <div class=" container-fluid" style="overflow-y: scroll; height:658px;scroll-behavior: smooth;overflow-x: hidden;max-width: 640pt;position: absolute;">

              <div class="row" >

                <div
                  v-for="(category,i) of items"
                  :id="'category'+category.categoryId"
                  :key="category.id"
                  class="collection col-12 my-4 px-4"
                >
                  <div
                    class="col-md-12 pt-3"
                    :style="{display:category.listItemInCategories.length>0?'block':'none'}"
                    :class="{'pt-3':i===0}"
                  >
                    <h5
                      v-if="search.length===0"
                      class="menu-title"
                    >
                      {{ category.categoryName }}
                    </h5>
                  </div>
                  <div
                    v-for="item of filteredList(category.listItemInCategories)"
                    :key="item.id"
                    class="col-lg-12 pt-3 cardlist"
                  >


                    <div class="col-lg-12 pt-3"   v-if="!isInService(item)"   style="pointer-events: none">
                      <div class="row" style="height: 90px;">
                        <div class="col-lg-12">
                          <div class="row">
                            <div class="col-3 col-lg-3 col-xl-3">
                              <div class="row">
                                <a
                                  class="w-100"
                                  @click="openImage(item.imageUri)"
                                >
                                  <div 
                                    class="item-image" style="border-radius: 20%;"
                                    :style="{backgroundImage:`url(${item.imageUri&&item.imageUri.length>1?item.imageUri:require('assets/empty-item.jpeg')})`}"
                                  />

                                </a>
                              </div>
                            </div>

                            <div class="col-6 col-lg-6 col-xl-6 text-left">
                              <div class="d-block mb-1" style="text-decoration: line-through">
                                 <a class="cartproname"> {{ item.itemName }} </a>
                              </div>
                              <div class="seller d-block" style="text-decoration: line-through">
                                <span>{{ item.itemDescription }}  </span>
                              </div>
                              <div class="cartviewprice d-block" style="text-decoration: line-through">
                                <span class="amt" style="text-decoration: line-through">    {{
                                  item.price.toLocaleString({
                                    style: 'currency',
                                    currency: 'JOD'
                                  })
                                }} JOD</span>
                              </div>
                            </div>
                            

                            <div class="col-3 col-lg-3 col-xl-3 p-0 qty">


                              <div v-if="" class="input-group">

                                <div class="input-group-prepend" >
                                  <button type="button" class="btn-qty" style="border-radius: 50%;" disabled>
                                    <i class="fa fa-minus" />
                                  </button>
                                </div>

                                <input
                                  style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                  id=""
                                  type="text"
                                  class="form-control item-qty form-control-sm text-center"
                                  aria-describedby=""
                                  :value="0"
                                  disabled
                                >
                                
                                <div class="input-group-append" @click="addItem(item)" >
                                  <button type="button" class="btn-qty" style="border-radius: 50%;"disabled>
                                    <i class="fa fa-plus" />
                                  </button>
                                </div>

                              </div>






                              <div v-if="isItemAdded(item)" class="input-group">
                                <div class="input-group-prepend" @click="removeItem(item)">
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-minus" />
                                  </button>
                                </div>
                                <input
                                  style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                  id=""
                                  type="text"
                                  class="form-control item-qty form-control-sm text-center"
                                  aria-describedby=""
                                  :value="selectedItems[getItem(item)].count"
                                  disabled
                                >
                                <div class="input-group-append" @click="addItem(item)">
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-plus" />
                                  </button>
                                </div>
                              </div>



                            </div>
                          </div>

                          <div style="margin-left: 30px;">
                            <div
                              v-for="itemAddOn in item.itemAdditionDtos"
                              :key="itemAddOn.id"
                              class="row "
                              style="margin-top: 22px;"
                            >
                              <div v-if="isItemAdded(item)" class="row " style="width: 500px;">
                                <div style="margin-right: 31px; width: 70px;">
                                  <a class="cartproname">{{ itemAddOn.name }}</a>
                                </div>

                                <div style="margin-right: 31px; width: 70px;">
                                  <span class="amt">    {{
                                    itemAddOn.price.toLocaleString({
                                      style: 'currency',
                                      currency: 'JOD'
                                    })
                                  }} JOD</span>
                                </div>

                                <div class="col-3 col-lg-3 col-xl-3 p-0 qty">
                                  <button
                                    v-if="!isItemAddOnAdded(itemAddOn)"
                                    type="button"
                                    class="btn btn-light btn-sm w-100 border"
                                    data-field="quantity"
                                    @click="addOnItem(item,itemAddOn)"
                                  >
                                    Add
                                  </button>

                                  <div v-if="isItemAddOnAdded(itemAddOn)" class="input-group">
                                    <div class="input-group-prepend" @click="removeItemAddOn(item,itemAddOn)">
                                      <button type="button" class="btn-qty" style="border-radius: 50%;">
                                        <i class="fa fa-minus" />
                                      </button>
                                    </div>
                                    <input
                                     style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                      id=""
                                      type="text"
                                      class="form-control item-qty form-control-sm text-center"
                                      aria-describedby=""
                                      :value="selectedItemsAddOn[getItemAddOn(itemAddOn)].count"
                                      disabled
                                    >
                                    <div class="input-group-append" @click="addOnItem(item,itemAddOn)">
                                      <button type="button" class="btn-qty" style="border-radius: 50%;">
                                        <i class="fa fa-plus" />
                                      </button>
                                    </div>
                                  </div>
                                </div>

                               
                              </div>
                            </div>
                          </div>

                        </div>
                      </div>
                      <div class="row">
                       <div   v-if="Language==='AR'">
                        <span  style="font-size: 23px; color: #aaa6a6; margin-left: 44px;margin-top: -58px; font-style: italic;"> هذا المنتج غير متوفر حاليا   </span>
                      
                       </div>
                       <div   v-if="Language==='EN'">                      
                       <span  style="font-size: 23px; color: #aaa6a6; margin-left: 44px;margin-top: -58px; font-style: italic;"> This product is not currently available  </span>
                       </div>
                        
                      </div>


                    </div>










                      <div class="col-lg-12"   v-else="isInService(item)"   >
                      <div class="row">
                        <div class="col-lg-12">
                          <div class="row">
                            <div class="col-3 col-lg-3 col-xl-3">
                              <div class="row" >
                                <a 
                                  class="w-100"
                                  @click="openImage(item.imageUri)"
                                >
                                  <div 
                                    class="item-image" style="border-radius: 20%;"
                                    :style="{backgroundImage:`url(${item.imageUri&&item.imageUri.length>1?item.imageUri:require('assets/empty-item.jpeg')})`}"
                                  />

                                </a>
                              </div>
                            </div>

                            <div class="col-6 col-lg-6 col-xl-6 text-left">
                              <div class="d-block mb-1">
                                <a class="cartproname">{{ item.itemName }}</a>
                              </div>
                              <div class="seller d-block">
                                <span> {{ item.itemDescription }}  </span>
                              </div>
                              <div class="cartviewprice d-block">
                                <span class="amt">    {{
                                  item.price.toLocaleString({
                                    style: 'currency',
                                    currency: 'JOD'
                                  })
                                }} JOD</span>
                              </div>
                            </div>

                            <div class="col-3 col-lg-3 col-xl-3 p-0 qty">

                              

                              <div v-if="!isItemAdded(item) && isInService(item)" class="input-group">

                                <div class="input-group-prepend" >
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-minus" />
                                  </button>
                                </div>

                                <input
                                  style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                  id=""
                                  type="text"
                                  class="form-control item-qty form-control-sm text-center"
                                  aria-describedby=""
                                  :value=0
                                  disabled
                                >
                                
                                <div class="input-group-append" @click="addItem(item)">
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-plus" />
                                  </button>
                                </div>

                              </div>



                              <div v-if="isItemAdded(item)" class="input-group">
                                <div class="input-group-prepend" @click="removeItem(item)">
                                  <button type="button" class="btn-qty">
                                    <i class="fa fa-minus" />
                                  </button>
                                </div>
                                <input
                                style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                  id=""
                                  type="text"
                                  class="form-control item-qty form-control-sm text-center"
                                  aria-describedby=""
                                  :value="selectedItems[getItem(item)].count"
                                  disabled
                                >
                                <div class="input-group-append" @click="addItem(item)">
                                  <button type="button" class="btn-qty">
                                    <i class="fa fa-plus" />
                                  </button>
                                </div>
                              </div>




                            </div>
                          </div>

                          <div style="margin-left: 30px;">
                            <div
                              v-for="itemAddOn in item.itemAdditionDtos"
                              :key="itemAddOn.id"
                             
                              style="margin-top: 22px;"
                            >
                              <div v-if="isItemAdded(item)" class="row " >

                                <div class="col-3 col-lg-3 col-xl-3 p-0 qty" >
                                  <a class="cartproname">{{ itemAddOn.name }}</a>
                                </div>

                                <div class="col-3 col-lg-3 col-xl-3 p-0 qty" >
                                  <span class="amt">    {{
                                    itemAddOn.price.toLocaleString({
                                      style: 'currency',
                                      currency: 'JOD'
                                    })
                                  }} JOD</span>
                                </div>

                                <div class="col-4 col-lg-4 col-xl-4 p-0 qty">



                              <div v-if="!isItemAddOnAdded(itemAddOn)" class="input-group">

                                <div class="input-group-prepend" >
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-minus" />
                                  </button>
                                </div>

                                <input
                                  style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                  id=""
                                  type="text"
                                  class="form-control item-qty form-control-sm text-center"
                                  aria-describedby=""
                                  :value=0
                                  disabled
                                >
                                
                                <div class="input-group-append" @click="addOnItem(item,itemAddOn)">
                                  <button type="button" class="btn-qty" style="border-radius: 50%;">
                                    <i class="fa fa-plus" />
                                  </button>
                                </div>

                              </div>






                                  <div v-if="isItemAddOnAdded(itemAddOn)" class="input-group">
                                  
                                    <div class="input-group-prepend" @click="removeItemAddOn(item,itemAddOn)">
                                      <button type="button" class="btn-qty">
                                        <i class="fa fa-minus" />
                                      </button>
                                    </div>
                                    <input
                                    style="background-color: white; border: 0px;margin-right: 3px; margin-left: 3px; padding: 4px;"
                                      id=""
                                      type="text"
                                      class="form-control item-qty form-control-sm text-center"
                                      aria-describedby=""
                                      :value="selectedItemsAddOn[getItemAddOn(itemAddOn)].count"
                                      disabled
                                    >
                                    <div class="input-group-append" @click="addOnItem(item,itemAddOn)">
                                      <button type="button" class="btn-qty">
                                        <i class="fa fa-plus" />
                                      </button>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

         </div>

         <div class="container-fluid max740 p-0" v-if="selectedItems.length>0">
                <div class="bottom-bar max740">
                     <div class="button-wrapper px-3">
                         
                         <button class="checkout-floater flex-row flex-grow flex-between align-center pushable px-3" :disabled="loadingSendOrder" @click="sendOrder()">
                            
                             <div class="cart flex" style="flex: 1 1 0%;">

                                  <div class="circular flex flex-start flex-center align-center">
                                    {{this.ItemCount}}
                                  </div>

                             </div>


                              <div class="label flex flex-center" style="flex: 1 1 0%;" >
                              <input   v-if="this.Language==='AR'" type="button"  class="btn btn-cust" value="إتمام الطلب"  >
                              <input  v-if="this.Language==='EN'" type="button"  class="btn btn-cust" value="Complete the order"  >
                                     			

                             </div>


                              <div class="price flex flex-end align-center" style="flex: 1 1 0%;">

                              <span class="amt">    {{
                                    total.toLocaleString({
                                      style: 'currency',
                                      currency: 'JOD'
                                    })
                                  }} JOD</span>
                                  

                             </div>
                          
                         </button>
                        
                      </div>
                </div>
         </div>

      </div>
        
        </div>

    

      </div>
<div  id="id01">
     <CoolLightBox   
     id="id02"  
      :items="selectedImage"
      :index="imageIndex"
      @close="imageIndex = null"
    />
    </div>

    </div>
    






    






 
  </div>































</template>

<script>

import axios from 'axios'
import CoolLightBox from 'vue-cool-lightbox'
import 'vue-cool-lightbox/dist/vue-cool-lightbox.min.css'
import vClickOutside from 'v-click-outside'





export default {
  directives: {
      clickOutside: vClickOutside.directive
    },
   created() {
    this.$root.$on("defocusApp", this.closeDialogues);
  },
  components: {
    CoolLightBox,
    vClickOutside
  },
  data () {
    return {
      items: [],
      selectedItems: [],
      selectedItemsAddOn: [],
      search: '',
      tenantID: '',
      contactId: '',
      total: 0,
      selectedImage: [],
      imageIndex: null,
      categories: [],
      loadingSendOrder: false

    }
  },

  mounted () {



    // const isOpen = this.$route.query.isOpen
    // if (!isOpen) {
    //   window.open(window.location + '&isOpen=true', '_self')
    // }
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.phone1=this.$route.query.PhoneNumber
    this.menuType=this.$route.query.Menu
    this.LanguageBot=this.$route.query.LanguageBot
    this.ItemCount=0;

    if(this.LanguageBot==1){
     this.Language='AR'
    }else{

      this.Language='EN'
    }
 
    this.getAllCategory()
    axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuItem2?TenantID=${this.tenantID}&MenuType=${this.menuType}&LanguageBotId=${this.LanguageBot}`, {
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*'
      }
    }).then((res) => {
      this.items = res.data.result
      this.$forceUpdate()
    })
  },
  methods: {


    
    externalClick () {

  
         window.onclick = function(event) {

                  var modal2 = document.getElementById('id01').onclick = function () { myFunction2() };

function myFunction2() {
  
 var button=document.getElementsByClassName("cool-lightbox-toolbar__btn");

 button[0].click();
}
       
         }

},
      closeDialogues() {
        
      this.isDialbogueOpen = false;
    },
    addItem (selectedItem) {
      const itemIndex = this.selectedItems.findIndex(item => item.id === selectedItem.id)

      if (itemIndex !== -1) {
        this.selectedItems[itemIndex].count += 1
        this.ItemCount++
      } else {
        this.ItemCount++
        selectedItem.count = 1
        this.selectedItems.push(selectedItem)
      }
      this.total = this.getTotal()
      this.$forceUpdate()
    },

    addOnItem (selectedItem, selectedItemAddOn) {
      const itemIndex = this.selectedItemsAddOn.findIndex(item => item.id === selectedItemAddOn.id)

      if (itemIndex !== -1) {
        this.selectedItemsAddOn[itemIndex].count += 1
      } else {
        selectedItemAddOn.count = 1
        this.selectedItemsAddOn.push(selectedItemAddOn)
      }
      this.total = this.getTotal()
      this.$forceUpdate()
    },
    removeItem (selectedItem) {
      const itemIndex = this.selectedItems.findIndex(item => item.id === selectedItem.id)
      if (itemIndex !== -1) {
        if (this.selectedItems[itemIndex].count - 1 === 0) {
          this.ItemCount--
          this.selectedItems.splice(itemIndex, 1)
        } else {
          this.ItemCount--
          this.selectedItems[itemIndex].count -= 1
        }
      }
      this.total = this.getTotal()

      this.$forceUpdate()
    },

    removeItemAddOn (selectedItem, selectedItemAddOn) {
      const itemIndex = this.selectedItemsAddOn.findIndex(item => item.id === selectedItemAddOn.id)
      if (itemIndex !== -1) {
        if (this.selectedItemsAddOn[itemIndex].count - 1 === 0) {
          this.selectedItemsAddOn.splice(itemIndex, 1)
        } else {
          this.selectedItemsAddOn[itemIndex].count -= 1
        }
      }
      this.total = this.getTotal()

      this.$forceUpdate()
    },

    isItemAdded (selectedItem) {
      const itemIndex = this.selectedItems.findIndex(item => item.id === selectedItem.id)
      if (itemIndex !== -1) {
        return true
      }

      return false
    },
     isInService (selectedItem) {
      if (selectedItem.isInService  == false) {
        return false
      }

      return true
    },

    isItemAddOnAdded (selectedItem) {
      const itemIndex = this.selectedItemsAddOn.findIndex(item => item.id === selectedItem.id)
      if (itemIndex !== -1) {
        return true
      }

      return false
    },
    getItem (item) {
      const itemIndex = this.selectedItems.findIndex(i => i.id === item.id)
      if (itemIndex !== -1) {
        return itemIndex
      }
      return null
    },

    getItemAddOn (item) {
      const itemIndex = this.selectedItemsAddOn.findIndex(i => i.id === item.id)
      if (itemIndex !== -1) {
        return itemIndex
      }
      return null
    },
    getTotal () {
      let total = 0
      this.selectedItems.forEach((item) => {
        total += (item.count * item.price)
      })

      this.selectedItemsAddOn.forEach((item) => {
        total += (item.count * item.price)
      })

      return total
    },

    getTotalAddOn () {
      let total = 0
      this.selectedItemsAddOn.forEach((item) => {
        total += (item.count * item.price)
      })
      return total
    }
    ,

   

    sendOrder () {

     var stringg1='الرجاء الانتظار قليلا'

          if(this.Language==='EN'){

           stringg1='Please wait a bit'
          
          }
          if(this.Language==='AR'){

              stringg1='الرجاء الانتظار قليلا'
        
          }



      this.$swal.fire(stringg1)
      this.$swal.showLoading()
      // window.close()
      if (this.selectedItems.length > 0) {
        const cartItem = []
        const cartItemExt = []
        let total = 0
        let totalExt = 0
        this.loadingSendOrder = true

        this.selectedItemsAddOn.forEach((item2) => {
          debugger
          cartItemExt.push({
            id:item2.id,
            quantity: item2.count,
            unitPrice: item2.price,
            total: item2.count * item2.price,
            name: item2.name,
             itemId: item2.itemId,
             itemAdditionsCategoryId :item2.itemAdditionsCategoryId
          })
debugger
          totalExt += (item2.count * item2.price)
        })

        this.selectedItems.forEach((item) => {
          cartItem.push({
            quantity: item.count,
            unitPrice: item.price,
            total: item.count * item.price,
            discount: 0,
            itemId: item.id,
            createExtraOrderDetailsModels: cartItemExt
          })

          total += (item.count * item.price)
        })

        total += totalExt

        const obj = {
          tenantId: this.tenantID,
          customerId: this.contactId,
          createOrderDetailsModels: cartItem,
          total,
          event: 'botjoin'
        }
      

        axios.post(`${this.$axios.defaults.baseURL}/api/MenuSystem/CreateOrder`, obj, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {



          axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/SendToBot/?CustomerId=${this.contactId}`, {
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        }).then((res) => {


          const phoneNumber='https://wa.me/' +this.phone1


          var string1='الطلب لم يكتمل بعد<br>'
          var string2='يرجى العودة الى واتسب '

          if(this.Language==='EN'){

           string1='The Order is not completed  yet<br>'
           string2='Please go back to WhatsApp '
          }
          if(this.Language==='AR'){

              string1='الطلب لم يكتمل بعد<br>'
           string2='يرجى العودة الى واتسب '
          }

          this.$swal.fire(
            string1,
            string2,
            'success'
          ).then(function () {   
               
           
            window.location = phoneNumber
          })

          
          this.selectedItems = []



        })

        })
      }
    },
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
      this.externalClick()
    },
    getAllCategory () {
      axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetAllWithTenantID?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.LanguageBot}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        this.categories = res.data.result

        this.loadingSendOrder = false
        this.$forceUpdate()

      })
    }
  }
}
</script>

<style scoped>

</style>
