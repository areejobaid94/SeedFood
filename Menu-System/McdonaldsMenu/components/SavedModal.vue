<template>
  <div class="modal-overlay">
    <div class="modal">
    
      <h6>Saved!</h6>
      <p>Your Details have been saved Successfully</p>
      <button>Go Home</button>
    </div>
    <div class="close" @click="$emit('close-modal')">
    </div>
  </div>
</template>

<script>
  export default {
}
</script>

