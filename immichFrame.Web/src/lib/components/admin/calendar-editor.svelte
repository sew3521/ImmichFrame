<script lang="ts">
    import { mdiDelete } from '@mdi/js';
    import { Button, Card, CardBody, CardHeader, CardTitle } from '@immich/ui';
    import type { CalendarSource } from '$lib/immichFrameApi';

    interface Props {
        calendar: CalendarSource;
        index: number;
        onremove: () => void;
    }

    let { calendar, index, onremove }: Props = $props();
</script>

<Card>
    <CardHeader>
        <div class="flex items-center justify-between">
            <CardTitle>{calendar.name || `Calendar ${index + 1}`}</CardTitle>
            <Button
                leadingIcon={mdiDelete}
                variant="outline"
                color="danger"
                size="small"
                onclick={onremove}
            >
                Remove
            </Button>
        </div>
    </CardHeader>

    <CardBody>
        <div class="grid gap-4 sm:grid-cols-2">
            <label class="flex flex-col gap-1">
                <span class="text-sm font-medium">Calendar name</span>
                <input
                    type="text"
                    class="rounded border p-2"
                    bind:value={calendar.name}
                    placeholder="Family"
                />
            </label>

            <label class="flex flex-col gap-1 sm:col-span-2">
                <span class="text-sm font-medium">Calendar URL</span>
                <input
                    type="text"
                    class="rounded border p-2"
                    bind:value={calendar.url}
                    placeholder="https://example.com/calendar.ics"
                />
            </label>

            <label class="flex flex-col gap-1">
                <span class="text-sm font-medium">Calendar color</span>
                <input
                    type="color"
                    class="h-10 w-20 cursor-pointer"
                    bind:value={calendar.color}
                />
            </label>

            <label class="flex items-center gap-2">
                <input type="checkbox" bind:checked={calendar.enabled} />
                <span class="text-sm font-medium">Enabled</span>
            </label>
        </div>
    </CardBody>
</Card>
