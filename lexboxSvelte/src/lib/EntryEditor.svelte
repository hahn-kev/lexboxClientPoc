<script lang="ts">
  import type { IEntry } from "./mini-lcm";
  import FieldEditor from "./FieldEditor.svelte";
  import {
    entryFieldConfigs,
    exampleFieldConfigs,
    senseFieldConfigs,
  } from "./config-data";

  export let entry: IEntry;
</script>

{#each entryFieldConfigs as fieldConfig}
  <FieldEditor on:change entity={entry} field={fieldConfig} />
{/each}

<!-- {#each customEntryFieldConfigs as fieldConfig}
  <FieldEditor on:change field={fieldConfig} />
{/each} -->

{#each entry.senses as sense, i}
  <h2 class="text-lg" id="sense{i + 1}">Sense {i + 1}</h2>
  {#each senseFieldConfigs as fieldConfig}
    <FieldEditor on:change entity={sense} field={fieldConfig} />
  {/each}

  {#each sense.exampleSentences as example, j}
    <h3 class="font-bold" id="example{i + 1}.{j + 1}">Example {j + 1}</h3>
    {#each exampleFieldConfigs as fieldConfig}
      <FieldEditor on:change entity={example} field={fieldConfig} />
    {/each}
  {/each}
{/each}
