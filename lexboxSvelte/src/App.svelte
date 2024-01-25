<script lang="ts">
  import {
    AppBar,
    Button,
    Checkbox,
    Dialog,
    Field,
    ListItem,
    TextField,
    cls,
  } from "svelte-ux";
  import { mdiMagnify, mdiCog } from "@mdi/js";
  import Editor from "./lib/Editor.svelte";
  import { entries } from "./lib/entry-data";
  import { firstDefVal, firstVal } from "./lib/utils";
  import { allFields } from "./lib/config-data";
  import { i18n } from "./lib/i18n";

  let showSearchDialog = false;
  let showConfigDialog = false;
  let fieldSearch = "";

  $: filteredFields = allFields.filter(
    (field) =>
      !fieldSearch || ("name" in field ? field.name : i18n[field.id])?.toLocaleLowerCase().includes(fieldSearch.toLocaleLowerCase())
  );
</script>

<div class="min-h-full flex flex-col">
  <AppBar title="Lexbox Svelte" class="bg-accent-300">
    <div class="flex-grow"></div>
    <Field
    classes={{input: 'my-1'}}
      on:click={() => (showSearchDialog = true)}
      class="flex-grow-[2] cursor-pointer opacity-80 hover:opacity-100"
      icon={mdiMagnify}>Search</Field
    >
    <div class="flex-grow"></div>
    <div slot="actions"></div>
  </AppBar>

  <main class="p-8 flex-grow flex flex-col">
    <div
      class="grid flex-grow"
      style="grid-template-columns: 1fr 4fr 1fr; grid-template-rows: auto 1fr;"
    >
      <h2 class="flex text-2xl font-semibold col-span-2">
        Welcome to LexBox Svelte
        <div class="flex-grow"></div>
        <div class="mr-12">
          <Button
            on:click={() => (showConfigDialog = true)}
            variant="outline"
            icon={mdiCog}
          />
        </div>
      </h2>
      <div class="ml-4 self-end">Overview</div>
      <div
        class="my-4 h-full grid grid-cols-subgrid flex-grow row-start-2 col-span-3"
      >
        <Editor />
      </div>
    </div>
  </main>
</div>

<Dialog bind:open={showSearchDialog} class="w-[700px]">
  <div slot="title">
    <TextField
      autofocus
      placeholder="Search entries"
      class="flex-grow-[2] cursor-pointer opacity-80 hover:opacity-100"
      icon={mdiMagnify}
    />
  </div>
  <div>
    {#each entries as entry}
      <ListItem
        title={firstVal(entry.lexemeForm)}
        subheading={firstDefVal(entry)}
        class={cls("cursor-pointer", "hover:bg-accent-50")}
        noShadow
      />
    {/each}
  </div>
  <div class="flex-grow"></div>
  <div slot="actions">actions</div>
</Dialog>

<Dialog bind:open={showConfigDialog} class="w-[700px]">
  <div slot="title">
    <TextField
      bind:value={fieldSearch}
      autofocus
      placeholder="Search fields"
      class="flex-grow-[2] cursor-pointer opacity-80 hover:opacity-100"
      icon={mdiMagnify}
    />
  </div>
  <div>
    {#each filteredFields as field}
      <label for={field.id} class="contents">
        <ListItem
          title={"name" in field ? field.name : i18n[field.id]}
          subheading={`Type: ${field.type}. WS: ${field.ws}.`}
          class={cls("cursor-pointer", "hover:bg-accent-50")}
          noShadow>
          <div slot="actions">
            <Checkbox id={field.id} circle dense />
          </div>
        </ListItem>
      </label>
        {:else}
        <div class="mx-8 my-4">
          No matching fields
        </div>
    {/each}
  </div>
  <div class="flex-grow"></div>
  <div slot="actions">actions</div>
</Dialog>
