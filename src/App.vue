<!-- src/App.vue -->
<template>
  <div id="app">
    <router-view></router-view>
    <!-- 路由内容会在这里显示 -->
  </div>
</template>

<script>
export default {
  name: "App",
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
