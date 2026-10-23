import test from 'node:test';
import assert from 'node:assert/strict';

import {
    navigateNext,
    navigatePrevious
} from './slideshow-navigation.ts';

const portrait = (id) => ({ id, portrait: true, video: false });
const landscape = (id) => ({ id, portrait: false, video: false });
const video = (id) => ({ id, portrait: false, video: true });

const options = {
    layout: 'splitview',
    lookahead: 8,
    maxHistory: 250,
    describeAsset: (asset) => asset
};

function initialState(assets) {
    return {
        backlog: assets,
        current: [],
        history: [],
        forwardHistory: []
    };
}

function ids(assets) {
    return assets.map((asset) => asset.id);
}

test('forward navigation pairs portraits and preserves skipped landscapes', () => {
    let state = initialState([
        portrait('A'),
        landscape('B'),
        portrait('C'),
        landscape('D')
    ]);

    state = navigateNext(state, options);

    assert.deepEqual(ids(state.current), ['A', 'C']);
    assert.deepEqual(ids(state.backlog), ['B', 'D']);

    state = navigateNext(state, options);

    assert.deepEqual(ids(state.current), ['B']);
    assert.deepEqual(ids(state.history[0]), ['A', 'C']);
});

test('Previous restores the exact displayed group', () => {
    let state = initialState([
        portrait('A'),
        landscape('B'),
        portrait('C')
    ]);

    state = navigateNext(state, options);
    state = navigateNext(state, options);
    state = navigatePrevious(state);

    assert.deepEqual(ids(state.current), ['A', 'C']);
    assert.deepEqual(ids(state.forwardHistory[0]), ['B']);
});

test('Next after Previous restores forward history', () => {
    let state = initialState([
        portrait('A'),
        landscape('B'),
        portrait('C')
    ]);

    state = navigateNext(state, options);
    state = navigateNext(state, options);
    state = navigatePrevious(state);
    state = navigateNext(state, options);

    assert.deepEqual(ids(state.current), ['B']);
    assert.deepEqual(state.forwardHistory, []);
});

test('repeated navigation does not duplicate or lose assets', () => {
    let state = initialState([
        portrait('A'),
        landscape('B'),
        portrait('C'),
        video('D'),
        portrait('E'),
        portrait('F')
    ]);

    for (const action of [
        'next', 'next', 'next',
        'previous', 'previous',
        'next', 'next', 'next'
    ]) {
        state = action === 'next'
            ? navigateNext(state, options)
            : navigatePrevious(state);
    }

    const all = [
        ...state.backlog,
        ...state.current,
        ...state.history.flat(),
        ...state.forwardHistory.flat()
    ].map((asset) => asset.id);

    assert.equal(new Set(all).size, all.length);
    assert.deepEqual(
        [...all].sort(),
        ['A', 'B', 'C', 'D', 'E', 'F']
    );
});

test('Previous at beginning leaves state unchanged', () => {
    const state = initialState([portrait('A')]);

    assert.strictEqual(navigatePrevious(state), state);
});

test('Next with no backlog or forward history leaves state unchanged', () => {
    const state = initialState([]);

    assert.strictEqual(navigateNext(state, options), state);
});

test('videos remain single-image displays', () => {
    let state = initialState([
        video('A'),
        portrait('B'),
        portrait('C')
    ]);

    state = navigateNext(state, options);

    assert.deepEqual(ids(state.current), ['A']);

    state = navigateNext(state, options);

    assert.deepEqual(ids(state.current), ['B', 'C']);
});

test('history is limited to configured number of groups', () => {
    let state = initialState([
        landscape('A'),
        landscape('B'),
        landscape('C'),
        landscape('D')
    ]);

    const limited = { ...options, maxHistory: 2 };

    for (let i = 0; i < 4; i++) {
        state = navigateNext(state, limited);
    }

    assert.equal(state.history.length, 2);
    assert.deepEqual(ids(state.history[0]), ['B']);
    assert.deepEqual(ids(state.history[1]), ['C']);
});
