import { defineStore } from 'pinia'
import { phonesAPI } from '@/services/api'

export const usePhonesStore = defineStore('phones', {
  state: () => ({
    smartphones: [],
    loading: false,
    error: null,
    totalCount: 0,
    parsingStatus: {
      is_running: false,
      progress: 0,
      total: 0,
      current_item: '',
      error: null,
    },
  }),

  actions: {
    async fetchSmartphones(params = {}) {
      this.loading = true
      this.error = null
      try {
        // Очищаем null/undefined значения из параметров
        const cleanParams = Object.fromEntries(
          Object.entries({ ...params, limit: 1000 }).filter(([_, v]) => v !== null && v !== undefined && v !== '')
        )
        const response = await phonesAPI.getSmartphones(cleanParams)
        this.smartphones = response.data
        await this.fetchCount()
      } catch (error) {
        this.error = error.message
        console.error('Error fetching smartphones:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchCount() {
      try {
        const response = await phonesAPI.getSmartphoneCount()
        this.totalCount = response.data.count
      } catch (error) {
        console.error('Error fetching count:', error)
      }
    },

    async fetchParsingStatus() {
      try {
        const response = await phonesAPI.getParsingStatus()
        this.parsingStatus = {
          is_running: response.data.is_running || false,
          progress: response.data.progress || 0,
          total: response.data.total || 0,
          current_item: response.data.current_item || '',
          error: response.data.error || null,
        }
      } catch (error) {
        console.error('Error fetching parsing status:', error)
        this.parsingStatus.error = error.message
      }
    },

    async startParsing(params = {}) {
      try {
        this.parsingStatus.is_running = true
        this.parsingStatus.error = null
        this.parsingStatus.progress = 0
        this.parsingStatus.total = 0
        this.parsingStatus.current_item = ''
        
        await phonesAPI.startParsing(params)
        
        // Начинаем отслеживание статуса
        await this.fetchParsingStatus()
      } catch (error) {
        this.error = error.message
        this.parsingStatus.error = error.message
        this.parsingStatus.is_running = false
        throw error
      }
    },
  },
})
