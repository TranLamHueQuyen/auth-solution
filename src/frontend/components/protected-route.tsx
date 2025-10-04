"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/store/auth-store";
import { getMe } from "@/lib/auth";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const router = useRouter();
  const { accessToken, setAccessToken, user, setUser } = useAuthStore();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function checkAuth() {
      let token = accessToken;

      // Lấy từ localStorage nếu không có trong store
      if (!token && typeof window !== "undefined") {
        token = localStorage.getItem("accessToken") || "";
        if (token) setAccessToken(token);
      }

      if (!token) {
        router.push("/login");
        return;
      }

      try {
        if (!user) {
          const userData = await getMe();
          setUser(userData);
        }
        setIsLoading(false);
      } catch (err) {
        console.error("[v0] Auth check failed:", err);
        localStorage.removeItem("accessToken"); // clear nếu token invalid
        router.push("/login");
      }
    }

    checkAuth();
  }, [accessToken, user, router, setAccessToken, setUser]);

  if (isLoading) return <div className="flex items-center justify-center min-h-screen">Loading...</div>;

  return <>{children}</>;
}
