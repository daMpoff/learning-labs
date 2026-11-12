<script setup>
import { computed } from 'vue';
import { useScanStore } from '@/stores/scan';
import FileUpload from '@/components/FileUpload.vue';
import ScanProgress from '@/components/ScanProgress.vue';
import ScanResults from '@/components/ScanResults.vue';

const scanStore = useScanStore();

const showUpload = computed(() => !scanStore.currentScanId && !scanStore.scanResults);
const showProgress = computed(() => scanStore.currentScanId && !scanStore.scanResults && !scanStore.isPolling);
const showResults = computed(() => scanStore.scanResults || (scanStore.currentScanId && scanStore.isPolling));
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-primary-50 to-white">
    <div class="container mx-auto px-4 py-8">
      <header class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-900 mb-2">
          🛡️ VirusTotal File Scanner
        </h1>
        <p class="text-gray-600">
          Загрузите файл для сканирования множеством антивирусных движков
        </p>
      </header>

      <main class="max-w-4xl mx-auto">
        <FileUpload v-if="showUpload" />
        <ScanProgress v-if="showProgress" />
        <ScanResults v-if="showResults" />
      </main>
    </div>
  </div>
</template>

<style scoped></style>
