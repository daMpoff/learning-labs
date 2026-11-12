<template>
  <div class="bg-white rounded-xl border border-primary-200 p-8 text-center">
    <div class="flex flex-col items-center space-y-4">
      <div class="relative w-20 h-20">
        <div class="absolute inset-0 border-4 border-primary-200 rounded-full"></div>
        <div
          class="absolute inset-0 border-4 border-primary-600 rounded-full border-t-transparent animate-spin"
        ></div>
      </div>
      
      <div>
        <h3 class="text-xl font-semibold text-gray-900 mb-2">Сканирование в процессе</h3>
        <p class="text-gray-600">{{ statusText }}</p>
        <p class="text-sm text-gray-500 mt-2">ID анализа: {{ scanId }}</p>
      </div>

      <button
        type="button"
        class="mt-4 px-4 py-2 text-primary-600 hover:text-primary-700 font-medium underline"
        @click="handleCancel"
      >
        Отменить и загрузить новый файл
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useScanStore } from '@/stores/scan';

const scanStore = useScanStore();

const scanId = computed(() => scanStore.currentScanId);
const scanStatus = computed(() => scanStore.scanStatus);

const statusText = computed(() => {
  const status = scanStatus.value;
  if (status === 'queued') return 'Файл в очереди на сканирование...';
  if (status === 'in-progress') return 'Сканирование выполняется...';
  return 'Обработка результатов...';
});

const handleCancel = () => {
  scanStore.resetScan();
};
</script>

