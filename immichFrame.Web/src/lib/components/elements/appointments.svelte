<script lang="ts">
	import * as api from '$lib/index';
	import { onMount } from 'svelte';
	import { format } from 'date-fns';
	import { configStore } from '$lib/stores/config.store';
	import { clientIdentifierStore } from '$lib/stores/persist.store';

	api.init();

	function formatEventTime(startTime: string, endTime: string) {
    const startDate = new Date(startTime);
    const endDate = new Date(endTime);
    const today = new Date();

    const isToday =
        startDate.getFullYear() === today.getFullYear() &&
        startDate.getMonth() === today.getMonth() &&
        startDate.getDate() === today.getDate();

    const sameDay =
        startDate.getFullYear() === endDate.getFullYear() &&
        startDate.getMonth() === endDate.getMonth() &&
        startDate.getDate() === endDate.getDate();

    const configuredFormat = $configStore.clockFormat ?? 'HH:mm';
    const is24Hour = configuredFormat.includes('H');
    const timeFormat = is24Hour ? 'H:mm' : 'h:mm';

    const prefix = isToday ? '' : format(startDate, 'EEEE') + ' ';

    if (sameDay) {
        return prefix +
            format(startDate, timeFormat) + ' - ' +
            format(endDate, timeFormat);
    }

    return prefix +
        format(startDate, timeFormat) + ' - ' +
        format(endDate, 'EEEE') + ' ' +
        format(endDate, timeFormat);
}

    let appointments: api.IAppointment[] = $state([]);
    let calendarStatuses: api.CalendarHealthStatusRead[] = $state([]);

	onMount(() => {
		GetAppointments();
		const appointmentInterval = setInterval(() => GetAppointments(), 10 * 60 * 1000); //every 10 minutes

		return () => {
			clearInterval(appointmentInterval);
		};
	});

    async function GetAppointments() {
        const results = await Promise.allSettled([
            api.getAppointments({
                clientIdentifier: $clientIdentifierStore
            }),
            api.getCalendarStatuses()
        ]);

        const [appointmentsResult, statusesResult] = results;

        if (appointmentsResult.status === 'fulfilled' &&
            appointmentsResult.value.status === 200) {
            appointments = [...appointmentsResult.value.data].sort((a, b) =>
                new Date(a.startTime ?? '').getTime() -
                new Date(b.startTime ?? '').getTime()
            );
        } else {
            console.warn('Unable to refresh calendar appointments.');
        }

        if (statusesResult.status === 'fulfilled' &&
            statusesResult.value.status === 200) {
            calendarStatuses = statusesResult.value.data;
        } else {
            console.warn('Unable to refresh calendar health statuses.');
        }
    }

</script>

{#if appointments}
	<div
		id="appointments"
		class="fixed top-0 right-0 w-auto z-10 text-center text-frame-primary m-5 max-w-[20%] hidden lg:block md:min-w-[10%]"
	>
		<!-- <div class="text-4xl mx-8 font-bold">Appointments</div> -->
		<div class="">
			{#each appointments as appointment}
				<div
                                        class="calendar-event-card bg-gray-600/90 mb-2 text-left rounded-md p-3"
                                        style:--calendar-accent={/^#[0-9a-fA-F]{6}$/.test(appointment.calendarColor ?? '') ? appointment.calendarColor : '#6299EC'}
                                >

					<p class="calendar-event-time text-xs">
						{formatEventTime(appointment.startTime ?? '', appointment.endTime ?? '')}
					</p>
					{appointment.summary}
					{#if appointment.description}
						<p class="text-xs font-light">{appointment.description}</p>
					{/if}
				</div>
			{/each}

                        {#each calendarStatuses.filter((status) => status.showWarning) as status (status.calendarId)}
                            <div
                                class="calendar-event-card bg-gray-600/90 mb-2 text-left rounded-md p-3"
                                style:--calendar-accent={/^#[0-9a-fA-F]{6}$/.test(status.calendarColor ?? '') ? status.calendarColor : '#6299EC'}
                            >
                                <p class="font-semibold">
                                    {status.calendarName} unavailable
                                </p>
                                <p class="text-xs font-light">
                                    Unable to load events. Retrying automatically.
                                </p>
                            </div>
                        {/each}
		</div>
	</div>
{/if}
