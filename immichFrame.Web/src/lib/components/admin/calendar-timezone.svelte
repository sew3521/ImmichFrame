<script lang="ts">
    interface Props {
        target: Record<string, unknown>;
        systemTimeZoneId?: string;
    }

    let { target, systemTimeZoneId = '' }: Props = $props();

    const timezones = [
        {
            value: '',
            label: 'System default (container timezone)'
        },
        { value: 'UTC', label: 'UTC' },
        { value: 'America/New_York', label: 'Eastern - New York' },
        { value: 'America/Chicago', label: 'Central - Chicago' },
        { value: 'America/Denver', label: 'Mountain - Denver' },
        { value: 'America/Phoenix', label: 'Arizona - Phoenix' },
        { value: 'America/Los_Angeles', label: 'Pacific - Los Angeles' },
        { value: 'America/Anchorage', label: 'Alaska - Anchorage' },
        { value: 'Pacific/Honolulu', label: 'Hawaii - Honolulu' },
        { value: 'Europe/London', label: 'United Kingdom - London' },
        { value: 'Europe/Paris', label: 'Central Europe - Paris' },
        { value: 'Asia/Tokyo', label: 'Japan - Tokyo' },
        { value: 'Australia/Sydney', label: 'Australia - Sydney' }
    ];

    let selected = $derived(
        typeof target.timeZoneId === 'string'
            ? target.timeZoneId
            : ''
    );

    function updateTimezone(value: string) {
        target.timeZoneId = value;
    }
</script>

<div class="col-span-full rounded-lg border p-4">
    <label for="calendar-timezone" class="font-semibold">
        Calendar timezone
    </label>

    <p class="mb-3 text-sm opacity-70">
        Choose the timezone used for calendar events and date boundaries.
        System default follows the timezone configured for the container.
    </p>

    <select
        id="calendar-timezone"
        class="w-full rounded border bg-transparent p-2"
        value={selected}
        onchange={(event) =>
            updateTimezone(event.currentTarget.value)}
    >
        {#each timezones as timezone}
            <option value={timezone.value}>
                {timezone.value === ''
                    ? `System default (${systemTimeZoneId || 'container timezone'})`
                    : timezone.label}
            </option>
        {/each}
    </select>

    <p class="mt-2 text-xs opacity-70">
        {selected
            ? `Calendar events use ${selected}.`
            : `Calendar events use ${systemTimeZoneId || 'the container timezone'}.`}
    </p>
</div>
