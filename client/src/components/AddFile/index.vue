<template>
<div class="q-pa-md">
    <q-uploader
      flat
      bordered
      label="Select your File"
      auto-upload
      hide-upload-btn
      :factory="factoryFn"
    >
    <template v-slot:list="scope">
    <div
    v-if="scope.files < 1"
    class="q-pa-xl flex flex-center column text-center"
    style="width:100%; height:100%;">
    <q-icon name="backup" class="text-grey-4" style="font-size: 100px">
    </q-icon>
    <span class="text-h6 text-weight-bold text-grey-6">Drag and Drop your file to sign</span>
    <span class="text-body1 text-grey-7">
       or <span class="text-blue" @click="scope.pickFiles()">browse</span> to choose a file</span>
    </div>
    </template>
    </q-uploader>
  </div>
</template>
<script>
export default {
  name: 'addFile',

  methods: {
    async factoryFn(files) {
      const reader = await new FileReader();
      reader.onload = (evt) => {
        const input = Buffer.from(evt.target.result);
        const output = new Uint8Array(64);
        console.log('hash:', this.$blake2b(output.length).update(input).digest('hex'));
      };
      reader.readAsArrayBuffer(files[0]);
    },
  },
};
</script>
<style lang="scss">
.q-uploader {
  width: inherit;
}
.q-uploader__subtitle {
    display: none !important;
}
.q-uploader__file-status {
    display: none;
}
.q-uploader__header {
    display: none;
}

.q-uploader--bordered {
    border: 1px dashed rgba(0, 0, 0, 0.12);
}
</style>
