<template>
  <div class="bg-white rounded-lg shadow-lg p-6">
    <h3 class="text-lg font-semibold text-gray-900 mb-4">Фильтры</h3>
    
    <div class="space-y-4">
      <!-- Бренды -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Бренды
        </label>
        <select
          v-model="localFilters.brands"
          multiple
          class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        >
          <option
            v-for="brand in availableBrands"
            :key="brand"
            :value="brand"
          >
            {{ brand }}
          </option>
        </select>
      </div>

      <!-- Цена -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Цена: {{ localFilters.min_price || 0 }} — {{ localFilters.max_price || '∞' }} ₽
        </label>
        <div class="grid grid-cols-2 gap-2">
          <input
            v-model.number="localFilters.min_price"
            type="number"
            placeholder="От"
            class="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            v-model.number="localFilters.max_price"
            type="number"
            placeholder="До"
            class="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      <!-- RAM -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          RAM (ГБ): от {{ localFilters.min_ram || 0 }}
        </label>
        <input
          v-model.number="localFilters.min_ram"
          type="number"
          min="0"
          placeholder="Минимум"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        />
      </div>

      <!-- Память -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Память (ГБ): от {{ localFilters.min_storage || 0 }}
        </label>
        <input
          v-model.number="localFilters.min_storage"
          type="number"
          min="0"
          placeholder="Минимум"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        />
      </div>

      <!-- Кнопки -->
      <div class="flex space-x-2 pt-2">
        <button
          @click="applyFilters"
          class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
        >
          Применить
        </button>
        <button
          @click="resetFilters"
          class="flex-1 px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition"
        >
          Сбросить
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  filters: {
    type: Object,
    default: () => ({}),
  },
  availableBrands: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(['update:filters', 'apply'])

const localFilters = ref({
  brands: [],
  min_price: null,
  max_price: null,
  min_ram: null,
  min_storage: null,
})

// Инициализируем фильтры из пропсов
if (props.filters.brands) {
  localFilters.value.brands = Array.isArray(props.filters.brands) 
    ? props.filters.brands 
    : typeof props.filters.brands === 'string' 
      ? props.filters.brands.split(',').map(b => b.trim())
      : []
} else {
  localFilters.value.brands = []
}

if (props.filters.min_price !== undefined) localFilters.value.min_price = props.filters.min_price
if (props.filters.max_price !== undefined) localFilters.value.max_price = props.filters.max_price
if (props.filters.min_ram !== undefined) localFilters.value.min_ram = props.filters.min_ram
if (props.filters.min_storage !== undefined) localFilters.value.min_storage = props.filters.min_storage

watch(() => props.filters, (newFilters) => {
  if (newFilters.brands) {
    localFilters.value.brands = Array.isArray(newFilters.brands) 
      ? newFilters.brands 
      : typeof newFilters.brands === 'string' 
        ? newFilters.brands.split(',').map(b => b.trim())
        : []
  } else {
    localFilters.value.brands = []
  }
  if (newFilters.min_price !== undefined) localFilters.value.min_price = newFilters.min_price
  if (newFilters.max_price !== undefined) localFilters.value.max_price = newFilters.max_price
  if (newFilters.min_ram !== undefined) localFilters.value.min_ram = newFilters.min_ram
  if (newFilters.min_storage !== undefined) localFilters.value.min_storage = newFilters.min_storage
}, { deep: true })

const applyFilters = () => {
  const cleanFilters = {}
  if (localFilters.value.brands && localFilters.value.brands.length > 0) {
    cleanFilters.brands = Array.isArray(localFilters.value.brands) 
      ? localFilters.value.brands.join(',') 
      : localFilters.value.brands
  }
  if (localFilters.value.min_price !== null && localFilters.value.min_price !== undefined && localFilters.value.min_price !== '') {
    cleanFilters.min_price = Number(localFilters.value.min_price)
  }
  if (localFilters.value.max_price !== null && localFilters.value.max_price !== undefined && localFilters.value.max_price !== '') {
    cleanFilters.max_price = Number(localFilters.value.max_price)
  }
  if (localFilters.value.min_ram !== null && localFilters.value.min_ram !== undefined && localFilters.value.min_ram !== '') {
    cleanFilters.min_ram = Number(localFilters.value.min_ram)
  }
  if (localFilters.value.min_storage !== null && localFilters.value.min_storage !== undefined && localFilters.value.min_storage !== '') {
    cleanFilters.min_storage = Number(localFilters.value.min_storage)
  }
  
  emit('apply', cleanFilters)
}

const resetFilters = () => {
  localFilters.value = {
    brands: [],
    min_price: null,
    max_price: null,
    min_ram: null,
    min_storage: null,
  }
  emit('apply', {})
}
</script>
