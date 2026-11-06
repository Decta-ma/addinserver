<?php

namespace App\Http\Controllers;

use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Http\Response;

class AddinController extends Controller
{
    private const DOC_TYPES = [
        'report'   => ['label' => 'Report',   'color' => '#DCE9F9'],
        'letter'   => ['label' => 'Letter',   'color' => '#FFF3CD'],
        'contract' => ['label' => 'Contract', 'color' => '#DDF1E4'],
    ];

    public function ribbon(): Response
    {
        $xml = view('ribbon', [
            'tabLabel'   => 'Server Add-in',
            'groupLabel' => 'Documents',
            'buttons'    => [['id' => 'create_document', 'label' => 'Create Document']],
        ])->render();

        return response($xml, 200)->header('Content-Type', 'application/xml; charset=utf-8');
    }

    public function form(string $commandId): JsonResponse
    {
        abort_unless($commandId === 'create_document', 404);

        $options = [];
        foreach (self::DOC_TYPES as $value => $type) {
            $options[] = ['value' => $value, 'label' => $type['label']];
        }

        return response()->json([
            'title'  => 'Create Document',
            'rtl'    => false, // برای فارسی بعداً true می‌کنیم
            'fields' => [
                ['name' => 'title',        'type' => 'text',     'label' => 'Title',         'required' => true],
                ['name' => 'doc_type',     'type' => 'select',   'label' => 'Document type', 'required' => true, 'options' => $options],
                ['name' => 'has_abstract', 'type' => 'checkbox', 'label' => 'Include abstract'],
            ],
        ]);
    }

    public function command(Request $request, string $commandId): JsonResponse
    {
        abort_unless($commandId === 'create_document', 404);

        $data = $request->validate([
            'title'        => ['required', 'string', 'max:200'],
            'doc_type'     => ['required', 'in:' . implode(',', array_keys(self::DOC_TYPES))],
            'has_abstract' => ['required', 'boolean'],
        ]);

        $actions = [
            ['type' => 'setPageColor', 'color' => self::DOC_TYPES[$data['doc_type']]['color']],
            ['type' => 'applyStyle',   'style' => 'Title'],
            ['type' => 'insertText',   'text'  => $data['title']],
        ];

        if ($request->boolean('has_abstract')) {
            $actions[] = ['type' => 'applyStyle', 'style' => 'Heading1'];
            $actions[] = ['type' => 'insertText', 'text'  => 'Abstract'];
            $actions[] = ['type' => 'applyStyle', 'style' => 'Normal'];
            $actions[] = ['type' => 'insertText', 'text'  => 'Write the abstract here.'];
        }

        $actions[] = ['type' => 'insertPageBreak'];

        return response()->json(['actions' => $actions]);
    }
}