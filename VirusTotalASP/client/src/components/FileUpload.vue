<template>
  <div class="w-full">
    <div
      class="border-2 border-dashed border-primary-300 rounded-xl p-12 text-center transition-all duration-200"
      :class="{
        'border-primary-500 bg-primary-50': isDragging,
        'border-primary-300 bg-white': !isDragging,
      }"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      @drop.prevent="handleDrop"
    >
      <input
        ref="fileInput"
        type="file"
        class="hidden"
        @change="handleFileSelect"
      />
      
      <div class="flex flex-col items-center space-y-4">
        <div class="w-16 h-16 bg-primary-100 rounded-full flex items-center justify-center">
          <svg
            class="w-8 h-8 text-primary-600"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
            />
          </svg>
        </div>
        
        <div>
          <p class="text-lg font-medium text-gray-700">
            Перетащите файл сюда или
            <button
              type="button"
              class="text-primary-600 hover:text-primary-700 font-semibold underline"
              @click="$refs.fileInput.click()"
            >
              выберите файл
            </button>
          </p>
          <p class="text-sm text-gray-500 mt-2">
            Максимальный размер: 32MB
          </p>
        </div>
      </div>
    </div>

    <div v-if="selectedFile" class="mt-6 bg-white rounded-lg border border-primary-200 p-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-3">
          <div class="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
            <svg
              class="w-5 h-5 text-primary-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
              />
            </svg>
          </div>
          <div>
            <p class="font-medium text-gray-900">{{ selectedFile.name }}</p>
            <p class="text-sm text-gray-500">{{ formatFileSize(selectedFile.size) }}</p>
          </div>
        </div>
        <button
          type="button"
          class="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="isUploading"
          @click="handleUpload"
        >
          {{ isUploading ? 'Загрузка...' : 'Сканировать' }}
        </button>
      </div>
    </div>

    <div v-if="error" class="mt-4 bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800 text-sm">{{ error }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useScanStore } from '@/stores/scan';

const scanStore = useScanStore();
const fileInput = ref(null);
const isDragging = ref(false);

const selectedFile = computed(() => scanStore.selectedFile);
const isUploading = computed(() => scanStore.isUploading);
const error = computed(() => scanStore.error);

const handleDrop = (event) => {
  isDragging.value = false;
  const files = event.dataTransfer.files;
  if (files.length > 0) {
    scanStore.setSelectedFile(files[0]);
  }
};

const handleFileSelect = (event) => {
  const files = event.target.files;
  if (files.length > 0) {
    scanStore.setSelectedFile(files[0]);
  }
};

const handleUpload = () => {
  scanStore.uploadFile();
};

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
};
</script>

