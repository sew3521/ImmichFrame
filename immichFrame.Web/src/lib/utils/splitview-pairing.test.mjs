import test from 'node:test';
import assert from 'node:assert/strict';
import { selectSplitViewAssets } from './splitview-pairing.ts';

const portrait = (id) => ({ id, portrait: true, video: false });
const landscape = (id) => ({ id, portrait: false, video: false });
const video = (id) => ({ id, portrait: false, video: true });

test('empty backlog returns no selection', () => {
    assert.deepEqual(selectSplitViewAssets([], 'splitview'), {
        selectedIndices: [],
        remainingIndices: []
    });
});

test('single portrait is displayed alone', () => {
    assert.deepEqual(
        selectSplitViewAssets([portrait('A')], 'splitview').selectedIndices,
        [0]
    );
});

test('adjacent portraits are paired', () => {
    assert.deepEqual(
        selectSplitViewAssets(
            [portrait('A'), portrait('B')],
            'splitview'
        ).selectedIndices,
        [0, 1]
    );
});

test('landscape images can be skipped for portrait pairing', () => {
    const result = selectSplitViewAssets(
        [portrait('A'), landscape('B'), portrait('C')],
        'splitview'
    );

    assert.deepEqual(result.selectedIndices, [0, 2]);
    assert.deepEqual(result.remainingIndices, [1]);
});

test('eight-image lookahead finds a portrait at index seven', () => {
    const assets = [
        portrait('A'),
        ...Array.from({ length: 6 }, (_, i) => landscape(String(i))),
        portrait('B')
    ];

    assert.deepEqual(
        selectSplitViewAssets(assets, 'splitview').selectedIndices,
        [0, 7]
    );
});

test('lookahead does not exceed eight images', () => {
    const assets = [
        portrait('A'),
        ...Array.from({ length: 7 }, (_, i) => landscape(String(i))),
        portrait('B')
    ];

    assert.deepEqual(
        selectSplitViewAssets(assets, 'splitview').selectedIndices,
        [0]
    );
});

test('videos are never paired', () => {
    assert.deepEqual(
        selectSplitViewAssets(
            [portrait('A'), video('B'), portrait('C')],
            'splitview'
        ).selectedIndices,
        [0, 2]
    );

    assert.deepEqual(
        selectSplitViewAssets(
            [video('A'), portrait('B')],
            'splitview'
        ).selectedIndices,
        [0]
    );
});

test('single layout never pairs images', () => {
    assert.deepEqual(
        selectSplitViewAssets(
            [portrait('A'), portrait('B')],
            'single'
        ).selectedIndices,
        [0]
    );
});

test('skipped assets retain their original order', () => {
    const result = selectSplitViewAssets(
        [
            portrait('A'),
            landscape('B'),
            landscape('C'),
            portrait('D'),
            landscape('E')
        ],
        'splitview'
    );

    assert.deepEqual(result.selectedIndices, [0, 3]);
    assert.deepEqual(result.remainingIndices, [1, 2, 4]);
});
