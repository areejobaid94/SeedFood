<template>
  <div class="product-details">


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
            </span><button type="button" class="btn btn-warning" style="width: 100%;">
              <i class="fa fa-shopping-cart">{{ $t('MyOrder') }}</i></button>
 </div>


     </div>

      <div class="bodycontainer pt-0" >

    <div class="d-flex justify-content-between">
      <h1>{{ $t('Crispy') }}</h1>
    <div class="back-btn-txt cursor-pointer " @click="back()"> <i class="fas fa-angle-left arrow-icon"></i> {{ getTranslationKey("رجوع","Back") }}</div>
  </div>

    <form method="post" @submit="checkOut">


        <div class="d-flex flex-column justify-content-start">

          <div id="extra" class="d-flex flex-column justify-content-start ">
            <div class="d-flex flex-column ">
              <div
                v-if="selectedExtraChoices&&selectedExtraChoices.length>0"
                class=" d-flex justify-content-start specification-title"
              >
              
              </div>
              <div>
                <ul class="row m-0">
                  <li
                    v-for="(extra,index) in selectedExtraChoices"
                    :id="'category-'+extra.id+'-'+index"
                    :key="extra.id+'ex'"
                    class="specification-chioce-title col-md-4 col-6 p-2"
                  >
                  <div class="card condiments-card">


                    <img class="condiments-img" :src="extra.imageUri" >

                    <div class="flex flex-column " style="white-space: initial;" v-if="extra.isInService">
                      <div class="mb-2 d-flex flex-column justify-content-center">
                        <div class="condiments-title text-center"> {{ getTranslationKey(extra.name, extra.nameEnglish) }}</div>

                      </div>

                    </div>
                    <div style="opacity: 0.5" class="flex flex-row justify-content-center" v-else>
                      <div class="mb-2 d-flex flex-row justify-content-between" >
                        <div > {{ getTranslationKey(extra.name, extra.nameEnglish) }}</div>

                      </div>
                      <div>

                        <span>{{ $t('UNAVAILABLE_ITEM') }}</span>

                      </div>
                    </div>

<div class="condiments-qty text-center">

                        <div class="d-flex flex-row qty-button-holder m-auto">
                          <button
                            type="button"
                            class="qty-button minus-button small-qty-btn"
                            :disabled="extra.quantity===0"
                            @click="minExtraQty(index)"
                          ></button>
                          <span :key="extra.quantity" class="px-3">{{
                              extra.quantity ? extra.quantity : getExtraQty(extra)
                            }}</span>
                          <button
                            type="button"
                            class="qty-button plus-button small-qty-btn"
                            @click="plusExtraQty(index)"
                          ></button>
                        </div>

                      </div>

                      <div class="flex flex-column" v-if="extra.isInService">
                      <div class="mb-2 d-flex flex-column justify-content-center">
                        <div class="text-center text-green"><span>   {{ extra.price > 0 ? toCurrencyFormat(extra.price) : $t('FREE') }}</span></div>
                      </div>
</div>

</div>

                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>



<br />
 <br />

      <div class="container d-flex flex-row justify-content-center cart-btn" >
      <!-- <div class="back-btn cursor-pointer mp-" @click="back()">
         <i class="fas fa-angle-left arrow-icon  mr-1"></i> Back
      </div>  -->
      <button type="submit" class="btn btn-checkout  ">
        {{ $t('ADD_TO_CART') }} ( {{
          toCurrencyFormat(totalExtra)
        }} )
           </button>
        </div>

    </form>
    </div>
  </div>
</template>
<script>
import axios from 'axios'
import {dragscroll} from 'vue-dragscroll'

