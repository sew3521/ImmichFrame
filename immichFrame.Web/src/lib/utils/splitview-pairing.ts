/**
 * Select assets for the next slideshow display.
 *
 * Inspired by ImmichFrame PR #568 by Zaida-3dO.
 * https://github.com/immichFrame/ImmichFrame/pull/568
 *
 * This version extends the lookahead to eight assets and
 * never indefinitely defers an unmatched portrait.
 */

export interface PairingAsset {
    id: string;
    portrait: boolean;
    video: boolean;
}

export interface PairingResult {
    selectedIndices: number[];
    remainingIndices: number[];
}

export function selectSplitViewAssets(
    assets: PairingAsset[],
    layout: string | null | undefined,
    lookahead = 8
): PairingResult {
    if (assets.length === 0) {
        return {
            selectedIndices: [],
            remainingIndices: []
        };
    }

    const selectedIndices = [0];

    if (
        layout?.trim().toLowerCase() === 'splitview' &&
        assets[0].portrait &&
        !assets[0].video
    ) {
        const limit = Math.min(
            assets.length,
            Math.max(2, Math.floor(lookahead))
        );

        for (let i = 1; i < limit; i++) {
            if (assets[i].portrait && !assets[i].video) {
                selectedIndices.push(i);
                break;
            }
        }
    }

    const selected = new Set(selectedIndices);

    return {
        selectedIndices,
        remainingIndices: assets
            .map((_, index) => index)
            .filter((index) => !selected.has(index))
    };
}
