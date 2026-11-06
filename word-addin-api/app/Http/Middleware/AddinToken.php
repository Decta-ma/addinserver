<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class AddinToken
{
    public function handle(Request $request, Closure $next): Response
    {
        if (! hash_equals((string) config('addin.token'), (string) $request->bearerToken())) {
            return response()->json(['message' => 'Invalid API token.'], 401);
        }

        return $next($request);
    }
}