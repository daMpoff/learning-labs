<template>
  <div v-if="results" class="space-y-6">
    <!-- Лучшая альтернатива -->
    <div class="bg-gradient-to-r from-green-500 to-green-600 rounded-lg shadow-lg p-6 text-white">
      <h2 class="text-2xl font-bold mb-4">🏆 Лучшая альтернатива</h2>
      <div class="bg-white bg-opacity-20 rounded-lg p-4">
        <h3 class="text-xl font-semibold mb-2">{{ results.best_alternative.name }}</h3>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
          <div>
            <div class="opacity-80">Цена</div>
            <div class="font-semibold">{{ formatPrice(results.best_alternative.price) }}</div>
          </div>
          <div>
            <div class="opacity-80">RAM</div>
            <div class="font-semibold">{{ results.best_alternative.ram }} ГБ</div>
          </div>
          <div>
            <div class="opacity-80">Память</div>
            <div class="font-semibold">{{ results.best_alternative.storage }} ГБ</div>
          </div>
          <div>
            <div class="opacity-80">Рейтинг TOPSIS</div>
            <div class="font-semibold">{{ (results.best_alternative.relative_closeness * 100).toFixed(2) }}%</div>
          </div>
        </div>
      </div>
    </div>

    <!-- График рейтинга -->
    <div class="bg-white rounded-lg shadow-lg p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Рейтинг TOPSIS (Топ-10)</h3>
      <div ref="rankingChartRef" class="h-96"></div>
    </div>

    <!-- Таблица результатов -->
    <div class="bg-white rounded-lg shadow-lg overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200">
        <h3 class="text-lg font-semibold text-gray-900">
          Все результаты ({{ results.total_alternatives }})
        </h3>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ранг</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Название</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Цена</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">RAM</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Память</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Камера</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Рейтинг</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="item in results.all_results"
              :key="item.smartphone_id"
              :class="item.rank === 1 ? 'bg-green-50' : 'hover:bg-gray-50'"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                #{{ item.rank }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                {{ item.name }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatPrice(item.price) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ item.ram }} ГБ
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ item.storage }} ГБ
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ item.camera_main }} МП
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                {{ (item.relative_closeness * 100).toFixed(2) }}%
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as echarts from 'echarts'

const props = defineProps({
  results: {
    type: Object,
    default: null,
  },
})

const rankingChartRef = ref(null)
let rankingChart = null

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 0,
  }).format(price)
}

const updateChart = () => {
  if (!props.results || !rankingChartRef.value || !rankingChart) return

  const top10 = props.results.top_10.slice(0, 10)
  const names = top10.map(item => item.name.substring(0, 30) + '...')
  const ratings = top10.map(item => (item.relative_closeness * 100).toFixed(2))

  rankingChart.setOption({
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow',
      },
    },
    xAxis: {
      type: 'category',
      data: names,
      axisLabel: {
        rotate: 45,
        interval: 0,
      },
    },
    yAxis: {
      type: 'value',
      name: 'Рейтинг (%)',
      max: 100,
    },
    series: [{
      data: ratings,
      type: 'bar',
      itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: '#10b981' },
          { offset: 1, color: '#059669' },
        ]),
      },
      label: {
        show: true,
        position: 'top',
        formatter: '{c}%',
      },
    }],
  })
}

onMounted(() => {
  if (rankingChartRef.value) {
    rankingChart = echarts.init(rankingChartRef.value)
  }
  updateChart()

  window.addEventListener('resize', () => {
    rankingChart?.resize()
  })
})

watch(() => props.results, updateChart, { deep: true })
</script>
