<script lang="ts">
    interface Props {
        target: Record<string, unknown>;
    }

    let { target }: Props = $props();

    const defaultFont = 'Inter';

    let enabled = $derived(target.useGoogleFont === true);

    let fontFamily = $derived(
        typeof target.googleFontFamily === 'string'
            ? target.googleFontFamily
            : defaultFont
    );

    function updateFont(value: string) {
        target.googleFontFamily = value;
    }

    function resetFont() {
        target.googleFontFamily = defaultFont;
    }
</script>

{#if enabled}
    <div class="col-span-full rounded-lg border p-4">
        <div class="mb-3">
            <label for="google-font-family" class="font-semibold">
                Google Font family
            </label>
            <p class="text-sm opacity-70">
                Enter a font family name from Google Fonts.
                The default is Inter.
            </p>
        </div>

        <div class="flex flex-wrap items-center gap-3">
            <input
                id="google-font-family"
                type="text"
                maxlength="100"
                placeholder="Inter"
                value={fontFamily}
                oninput={(event) =>
                    updateFont(event.currentTarget.value)}
                class="min-w-0 flex-1 rounded border p-2"
            />

            <button
                type="button"
                class="rounded border px-3 py-2 text-sm"
                onclick={resetFont}
            >
                Reset to Inter
            </button>
        </div>

        <a
            href="https://fonts.google.com/"
            target="_blank"
            rel="noopener noreferrer"
            class="mt-3 inline-block text-sm underline"
        >
            Browse Google Fonts
        </a>

        <p class="mt-2 text-xs opacity-70">
            The slideshow device needs internet access to load
            Google Fonts. The original font is used as a fallback.
        </p>
    </div>
{/if}
