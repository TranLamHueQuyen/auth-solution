"use client"

import { ProtectedRoute } from "@/components/protected-route"
import { useAuthStore } from "@/store/auth-store"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { useRouter } from "next/navigation"
import { ArrowLeft, User, Shield, Calendar } from "lucide-react"

function ProfileContent() {
  const { user } = useAuthStore()
  const router = useRouter()

  return (
    <div className="min-h-screen bg-muted/30 p-8">
      <div className="max-w-3xl mx-auto space-y-8">
        <Button variant="ghost" onClick={() => router.push("/dashboard")} className="gap-2">
          <ArrowLeft className="h-4 w-4" />
          Back to Dashboard
        </Button>

        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight">Profile</h1>
          <p className="text-muted-foreground">View and manage your account information</p>
        </div>

        <Card>
          <CardHeader>
            <div className="flex items-center gap-4">
              <div className="h-16 w-16 rounded-full bg-primary/10 flex items-center justify-center">
                <User className="h-8 w-8 text-primary" />
              </div>
              <div>
                <CardTitle className="text-2xl">{user?.username}</CardTitle>
                <CardDescription>User ID: {user?.id}</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-4">
              <div className="flex items-center gap-3 p-4 bg-muted/50 rounded-lg">
                <Shield className="h-5 w-5 text-muted-foreground" />
                <div className="flex-1">
                  <p className="text-sm font-medium">Role</p>
                  <p className="text-sm text-muted-foreground">Account permissions level</p>
                </div>
                <Badge variant="secondary" className="capitalize">
                  {user?.role}
                </Badge>
              </div>

              <div className="flex items-center gap-3 p-4 bg-muted/50 rounded-lg">
                <User className="h-5 w-5 text-muted-foreground" />
                <div className="flex-1">
                  <p className="text-sm font-medium">Username</p>
                  <p className="text-sm text-muted-foreground">Your unique identifier</p>
                </div>
                <span className="text-sm font-medium">{user?.username}</span>
              </div>

              <div className="flex items-center gap-3 p-4 bg-muted/50 rounded-lg">
                <Calendar className="h-5 w-5 text-muted-foreground" />
                <div className="flex-1">
                  <p className="text-sm font-medium">Account Status</p>
                  <p className="text-sm text-muted-foreground">Current session state</p>
                </div>
                <Badge variant="default" className="bg-green-500">
                  Active
                </Badge>
              </div>
            </div>

            <div className="pt-4 border-t">
              <Button variant="outline" className="w-full bg-transparent" disabled>
                Edit Profile (Coming Soon)
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Security</CardTitle>
            <CardDescription>Manage your account security settings</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <Button variant="secondary" className="w-full" disabled>
              Change Password
            </Button>
            <Button variant="secondary" className="w-full" disabled>
              Two-Factor Authentication
            </Button>
            <Button variant="secondary" className="w-full" disabled>
              Active Sessions
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default function ProfilePage() {
  return (
    <ProtectedRoute>
      <ProfileContent />
    </ProtectedRoute>
  )
}
