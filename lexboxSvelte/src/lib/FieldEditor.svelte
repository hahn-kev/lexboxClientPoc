<script lang="ts">
  import type { Readable } from "svelte/store";

  import type { WritingSystems } from "./mini-lcm";

  import { i18n } from "./i18n";

  import { TextField } from "svelte-ux";
  import { getContext } from "svelte";
  import type { EntityFieldConfig } from "./types";
  import { pickWritingSystems } from "./utils";

  const writingSystems = getContext<Readable<WritingSystems>>("writingSystems");

  type T = $$Generic<{}>;

  export let entity: T;
  export let field: EntityFieldConfig<T>;

</script>

{#if field.type === 'multi'}
  <div class="multi-field field">
    <span class="name w-32" title={field.id}>{i18n[field.id]}</span>
    <div class="fields">
      {#each pickWritingSystems(field.ws, $writingSystems) as ws}
        <TextField
          on:change
          class="ws-field"
          bind:value={entity[field.id].values[ws.id]}
          label={ws.abbreviation}
          labelPlacement="left"
        />
      {/each}
    </div>
  </div>
{:else if field.type === 'single'}
  <div class="single-field field">
    <span class="name w-32" title={field.id}>{i18n[field.id]}</span>
    <div class="fields">
        <TextField
          on:change
          class="ws-field"
          bind:value={entity[field.id]}
        />
    </div>
  </div>
{/if}

<style lang="postcss">
  .field {
    @apply grid grid-cols-subgrid col-span-3;

    &:not(:last-child) {
      @apply mb-4;
    }

    .fields {
      @apply grid grid-cols-subgrid col-span-2;
      gap: 4px;
    }
  }
  
  :global(.multi-field .fields .ws-field) {
    @apply grid grid-cols-subgrid col-span-2;
  }
  
  :global(.single-field .fields .ws-field) {
    @apply grid grid-cols-subgrid;
    grid-column-start: 2;
  }
</style>
