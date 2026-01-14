import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:8000',
  headers: {
    'Content-Type': 'application/json',
  },
})

export const phonesAPI = {
  getSmartphones: (params = {}) => api.get('/api/parser/smartphones', { params }),
  getSmartphoneCount: () => api.get('/api/parser/smartphones/count'),
  getParsingStatus: () => api.get('/api/parser/status'),
  startParsing: (params) => api.post('/api/parser/start', null, { params }),
}

export const analysisAPI = {
  getStatistics: () => api.get('/api/analysis/statistics'),
  calculateTopsis: (data) => {
    const { filters, weights } = data
    return api.post('/api/analysis/topsis', {
      filters: filters || null,
      weights: weights || null,
    })
  },
}

export default api
