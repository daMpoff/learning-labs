import { defineStore } from 'pinia';
import api from '@/services/api';

export const useScanStore = defineStore('scan', {
  state: () => ({
    selectedFile: null,
    currentScanId: null,
    currentFileId: null,
    scanStatus: null,
    scanResults: null,
    error: null,
    isUploading: false,
    isPolling: false,
  }),

  actions: {
    setSelectedFile(file) {
      this.selectedFile = file;
      this.error = null;
      this.scanResults = null;
    },

    async uploadFile() {
      if (!this.selectedFile) {
        this.error = 'Please select a file first';
        return;
      }

      this.isUploading = true;
      this.error = null;

      try {
        const response = await api.scanFile(this.selectedFile);
        
        if (response.status === 'error') {
          this.error = response.message || 'Failed to upload file';
          this.isUploading = false;
          return;
        }

        this.currentScanId = response.analysisId || response.fileId;
        this.currentFileId = response.fileId;
        this.scanStatus = response.status || 'queued';
        
        // Начинаем опрос результатов немедленно
        // Первый опрос происходит сразу, затем продолжается с интервалом
        this.startPolling(true);
      } catch (err) {
        this.error = err.message || 'An error occurred while uploading the file';
      } finally {
        this.isUploading = false;
      }
    },

    async startPolling(immediate = false) {
      if (this.isPolling && !immediate) return;
      
      this.isPolling = true;
      const pollInterval = 3000; // 3 секунды
      let pollCount = 0;
      const maxPolls = 300; // Максимум 15 минут (300 * 3 секунды)

      const poll = async () => {
        try {
          pollCount++;
          
          // Останавливаем, если превышен максимум опросов
          if (pollCount > maxPolls) {
            this.isPolling = false;
            console.warn('Polling stopped: maximum attempts reached');
            return;
          }

          // Пробуем сначала endpoint анализа, затем endpoint файла
          let result;
          try {
            result = await api.getScanResult(this.currentScanId, 'analysis');
          } catch {
            if (this.currentFileId) {
              result = await api.getScanResult(this.currentFileId, 'file');
            } else {
              throw new Error('No file ID available');
            }
          }

          this.scanStatus = result.status;
          
          // Всегда обновляем результаты - объединяем с существующими для сохранения данных
          const hasResults = result.results && Object.keys(result.results).length > 0;
          const hasStats = result.stats;
          
          // Вспомогательная функция для создания объекта статистики
          const createStatsObject = (stats) => {
            if (!stats) return null;
            return {
              harmless: stats.harmless ?? 0,
              malicious: stats.malicious ?? 0,
              suspicious: stats.suspicious ?? 0,
              undetected: stats.undetected ?? 0,
              typeUnsupported: stats.typeUnsupported ?? 0,
              confirmedTimeout: stats.confirmedTimeout ?? 0,
              timeout: stats.timeout ?? 0,
              failure: stats.failure ?? 0,
            };
          };
          
          if (this.scanResults) {
            // Объединение: сохраняем существующие результаты, добавляем новые
            const existingResults = this.scanResults.results || {};
            const newResults = result.results || {};
            
            // Объединяем объекты результатов - новые результаты перезаписывают существующие
            // Всегда создаем новый объект для обеспечения реактивности
            const mergedResults = hasResults 
              ? { ...existingResults, ...newResults }
              : (Object.keys(existingResults).length > 0 ? existingResults : {});
            
            // Всегда используем новую статистику, если доступна, иначе сохраняем существующую
            // Создаем новый объект каждый раз для обеспечения реактивности
            const newStats = hasStats 
              ? createStatsObject(result.stats)
              : (this.scanResults.stats ? createStatsObject(this.scanResults.stats) : null);
            
            // Отладка: логируем обновление статистики
            if (hasStats && newStats) {
              console.log('Updating stats from API:', {
                harmless: newStats.harmless,
                malicious: newStats.malicious,
                suspicious: newStats.suspicious,
                undetected: newStats.undetected,
              });
            }
            
            // Создаем полностью новый объект для запуска реактивности
            this.scanResults = {
              status: result.status || this.scanResults.status,
              stats: newStats,
              results: mergedResults,
              message: result.message || this.scanResults.message,
            };
          } else {
            // Первый раз - устанавливаем начальные результаты
            this.scanResults = {
              status: result.status,
              stats: hasStats ? createStatsObject(result.stats) : null,
              results: result.results ? { ...result.results } : {},
              message: result.message,
            };
          }
          
          // Отладочное логирование
          if (hasResults) {
            console.log('Updated results:', {
              totalEngines: Object.keys(this.scanResults.results || {}).length,
              newEngines: Object.keys(result.results || {}).length,
              status: result.status
            });
          }

          // Определяем, продолжается ли сканирование
          const status = result.status?.toLowerCase() || '';
          // hasResults уже объявлен выше, переиспользуем его
          const hasStatsForCheck = result.stats && (
            result.stats.harmless !== undefined ||
            result.stats.malicious !== undefined ||
            result.stats.undetected !== undefined
          );

          // Останавливаем опрос, если статус завершен или ошибка
          const isCompleted = status === 'completed';
          const isError = status === 'error';
          
          if (isCompleted) {
            // Когда завершено, пытаемся получить полные результаты из endpoint файла
            // Но не падаем, если endpoint файла возвращает 404 - используем результаты анализа
            if (this.currentFileId && (!hasResults || Object.keys(result.results || {}).length === 0)) {
              try {
                const fileResult = await api.getScanResult(this.currentFileId, 'file');
                if (fileResult.stats || fileResult.results) {
                  // Используем результаты файла, так как они более полные
                  this.scanResults = {
                    ...fileResult,
                    stats: fileResult.stats || this.scanResults?.stats,
                    results: fileResult.results && Object.keys(fileResult.results).length > 0
                      ? { ...this.scanResults?.results, ...fileResult.results }
                      : this.scanResults?.results,
                  };
                }
              } catch (err) {
                // 404 нормально - файл может быть еще не проиндексирован, используем результаты анализа
                if (!err.message?.includes('404') && !err.message?.includes('NotFound')) {
                  console.error('Failed to get file results:', err);
                }
                // Сохраняем существующие результаты из endpoint анализа
              }
            }
            this.isPolling = false;
          } else if (isError) {
            this.isPolling = false;
          } else if (status === 'queued' || status === 'in-progress') {
            // Продолжаем опрос, если сканирование еще идет - даже если результаты пустые
            setTimeout(poll, pollInterval);
          } else {
            // Неизвестный статус - продолжаем опрос, если результатов еще нет
            // Останавливаем только если есть результаты и статус не queued/in-progress
            if (hasResults && status !== 'queued' && status !== 'in-progress') {
              this.isPolling = false;
              console.warn('Polling stopped: unknown status but results available', status);
            } else {
              // Продолжаем опрос, если результатов еще нет или неизвестный статус
              setTimeout(poll, pollInterval);
            }
          }
        } catch (err) {
          this.error = err.message || 'Failed to get scan results';
          this.isPolling = false;
        }
      };

      // Начинаем немедленно или ждем интервала
      if (immediate) {
        poll();
      } else {
        setTimeout(poll, pollInterval);
      }
    },

    resetScan() {
      this.selectedFile = null;
      this.currentScanId = null;
      this.currentFileId = null;
      this.scanStatus = null;
      this.scanResults = null;
      this.error = null;
      this.isUploading = false;
      this.isPolling = false;
    },
  },
});

