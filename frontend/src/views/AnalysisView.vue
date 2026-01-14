<template>
  <div class="px-4 py-8">
    <div class="mb-6">
      <h1 class="text-3xl font-bold text-gray-900">Анализ данных</h1>
      <p class="text-gray-600 mt-2">Просмотр, фильтрация и сортировка смартфонов</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <!-- Фильтры -->
      <div class="lg:col-span-1">
        <FiltersPanel
          :filters="filters"
          :available-brands="availableBrands"
          @apply="handleFilterApply"
        />
      </div>

      <!-- Основной контент -->
      <div class="lg:col-span-3 space-y-6">
        <!-- Графики -->
        <ChartsSection :phones="phones" />

        <!-- Переключатель вида -->
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">
            Смартфоны ({{ phones.length }})
          </h2>
          <div class="flex items-center space-x-4">
            <div class="flex bg-gray-100 rounded-lg p-1">
              <button
                @click="viewMode = 'cards'"
                :class="viewMode === 'cards' ? 'bg-white text-blue-600 shadow' : 'text-gray-600'"
                class="px-4 py-2 rounded-md text-sm font-medium transition"
              >
                Карточки
              </button>
              <button
                @click="viewMode = 'table'"
                :class="viewMode === 'table' ? 'bg-white text-blue-600 shadow' : 'text-gray-600'"
                class="px-4 py-2 rounded-md text-sm font-medium transition"
              >
                Таблица
              </button>
            </div>
            <button
              @click="fetchPhones"
              :disabled="loading"
              class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              {{ loading ? 'Загрузка...' : 'Обновить' }}
            </button>
          </div>
        </div>

        <!-- Контент -->
        <div v-if="loading" class="text-center py-12">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
        <div v-else-if="error" class="text-red-600 bg-red-50 p-4 rounded-lg">
          {{ error }}
        </div>
        <div v-else>
          <PhoneCards v-if="viewMode === 'cards'" :phones="phones" :sort-field="sortField" :sort-order="sortOrder" />
          <PhoneTable v-else :phones="phones" @sort="handleSort" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { usePhonesStore } from '@/stores/phones'
import PhoneTable from '@/components/PhoneTable.vue'
import PhoneCards from '@/components/PhoneCards.vue'
import FiltersPanel from '@/components/FiltersPanel.vue'
import ChartsSection from '@/components/ChartsSection.vue'

const phonesStore = usePhonesStore()
const { smartphones, loading, error } = storeToRefs(phonesStore)

const filters = ref({})
const viewMode = ref('cards')
const sortField = ref(null)
const sortOrder = ref('asc')

const phones = computed(() => smartphones.value || [])

const availableBrands = computed(() => {
  const brands = new Set()
  if (smartphones.value) {
    smartphones.value.forEach(phone => {
      if (phone.brand) brands.add(phone.brand)
    })
  }
  return Array.from(brands).sort()
})

const handleFilterApply = (newFilters) => {
  filters.value = newFilters
  fetchPhones()
}

const handleSort = (field, order) => {
  sortField.value = field
  sortOrder.value = order
}

const fetchPhones = () => {
  phonesStore.fetchSmartphones(filters.value)
}

onMounted(() => {
  fetchPhones()
})
</script>
