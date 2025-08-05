import axios from 'axios';

const instance = axios.create({
  baseURL: 'http://localhost:5180/api/account/register', // або твоя реальна адреса
  withCredentials: true // щоб працювали куки (наприклад, refresh token)
});
axios.defaults.withCredentials = true;

export default instance;
