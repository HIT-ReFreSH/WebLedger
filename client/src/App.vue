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
  display: flex;
  flex-direction: column;
}
</style>
