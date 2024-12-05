import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/Record.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path:"/",
      name:'login',
      component:()=>import('../views/Login.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/Register.vue')
    },
    {
      path: '/home',
      name: 'home',
      component: ()=>import('../views/Record.vue')
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/View.vue')
    },
    {
      path: '/setting',
      name: 'setting',
      component: () => import('../views/Setting.vue')
    }
  ]
})

export default router
