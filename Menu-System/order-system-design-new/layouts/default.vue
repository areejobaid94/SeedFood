<template>
  <div class=" layout" :class="{'container-fluid':screenWidth>=1024}">
    <Nuxt />
  </div>
</template>
<script>
export default {
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
