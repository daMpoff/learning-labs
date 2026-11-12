<template>
  <div class="space-y-6">
    <div class="bg-white rounded-lg shadow-lg p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Распределение цен</h3>
      <div ref="priceChartRef" class="h-64"></div>
    </div>

    <div class="bg-white rounded-lg shadow-lg p-6">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-semibold text-gray-900">Зависимость цены от характеристик</h3>
        <select
          v-model="selectedCriterion"
          class="bg-white border border-gray-300 rounded-md px-3 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="ram">RAM</option>
          <option value="storage">Память</option>
          <option value="camera_main">Камера</option>
          <option value="cores">Ядра</option>
          <option value="screen_pixels">Пиксели экрана</option>
        </select>
      </div>
      <div ref="scatterChartRef" class="h-64"></div>
    </div>

    <div class="bg-white rounded-lg shadow-lg p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Распределение по брендам</h3>
      <div ref="brandChartRef" class="h-64"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as echarts from 'echarts'

const props = defineProps({
  phones: {
    type: Array,
    default: () => [],
  },
})

const priceChartRef = ref(null)
const scatterChartRef = ref(null)
const brandChartRef = ref(null)

const selectedCriterion = ref('ram')

const criterionNames = {
  ram: 'RAM (ГБ)',
  storage: 'Память (ГБ)',
  camera_main: 'Камера (МП)',
  cores: 'Ядра',
  screen_pixels: 'Пиксели экрана',
}

let priceChart = null
let scatterChart = null
let brandChart = null

const updateCharts = () => {
  if (!props.phones.length) return

  // График 1: Распределение цен (гистограмма)
  if (priceChartRef.value && priceChart) {
    const prices = props.phones.map(p => p.price).filter(p => p)
    const bins = 20
    const min = Math.min(...prices)
    const max = Math.max(...prices)
    const binSize = (max - min) / bins
    const data = new Array(bins).fill(0)
    
    prices.forEach(price => {
      const binIndex = Math.min(Math.floor((price - min) / binSize), bins - 1)
      data[binIndex]++
    })

    priceChart.setOption({
      xAxis: {
        type: 'category',
        data: Array.from({ length: bins }, (_, i) => Math.round(min + i * binSize)),
        name: 'Цена (₽)',
      },
      yAxis: {
        type: 'value',
        name: 'Количество',
      },
      series: [{
        data: data,
        type: 'bar',
        itemStyle: {
          color: '#3b82f6',
        },
      }],
      tooltip: {
        trigger: 'axis',
      },
    })
  }

  // График 2: Зависимость цены от выбранного критерия (scatter)
  if (scatterChartRef.value && scatterChart) {
    const criterion = selectedCriterion.value
    const phonesData = props.phones.filter(p => p.price && p[criterion] !== null && p[criterion] !== undefined)
    
    const scatterData = phonesData.map((p, index) => ({
      value: [p[criterion], p.price],
      name: p.name || 'Неизвестно',
      phoneIndex: index
    }))

    const criterionName = criterionNames[criterion] || criterion

    scatterChart.setOption({
      xAxis: {
        type: 'value',
        name: criterionName,
      },
      yAxis: {
        type: 'value',
        name: 'Цена (₽)',
      },
      series: [{
        data: scatterData,
        type: 'scatter',
        symbolSize: 8,
        itemStyle: {
          color: '#10b981',
        },
      }],
      tooltip: {
        trigger: 'item',
        enterable: true,
        renderMode: 'html',
        backgroundColor: 'transparent',
        borderWidth: 0,
        padding: 0,
        formatter: (params) => {
          const phoneIndex = params.data.phoneIndex
          const phone = phonesData[phoneIndex]
          if (!phone) return ''
          
          const value = params.value[0]
          const price = params.value[1]
          const name = phone.name || 'Неизвестно'
          const brand = phone.brand || ''
          const imageUrl = phone.image_url || ''
          const unit = criterion === 'screen_pixels' ? ' пикс.' : (criterion === 'cores' ? '' : (criterion === 'camera_main' ? ' МП' : ' ГБ'))
          
          const imageHtml = imageUrl 
            ? `<img src="${imageUrl}" alt="${name}" style="width: 150px; height: 150px; object-fit: contain; border-radius: 8px 8px 0 0; background: #f3f4f6; display: block;" onerror="this.style.display='none'">`
            : '<div style="width: 150px; height: 150px; background: #f3f4f6; border-radius: 8px 8px 0 0; display: flex; align-items: center; justify-content: center; color: #9ca3af;">Нет фото</div>'
          
          return `
            <div style="background: white; border: 1px solid #e5e7eb; border-radius: 8px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06); overflow: hidden; width: 300px; box-sizing: border-box;">
              ${imageHtml}
              <div style="padding: 12px; background: white; box-sizing: border-box;">
                <div style="font-weight: bold; font-size: 14px; margin-bottom: 6px; color: #1f2937; line-height: 1.4; word-break: break-word; overflow-wrap: break-word; white-space: normal; display: block;">${name}</div>
                ${brand ? `<div style="font-size: 12px; color: #6b7280; margin-bottom: 8px; word-break: break-word; overflow-wrap: break-word; white-space: normal; display: block;">${brand}</div>` : ''}
                <div style="font-size: 12px; color: #374151; margin-bottom: 8px; word-break: break-word; overflow-wrap: break-word; white-space: normal; display: block;">
                  <strong>${criterionName}:</strong> ${typeof value === 'number' ? value.toLocaleString('ru-RU') : value}${unit}
                </div>
                <div style="font-size: 16px; font-weight: bold; color: #059669; padding-top: 8px; border-top: 1px solid #e5e7eb; display: block;">
                  ${price.toLocaleString('ru-RU')} ₽
                </div>
              </div>
            </div>
          `
        },
      },
    })
  }

  // График 3: Распределение по брендам (pie)
  if (brandChartRef.value && brandChart) {
    const brandCount = {}
    props.phones.forEach(phone => {
      const brand = phone.brand || 'Неизвестно'
      brandCount[brand] = (brandCount[brand] || 0) + 1
    })

    const pieData = Object.entries(brandCount).map(([name, value]) => ({ name, value }))

    brandChart.setOption({
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b}: {c} ({d}%)',
      },
      series: [{
        name: 'Бренды',
        type: 'pie',
        radius: '60%',
        data: pieData,
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)',
          },
        },
      }],
    })
  }
}

onMounted(() => {
  if (priceChartRef.value) {
    priceChart = echarts.init(priceChartRef.value)
  }
  if (scatterChartRef.value) {
    scatterChart = echarts.init(scatterChartRef.value)
  }
  if (brandChartRef.value) {
    brandChart = echarts.init(brandChartRef.value)
  }
  updateCharts()

  window.addEventListener('resize', () => {
    priceChart?.resize()
    scatterChart?.resize()
    brandChart?.resize()
  })
})

watch(() => props.phones, updateCharts, { deep: true })
watch(selectedCriterion, updateCharts)
</script>
