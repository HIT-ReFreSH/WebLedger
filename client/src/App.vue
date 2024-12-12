<script setup lang="ts">
</script>

<template>
  <div id="hero">
    <router-view></router-view>
  </div>
</template>

<style lang="scss" scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity .5s;
}

.fade-enter-to,
.fade-leave-from {
  opacity: 1;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

#hero {
  position: relative;
  background: url(assets/img/lake.jpg) no-repeat;
  background-size: cover;
  background-position: center;
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}
</style>
