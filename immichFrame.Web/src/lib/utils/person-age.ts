/**
 * Format a person's age when a photograph was taken.
 *
 * Birthdays are calendar dates. Comparing date components avoids
 * timezone offsets changing someone's age near midnight.
 */

interface CalendarDate {
    year: number;
    month: number;
    day: number;
}

function parseCalendarDate(value: string): CalendarDate | null {
    const match = /^(\d{4})-(\d{2})-(\d{2})(?:$|T)/.exec(value);

    if (!match) return null;

    const year = Number(match[1]);
    const month = Number(match[2]);
    const day = Number(match[3]);

    const date = new Date(Date.UTC(2000, month - 1, day));
    date.setUTCFullYear(year);

    if (
        date.getUTCFullYear() !== year ||
        date.getUTCMonth() + 1 !== month ||
        date.getUTCDate() !== day
    ) {
        return null;
    }

    return { year, month, day };
}

export function formatPersonAge(
    name: string,
    birthDate: string | null | undefined,
    photoDate: string | null | undefined,
    enabled = true
): string {
    if (!enabled || !birthDate || !photoDate) return name;

    const birth = parseCalendarDate(birthDate);
    const photo = parseCalendarDate(photoDate);

    if (!birth || !photo) return name;

    // setUTCFullYear handles years 0-99 without Date.UTC's 1900 offset.
    function utcDay(date: CalendarDate): number {
        const value = new Date(0);
        value.setUTCFullYear(date.year, date.month - 1, date.day);
        return value.getTime();
    }

    const birthUTC = utcDay(birth);
    const photoUTC = utcDay(photo);

    if (photoUTC < birthUTC) return name;

    const days = Math.floor(
        (photoUTC - birthUTC) / 86400000
    );

    let months =
        (photo.year - birth.year) * 12 +
        photo.month - birth.month;

    // A birthday on the 29th-31st reaches its monthly anniversary
    // on the last day of a shorter month.
    const daysInPhotoMonth = new Date(
        Date.UTC(photo.year, photo.month, 0)
    ).getUTCDate();

    const anniversaryDay = Math.min(birth.day, daysInPhotoMonth);

    if (photo.day < anniversaryDay) {
        months--;
    }

    if (months < 1) {
        return `${name} (${days} ${days === 1 ? 'day' : 'days'})`;
    }

    if (months < 24) {
        return `${name} (${months} ${months === 1 ? 'month' : 'months'})`;
    }

    const years = Math.floor(months / 12);

    return `${name} (${years})`;
}
