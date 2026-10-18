<script lang="ts">
    interface Props {
        target: Record<string, unknown>;
    }

    let { target }: Props = $props();

    const defaultOpacity = 45;


    let enabled = $derived(target.enableFrostedGlassUI === true);
    let opacity = $derived(
        typeof target.frostedGlassOpacity === 'number'
            ? target.frostedGlassOpacity
            : defaultOpacity
    );

    function updateOpacity(value: number) {
        if (!Number.isFinite(value)) return;
        target.frostedGlassOpacity = Math.max(10, Math.min(95, Math.round(value)));
    }

    function resetOpacity() {
        target.frostedGlassOpacity = defaultOpacity;
    }
</script>

{#if enabled}
    <div class="col-span-full rounded-lg border p-4">
        <div class="mb-3 flex items-center justify-between gap-3">
            <div>
                <label for="glass-opacity" class="font-semibold">
                    Frosted Glass opacity
                </label>
                <p class="text-sm opacity-70"
                   title="Default: 45%. Higher values improve readability over bright photos.">
                    Default: 45%. Higher values improve readability over bright photos.
                </p>
            </div>

            <div class="flex items-center gap-2">
                <input
                    type="number"
                    min="10"
                    max="95"
                    step="1"
                    class="w-20 rounded border p-2"
                    value={opacity}
                    oninput={(event) =>
                        updateOpacity(event.currentTarget.valueAsNumber)}
                />
                <span>%</span>
            </div>
        </div>

        <input
            id="glass-opacity"
            type="range"
            min="10"
            max="95"
            step="5"
            value={opacity}
            oninput={(event) =>
                updateOpacity(event.currentTarget.valueAsNumber)}
            class="w-full"
        />

        <div class="mt-2 flex items-center justify-between">
            <span class="text-xs opacity-70">More transparent</span>
            <button
                type="button"
                class="rounded border px-3 py-1 text-sm"
                onclick={resetOpacity}
            >
                Reset to 45%
            </button>
            <span class="text-xs opacity-70">More opaque</span>
        </div>

        <div
            class="mt-4 rounded-lg p-5"
            style="background: linear-gradient(135deg, #6b8aa8, #d7e5ef, #a3b5a8)"
        >
            <div
                class="rounded-lg border border-white/40 p-4 shadow-lg"
                style:background={`rgba(255, 255, 255, ${opacity / 100})`}
                style:backdrop-filter="blur(16px)"
            >
                <div class="font-semibold text-slate-800">
                    Calendar preview
                </div>
                <div class="text-sm text-slate-700">
                    Saturday 9:15 - 9:45 AM
                </div>
            </div>
        </div>
    </div>
{/if}
