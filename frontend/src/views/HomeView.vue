<template>
  <div class="px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-900 mb-4">
          Система выбора оптимального смартфона
        </h1>
        <p class="text-xl text-gray-600 mb-8">
          На основе метода TOPSIS (Technique for Order Preference by Similarity to Ideal Solution)
        </p>
      </div>

      <!-- Статистика данных -->
      <div class="bg-white rounded-lg shadow-lg p-6 mb-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-2xl font-semibold text-gray-900">Статистика данных</h2>
          <button
            @click="fetchStatistics"
            :disabled="analysisStore.loading"
            class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition"
          >
            Обновить статистику
          </button>
        </div>
        <div v-if="analysisStore.loading" class="text-center py-8">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
        <div v-else-if="analysisStore.error" class="text-red-600 bg-red-50 p-4 rounded-lg">{{ analysisStore.error }}</div>
        <div v-else-if="analysisStore.statistics" class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="bg-blue-50 p-4 rounded-lg">
            <div class="text-sm text-gray-600">Всего записей</div>
            <div class="text-3xl font-bold text-blue-600">{{ analysisStore.statistics.total_count }}</div>
          </div>
          <div class="bg-green-50 p-4 rounded-lg">
            <div class="text-sm text-gray-600">Валидных записей</div>
            <div class="text-3xl font-bold text-green-600">{{ analysisStore.statistics.valid_count }}</div>
          </div>
          <div class="bg-purple-50 p-4 rounded-lg">
            <div class="text-sm text-gray-600">Количество брендов</div>
            <div class="text-3xl font-bold text-purple-600">{{ analysisStore.statistics.brands_count }}</div>
          </div>
        </div>
      </div>

      <!-- Управление парсингом -->
      <div class="bg-white rounded-lg shadow-lg p-6 mb-6">
        <h2 class="text-2xl font-semibold text-gray-900 mb-4">Обновление базы данных</h2>
        <div class="space-y-4">
          <button
            @click="startParsing"
            :disabled="parsingStatus.is_running || phonesStore.loading"
            class="w-full px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition font-semibold text-lg"
          >
            {{ parsingStatus.is_running ? 'Парсинг выполняется...' : 'Запустить парсинг (Обновить базу данных)' }}
          </button>

          <!-- Статус парсинга -->
          <div v-if="parsingStatus.is_running" class="bg-blue-50 p-4 rounded-lg border border-blue-200">
            <div class="flex justify-between items-center mb-2">
              <span class="font-semibold text-gray-900">Прогресс парсинга:</span>
              <span class="text-blue-600 font-bold">
                {{ parsingStatus.progress }} / {{ parsingStatus.total }}
                ({{ parsingProgressPercent }}%)
              </span>
            </div>
            <div class="w-full bg-gray-200 rounded-full h-3 mb-2">
              <div
                class="bg-blue-600 h-3 rounded-full transition-all duration-300"
                :style="{ width: parsingProgressPercent + '%' }"
              ></div>
            </div>
            <div v-if="parsingStatus.current_item" class="text-sm text-gray-600">
              Текущий товар: {{ parsingStatus.current_item }}
            </div>
          </div>

          <div v-if="parsingStatus.error" class="bg-red-50 p-4 rounded-lg border border-red-200">
            <div class="font-semibold text-red-800 mb-1">Ошибка парсинга:</div>
            <div class="text-red-600">{{ parsingStatus.error }}</div>
          </div>

          <div v-if="!parsingStatus.is_running && parsingStatus.total > 0" class="bg-green-50 p-4 rounded-lg border border-green-200">
            <div class="text-green-800 font-semibold">Парсинг завершен успешно!</div>
            <div class="text-green-600 text-sm mt-1">
              Обработано {{ parsingStatus.total }} товаров
            </div>
          </div>
        </div>
      </div>

      <!-- Быстрые действия -->
      <div class="bg-white rounded-lg shadow-lg p-6">
        <h2 class="text-2xl font-semibold text-gray-900 mb-4">Быстрые действия</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <router-link
            to="/analysis"
            class="block p-6 bg-gradient-to-r from-blue-500 to-blue-600 text-white rounded-lg hover:from-blue-600 hover:to-blue-700 transition"
          >
            <h3 class="text-xl font-semibold mb-2">Анализ данных</h3>
            <p>Просмотр, фильтрация и сортировка смартфонов</p>
          </router-link>
          <router-link
            to="/results"
            class="block p-6 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg hover:from-green-600 hover:to-green-700 transition"
          >
            <h3 class="text-xl font-semibold mb-2">Результаты TOPSIS</h3>
            <p>Расчет и просмотр лучших альтернатив</p>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, computed, onUnmounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useAnalysisStore } from '@/stores/analysis'
import { usePhonesStore } from '@/stores/phones'

const analysisStore = useAnalysisStore()
const phonesStore = usePhonesStore()
const { statistics, loading, error } = storeToRefs(analysisStore)

// Используем storeToRefs для реактивности parsingStatus
const { parsingStatus } = storeToRefs(phonesStore)

const parsingProgressPercent = computed(() => {
  if (parsingStatus.value.total === 0) return 0
  return Math.round((parsingStatus.value.progress / parsingStatus.value.total) * 100)
})

let statusInterval = null

const fetchStatistics = async () => {
  await analysisStore.fetchStatistics()
}

const startParsing = async () => {
  try {
    // Останавливаем предыдущий интервал, если он есть
    if (statusInterval) {
      clearInterval(statusInterval)
      statusInterval = null
    }
    
    await phonesStore.startParsing({ use_selenium: true, use_page_all: true })
    
    // Начинаем отслеживание статуса с задержкой
    statusInterval = setInterval(async () => {
      await phonesStore.fetchParsingStatus()
      if (!parsingStatus.value.is_running) {
        clearInterval(statusInterval)
        statusInterval = null
        // Обновляем статистику после завершения парсинга
        await fetchStatistics()
        await phonesStore.fetchCount()
      }
    }, 3000) // Увеличил интервал до 3 секунд
  } catch (error) {
    console.error('Ошибка при запуске парсинга:', error)
    if (statusInterval) {
      clearInterval(statusInterval)
      statusInterval = null
    }
  }
}

onMounted(() => {
  fetchStatistics()
  phonesStore.fetchParsingStatus()
  phonesStore.fetchCount()
})

onUnmounted(() => {
  if (statusInterval) {
    clearInterval(statusInterval)
    statusInterval = null
  }
})
</script>
