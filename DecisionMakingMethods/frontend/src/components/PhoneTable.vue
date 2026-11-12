<template>
  <div class="bg-white rounded-lg shadow-lg overflow-hidden">
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              v-for="column in columns"
              :key="column.key"
              @click="handleSort(column.key)"
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
            >
              <div class="flex items-center space-x-1">
                <span>{{ column.label }}</span>
                <span v-if="sortField === column.key">
                  {{ sortOrder === 'asc' ? '↑' : '↓' }}
                </span>
              </div>
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr
            v-for="phone in sortedPhones"
            :key="phone.id"
            class="hover:bg-gray-50"
          >
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ phone.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.brand || '—' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatPrice(phone.price) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.screen_pixels ? formatNumber(phone.screen_pixels) : '—' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.ram ? `${phone.ram} ГБ` : '—' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.storage ? `${phone.storage} ГБ` : '—' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.camera_main ? `${phone.camera_main} МП` : '—' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ phone.cores || '—' }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="phones.length === 0" class="text-center py-12 text-gray-500">
      Нет данных для отображения
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'

const props = defineProps({
  phones: {
    type: Array,
    default: () => [],
  },
})

const sortField = ref(null)
const sortOrder = ref('asc')

const columns = [
  { key: 'name', label: 'Название' },
  { key: 'brand', label: 'Бренд' },
  { key: 'price', label: 'Цена' },
  { key: 'screen_pixels', label: 'Пиксели экрана' },
  { key: 'ram', label: 'RAM' },
  { key: 'storage', label: 'Память' },
  { key: 'camera_main', label: 'Камера' },
  { key: 'cores', label: 'Ядра' },
]

const emit = defineEmits(['sort'])

const handleSort = (field) => {
  if (sortField.value === field) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortOrder.value = 'asc'
  }
  emit('sort', sortField.value, sortOrder.value)
}

const sortedPhones = computed(() => {
  if (!sortField.value) return props.phones

  return [...props.phones].sort((a, b) => {
    let aVal = a[sortField.value]
    let bVal = b[sortField.value]

    if (aVal === null || aVal === undefined) aVal = 0
    if (bVal === null || bVal === undefined) bVal = 0

    if (typeof aVal === 'string') {
      aVal = aVal.toLowerCase()
      bVal = bVal.toLowerCase()
    }

    if (sortOrder.value === 'asc') {
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
