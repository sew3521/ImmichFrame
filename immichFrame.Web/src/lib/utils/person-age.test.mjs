import test from 'node:test';
import assert from 'node:assert/strict';
import { formatPersonAge } from './person-age.ts';

test('Callen at three months', () => {
    assert.equal(
        formatPersonAge(
            'Callen',
            '2023-09-14',
            '2023-12-18T14:00:00Z'
        ),
        'Callen (3 months)'
    );
});

test('newborn age in days', () => {
    assert.equal(
        formatPersonAge('Baby', '2026-03-27', '2026-04-08'),
        'Baby (12 days)'
    );
});

test('one day uses singular', () => {
    assert.equal(
        formatPersonAge('Baby', '2026-03-27', '2026-03-28'),
        'Baby (1 day)'
    );
});

test('one month uses singular', () => {
    assert.equal(
        formatPersonAge('Baby', '2026-03-27', '2026-04-27'),
        'Baby (1 month)'
    );
});

test('23 months remains months', () => {
    assert.equal(
        formatPersonAge('Child', '2023-01-15', '2024-12-15'),
        'Child (23 months)'
    );
});

test('24 months switches to years', () => {
    assert.equal(
        formatPersonAge('Child', '2023-01-15', '2025-01-15'),
        'Child (2)'
    );
});

test('birthday has not occurred yet', () => {
    assert.equal(
        formatPersonAge('Child', '2023-09-14', '2025-09-13'),
        'Child (23 months)'
    );
});

test('leap day calculation', () => {
    assert.equal(
        formatPersonAge('Baby', '2024-02-28', '2024-03-01'),
        'Baby (2 days)'
    );
});

test('future birthday does not display age', () => {
    assert.equal(
        formatPersonAge('Child', '2025-01-01', '2024-12-31'),
        'Child'
    );
});

test('invalid dates do not display age', () => {
    assert.equal(
        formatPersonAge('Child', '2023-02-30', '2024-01-01'),
        'Child'
    );
});

test('disabled setting preserves name', () => {
    assert.equal(
        formatPersonAge('Child', '2023-01-01', '2025-01-01', false),
        'Child'
    );
});

test('missing birthday preserves name', () => {
    assert.equal(
        formatPersonAge('Child', null, '2025-01-01'),
        'Child'
    );
});

test('January 31 monthly anniversary in February', () => {
    assert.equal(
        formatPersonAge('Baby', '2025-01-31', '2025-02-28'),
        'Baby (1 month)'
    );
});

test('January 31 before February anniversary', () => {
    assert.equal(
        formatPersonAge('Baby', '2025-01-31', '2025-02-27'),
        'Baby (27 days)'
    );
});

test('January 31 in a leap year', () => {
    assert.equal(
        formatPersonAge('Baby', '2024-01-31', '2024-02-29'),
        'Baby (1 month)'
    );
});

test('March 31 monthly anniversary in April', () => {
    assert.equal(
        formatPersonAge('Baby', '2025-03-31', '2025-04-30'),
        'Baby (1 month)'
    );
});
