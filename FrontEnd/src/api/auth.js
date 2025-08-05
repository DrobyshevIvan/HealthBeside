import axios from './axios';

export const register = async (userData) => {
  const response = await axios.post('/register', userData);
  return response.data;
};

export const login = async (userData) => {
  const response = await axios.post('/login', userData);
  return response.data;
};

export const logout = async () => {
  const response = await axios.post('/logout');
  return response.data;
};

export const refreshToken = async () => {
  const response = await axios.post('/refresh-token');
  return response.data;
};

export const getUserInfo = async () => {
  const response = await axios.get('/get-user-info');
  return response.data;
};
