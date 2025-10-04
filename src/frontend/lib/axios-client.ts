import axios from "axios";
import { useAuthStore } from "@/store/auth-store";

export const axiosClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || "http://localhost:5038/api/auth",
  headers: { "Content-Type": "application/json" },
  withCredentials: true,
});

axiosClient.interceptors.request.use(
  (config) => {
    const accessToken = useAuthStore.getState().accessToken;
    if (accessToken) config.headers.Authorization = `Bearer ${accessToken}`;
    return config;
  },
  (error) => Promise.reject(error)
);

let isRefreshing = false;
let failedQueue: Array<{ resolve: (value?: any) => void; reject: (err?: any) => void }> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(p => { if (error) p.reject(error); else p.resolve(token); });
  failedQueue = [];
};

axiosClient.interceptors.response.use(
  res => res,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => failedQueue.push({ resolve, reject }))
          .then(token => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return axiosClient(originalRequest);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const response = await axios.post(
          `${process.env.NEXT_PUBLIC_API_URL || "http://localhost:5038/api"}/auth/refresh`,
          {},
          { withCredentials: true }
        );

        const newAccessToken = response.data.accessToken;
        useAuthStore.getState().setAccessToken(newAccessToken);
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        processQueue(null, newAccessToken);

        return axiosClient(originalRequest);
      } catch (err) {
        processQueue(err, null);
        useAuthStore.getState().logout();
        if (typeof window !== "undefined") window.location.href = "/login";
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default axiosClient;
