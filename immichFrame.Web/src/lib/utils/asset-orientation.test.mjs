import test from 'node:test';
import assert from 'node:assert/strict';
import { isPortraitImage } from './asset-orientation.ts';

const image = (values) => ({
    type: 'image',
    ...values
});

test('EXIF portrait is recognized', () => {
    assert.equal(isPortraitImage(image({
        exifInfo: {
            exifImageWidth: 2000,
            exifImageHeight: 3000
        }
    })), true);
});

test('EXIF landscape is recognized', () => {
    assert.equal(isPortraitImage(image({
        exifInfo: {
            exifImageWidth: 3000,
            exifImageHeight: 2000
        }
    })), false);
});

test('rotated EXIF image is recognized', () => {
    assert.equal(isPortraitImage(image({
        exifInfo: {
            exifImageWidth: 3000,
            exifImageHeight: 2000,
            orientation: '6'
        }
    })), true);
});

test('top-level dimensions provide portrait fallback', () => {
    assert.equal(isPortraitImage(image({
        width: 2000,
        height: 3000
    })), true);
});

test('top-level dimensions provide landscape fallback', () => {
    assert.equal(isPortraitImage(image({
        width: 3000,
        height: 2000
    })), false);
});

test('incomplete EXIF falls back to top-level dimensions', () => {
    assert.equal(isPortraitImage(image({
        width: 2000,
        height: 3000,
        exifInfo: {
            exifImageWidth: 0,
            exifImageHeight: 3000
        }
    })), true);
});

test('videos are never considered portraits', () => {
    assert.equal(isPortraitImage({
        type: 'video',
        width: 1080,
        height: 1920
    }), false);
});

test('unknown dimensions are not considered portraits', () => {
    assert.equal(isPortraitImage(image({})), false);
});

test('square images are not considered portraits', () => {
    assert.equal(isPortraitImage(image({
        width: 2000,
        height: 2000
    })), false);
});

test('invalid dimensions are ignored', () => {
    assert.equal(isPortraitImage(image({
        width: NaN,
        height: 3000
    })), false);
});
