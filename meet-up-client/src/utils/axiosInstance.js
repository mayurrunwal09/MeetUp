// src/utils/axiosInstance.js
import axios from 'axios';
import BASE_URL from '../config/baseUrl';

const axiosInstance = axios.create({
  baseURL: `${BASE_URL}api/`
});

axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default axiosInstance;
