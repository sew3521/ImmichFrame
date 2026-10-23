import type { ClientSettingsDto } from '$lib/immichFrameApi';

const FONT_LINK_ID = 'immichframe-google-font';
const FONT_FAMILY_PATTERN = /^[A-Za-z0-9][A-Za-z0-9 &'()-]*$/;
const DEFAULT_FONT = 'Inter';

// Apply a Google Font only to the slideshow container.
// Returning a cleanup function lets Svelte restore the original styling.
export function applyGoogleFont(
    element: HTMLElement,
    config: ClientSettingsDto
): () => void {
    const enabled = config.useGoogleFont === true;
    const family = config.googleFontFamily?.trim() || DEFAULT_FONT;

    if (!enabled || family.length > 100 || !FONT_FAMILY_PATTERN.test(family)) {
        element.style.removeProperty('--slideshow-font-family');
        removeGoogleFontLink();
        return () => {};
    }

    const url =
        'https://fonts.googleapis.com/css2?family=' +
        encodeURIComponent(family).replace(/%20/g, '+') +
        '&display=swap';

    const existing = document.getElementById(
        FONT_LINK_ID
    ) as HTMLLinkElement | null;

    // Reuse the stylesheet when the selected font hasn't changed.
    if (!existing || existing.href !== url) {
        existing?.remove();

        const link = document.createElement('link');
        link.id = FONT_LINK_ID;
        link.rel = 'stylesheet';
        link.href = url;

        document.head.appendChild(link);
    }

    // Keep Overpass as the local fallback.
    element.style.setProperty(
        '--slideshow-font-family',
        `"${family}", Overpass, sans-serif`
    );

    return () => {
        // Remove styling when this effect is disposed.
        // The stylesheet is reused until the font changes or is disabled.
        element.style.removeProperty('--slideshow-font-family');
    };
}

export function removeGoogleFontLink() {
    document.getElementById(FONT_LINK_ID)?.remove();
}
