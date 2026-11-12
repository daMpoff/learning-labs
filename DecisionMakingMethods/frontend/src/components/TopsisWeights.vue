<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h3 class="text-lg font-semibold text-gray-900 mb-4">Настройка весов критериев</h3>
    <div class="space-y-4">
      <div
        v-for="(weight, key) in localWeights"
        :key="key"
        class="space-y-2"
      >
        <div class="flex justify-between">
          <label class="text-sm font-medium text-gray-700">
            {{ getLabel(key) }}
          </label>
          <span class="text-sm font-semibold text-blue-600">{{ (weight * 100).toFixed(0) }}%</span>
        </div>
        <input
          v-model.number="localWeights[key]"
          type="range"
          min="0"
          max="1"
          step="0.05"
          class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
        />
      </div>
      <div class="pt-4 border-t border-gray-200">
        <div class="flex justify-between text-sm">
          <span class="text-gray-600">Сумма весов:</span>
          <span :class="sumWeights === 1 ? 'text-green-600' : 'text-red-600'">
            {{ (sumWeights * 100).toFixed(0) }}%
          </span>
        </div>
        <button
          v-if="sumWeights !== 1"
          @click="normalizeWeights"
          class="mt-2 text-sm text-blue-600 hover:text-blue-700"
        >
          Нормализовать автоматически
        </button>
      </div>
      <button
        @click="applyWeights"
        class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
      >
        Применить веса
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  weights: {
    type: Object,
    default: () => ({
      price: 0.20,
      screen_pixels: 0.15,
      ram: 0.15,
      storage: 0.15,
      camera_main: 0.15,
      cores: 0.20,
    }),
  },
})

const emit = defineEmits(['update:weights', 'apply'])

const localWeights = ref({ ...props.weights })

const labels = {
  price: 'Цена',
  screen_pixels: 'Пиксели экрана',
  ram: 'RAM',
  storage: 'Память',
  camera_main: 'Камера',
  cores: 'Количество ядер',
}

const getLabel = (key) => labels[key] || key

const sumWeights = computed(() => {
  return Object.values(localWeights.value).reduce((sum, val) => sum + val, 0)
})

watch(() => props.weights, (newWeights) => {
  localWeights.value = { ...newWeights }
}, { deep: true })

const normalizeWeights = () => {
  const sum = sumWeights.value
  if (sum > 0) {
    Object.keys(localWeights.value).forEach(key => {
      localWeights.value[key] = localWeights.value[key] / sum
    })
  }
}

const applyWeights = () => {
  if (sumWeights.value !== 1) {
    normalizeWeights()
  }
  emit('apply', { ...localWeights.value })
}
</script>
