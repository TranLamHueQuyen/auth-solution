import axiosClient from "./axios-client";
import { useAuthStore } from "@/store/auth-store";

// Interfaces
export interface LoginCredentials { username: string; password: string }
export interface RegisterCredentials { username: string; password: string }
export interface AuthResponse { accessToken: string } // đồng bộ với login
export interface UserResponse { id: string; username: string; role: string }

// Register user
export async function register(credentials: RegisterCredentials): Promise<AuthResponse> {
  const res = await axiosClient.post<AuthResponse>("/register", credentials);
  return res.data;
}

// Login user
export async function login(credentials: LoginCredentials): Promise<{ accessToken: string }> {
  const res = await axiosClient.post("/login", credentials)
  return res.data
}

// Get current user
export async function getMe(): Promise<UserResponse> {
  const res = await axiosClient.get<UserResponse>("/me");
  return res.data;
}

// Logout user
export async function logout() {
  const { user, logout: clearAuth } = useAuthStore.getState()
  try {
    // Gửi userId nếu có
    if (user?.id) {
      await axiosClient.post("/logout", { userId: user.id })
    } else {
      await axiosClient.post("/logout", {})
    }
  } catch (err) {
    console.error("[v0] Logout API error:", err)
  } finally {
    clearAuth()  // xóa access token + user trong store
    if (typeof window !== "undefined") window.location.href = "/login"
  }
}


