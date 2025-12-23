const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5088/api';

class ApiService {
  async scanFile(file) {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_BASE_URL}/scan/file`, {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Failed to upload file' }));
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    return await response.json();
  }

  async getScanResult(id, type = 'analysis') {
    const response = await fetch(`${API_BASE_URL}/scan/result/${id}?type=${type}`, {
      method: 'GET',
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Failed to get scan result' }));
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    return await response.json();
  }
}

export default new ApiService();

