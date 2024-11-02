// src/router/index.js
import Vue from "vue";
import VueRouter from "vue-router"; // 注意这里使用了 VueRouter 而不是 vueRouter
import HomePage from "../view/HomePage.vue"; // 确保这里的路径和文件名是正确的

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "home",
    component: HomePage,
  },
];

const router = new VueRouter({
  routes,
});

export default router;
