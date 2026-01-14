import { defineStore } from 'pinia'
import { analysisAPI } from '@/services/api'

export const useAnalysisStore = defineStore('analysis', {
  state: () => ({
    statistics: null,
    topsisResults: null,
    loading: false,
    error: null,
  }),

  actions: {
    async fetchStatistics() {
      this.loading = true
      this.error = null
      try {
        const response = await analysisAPI.getStatistics()
        this.statistics = response.data
      } catch (error) {
        this.error = error.message
        console.error('Error fetching statistics:', error)
      } finally {
        this.loading = false
      }
    },

    async calculateTopsis(filters = null, weights = null) {
      this.loading = true
      this.error = null
      try {
        const response = await analysisAPI.calculateTopsis({
          filters,
          weights,
        })
        this.topsisResults = response.data
        return response.data
      } catch (error) {
        this.error = error.message
        console.error('Error calculating TOPSIS:', error)
        throw error
      } finally {
        this.loading = false
      }
    },
  },
})
