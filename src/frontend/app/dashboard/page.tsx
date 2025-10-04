"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { useAuthStore } from "@/store/auth-store"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useRouter } from "next/navigation"
import { logout } from "@/lib/auth"
import { useState } from "react"

function DashboardContent() {
  const { user } = useAuthStore()
  const router = useRouter()
  const [isLoggingOut, setIsLoggingOut] = useState(false)

  async function handleLogout() {
    setIsLoggingOut(true)
    await logout()
  }

  return (
    <div className="min-h-screen bg-muted/30 p-8">
      <div className="max-w-4xl mx-auto space-y-8">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
          <p className="text-muted-foreground">Welcome back, {user?.username}!</p>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle>User Information</CardTitle>
              <CardDescription>Your account details</CardDescription>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Username:</span>
                <span className="font-medium">{user?.username}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">User ID:</span>
                <span className="font-medium">{user?.id}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Role:</span>
                <span className="font-medium capitalize">{user?.role}</span>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Quick Actions</CardTitle>
              <CardDescription>Manage your account</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <Button variant="secondary" className="w-full" onClick={() => router.push("/profile")}>
                View Profile
              </Button>
              <Button variant="secondary" className="w-full" disabled>
                Settings (Coming Soon)
              </Button>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Authentication Status</CardTitle>
            <CardDescription>Your session information</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex items-center gap-2">
              <div className="h-2 w-2 rounded-full bg-green-500" />
              <span className="text-sm">Authenticated and active</span>
            </div>
            <p className="text-sm text-muted-foreground mt-2">
              Your session is protected with token-based authentication. Tokens are automatically refreshed when needed.
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default function DashboardPage() {
  return (
    <ProtectedRoute>
      <DashboardContent />
    </ProtectedRoute>
  )
}
