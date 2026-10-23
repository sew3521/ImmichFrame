<script lang="ts">
    interface Props {
        target: Record<string, unknown>;
    }

    let { target }: Props = $props();

    const defaultLookahead = 4;

    let enabled = $derived(target.layout === 'splitview');

    let lookahead = $derived(
        typeof target.splitViewLookahead === 'number'
            ? target.splitViewLookahead
            : defaultLookahead
    );

    function updateLookahead(value: number) {
        if (!Number.isFinite(value)) return;

        target.splitViewLookahead = Math.max(
            2,
            Math.min(8, Math.round(value))
        );
    }

    function resetLookahead() {
        target.splitViewLookahead = defaultLookahead;
    }
</script>

{#if enabled}
    <div class="col-span-full rounded-lg border p-4">
        <div class="mb-3 flex items-center justify-between gap-3">
            <div>
                <label for="splitview-lookahead" class="font-semibold">
                    Portrait pairing lookahead
                </label>
                <p class="text-sm opacity-70">
                    How many queued photos to search for a matching portrait.
                    Higher values produce more portrait pairs.
                </p>
            </div>

            <span class="whitespace-nowrap font-semibold">
                {lookahead} images
            </span>
        </div>

        <input
            id="splitview-lookahead"
            type="range"
            min="2"
            max="8"
            step="1"
            value={lookahead}
            oninput={(event) =>
                updateLookahead(event.currentTarget.valueAsNumber)}
            class="w-full"
        />

        <div class="mt-2 flex items-center justify-between gap-3">
            <span class="text-xs opacity-70">Fewer pairs</span>

            <button
                type="button"
                class="rounded border px-3 py-1 text-sm"
                onclick={resetLookahead}
            >
                Reset to 4
            </button>

            <span class="text-xs opacity-70">More pairs</span>
        </div>
    </div>
{/if}