export default {
  name: 'Crispy',
  directives: {
    dragscroll
  },
  data() {
    return {
      qty: 1,
      itemPrice: 4,
      total: 0,
      item: null,
      itemCon:null,
      itemCard:[],
      tenantID: '',
      contactId: '',
      menuType: '',
      languageBot: '',
      lang2: 'ar',
      totalExtra: 0,
      selectedChoices: [],
      selectedExtraChoices: [],
      totalChoise: 0
    }
  },
  beforeMount() {
    this.tenantID = this.$route.query.TenantID
    this.contactId = this.$route.query.ContactId
    this.menuType = this.$route.query.Menu
    this.languageBot = this.$route.query.LanguageBot
    this.lang2 = this.$route.query.lang
    this.isMixSelect = false
 this.GetMenu();
    this.isSelectEMix = true

    this.IdSelector = 0

    this.GetCrispy()

   // this.goTop()
  },
  mounted() {



    if (this.item) {
      this.total = this.item.price
    }
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

    plusQty() {
       if(this.qty < 9){
      this.qty += 1
      this.updateTotal()
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
    },
    minQty() {
      if (this.qty > 1) {
        this.qty -= 1
      }
      this.refundTotal()
    },
    plusExtraQty(index) {

     if(this.selectedExtraChoices[index].quantity < 9)
      {
      this.selectedExtraChoices[index].quantity += 1
      this.totalExtra += this.selectedExtraChoices[index].price
      console.log(this.totalExtra)
      this.addExtraTotal(this.selectedExtraChoices[index].price)
      this.$forceUpdate()
      }
    },
    minExtraQty(index) {

      if (this.selectedExtraChoices[index].quantity && this.selectedExtraChoices[index].quantity > 0) {
        this.selectedExtraChoices[index].quantity -= 1
        const extraItem = this.selectedExtraChoices[index]
        if (!extraItem.selected) {
          this.totalExtra -= this.selectedExtraChoices[index].price
        } else {
          extraItem.selected = true
        }

        this.refundTotalExtra(this.selectedExtraChoices[index].price)
        this.$forceUpdate()
      }
    },
    async checkOut(e) {
      e.preventDefault()

      const extraAdded = this.getExtraAdded()
if (extraAdded.length > 0 ){
      const item = {
        quantity: this.qty,
        unitPrice: 0,
        total: this.totalExtra,
        discount: 0,
        itemId: 0,
        itemName: "كرسبي",
        isCrispy:true,
        itemNameEnglish: "Crispy",

        imageUri:require('assets/images/PieceCrispyChicken.png'),
        itemDescription: "Crispy",
        itemDescriptionEnglish: "Crispy",
        createExtraOrderDetailsModels: extraAdded,
        itemSpecifications: this.selectedChoices
      }
      await this.$store.dispatch('item/addItemToCard', item)
      this.back()
    }
    },
    async back() {

      await this.$router.push({
        path: '/cart',
        query: {
          ...this.$route.query
        }
      })
    },
    updateTotal(totalExtra = this.totalExtra, totalChoice = this.totalChoise) {
      // debugger
      // if(this.tenantID==34 && this.item.viewPrice==0){

      // this.total = this.qty * (this.item.viewPrice + totalExtra + totalChoice)

      // }else
      // {
      // this.total = this.qty * (this.item.price + totalExtra + totalChoice)
      // }

    },
    addExtraTotal(price) {
      this.total += this.qty * price
    },
    refundTotal(totalExtra = this.totalExtra, totalChoice = this.totalChoise) {
      console.log(this.total, this.qty, (this.total / (this.qty + 1)))
      this.total = this.total - (this.total / (this.qty + 1))
    },
    refundTotalExtra(price) {
      this.total -= (price * this.qty)
    },
    addChoice(event, category, choice, isCheckbox, i = 0) {

      if (!isCheckbox) {
        const prvSelected = this.getChoiceRadioButton(category)
        if (prvSelected && prvSelected.id !== choice.id) {
          this.totalChoise -= prvSelected.price
        }
      }

      if (event.target.checked) {

        this.addToChoiceCategory(event, category, choice, isCheckbox)


        if (!isCheckbox) {
          this.isMixSelect = true;

        }


        if (category.maxSelectNumber > 0) {

          if (this.isSelectEMix) {
            this.totalChoise += Number(event.target.value)

          }

        } else {

          this.totalChoise += Number(event.target.value)
        }

        this.updateTotal()

      } else {

        this.removeSelectedChoiceCategory(category, choice)
        this.totalChoise -= Number(event.target.value)
        this.updateTotal(this.totalExtra, this.totalChoise)
      }
      const nextDiv = document.getElementById('category-' + category.id + '-' + i).nextElementSibling


      if (nextDiv && nextDiv.id && this.isMixSelect) {
        document.getElementById(nextDiv.id).scrollIntoView({behavior: 'smooth'})
      }

      if (isCheckbox) {

        this.setValidationOnCheckBox('category-' + category.id + '-' + i, !event.target.checked, category)
      }
    },
    getItemById(id) {
      axios.get(`${this.$axios.defaults.baseURL}/api/services/app/Items/GetitemView?TenantID=${this.tenantID}&id=${id}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        this.item = res.data.result


        this.$store.dispatch('item/setSelectedItem', this.item)

        if(this.tenantID==34 && this.item.viewPrice!=null && this.item.viewPrice!=-1 ){

          this.total=0

        }else{

         this.total = this.item.price
        }


        this.updateTotal()
        this.$forceUpdate()
      })
    },
    GetCrispy() {
      axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetCrispy?tenantID=${this.tenantID}&menuType=${this.menuType}`, {
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      }).then((res) => {
        debugger
        //this.itemCard = JSON.parse(JSON.stringify(this.$store.state.item.cart))

        //this.item =this.itemCard[0]




         this.itemCon= res.data.result

        debugger
        this.itemCon.forEach((item2) => {

          this.selectedExtraChoices.push({
            id: item2.id,
            quantity: 0,
            unitPrice: item2.price,
            price: item2.price,
            name: item2.name,
            nameEnglish: item2.nameEnglish,
            itemId: item2.itemId,
            itemAdditionsCategoryId: item2.itemAdditionsCategoryId,
            isInService: item2.isInService,
            imageUri:item2.imageUri
          })

        })
       this.selectedExtraChoices
        debugger
        this.updateTotal()
        this.$forceUpdate()
      })
    },
    getExtraQty(extra) {
      if (extra.quantity) {
        return extra.quantity
      } else {
        extra.quantity = 0
        return extra.quantity
      }
    },
    getExtraAdded() {
      if (!this.selectedExtraChoices) {
        return []
      }
      return this.selectedExtraChoices.filter(item => item.quantity > 0)
    },
    addToChoiceCategory(event, category, selectedChoice, isCheckbox) {
      debugger
      const index = this.selectedChoices.findIndex(categoryMain => categoryMain.id === category.id)
      if (index !== -1) {

        const indexChoices = this.selectedChoices[index].specificationChoices.findIndex(categoryMainChoices => categoryMainChoices.specificationId === selectedChoice.specificationId)
        if (indexChoices !== -1) {


          if (isCheckbox) {

            if (this.selectedChoices[index].specificationChoices.length < category.maxSelectNumber) {
              if (category.maxSelectNumber > 0) {
                this.isSelectEMix = true;
                this.isMixSelect = false;
              }
              this.IdSelector = selectedChoice.id;
              this.selectedChoices[index].specificationChoices.push(
                {
                  id: selectedChoice.id,
                  specificationChoiceDescription: selectedChoice.specificationChoiceDescription,
                  specificationChoiceDescriptionEnglish: selectedChoice.specificationChoiceDescriptionEnglish,
                  specificationId: selectedChoice.specificationId,
                  price: selectedChoice.price
                })
              if (this.selectedChoices[index].specificationChoices.length + 1 > category.maxSelectNumber) {
                this.isMixSelect = true;
              }
            } else {
              if (category.maxSelectNumber > 0) {
                this.isSelectEMix = false;
                event.target.checked = false;
                this.isMixSelect = true;
              }

            }


          } else {

            if (category.maxSelectNumber > 0) {


              this.isMixSelect = false;
            }
            this.selectedChoices[index].specificationChoices.splice(indexChoices, 1)


            this.IdSelector = selectedChoice.id;
            this.selectedChoices[index].specificationChoices.push(
              {
                id: selectedChoice.id,
                specificationChoiceDescription: selectedChoice.specificationChoiceDescription,
                specificationChoiceDescriptionEnglish: selectedChoice.specificationChoiceDescriptionEnglish,
                specificationId: selectedChoice.specificationId,
                price: selectedChoice.price
              })
          }


        } else {

          this.selectedChoices[index].specificationChoices.push(
            {
              id: selectedChoice.id,
              specificationChoiceDescription: selectedChoice.specificationChoiceDescription,
              specificationChoiceDescriptionEnglish: selectedChoice.specificationChoiceDescriptionEnglish,
              specificationId: selectedChoice.specificationId,
              price: selectedChoice.price
            })

        }
      } else {

        if (category.maxSelectNumber > 0) {

          this.isMixSelect = false

          this.isSelectEMix = true
        }

        const newCategory = {
          id: category.id,
          itemId: this.item.id,
          specificationDescription: category.specificationDescription,
          specificationDescriptionEnglish: category.specificationDescriptionEnglish,
          specificationChoices: []
        }

        newCategory.specificationChoices.push({
          id: selectedChoice.id,
          specificationChoiceDescription: selectedChoice.specificationChoiceDescription,
          specificationChoiceDescriptionEnglish: selectedChoice.specificationChoiceDescriptionEnglish,
          specificationId: selectedChoice.specificationId,
          price: selectedChoice.price
        })
        this.selectedChoices.push(newCategory)
      }
    },
    removeSelectedChoiceCategory(category, selectedChoice) {
      const index = this.selectedChoices.findIndex(categoryMain => categoryMain.id === category.id)
      if (index !== -1) {
        const choiceIndex = this.selectedChoices[index].specificationChoices.findIndex(choice => choice.id === selectedChoice.id)
        if (choiceIndex !== -1) {

          this.isMixSelect = false;

          this.selectedChoices[index].specificationChoices.splice(choiceIndex, 1)
        }
      }
    },
    setValidationOnCheckBox(groupId, isRequired, category) {

      const inputs = document.querySelectorAll(`#${groupId} input`)
      var count = 0
      for (let i = 0; i < inputs.length; i++) {


        if (inputs[i].checked) {
          count++
        }


        if (count == category.maxSelectNumber) {

          break
        }

        if (isRequired) {

          inputs[i].setAttribute('required', true)
        }

      }


      if (count == category.maxSelectNumber || !category.isRequired) {

        for (let i = 0; i < inputs.length; i++) {
          inputs[i].removeAttribute('required')

        }

      }

    },
    getChoiceRadioButton(category) {
      const index = this.selectedChoices.findIndex(categoryMain => categoryMain.id === category.id)
      if (index !== -1) {
        return this.selectedChoices[index].specificationChoices[0]
      }
      return 0
    },
      async openCart() {
      await this.$router.push({
        path: '/cart',
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
     }

      , async changeMenu(menuItem) {
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
  }
}
</script>

<style scoped>
@supports (-webkit-appearance: none) or (-moz-appearance: none) {
  input[type=checkbox],
  input[type=radio] {
    --active: #275EFE;
    --active-inner: #fff;
    --focus: 2px rgba(39, 94, 254, .3);
    --border: #BBC1E1;
    --border-hover: #275EFE;
    --background: #fff;
    --disabled: #F6F8FF;
    --disabled-inner: #E1E6F9;
    -webkit-appearance: none;
    -moz-appearance: none;
    height: 21px;
    outline: none;
    display: inline-block;
    vertical-align: top;
    position: relative;
    margin: 0;
    cursor: pointer;
    border: 1px solid var(--bc, var(--border));
    background: var(--b, var(--background));
    transition: background 0.3s, border-color 0.3s, box-shadow 0.2s;
  }

  input[type=checkbox]:after,
  input[type=radio]:after {
    content: "";
    display: block;
    left: 0;
    top: 0;
    position: absolute;
    transition: transform var(--d-t, 0.3s) var(--d-t-e, ease), opacity var(--d-o, 0.2s);
  }

  input[type=checkbox]:checked,
  input[type=radio]:checked {
    --b: var(--active);
    --bc: var(--active);
    --d-o: .3s;
    --d-t: .6s;
    --d-t-e: cubic-bezier(.2, .85, .32, 1.2);
  }

  input[type=checkbox]:disabled,
  input[type=radio]:disabled {
    --b: var(--disabled);
    cursor: not-allowed;
    opacity: 0.9;
  }

  input[type=checkbox]:disabled:checked,
  input[type=radio]:disabled:checked {
    --b: var(--disabled-inner);
    --bc: var(--border);
  }

  input[type=checkbox]:disabled + label,
  input[type=radio]:disabled + label {
    cursor: not-allowed;
  }

  input[type=checkbox]:hover:not(:checked):not(:disabled),
  input[type=radio]:hover:not(:checked):not(:disabled) {
    --bc: var(--border-hover);
  }

  input[type=checkbox]:focus,
  input[type=radio]:focus {
    box-shadow: 0 0 0 var(--focus);
  }

  input[type=checkbox]:not(.switch),
  input[type=radio]:not(.switch) {
    width: 21px;
  }

  input[type=checkbox]:not(.switch):after,
  input[type=radio]:not(.switch):after {
    opacity: var(--o, 0);
  }

  input[type=checkbox]:not(.switch):checked,
  input[type=radio]:not(.switch):checked {
    --o: 1;
  }

  input[type=checkbox] + label,
  input[type=radio] + label {
    font-size: 14px;
    line-height: 21px;
    display: inline-block;
    vertical-align: top;
    cursor: pointer;
    margin-left: 4px;
  }

  input[type=checkbox]:not(.switch) {
    border-radius: 7px;
  }

  input[type=checkbox]:not(.switch):after {
    width: 5px;
    height: 9px;
    border: 2px solid var(--active-inner);
    border-top: 0;
    border-left: 0;
    left: 7px;
    top: 4px;
    transform: rotate(var(--r, 20deg));
  }

  input[type=checkbox]:not(.switch):checked {
    --r: 43deg;
  }

  input[type=checkbox].switch {
    width: 38px;
    border-radius: 11px;
  }

  input[type=checkbox].switch:after {
    left: 2px;
    top: 2px;
    border-radius: 50%;
    width: 15px;
    height: 15px;
    background: var(--ab, var(--border));
    transform: translateX(var(--x, 0));
  }

  input[type=checkbox].switch:checked {
    --ab: var(--active-inner);
    --x: 17px;
  }

  input[type=checkbox].switch:disabled:not(:checked):after {
    opacity: 0.6;
  }

  input[type=radio] {
    border-radius: 50%;
  }

  input[type=radio]:after {
    width: 19px;
    height: 19px;
    border-radius: 50%;
    background: var(--active-inner);
    opacity: 0;
    transform: scale(var(--s, 0.7));
  }

  input[type=radio]:checked {
    --s: .5;
  }
}

ul {
  padding: 0;
  list-style: none;
  width: 100%;
  max-width: 100%;
}

ul li {
  margin: 0px 0  6px 0;
  position: relative;
}

.bottom-0 {
  bottom: 0
}

</style>
