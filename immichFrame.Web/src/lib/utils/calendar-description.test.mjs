import test from 'node:test';
import assert from 'node:assert/strict';
import { truncateCalendarDescription } from './calendar-description.ts';

test('short descriptions remain unchanged', () => {
    assert.equal(
        truncateCalendarDescription('Doctor appointment'),
        'Doctor appointment'
    );
});

test('multiline descriptions are normalized', () => {
    assert.equal(
        truncateCalendarDescription('First line\n\nSecond line'),
        'First line Second line'
    );
});

test('long descriptions are truncated', () => {
    const result = truncateCalendarDescription('word '.repeat(100));

    assert.ok(result.length <= 160);
    assert.ok(result.endsWith('...'));
});

test('custom character limits work', () => {
    const result = truncateCalendarDescription('word '.repeat(100), 80);

    assert.ok(result.length <= 80);
    assert.ok(result.endsWith('...'));
});

test('invalid limits use the default', () => {
    const result = truncateCalendarDescription('word '.repeat(100), NaN);

    assert.ok(result.length <= 160);
});

test('empty descriptions remain empty', () => {
    assert.equal(truncateCalendarDescription('   \n  '), '');
});

test('Unicode characters are preserved', () => {
    const result = truncateCalendarDescription('Hello 😀 '.repeat(100), 80);

    assert.ok(Array.from(result).length <= 80);
    assert.ok(result.endsWith('...'));
    assert.ok(!result.includes('\uFFFD'));
});
