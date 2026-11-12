<template>
  <div class="w-full space-y-6">
    <div class="flex items-center justify-between">
      <h2 class="text-2xl font-bold text-gray-900">Результаты сканирования</h2>
      <button
        type="button"
        class="px-4 py-2 text-primary-600 hover:text-primary-700 font-medium underline"
        @click="handleReset"
      >
        Сканировать другой файл
      </button>
    </div>

    <div v-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800">{{ error }}</p>
    </div>

    <!-- Progress indicator when scanning -->
    <div v-if="isScanning && !error" class="bg-primary-50 border border-primary-200 rounded-lg p-4 flex items-center space-x-3">
      <div class="relative w-5 h-5">
        <div class="absolute inset-0 border-2 border-primary-200 rounded-full"></div>
        <div class="absolute inset-0 border-2 border-primary-600 rounded-full border-t-transparent animate-spin"></div>
      </div>
      <p class="text-sm text-primary-700 font-medium">
        Сканирование в процессе... Результаты обновляются в реальном времени
      </p>
    </div>

    <div v-if="stats" class="space-y-4">
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div class="bg-green-50 border border-green-200 rounded-lg p-4 text-center transition-all duration-300">
          <div class="text-3xl font-bold text-green-700 transition-all duration-300">{{ calculatedStats.harmless }}</div>
          <div class="text-sm text-green-600 mt-1">Безопасно</div>
          <div class="text-xs text-green-500 mt-1" title="Антивирусы определили файл как безопасный">Без угроз</div>
        </div>
        
        <div class="bg-red-50 border border-red-200 rounded-lg p-4 text-center transition-all duration-300">
          <div class="text-3xl font-bold text-red-700 transition-all duration-300">{{ calculatedStats.malicious }}</div>
          <div class="text-sm text-red-600 mt-1">Вредоносное</div>
          <div class="text-xs text-red-500 mt-1" title="Антивирусы определили файл как вредоносный">Обнаружена угроза</div>
        </div>
        
        <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-4 text-center transition-all duration-300">
          <div class="text-3xl font-bold text-yellow-700 transition-all duration-300">{{ calculatedStats.suspicious }}</div>
          <div class="text-sm text-yellow-600 mt-1">Подозрительное</div>
          <div class="text-xs text-yellow-500 mt-1" title="Антивирусы определили файл как подозрительный">Требует внимания</div>
        </div>
        
        <div class="bg-gray-50 border border-gray-200 rounded-lg p-4 text-center transition-all duration-300">
          <div class="text-3xl font-bold text-gray-700 transition-all duration-300">{{ calculatedStats.undetected }}</div>
          <div class="text-sm text-gray-600 mt-1">Не обнаружено</div>
          <div class="text-xs text-gray-500 mt-1" title="Антивирусы не обнаружили угроз">Нет детекта</div>
        </div>
      </div>
      
      <!-- Info box explaining categories -->
      <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <h4 class="text-sm font-semibold text-blue-900 mb-2">📋 Объяснение категорий:</h4>
        <div class="text-xs text-blue-800 space-y-1">
          <p><strong>Безопасно (Harmless):</strong> Антивирусы определили файл как безопасный, без угроз.</p>
          <p><strong>Вредоносное (Malicious):</strong> Файл содержит вредоносный код или вирус.</p>
          <p><strong>Подозрительное (Suspicious):</strong> Файл имеет подозрительные характеристики, но не определен как вредоносный.</p>
          <p><strong>Не обнаружено (Undetected):</strong> Антивирус не обнаружил угроз, но и не подтвердил безопасность.</p>
        </div>
      </div>
    </div>

    <div v-if="results && Object.keys(results).length > 0" class="bg-white rounded-lg border border-primary-200 overflow-hidden">
      <div class="bg-primary-50 px-6 py-4 border-b border-primary-200">
        <h3 class="text-lg font-semibold text-gray-900">Детальные результаты</h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Антивирус
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Результат
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Категория
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Метод
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="(result, engine) in results"
              :key="`${engine}-${result.category || 'none'}-${result.result || 'none'}`"
              class="hover:bg-gray-50 transition-colors duration-200"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">
                  {{ result.engine_Name || result.engine_name || engine }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  class="px-2 py-1 text-xs font-semibold rounded-full"
                  :class="getResultBadgeClass(result.category)"
                >
                  {{ result.result ?? 'N/A' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ result.category || 'N/A' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ result.method || 'N/A' }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useScanStore } from '@/stores/scan';

const scanStore = useScanStore();

const results = computed(() => {
  const res = scanStore.scanResults?.results;
  // Убеждаемся, что возвращаем объект, а не null/undefined
  return res && typeof res === 'object' ? res : {};
});
const error = computed(() => scanStore.error);
const isScanning = computed(() => {
  if (!scanStore) return false;
  const status = scanStore.scanStatus;
  return (status === 'queued' || status === 'in-progress' || scanStore.isPolling) ?? false;
});

// Вычисляем статистику из результатов, если не предоставлена API
const stats = computed(() => {
  const apiStats = scanStore.scanResults?.stats;
  const scanResults = results.value;
  
  // Если есть статистика от API, используем её (она более точная)
  if (apiStats && (apiStats.harmless !== undefined || apiStats.malicious !== undefined)) {
    return apiStats;
  }
  
  // Иначе вычисляем из результатов
  if (!scanResults || Object.keys(scanResults).length === 0) {
    return null;
  }
  
  const calculatedStats = {
    harmless: 0,
    malicious: 0,
    suspicious: 0,
    undetected: 0,
    typeUnsupported: 0,
    confirmedTimeout: 0,
    timeout: 0,
    failure: 0,
  };
  
  Object.values(scanResults).forEach((result) => {
    const category = result?.category?.toLowerCase() || '';
    
    if (category === 'harmless') {
      calculatedStats.harmless++;
    } else if (category === 'malicious') {
      calculatedStats.malicious++;
    } else if (category === 'suspicious') {
      calculatedStats.suspicious++;
    } else if (category === 'undetected') {
      calculatedStats.undetected++;
    } else if (category === 'type-unsupported' || category === 'typeunsupported') {
      calculatedStats.typeUnsupported++;
    } else if (category === 'confirmed-timeout' || category === 'confirmedtimeout') {
      calculatedStats.confirmedTimeout++;
    } else if (category === 'timeout') {
      calculatedStats.timeout++;
    } else if (category === 'failure') {
      calculatedStats.failure++;
    } else if (category) {
      // Считаем только если категория существует, не используем значение по умолчанию undetected
      // Неизвестные категории не учитываются
      console.warn('Unknown category:', category);
    }
  });
  
  return calculatedStats;
});

// Computed для отображаемых значений - используем статистику API, если доступна, иначе вычисленную
const calculatedStats = computed(() => {
  const s = stats.value;
  
  if (!s) {
    return {
      harmless: 0,
      malicious: 0,
      suspicious: 0,
      undetected: 0,
    };
  }
  
  // Используем статистику API напрямую - она более точная
  // Убеждаемся, что возвращаем числа, а не null/undefined
  // Статистика API является авторитетной - используем её как есть
  const result = {
    harmless: Number(s.harmless ?? 0),
    malicious: Number(s.malicious ?? 0),
    suspicious: Number(s.suspicious ?? 0),
    undetected: Number(s.undetected ?? 0),
  };
  
  return result;
});

const getResultBadgeClass = (category) => {
  if (category === 'malicious') {
    return 'bg-red-100 text-red-800';
  }
  if (category === 'suspicious') {
    return 'bg-yellow-100 text-yellow-800';
  }
  if (category === 'harmless' || category === 'undetected') {
    return 'bg-green-100 text-green-800';
  }
  return 'bg-gray-100 text-gray-800';
};

const handleReset = () => {
  scanStore.resetScan();
};
</script>

