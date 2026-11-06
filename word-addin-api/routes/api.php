<?php

use App\Http\Controllers\AddinController;
use Illuminate\Support\Facades\Route;

Route::prefix('v1')->middleware('addin.token')->group(function () {
    Route::get('ribbon', [AddinController::class, 'ribbon']);
    Route::get('forms/{commandId}', [AddinController::class, 'form']);
    Route::post('commands/{commandId}', [AddinController::class, 'command']);
});