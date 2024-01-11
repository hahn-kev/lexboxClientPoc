<svelte:options customElement="lexbox-counter" />

<script lang="ts">
  let count: number = 0
  const increment = () => {
    count += 1
  }
</script>

<button style="border: solid pink 2px" on:click={increment}>
  Svelte count is {count}

  Slot content: "<slot />"
</button>
