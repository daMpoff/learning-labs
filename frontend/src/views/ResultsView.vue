<template>
  <div class="px-4 py-8">
    <div class="mb-6">
      <h1 class="text-3xl font-bold text-gray-900">Результаты TOPSIS</h1>
      <p class="text-gray-600 mt-2">Расчет оптимального смартфона методом TOPSIS</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <!-- Настройки весов -->
      <div class="lg:col-span-1">
        <TopsisWeights
          :weights="weights"
          @apply="handleWeightsApply"
        />
      </div>

      <!-- Результаты -->
      <div class="lg:col-span-3 space-y-6">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">Результаты анализа</h2>
          <button
            @click="calculateTopsis"
            :disabled="loading"
            class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
          >
            {{ loading ? 'Расчет...' : 'Рассчитать TOPSIS' }}
          </button>
        </div>

        <div v-if="loading" class="text-center py-12">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-green-600"></div>
          <p class="mt-4 text-gray-600">Выполняется расчет...</p>
        </div>

        <div v-else-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
          {{ error }}
        </div>

        <TopsisResults v-else-if="results" :results="results" />

        <div v-else class="bg-gray-50 rounded-lg p-12 text-center">
          <p class="text-gray-600 text-lg">
            Настройте веса критериев и нажмите "Рассчитать TOPSIS" для получения результатов
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAnalysisStore } from '@/stores/analysis'
import TopsisWeights from '@/components/TopsisWeights.vue'
import TopsisResults from '@/components/TopsisResults.vue'

const analysisStore = useAnalysisStore()
const { topsisResults, loading, error } = analysisStore

const weights = ref({
  price: 0.20,
  screen_pixels: 0.15,
  ram: 0.15,
  storage: 0.15,
  camera_main: 0.15,
  cores: 0.20,
})

const results = ref(null)

const handleWeightsApply = (newWeights) => {
  weights.value = newWeights
}

const calculateTopsis = async () => {
  try {
    const result = await analysisStore.calculateTopsis(null, weights.value)
    results.value = result
  } catch (err) {
    console.error('Error calculating TOPSIS:', err)
  }
}
</script>
