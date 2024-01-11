<svelte:options customElement="lexbox-counter2" />

<script lang="ts">
  let count: number = 0
  const increment = () => {
    count += 10
  }
</script>

<button style="border: solid blue 2px" on:click={increment}>
  Svelte count is {count}
</button>
