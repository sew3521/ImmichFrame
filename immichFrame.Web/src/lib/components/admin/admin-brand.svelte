<script lang="ts">
    import { Heading, Text } from '@immich/ui';

    interface Props {
        subtitle: string;
        version?: string;
    }

    let { subtitle, version = '' }: Props = $props();
</script>

<div class="flex items-center gap-3">
    <img src="/logo.svg" alt="" class="h-9 w-9 shrink-0" />
    <div class="flex flex-col">
        <Heading tag="h1" size="medium" class="leading-tight">
            ImmichFrame: Artificially Enhanced
        </Heading>
        <Text color="muted" size="small">
            {subtitle}{version ? ` | v${version}` : ''}
        </Text>
    </div>
</div>
