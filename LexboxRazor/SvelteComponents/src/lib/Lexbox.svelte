<svelte:options customElement="lexbox-main" />

<script lang="ts">
  import { DotNetServiceProvider, DotNetServices } from "../lexbox";
  import { writable } from "svelte/store";
  import * as jsonpatch from "fast-json-patch";

  let entries = writable<any[]>([]);
  let originalEntries: any[];
  var lexboxApi = DotNetServiceProvider.getService(DotNetServices.LexboxApi);

  refresh();

  function updateEntry(entry: any) {
    const originalEntry = originalEntries[entry.id];
    const patchDocument = jsonpatch.compare(originalEntry, entry);
    console.log(patchDocument);
    lexboxApi
      .invokeMethodAsync("UpdateEntry", entry.id, patchDocument)
      .then(refresh);
  }

  function refresh() {
    lexboxApi.invokeMethodAsync<any[]>("GetEntries", null).then((result) => {
      originalEntries = Object.fromEntries(
        result.map((entry) => [entry.id, entry])
      );
      entries.set(jsonpatch.deepClone(result));
    });
  }
</script>

{#each $entries as entry}
  <div class="entry">
    <input
      type="text"
      bind:value={entry.lexemeForm.values.en}
      on:change={() => updateEntry(entry)}
    />
    <div class="sense">
      {#each entry.senses as sense}
        <p>- {sense.gloss.values.en}: {sense.definition.values.en}</p>
      {/each}
    </div>
  </div>
{/each}

<button class="btn btn-primary" on:click={refresh}> Refresh </button>
