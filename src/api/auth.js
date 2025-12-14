import axios from 'axios';
import BASE_URL from '../config/baseUrl';

const API_URL = `${BASE_URL}api/Auth`;

export const login = async (username, password) => {
  const response = await axios.post(`${API_URL}/login`, {
    username,
    password
  });
  return response.data;
};
