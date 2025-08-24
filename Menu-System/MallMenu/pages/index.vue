
<template>
  <div>
    <div style="font-size: large">
      {{ this.ERROR }}
    </div>
    <div class="d-flex mb-2 relative full-background">
      <div class="app-langague d-flex justify-content-end" @click="changeLang">
        <span>{{ lang === "ar" ? "En" : "ar" }}</span>
      </div>
      <div class="background-container w-100">
        <img class="bg-img" :src="this.bgImg" />
      </div>

      <div class="d-flex absolute restaurant-info">
        <div class="logo flex-column">
          <img :src="this.logoImag" />
        </div>
        <div class="restaurant-name py-1">
          <!--        <div class="" style=" color: aquamarine; ">           -->
          <!--         {{this.tname}}                                        -->
          <!--        </div>                                                  -->
          <div class="about_us" @click="changeLang()"></div>
        </div>
      </div>
    </div>

    <div id="scrolling_div" class="container-fluid items">
      <div class="d-flex mb-2">
        <div class="relative w-100">
          <input
            v-model="search"
            class="search_input"
            type="text"
            name=""
            :placeholder="$t('SEARCH')"
            @keyup.enter="searchForCustomerClicked"
          />
          <a href="#" @click="searchForCustomerClicked" class="search_icon">
            <i class="fas fa-search"></i>
          </a>
        </div>

        <div class="relative w-40" style="padding: 7px" v-if="tenantID == '34'">
          <select @change="onChangeD($event)" class="SortD">
            <option value="0" selected disabled hidden>{{ $t("Sort") }}</option>
            <option value="1">{{ $t("lowest_price") }}</option>
            <option value="2">{{ $t("highest_price") }}</option>
            <option value="3">{{ $t("least_discount") }}</option>
            <option value="4">{{ $t("highest_discount") }}</option>
          </select>
        </div>
      </div>

      <!--       \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\    -->
      <div
        class="d-flex flex-column category-items"
        style="  lang === 'ar' ?    direction: rtl; text-align: right;  : direction: ltr; text-align: left;"
      >
        <nav
          v-dragscroll="true"
          class="
            navbar navbar-expand
            align-center
            px-0
            dragscroll
            overflow-hidden
          "
          style="
            font-weight: bold;
            font-size: 15px;
            white-space: nowrap;
            background: #f4f4f4;
            border-radius: 25px;
          "
        >
          <div
            v-for="(category, i) of categories"
            :key="i"
            class="nav-item nav-link"
            :class="{ active: selectedCategory === category.categoryId }"
            style="margin-left: 10px; margin-right: 10px"
            @click="goToCategory(category, true)"
          >
            {{
              getTranslationKey(
                category.categoryName,
                category.categoryNameEnglish
              )
            }}
          </div>
        </nav>

        <div>
          <div>
            <nav
              style="white-space: nowrap"
              v-dragscroll="true"
              class="
                navbar navbar-expand
                align-center
                px-0
                dragscroll
                overflow-hidden
              "
            >
              <div
                v-for="(subcategory, ii) of this.selectedCategorySub
                  .subCategorysInItemModels"
                :key="ii"
                class="nav-item nav-link"
                :class="{
                  active:
                    subCategorySelected.subcategoryId ===
                    subcategory.subcategoryId,
                }"
                style="margin-left: 10px; margin-right: 10px; font-size: 16px"
                @click="goToSubCategory(subcategory, selectedCategorySub)"
              >
                {{
                  getTranslationKey(
                    subcategory.categoryName,
                    subcategory.categoryNameEnglish
                  )
                }}
              </div>
            </nav>
          </div>
        </div>

        <!--    <br> <strong> ( {{subcategory.itemCount}} ) </strong>     -->

        <div class="scroll-arrow"></div>

        <!--      <div v-if="subCategorySelected!=null " class="collection-title " style="text-align: center; border-width: 4px;border-color: #e22b2b;border-style: solid; border-radius: 22px; ">    -->
        <!--      {{ getTranslationKey(subCategorySelected.categoryName, subCategorySelected.categoryNameEnglish) }}        -->
        <!--      </div>         -->

        <!--       \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\    -->

        <div
          v-if="!categorySelected"
          v-for="(category, categoryIndex) of items"
          :id="'category' + category.categoryId"
          :key="category.id"
          style="scroll-margin-top: 70px"
        >
          <div
            v-if="filteredList(category.listItemInCategories).length > 0"
            class="collection-title d-flex justify-start"
          >
            {{
              getTranslationKey(
                category.categoryName,
                category.categoryNameEnglish
              )
            }}
          </div>

          <div class="row">
            <div
              v-for="(item, itemIndex) of filteredList(
                category.listItemInCategories
              )"
              :key="item.id"
              :id="'item' + item.id"
              class="panel-wrapper col-sm-12 col-md-6 p-0"
              :class="{
                'last-item':
                  itemIndex === category.listItemInCategories.length - 1 &&
                  categoryIndex === items.length - 1,
                'opacity-5': !item.isInService,
              }"
            >
              <div
                class="
                  product-panel
                  d-flex
                  flex-row
                  justify-content-between
                  px-3
                  py-2
                "
                @click="item.isInService ? goToItem(item) : ''"
              >
                <div
                  class="
                    product-info
                    d-flex
                    flex-column flex-grow
                    justify-content-between
                  "
                  style="width: 40%"
                >
                  <div
                    v-if="lang0 === 'ar'"
                    class="title overflow-hidden mb-1"
                    style="direction: rtl; text-align: right; font-weight: bold"
                  >
                    {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                  </div>
                  <div
                    v-else
                    class="title overflow-hidden mb-1"
                    style="direction: ltr; text-align: left; font-weight: bold"
                  >
                    {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                  </div>

                  <div
                    class="description truncate"
                    style="direction: rtl; text-align: right; font-size: 12px"
                    v-if="lang0 === 'ar'"
                  >
                    {{
                      getTranslationKey(
                        item.itemDescription,
                        item.itemDescriptionEnglish
                      )
                    }}
                  </div>
                  <div
                    class="description truncate"
                    style="direction: ltr; text-align: left; font-size: 12px"
                    v-else
                  >
                    {{
                      getTranslationKey(
                        item.itemDescription,
                        item.itemDescriptionEnglish
                      )
                    }}
                  </div>

                  <div class="d-flex justify-content-start mb-2">
                    <div v-if="item.isInService" class="product-price">
                      <span
                        v-if="
                          item.oldPrice != null &&
                          item.oldPrice != 0 &&
                          item.oldPrice != item.price
                        "
                        style="text-decoration: line-through"
                      >
                        {{ toCurrencyFormat(item.oldPrice) }}
                      </span>
                    </div>
                  </div>

                  <div class="d-flex justify-content-start mb-2">
                    <div v-if="item.isInService" class="product-price">
                      <span
                        v-if="item.price > 0"
                        style="font-weight: bold; color: black"
                      >
                        {{ toCurrencyFormat(item.price) }}
                      </span>
                      <span v-else>
                        <!--    {{ $t('FREE') }} -->
                      </span>
                    </div>
                    <div v-else class="product-price">
                      <span>{{ $t("UNAVAILABLE_ITEM") }}</span>
                    </div>
                  </div>
                </div>

                <div
                  v-if="
                    (tenantID == '34' || tenantID == '47') &&
                    item.oldPrice != null &&
                    item.oldPrice != 0 &&
                    item.oldPrice != item.price
                  "
                  class="
                    product-image
                    cover
                    flex-column
                    position-relative
                    round
                    plx-4
                  "
                  style="width: 15%"
                >
                  <div>
                    <img class="card-img" :src="item.discountImg" />
                  </div>

                  <div>
                    <span style="font-size: large; color: crimson">{{
                      item.discount
                    }}</span>
                  </div>
                </div>

                <div
                  class="
                    product-image
                    cover
                    flex-column
                    position-relative
                    round
                    plx-4
                  "
                  style="width: 32%"
                >
                  <img :src="item.imageUri" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div
          class="row"
          v-if="
            categorySelected && categorySelected.listItemInCategories.length > 0
          "
        >
          <div
            v-for="(item, itemIndex) of filteredList(
              categorySelected.listItemInCategories
            )"
            :key="item.id"
            :id="'item' + item.id"
            class="panel-wrapper col-sm-12 col-md-6 p-0"
          >
            <div
              class="
                product-panel
                d-flex
                flex-row
                justify-content-between
                px-3
                py-2
              "
              @click="item.isInService ? goToItem(item) : ''"
            >
              <div
                class="
                  product-info
                  d-flex
                  flex-column flex-grow
                  justify-content-between
                "
                style="width: 40%"
              >
                <div
                  v-if="lang0 === 'ar'"
                  class="title overflow-hidden mb-1"
                  style="direction: rtl; text-align: right; font-weight: bold"
                >
                  {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                </div>
                <div
                  v-else
                  class="title overflow-hidden mb-1"
                  style="direction: ltr; text-align: left; font-weight: bold"
                >
                  {{ getTranslationKey(item.itemName, item.itemNameEnglish) }}
                </div>

                <div
                  class="description truncate"
                  style="direction: rtl; text-align: right; font-size: 12px"
                  v-if="lang0 === 'ar'"
                >
                  {{
                    getTranslationKey(
                      item.itemDescription,
                      item.itemDescriptionEnglish
                    )
                  }}
                </div>
                <div
                  class="description truncate"
                  style="direction: ltr; text-align: left; font-size: 12px"
                  v-else
                >
                  {{
                    getTranslationKey(
                      item.itemDescription,
                      item.itemDescriptionEnglish
                    )
                  }}
                </div>

                <div class="d-flex justify-content-start mb-2">
                  <div
                    v-if="
                      item.oldPrice != null &&
                      item.oldPrice != 0 &&
                      item.oldPrice != item.price
                    "
                    class="product-price"
                    style="text-decoration: line-through"
                  >
                    <span v-if="item.oldPrice != null">
                      {{ toCurrencyFormat(item.oldPrice) }}
                    </span>
                  </div>
                </div>

                <div class="d-flex justify-content-start mb-2">
                  <div v-if="item.isInService" class="product-price">
                    <span
                      v-if="item.price > 0"
                      style="font-weight: bold; color: black"
                    >
                      {{ toCurrencyFormat(item.price) }}
                    </span>
                    <span v-else>
                      <!--    {{ $t('FREE') }} -->
                    </span>
                  </div>
                  <div v-else class="product-price">
                    <span>{{ $t("UNAVAILABLE_ITEM") }}</span>
                  </div>
                </div>
              </div>

              <div
                v-if="
                  item.oldPrice != null &&
                  item.oldPrice != 0 &&
                  item.oldPrice != item.price
                "
                class="
                  product-image2
                  cover
                  flex-column
                  position-relative
                  round
                  plx-4
                "
                style="width: 15%"
              >
                <div>
                  <img class="card-img" :src="item.discountImg" />
                </div>

                <div>
                  <span style="font-size: large; color: crimson">{{
                    item.discount
                  }}</span>
                </div>
              </div>

              <div
                class="
                  product-image
                  cover
                  flex-column
                  position-relative
                  round
                  plx-4
                "
                style="width: 32%"
              >
                <img :src="item.imageUri" />
              </div>
            </div>
          </div>
          <transition name="fade">
            <p v-if="loading" class="loading">Loading</p>
          </transition>
        </div>
        <div class="d-flex justify-content-center">
          <div
            class="back-btn cursor-pointer"
            @click="goTop()"
            style="position: fixed; right: 0px; left: 23px; bottom: 0"
          >
            <i class="fas fa-arrow-up" style="color: black"></i>
          </div>

          <div
            v-if="$store.state.item.cart.length > 0"
            class="container d-flex flex-row checkout-btn cart-btn"
            @click="openCart()"
          >
            <div class="cart-icon">
              <span class="cart-count px-3">{{
                toCurrencyFormat(getTotal())
              }}</span>
            </div>
            <div>{{ $t("VIEW_BASKET") }}</div>
          </div>
        </div>
      </div>

      <!--    <CoolLightBox                 -->
      <!--      :items="selectedImage"      -->
      <!--      :index="imageIndex"         -->
      <!--      @close="imageIndex = null"  -->
      <!--    />                            -->
    </div>
  </div>
</template>

<script>
import axios from "axios";
import CoolLightBox from "vue-cool-lightbox";
import "vue-cool-lightbox/dist/vue-cool-lightbox.min.css";
import { dragscroll } from "vue-dragscroll";

export default {
  components: {
    CoolLightBox,
  },
  directives: {
    dragscroll,
  },
  data() {
    return {
      ERROR: "",
      selectSubName: "",
      counSubOn: 1,
      logoImag: "",
      bgImg: "",
      tname: "",
      tnameEnglish: "",
      items: [],
      selectedItems: [],
      selectedItemsAddOn: [],
      search: "",
      tenantID: "",
      languageBot: "1",
      contactId: "",
      menuType: 0,
      total: 0,
      phone: "",
      selectedImage: [],
      imageIndex: null,
      categories: [],
      selectedCategory: -1,
      selectedCategorySub: [],
      categorySelected: null,
      subCategorySelected: null,
      loadingSendOrder: false,
      lang: "ar",
      lang0: "ar",
      isLangChange: false,
      subCategory: [],
      page: 0,
      hideShowMore: false,
      loading: false,
      pageSize: 20,
      IsSearch: false,
      oldlinght: -1,
      SortValue: 0,
      onChangeD(e) {
        this.SortValue = e.target.value;
        console.log(e.target.value);
        this.goToSubCategory(this.subCategorySelected, this.categorySelected);
      },
    };
  },

  beforeMount() {

    
      localStorage.removeItem("categorySelected");
      this.selectedItems = [];
   


    //this.ERROR="dsdsd";
    debugger;
    //localStorage.removeItem("logoImag");
    this.lang0 = this.$route.query.lang === "ar" ? "ar" : "en";
    this.tenantID = this.$route.query.TenantID;
    this.contactId = this.$route.query.ContactId;
    this.menuType = this.$route.query.Menu;
    this.languageBot = this.$route.query.LanguageBot;
    this.phone = this.$route.query.PhoneNumber;

    if (JSON.parse(localStorage.getItem("menuType")) != this.menuType) {
      localStorage.removeItem("categorySelected");
      localStorage.removeItem("logoImag");
      this.cartItems = [];
      this.$store.dispatch("item/setCartData", []);
      this.selectedItems = [];
    }

    localStorage.setItem("menuType", JSON.stringify(this.menuType));

    if (JSON.parse(localStorage.getItem("tenantID")) != this.tenantID) {
      localStorage.removeItem("categorySelected");
      localStorage.removeItem("logoImag");
      this.cartItems = [];
      this.$store.dispatch("item/setCartData", []);
      this.selectedItems = [];
    }

    // if(this.tenantID=="46"){

    //    localStorage.removeItem('categorySelected');
    //    localStorage.removeItem('logoImag');
    // }

    localStorage.setItem("tenantID", JSON.stringify(this.tenantID));

    if (JSON.parse(localStorage.getItem("logoImag")) != null) {
      this.logoImag = JSON.parse(localStorage.getItem("logoImag"));
      this.bgImg = JSON.parse(localStorage.getItem("bgImg"));
      this.tname = JSON.parse(localStorage.getItem("tname"));
      this.tnameEnglish = JSON.parse(localStorage.getItem("tnameEnglish"));
    } else {
      axios
        .get(
          `${this.$axios.defaults.baseURL}/api/MenuSystem/GetInfoTenant?TenantID=${this.tenantID}&menu=${this.menuType}&LanguageBotId=${this.languageBot}`,
          {
            headers: {
              "Content-Type": "application/json",
              "Access-Control-Allow-Origin": "*",
            },
          }
        )
        .then((res1) => {
          localStorage.setItem(
            "logoImag",
            JSON.stringify(res1.data.result.logoImag)
          );
          localStorage.setItem(
            "bgImg",
            JSON.stringify(res1.data.result.bgImag)
          );
          localStorage.setItem("tname", JSON.stringify(res1.data.result.name));
          localStorage.setItem(
            "tnameEnglish",
            JSON.stringify(res1.data.result.nameEnglish)
          );

          this.logoImag = res1.data.result.logoImag;
          this.bgImg = res1.data.result.bgImag;
          this.tname = res1.data.result.name;
          this.tnameEnglish = res1.data.result.nameEnglish;

          localStorage.setItem("currencyCode", res1.data.result.currencyCode);
        });
    }

    debugger;

    if (this.tenantID != "34" && this.tenantID != "47") {
      if (
        localStorage.getItem("cart") &&
        localStorage.getItem("cart").length > 0
      ) {
        const data = JSON.parse(localStorage.getItem("cart"));
        this.$store.dispatch("item/setCartData", data);
      }

      if (this.$store.state.item.items.length === 0) {
        this.getAllCategory();
      } else {
        this.items = this.$store.state.item.items;
        this.categories = this.$store.state.item.items;
      }
    } else {
      if (
        localStorage.getItem("cart") &&
        localStorage.getItem("cart").length > 0
      ) {
        const data = JSON.parse(localStorage.getItem("cart"));
        this.$store.dispatch("item/setCartData", data);
      }
      if(this.tenantID == "47")
      this.getAllCategory();


      if (localStorage.getItem("categorySelected") == null) {
        this.getAllCategory();
      } else {
        debugger;

        this.categories = JSON.parse(
          localStorage.getItem("AllcategorySelected")
        );
        this.selectedCategory = JSON.parse(
          localStorage.getItem("selectedCategory")
        );
        this.selectedCategorySub = JSON.parse(
          localStorage.getItem("selectedCategorySub")
        );
        this.subCategorySelected = JSON.parse(
          localStorage.getItem("subCategorySelected")
        );
        this.subCategorySelected.subCategorysInItemModels = [];

        this.items = [];
        this.categorySelected = JSON.parse(
          localStorage.getItem("categorySelected")
        );
        this.categorySelected.listItemInCategories = JSON.parse(
          localStorage.getItem("categorySelected_listItemInCategories")
        );
        this.items = JSON.parse(localStorage.getItem("items"));
        this.$forceUpdate();
      }
    }
  },
  updated() {
    if (this.$store.getters.dynamicComponent) {
      console.log("This component has been mounted");
      //localStorage.removeItem('categorySelected');
      //localStorage.removeItem('logoImag');
    }
  },
  created() {
    setTimeout(function () {
      localStorage.removeItem("categorySelected");
    }, 3600 * 2 * 1000);

    this.tenantID = this.$route.query.TenantID;
    this.contactId = this.$route.query.ContactId;
    this.menuType = this.$route.query.Menu;
    this.languageBot = this.$route.query.LanguageBot;
    this.phone = this.$route.query.PhoneNumber;
    this.isback = this.$route.query.itemId;

    debugger;
    if (this.isback == null) {
      this.isback = false;
    }

    if (this.isback == -1) {
      this.isback = true;
    }

    if (
      performance.navigation.type == 1 &&
      this.tenantID == "34" &&
      this.tenantID == "47" &&
      !this.isback
    ) {
      ///performance.navigation.type == 1 && this.tenantID == "34"   && !this.isback

      console.info("This page is reloaded");
      localStorage.removeItem("categorySelected");
      localStorage.removeItem("logoImag");
      this.cartItems = [];
      this.$store.dispatch("item/setCartData", []);
      this.selectedItems = [];
    } else {
      console.info("This page is not reloaded");
    }
    this.back();
  },

  mounted() {
    this.openNav();
    if (this.tenantID !== "47") {
      window.addEventListener("touchend", this.loadMore);
      window.addEventListener("wheel", this.loadMore);
    }
  },
  methods: {
    filteredList(item) {
      if (this.search.length > 0) {
        if (this.tenantID === "34" || this.tenantID === "47") {
        } else {
          return item.filter((post) => {
            return post.itemName
              .toLowerCase()
              .includes(this.search.toLowerCase());
          });
        }
      }
      return item;
    },
    openImage(image) {
      this.selectedImage = [image];
      this.imageIndex = 0;
    },

    getAllCategory() {
      // this.ERROR="getAllCategory";
      // axios.get(`${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=0&SubCategoryId=3&PageSize=20&PageNumber=0&LanguageBotId=${this.languageBot}`, {

      try {
        axios
          .get(
            `${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuItem?TenantId=${this.tenantID}&MenuType=${this.menuType}&SubCategoryId=0&PageSize=${this.pageSize}&PageNumber=0`,
            {
              headers: {
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
              },
            }
          )
          .then((res) => {
            // this.ERROR="hasan";//JSON.stringify(res)
            this.categories = res.data.result;
            this.items = res.data.result;

            localStorage.removeItem("AllcategorySelected");
            localStorage.setItem(
              "AllcategorySelected",
              JSON.stringify(this.categories)
            );

            if (
              this.categories[0] &&
              this.categories[0].subCategorysInItemModels.length > 0
            ) {
              this.goToCategory(this.categories[0], true);
            }
            this.$forceUpdate();
          });
      } catch (error) {
        this.ERROR = "Form catch : " + error;
      }
    },
    async goToItem(item) {
      await this.$store.dispatch("item/setSelectedItem", item);

      await this.$router.push({
        path: "/product-info",
        query: {
          ...this.$route.query,
          //   itemId: item.id
        },
      });
    },
    goToCategory(category, selectSub = false) {
      this.search = "";
      this.page = 0;

      if (category.subCategorysInItemModels != null) {
        selectSub = true;
      } else {
        selectSub = false;
      }

      this.selectedCategory = category.categoryId;
      this.selectedCategorySub = category;

      localStorage.removeItem("selectedCategory");
      localStorage.removeItem("selectedCategorySub");
      localStorage.setItem(
        "selectedCategory",
        JSON.stringify(this.selectedCategory)
      );
      localStorage.setItem(
        "selectedCategorySub",
        JSON.stringify(this.selectedCategorySub)
      );

      if (selectSub) {
        this.goToSubCategory(category.subCategorysInItemModels[0], category);
      } else {
        if (document.querySelector(`#category${category.categoryId}`)) {
          document
            .querySelector(`#category${category.categoryId}`)
            .scrollIntoView({
              behavior: "smooth",
              inline: "nearest",
            });
        }
      }
    },
    goToSubCategory(subCat, category) {
      this.search = "";
      this.hideShowMore = true;
      this.subCategorySelected = subCat;
      localStorage.removeItem("categorySelected_listItemInCategories");

      localStorage.removeItem("subCategorySelected");
      localStorage.setItem(
        "subCategorySelected",
        JSON.stringify(this.subCategorySelected)
      );
      // the.selectSubName=subCat.
      if (this.tenantID === "34" || this.tenantID === "47") {
        axios
          .get(
            `${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=${subCat.categoryId}&SubCategoryId=${subCat.subcategoryId}&PageSize=${this.pageSize}&PageNumber=0&Search=null&IsSort=${this.SortValue}`,
            {
              headers: {
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
              },
            }
          )
          .then((res) => {
            this.items = [];
            this.categorySelected = category;
            this.categorySelected.listItemInCategories = [];
            this.categorySelected.listItemInCategories =
              res.data.result.listItemInCategories;
            this.items.push(res.data.result.listItemInCategories);

            localStorage.removeItem("categorySelected");
            localStorage.removeItem("categorySelected_listItemInCategories");
            localStorage.removeItem("items");

            localStorage.setItem(
              "categorySelected",
              JSON.stringify(this.categorySelected)
            );
            localStorage.setItem(
              "categorySelected_listItemInCategories",
              JSON.stringify(this.categorySelected.listItemInCategories)
            );
            localStorage.setItem("items", JSON.stringify(this.items));

            this.$forceUpdate();
          });
      } else {
        this.selectedCategory = subCat.categoryId;
        this.items = [];
        this.items.push(subCat);
      }
    },
    async openCart() {
      await this.$router.push({
        path: "/cart",
        query: {
          ...this.$route.query,
        },
      });
    },
    changeLang() {
      this.lang = this.$route.query.lang === "ar" ? "en" : "ar";
      this.$router.push({
        path: this.$router.currentRoute.path,
        query: {
          ...this.$route.query,
          lang: this.lang,
        },
      });
      this.lang0 = this.$route.query.lang === "ar" ? "en" : "ar";
    },
    loadMore(e) {
      if (this.search.length == 0) {
        if (
          document.body.scrollTop + 900 + document.body.offsetHeight >=
          document.body.scrollHeight
        ) {
          let { scrollTop, clientHeight, scrollHeight } = e.target;
          if (
            !this.loading &&
            scrollTop + clientHeight >= (scrollHeight * 4) / 5
          ) {
            this.loading = true;

            setTimeout(async () => {
              this.page++;
              let data = await axios.get(
                `${this.$axios.defaults.baseURL}/api/MenuSystem/GetMenuSubCategorys?TenantId=${this.tenantID}&MenuType=${this.menuType}&CategoriesID=${this.categorySelected.categoryId}&SubCategoryId=${this.subCategorySelected.subcategoryId}&PageSize=${this.pageSize}&PageNumber=${this.page}&Search=null&IsSort=${this.SortValue}`,
                {
                  headers: {
                    "Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*",
                  },
                }
              );
              this.loading = false;
              this.items = [];

              let subItems = data.data.result.listItemInCategories
                ? data.data.result.listItemInCategories
                : [];

              this.categorySelected.listItemInCategories =
                this.categorySelected.listItemInCategories.concat(subItems);

              // if(subItems[0].itemCategoryId==this.categorySelected.listItemInCategories[0].itemCategoryId  && subItems[0].itemSubCategoryId ==this.categorySelected.listItemInCategories[0].itemSubCategoryId)
              //  {
              //  this.categorySelected.listItemInCategories = this.categorySelected.listItemInCategories.concat(subItems)
              // }else{

              //   this.categorySelected.listItemInCategories=[]
              //   this.categorySelected.listItemInCategories =subItems
              // }

              this.items.push(data.data.result.listItemInCategories);

              localStorage.removeItem("categorySelected");
              localStorage.removeItem("categorySelected_listItemInCategories");
              localStorage.removeItem("items");

              localStorage.setItem(
                "categorySelected",
                JSON.stringify(this.categorySelected)
              );
              localStorage.setItem(
                "categorySelected_listItemInCategories",
                JSON.stringify(this.categorySelected.listItemInCategories)
              );
              localStorage.setItem("items", JSON.stringify(this.items));

              this.sortedArray();
              this.$forceUpdate();
            }, 1000);
          }
        }
      }
    },
    searchForCustomerClicked() {
      if (this.tenantID === "34" || this.tenantID === "47") {
        this.hideShowMore = true;
        this.pageSize = 20;

        axios
          .get(
            `${
              this.$axios.defaults.baseURL
            }/api/MenuSystem/GetMenuSubCategorys?TenantId=${
              this.tenantID
            }&MenuType=${
              this.menuType
            }&CategoriesID=0&SubCategoryId=0&PageSize=${
              this.pageSize
            }&PageNumber=0&Search=${this.search.toLowerCase()}&IsSort=${
              this.SortValue
            }`,
            {
              headers: {
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
              },
            }
          )
          .then((res) => {
            let subItems = res.data.result.listItemInCategories
              ? res.data.result.listItemInCategories
              : [];
            if (res.data.result.listItemInCategories == null) {
              this.categorySelected.listItemInCategories = []; // this.categorySelected.listItemInCategories.concat(subItems)
              this.items = [];
              this.items = [];
            } else {
              this.categorySelected.listItemInCategories =
                res.data.result.listItemInCategories; // this.categorySelected.listItemInCategories.concat(subItems)
              this.items = [];
              this.items = res.data.result.listItemInCategories;
            }

            this.$forceUpdate();
          });
      }
    },
    openNav() {
      debugger;
      // document.getElementById("myNav").style.height = "100%";

      var id = "item" + JSON.parse(localStorage.getItem("itemSelectedID"));

      var itemSelectedID = document.getElementById(id);

      if (itemSelectedID != null) {
        var topPos = itemSelectedID.offsetTop;
        document.body.scrollTop = topPos;
        this.$forceUpdate();
      }
      this.$forceUpdate();
    },

    closeNav() {
      debugger;
      document.getElementById("myNav").style.height = "0%";
    },
    async back() {
      await this.$router.push({
        path: "/",
        query: {
          ...this.$route.query,
        },
      });
    },
    goTop() {
      //  var id='item'+JSON.parse(localStorage.getItem('itemSelectedID'))
      var itemSelectedID = document.getElementById("__layout");
      if (itemSelectedID != null) {
        // var topPos = itemSelectedID.offsetTop;
        document.body.scrollTop = 0;
        this.$forceUpdate();
      }
      this.$forceUpdate();
    },
    test1() {
      debugger;
      var x = document.body.offsetTop;
      console.info(x);

      var y = document.getElementById("scrolling_div").offsetHeight;
      console.info(y);

      if (
        document.body.scrollTop + document.body.offsetHeight ==
        document.body.scrollHeight
      ) {
        console.info("yessssssssssssssss");
      }
    },
  },

  sortedArray() {
    return this.categorySelected.listItemInCategories.sort(
      (a, b) => a.itemName - b.itemName
    );
  },
  onChangeD(event) {
    debugger;
  },
};
</script>