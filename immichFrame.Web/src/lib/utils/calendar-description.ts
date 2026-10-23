/**
 * Prepare a calendar event description for display.
 *
 * This only changes the displayed text. The original event data
 * remains untouched.
 */
export function truncateCalendarDescription(
    description: string,
    maxCharacters = 160
): string {
    // Collapse newlines, tabs, and repeated spaces.
    const normalized = description.replace(/\s+/g, ' ').trim();

    if (!normalized) {
        return '';
    }

    // Keep the configured limit within a reasonable range.
    // Invalid values fall back to the default of 160.
    const requestedLimit = Number.isFinite(maxCharacters)
        ? Math.floor(maxCharacters)
        : 160;

    const limit = Math.max(40, Math.min(500, requestedLimit));

    // Count Unicode code points rather than UTF-16 code units.
    const characters = Array.from(normalized);

    if (characters.length <= limit) {
        return normalized;
    }

    // Reserve room for the ellipsis.
    const shortened = characters
        .slice(0, limit - 3)
        .join('')
        .trimEnd();

    // Prefer ending at a word boundary when practical.
    const lastSpace = shortened.lastIndexOf(' ');

    if (lastSpace > shortened.length * 0.75) {
        return shortened.slice(0, lastSpace).trimEnd() + '...';
    }

    return shortened + '...';
}
