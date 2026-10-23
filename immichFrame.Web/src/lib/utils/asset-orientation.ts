/**
 * Determine whether an image should be displayed as a portrait.
 *
 * Prefer EXIF dimensions when available. Fall back to the asset's
 * top-level dimensions when EXIF information is incomplete.
 *
 * Top-level dimensions are assumed to represent display orientation,
 * so EXIF rotation is applied only to EXIF dimensions.
 */

export interface OrientationAsset {
    type: string;
    width?: number | null;
    height?: number | null;
    exifInfo?: {
        exifImageWidth?: number | null;
        exifImageHeight?: number | null;
        orientation?: string | null;
    } | null;
}

function validDimension(value: number | null | undefined): boolean {
    return typeof value === 'number' &&
        Number.isFinite(value) &&
        value > 0;
}

export function isPortraitImage(asset: OrientationAsset): boolean {
    if (asset.type !== 'image') {
        return false;
    }

    let width = asset.exifInfo?.exifImageWidth;
    let height = asset.exifInfo?.exifImageHeight;

    if (validDimension(width) && validDimension(height)) {
        const orientation = Number(asset.exifInfo?.orientation ?? 0);

        if ([5, 6, 7, 8].includes(orientation)) {
            [width, height] = [height, width];
        }

        return height! > width!;
    }

    if (validDimension(asset.width) && validDimension(asset.height)) {
        return asset.height! > asset.width!;
    }

    // Unknown dimensions: don't guess.
    return false;
}
