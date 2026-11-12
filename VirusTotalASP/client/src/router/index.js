import { createRouter, createWebHistory } from 'vue-router'

// Поскольку мы не используем router-view, создаем простой компонент
const Home = { template: '<div></div>' }

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
  ],
})

export default router
