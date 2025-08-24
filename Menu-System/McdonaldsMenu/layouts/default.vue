<template>
  <div class=" layout" :class="{'container-fluid':screenWidth>=1024}">
    <Nuxt />
  </div>
</template>
<script>
export default {

head() {

var filename="arrtl";
if (this.$route.query.lang =="en"){
filename="style";

}
 return {

link: [

{

rel: "stylesheet",

href: `/${filename}.css`

}

]

};

},


  data () {

   
    return {
     
      screenWidth: 100000
    }
  },
  mounted () {
    this.screenWidth = screen.width
  }
}
</script>
