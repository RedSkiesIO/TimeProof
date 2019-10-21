<template>
<div class="q-pa-md">
    <q-uploader
      :factory="factoryFn"
      multiple
      style="max-width: 300px"
    />
  </div>
</template>
<script>
export default {
  name: 'addFile',

  methods: {
    async factoryFn(files) {
      // returning a Promise
      const reader = await new FileReader();
      reader.onload = (evt) => {
        const input = Buffer.from(evt.target.result);
        const output = new Uint8Array(64);
        console.log('hash:', this.$blake2b(output.length).update(input).digest('hex'));
      };
      reader.readAsArrayBuffer(files[0]);

    //   return new Promise((resolve) => {
    //     // simulating a delay of 2 seconds
    //     setTimeout(() => {
    //       resolve();
    //     }, 2000);
    //   });
    },
  },
};
</script>
<style lang="scss">
</style>
