<template>
  <div v-if="phones.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
    <div
      v-for="phone in sortedPhones"
      :key="phone.id"
      class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow overflow-hidden"
    >
      <!-- Изображение -->
      <div class="h-48 bg-gray-100 flex items-center justify-center overflow-hidden">
        <img
          v-if="phone.image_url"
          :src="phone.image_url"
          :alt="phone.name"
          class="w-full h-full object-contain"
          @error="handleImageError"
          loading="lazy"
        />
        <div v-else class="text-gray-400 text-sm text-center px-2">Нет изображения</div>
      </div>

      <!-- Контент -->
      <div class="p-4">
        <h3 class="font-semibold text-gray-900 mb-2 line-clamp-2" :title="phone.name">
          {{ phone.name }}
        </h3>
        <div class="space-y-2 text-sm">
          <div v-if="phone.brand" class="text-gray-600">
            <span class="font-medium">Бренд:</span> {{ phone.brand }}
          </div>
          <div class="text-lg font-bold text-blue-600">
            {{ formatPrice(phone.price) }}
          </div>
          <div class="grid grid-cols-2 gap-2 text-xs text-gray-500">
            <div v-if="phone.ram">
              <span class="font-medium">RAM:</span> {{ phone.ram }} ГБ
            </div>
            <div v-if="phone.storage">
              <span class="font-medium">Память:</span> {{ phone.storage }} ГБ
            </div>
            <div v-if="phone.camera_main">
              <span class="font-medium">Камера:</span> {{ phone.camera_main }} МП
            </div>
            <div v-if="phone.cores">
              <span class="font-medium">Ядра:</span> {{ phone.cores }}
            </div>
            <div v-if="phone.screen_pixels" class="col-span-2">
              <span class="font-medium">Экран:</span> {{ formatNumber(phone.screen_pixels) }} пикс.
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else class="text-center py-12 text-gray-500">
    Нет данных для отображения
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'

const props = defineProps({
  phones: {
    type: Array,
    default: () => [],
  },
  sortField: {
    type: String,
    default: null,
  },
  sortOrder: {
    type: String,
    default: 'asc',
  },
})

const handleImageError = (event) => {
  const parent = event.target.parentElement
  if (parent) {
    parent.innerHTML = '<div class="text-gray-400 text-sm text-center px-2">Ошибка загрузки изображения</div>'
  }
}

const sortedPhones = computed(() => {
  if (!props.sortField) return props.phones

  return [...props.phones].sort((a, b) => {
    let aVal = a[props.sortField]
    let bVal = b[props.sortField]

    if (aVal === null || aVal === undefined) aVal = 0
    if (bVal === null || bVal === undefined) bVal = 0

    if (typeof aVal === 'string') {
      aVal = aVal.toLowerCase()
      bVal = bVal.toLowerCase()
    }

    if (props.sortOrder === 'asc') {
      return aVal > bVal ? 1 : aVal < bVal ? -1 : 0
    } else {
      return aVal < bVal ? 1 : aVal > bVal ? -1 : 0
    }
  })
})

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 0,
  }).format(price)
}

const formatNumber = (num) => {
  return new Intl.NumberFormat('ru-RU').format(num)
}
</script>
