import { selectSplitViewAssets } from './splitview-pairing.ts';

function simulate(portraitPercent, lookahead, total = 1000) {
    let backlog = Array.from({ length: total }, (_, i) => ({
        id: String(i),
        portrait: Math.random() * 100 < portraitPercent,
        video: false
    }));

    let portraitSlides = 0;
    let pairedSlides = 0;
    let totalSlides = 0;
    let portraitPhotos = 0;

    while (backlog.length) {
        const candidates = backlog.slice(0, lookahead);

        const selection = selectSplitViewAssets(
            candidates,
            'splitview',
            lookahead
        );

        const selected = selection.selectedIndices.map(i => backlog[i]);

        if (selected.some(asset => asset.portrait)) {
            portraitSlides++;
        }

        portraitPhotos += selected.filter(asset => asset.portrait).length;

        if (selected.length === 2) {
            pairedSlides++;
        }

        const removed = new Set(selection.selectedIndices);

        backlog = backlog.filter((_, i) => !removed.has(i));
        totalSlides++;
    }

    return {
        portraitPercent,
        lookahead,
        totalSlides,
        portraitSlidesPercent: Number(
            (portraitSlides / totalSlides * 100).toFixed(1)
        ),
        pairedSlidesPercent: Number(
            (pairedSlides / totalSlides * 100).toFixed(1)
        ),
        portraitPhotos
    };
}

const results = [];

for (const portraitPercent of [25, 40, 60]) {
    for (const lookahead of [2, 4, 8]) {
        const runs = Array.from({ length: 10 }, () =>
            simulate(portraitPercent, lookahead)
        );

        results.push({
            portraitPercent,
            lookahead,
            averageSlides: Math.round(
                runs.reduce((sum, run) => sum + run.totalSlides, 0) / runs.length
            ),
            portraitSlidesPercent: (
                runs.reduce((sum, run) => sum + run.portraitSlidesPercent, 0) /
                runs.length
            ).toFixed(1) + '%',
            pairedSlidesPercent: (
                runs.reduce((sum, run) => sum + run.pairedSlidesPercent, 0) /
                runs.length
            ).toFixed(1) + '%'
        });
    }
}

console.table(results);
