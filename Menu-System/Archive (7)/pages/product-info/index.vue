<template>
  <div v-if="item" class="product-details">
    <form method="post" @submit="checkOut">
      <div class="back-btn cursor-pointer" @click="back()">
        <i class="fas fa-arrow-left"/>
      </div>
      <div class="d-flex mb-2 relative full-background">
        <div class="background-container w-100">
          <img
            class="bg-img"
            :src="item.imageUri"
          >
        </div>
      </div>
      <div v-dragscroll="true" class="p-2 container-fluid item-details overflow-hidden dragscroll mb-2">
        <div class="d-flex flex-row justify-content-between mb-3">
          <div class="text-right">
            {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
          </div>
          <div class="product-price">
            <div class="product-price">
              <span v-if="item.price>0">{{ toCurrencyFormat(item.price) }}</span>

              <span v-else>
             <!--  {{ $t('FREE') }} -->
              </span>
            </div>
          </div>
        </div>

        <div class="d-flex flex-column justify-content-start">
          <div
            v-for="(choiceCategory,i) in item.itemSpecifications"
            :id="'category-'+choiceCategory.id+'-'+i"
            :key="choiceCategory.id+'-category'+'-'+i"
            class="d-flex flex-column"
          >
            <div class="mb-2 specification-title d-flex justify-content-start">
              <span class="">{{
                  getTranslationKey(choiceCategory.specificationDescription, choiceCategory.specificationDescriptionEnglish)
                }}</span>
              <span
                v-if="choiceCategory.isRequired"
                class="required"
              >( {{ $t('REQUIRED') }})</span>

              <span
                v-if="choiceCategory.maxSelectNumber > 0"
                class="required"
              >( {{ $t('SELECTMAX') }}  {{ choiceCategory.maxSelectNumber }} )  </span>

            </div>


            <div style="text-align: right;" v-if="lang2 === 'ar'">

              <ul :id="'list-category-'+choiceCategory.id">
                <li
                  v-for="choice in choiceCategory.specificationChoices"
                  :key="choice.id+'choice'"
                  class="specification-chioce-title"
                >
                  <div class="d-flex flex-row justify-content-between" v-if="choice.isInService">
                    <div>
                      <input
                        :id="choice.id"
                        :required="choiceCategory.isRequired"
                        :name="choiceCategory.id"
                        :type="choiceCategory.isMultipleSelection?'checkbox':'radio'"
                        :value="choice.price"
                        oninvalid="this.setCustomValidity($t('REQUIRED_FIELD'))"
                        @change="addChoice($event,choiceCategory,choice,choiceCategory.isMultipleSelection,i)"
                      >
                      <label :for="choice.id">{{
                          getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                        }}</label>
                    </div>
                    <div>
                      <span class="ml-auto">
                        {{ choice.price > 0 ? toCurrencyFormat(choice.price) : $t('FREE') }}</span>
                    </div>
                  </div>

                  <div style="opacity: 0.5;" class="d-flex flex-row justify-content-between" v-else>
                    <div>
                      <input disabled
                             :id="choice.id"
                             :required="choiceCategory.isRequired"
                             :name="choiceCategory.id"
                             :type="choiceCategory.isMultipleSelection?'checkbox':'radio'"
                             :value="choice.price"
                             oninvalid="this.setCustomValidity($t('REQUIRED_FIELD'))"
                             @change="addChoice($event,choiceCategory,choice,choiceCategory.isMultipleSelection,i)"
                      >
                      <label :for="choice.id">{{
                          getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                        }}</label>
                    </div>
                    <div>
                      <span>{{ $t('UNAVAILABLE_ITEM') }}</span>
                    </div>
                  </div>

                </li>
              </ul>


            </div>
            <div style="text-align: left;" v-else>
              <ul :id="'list-category-'+choiceCategory.id">
                <li
                  v-for="choice in choiceCategory.specificationChoices"
                  :key="choice.id+'choice'"
                  class="specification-chioce-title"
                >
                  <div class="d-flex flex-row justify-content-between" v-if="choice.isInService">
                    <div>
                      <input
                        :id="choice.id"
                        :required="choiceCategory.isRequired"
                        :name="choiceCategory.id"
                        :type="choiceCategory.isMultipleSelection?'checkbox':'radio'"
                        :value="choice.price"
                        oninvalid="this.setCustomValidity($t('REQUIRED_FIELD'))"
                        @change="addChoice($event,choiceCategory,choice,choiceCategory.isMultipleSelection,i)"
                      >
                      <label :for="choice.id">{{
                          getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                        }}</label>
                    </div>
                    <div>
                      <span class="ml-auto">
                        {{ choice.price > 0 ? toCurrencyFormat(choice.price) : $t('FREE') }}</span>
                    </div>
                  </div>
                  <div style="opacity: 0.5;" class="d-flex flex-row justify-content-between" v-else>
                    <div>
                      <input
                        :id="choice.id"
                        :required="choiceCategory.isRequired"
                        :name="choiceCategory.id"
                        :type="choiceCategory.isMultipleSelection?'checkbox':'radio'"
                        :value="choice.price"
                        oninvalid="this.setCustomValidity($t('REQUIRED_FIELD'))"
                        @change="addChoice($event,choiceCategory,choice,choiceCategory.isMultipleSelection,i)"
                      >
                      <label :for="choice.id">{{
                          getTranslationKey(choice.specificationChoiceDescription, choice.specificationChoiceDescriptionEnglish)
                        }}</label>
                    </div>
                    <div>
                      <span>{{ $t('UNAVAILABLE_ITEM') }}</span>
                    </div>
                  </div>


                </li>
              </ul>
            </div>


          </div>
          <div id="extra" class="d-flex flex-column justify-content-start mb-2">
            <div class="d-flex flex-column ">
              <div
                v-if="selectedExtraChoices&&selectedExtraChoices.length>0"
                class="mb-2 font-18 d-flex justify-content-start specification-title border-b border-bottom"
              >
                {{ $t('EXTRA') }}
              </div>
              <div>
                <ul>
                  <li
                    v-for="(extra,index) in selectedExtraChoices"
                    :id="'category-'+extra.id+'-'+index"
                    :key="extra.id+'ex'"
                    class="specification-chioce-title"
                  >

                    <div class="flex flex-row" v-if="extra.isInService">
                      <div class="mb-2 d-flex flex-row justify-content-between">
                        <div> {{ getTranslationKey(extra.name, extra.nameEnglish) }}</div>
                        <div><span>   {{ extra.price > 0 ? toCurrencyFormat(extra.price) : $t('FREE') }}</span></div>
                      </div>
                      <div>

                        <div class="d-flex flex-row">
                          <button
                            type="button"
                            class="qty-button minus-button small-qty-btn"
                            :disabled="extra.quantity===0"
                            @click="minExtraQty(index)"
                          />
                          <span :key="extra.quantity" class="px-3">{{
                              extra.quantity ? extra.quantity : getExtraQty(extra)
                            }}</span>
                          <button
                            type="button"
                            class="qty-button plus-button small-qty-btn"
                            @click="plusExtraQty(index)"
                          />
                        </div>

                      </div>
                    </div>
                    <div style="opacity: 0.5;" class="flex flex-row" v-else>
                      <div class="mb-2 d-flex flex-row justify-content-between">
                        <div> {{ getTranslationKey(extra.name, extra.nameEnglish) }}</div>

                      </div>
                      <div>

                        <span>{{ $t('UNAVAILABLE_ITEM') }}</span>

                      </div>
                    </div>


                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <div class="d-flex flex-row justify-content-between">
          <div class="py-1 font-18">
            {{ $t('QTY') }}
          </div>
          <div class="d-flex flex-row">
            <button type="button" :disabled="qty===1" class="qty-button minus-button" @click="minQty"/>
            <span class="px-3 py-1">{{ qty }}</span>
            <button type="button" class="qty-button plus-button " @click="plusQty"/>
          </div>
        </div>
      </div>
      <button type="submit" class="btn checkout-btn  fixed-button">
        {{ $t('ADD_TO_CART') }} ( {{
          toCurrencyFormat(total)
        }} )
      </button>
    </form>
  </div>
</template>
<script>
import axios from 'axios'
import {dragscroll} from 'vue-dragscroll'

export default {
  name: 'ProductDetails',
  directives: {
    dragscroll
  },
  data() {
    return {
      qty: 1,
      itemPrice: 4,
      total: 0,
      item: null,
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

    this.isSelectEMix = true

    this.IdSelector = 0

    if (this.$store.state.item.selectedItem) {

      this.item = this.$store.state.item.selectedItem

      if (this.item.itemAdditionDtos) {
        this.item.itemAdditionDtos.forEach((item2) => {
          this.selectedExtraChoices.push({
            id: item2.id,
            quantity: 0,
            unitPrice: item2.price,
            price: item2.price,
            name: item2.name,
            nameEnglish: item2.nameEnglish,
            itemId: item2.itemId,
            itemAdditionsCategoryId: item2.itemAdditionsCategoryId,
            isInService: item2.isInService
          })

        })
      }

    } else {

      this.getItemById(this.$route.query.itemId)
    }
  },
  mounted() {
    if (this.item) {
      this.total = this.item.price
    }
  },
  methods: {
    plusQty() {
      this.qty += 1
      this.updateTotal()
    },
    minQty() {
      if (this.qty > 1) {
        this.qty -= 1
      }
      this.refundTotal()
    },
    plusExtraQty(index) {
      this.selectedExtraChoices[index].quantity += 1
      this.totalExtra += this.selectedExtraChoices[index].price
      console.log(this.totalExtra)
      this.addExtraTotal(this.selectedExtraChoices[index].price)
      this.$forceUpdate()
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

      const item = {
        quantity: this.qty,
        unitPrice: this.item.price,
        total: this.total,
        discount: 0,
        itemId: this.item.id,
        itemName: this.item.itemName,
        itemNameEnglish: this.item.itemNameEnglish,
        imageUri: this.item.imageUri,
        itemDescription: this.item.itemDescription,
        itemDescriptionEnglish: this.item.itemDescriptionEnglish,
        createExtraOrderDetailsModels: extraAdded,
        itemSpecifications: this.selectedChoices
      }
      await this.$store.dispatch('item/addItemToCard', item)
      this.back()
    },
    async back() {
      await this.$router.push({
        path: '/',
        query: {
          ...this.$route.query,
          itemId: -1
        }
      })
    },
    updateTotal(totalExtra = this.totalExtra, totalChoice = this.totalChoise) {
      this.total = this.qty * (this.item.price + totalExtra + totalChoice)
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
        debugger
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
        debugger
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
        this.total = this.item.price
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
      debugger
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
          debugger
          inputs[i].setAttribute('required', true)
        }

      }


      if (count == category.maxSelectNumber || !category.isRequired) {
        debugger
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
    }
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
  margin: 16px 0;
  position: relative;
}

.bottom-0 {
  bottom: 0
}

</style>
