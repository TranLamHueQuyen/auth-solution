import Link from "next/link"
import { Button } from "@/components/ui/button"

export default function HomePage() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[calc(100vh-4rem)] bg-gradient-to-b from-background to-muted/30">
      <div className="max-w-2xl px-4 text-center space-y-8">
        <h1 className="text-4xl font-bold tracking-tight sm:text-6xl text-balance">Welcome to Auth System</h1>
        <p className="text-lg text-muted-foreground text-pretty">
          A secure authentication system built with Next.js, featuring token-based auth with automatic refresh and
          protected routes.
        </p>
        <div className="flex gap-4 justify-center">
          <Button asChild size="lg">
            <Link href="/login">Login</Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link href="/register">Register</Link>
          </Button>
        </div>
      </div>
    </div>
  )
}
