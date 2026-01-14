import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import AnalysisView from '@/views/AnalysisView.vue'
import ResultsView from '@/views/ResultsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/analysis',
      name: 'analysis',
      component: AnalysisView,
    },
    {
      path: '/results',
      name: 'results',
      component: ResultsView,
    },
  ],
})

export default router
