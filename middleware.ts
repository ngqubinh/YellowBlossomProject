import { NextResponse, type NextRequest } from 'next/server';

// Paths that don't require authentication
const publicPaths = ['/auth/login', '/auth/register', '/auth/reset-password', '/', 'product'];

export function middleware(request: NextRequest) {
  // Store current path in cookie for layout decision
  const response = NextResponse.next();
  response.cookies.set('next-url', request.nextUrl.pathname, { 
    path: '/',
    maxAge: 60 * 5, // 5 minutes
    httpOnly: true
  });

  // Check if the path is in publicPaths
  const isPublicPath = publicPaths.some(path => 
    request.nextUrl.pathname === path || // Exact match for root
    (path !== '/' && request.nextUrl.pathname.startsWith(path)) // Prefix match for others
  );

  // Get the authentication token from cookies
  const token = request.cookies.get('accessToken')?.value;
  const isAuthenticated = !!token; // Simple check - we'll rely on client-side validation for token expiry

  // If the path requires authentication and the user is not authenticated
  if (!isPublicPath && !isAuthenticated) {
    const url = request.nextUrl.clone();
    url.pathname = '/auth/login';
    url.searchParams.set('from', request.nextUrl.pathname);
    return NextResponse.redirect(url);
  }

  // If the user is authenticated and trying to access login page, redirect to home
  if (isAuthenticated && request.nextUrl.pathname.startsWith('/auth/')) {
    const url = request.nextUrl.clone();
    url.pathname = '/';
    return NextResponse.redirect(url);
  }

  return response;
}

// Specify which paths this middleware should run on
export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico|.*\\.png$).*)'],
};