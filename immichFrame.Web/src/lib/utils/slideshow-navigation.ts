import {
    selectSplitViewAssets,
    type PairingAsset
} from './splitview-pairing';

/**
 * Slideshow navigation state.
 *
 * History stores complete displayed groups so navigating backward
 * restores the exact images previously shown together.
 */
export interface NavigationState<T> {
    backlog: T[];
    current: T[];
    history: T[][];
    forwardHistory: T[][];
}

export interface NavigationOptions<T> {
    layout: string | null | undefined;
    lookahead?: number;
    maxHistory?: number;
    describeAsset: (asset: T) => PairingAsset;
}

export function navigateNext<T>(
    state: NavigationState<T>,
    options: NavigationOptions<T>
): NavigationState<T> {
    const history = state.history.map((group) => [...group]);
    const forwardHistory = state.forwardHistory.map((group) => [...group]);
    let backlog = [...state.backlog];

    let next: T[];

    if (forwardHistory.length) {
        // Revisit an existing group without selecting new assets.
        next = forwardHistory.pop()!;
    } else {
        if (!backlog.length) {
            return state;
        }

        const candidates = backlog
            .slice(0, options.lookahead ?? 8)
            .map(options.describeAsset);

        const selection = selectSplitViewAssets(
            candidates,
            options.layout,
            options.lookahead ?? 8
        );

        next = selection.selectedIndices.map((i) => backlog[i]);

        const selected = new Set(selection.selectedIndices);
        backlog = backlog.filter((_, i) => !selected.has(i));
    }

    if (state.current.length) {
        history.push([...state.current]);
    }

    const maxHistory = options.maxHistory ?? 250;

    return {
        backlog,
        current: next,
        history: history.slice(-maxHistory),
        forwardHistory
    };
}

export function navigatePrevious<T>(
    state: NavigationState<T>
): NavigationState<T> {
    if (!state.history.length) {
        return state;
    }

    const history = state.history.map((group) => [...group]);
    const forwardHistory = state.forwardHistory.map((group) => [...group]);

    const previous = history.pop()!;

    if (state.current.length) {
        forwardHistory.push([...state.current]);
    }

    return {
        backlog: [...state.backlog],
        current: previous,
        history,
        forwardHistory
    };
}
