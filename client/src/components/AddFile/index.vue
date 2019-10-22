<template>
  <div class="q-pa-md">
    <q-uploader
      flat
      bordered
      :label="$t('selectFile')"
      auto-upload
      hide-upload-btn
      :factory="factoryFn"
    >
      <template v-slot:list="scope">
        <div
          v-if="scope.files < 1"
          class="q-pa-xl flex flex-center column text-center"
          style="height: -webkit-fill-available;"
        >
          <q-icon
            name="backup"
            class="text-grey-4"
            style="font-size: 100px"
          />
          <span class="text-h6 text-weight-bold text-grey-6">{{ $t('dragDrop') }}</span>
          <span class="text-body1 text-grey-7">
            {{ $t('or') }} <span
              class="text-blue"
              @click="scope.pickFiles()"
            >{{ $t('browse') }}</span> {{ $t('chooseFile') }}</span>
        </div>
        <div
          v-else
          class="q-pa-xl flex flex-center column text-center"
        >
          <q-icon
            name="fas fa-file-signature"
            class="text-grey-4"
            style="font-size: 100px"
          />
          <span class="q-mt-md text-h6 text-primary">
            {{ file.name }}</span>
          <span
            v-if="file.type"
            class="text-body1 text-grey-7"
          >
            {{ $t('type') }}: {{ file.type }}</span>
          <span class="q-mb-lg text-body1 text-grey-7">
            {{ $t('size') }}: {{ getSize }}</span>
          <q-btn
            unelevated
            size="lg"
            color="primary"
            label="Sign"
          />
          <span
            class="q-mt-sm text-blue"
            @click="scope.reset()"
          >{{ $t('differentFile') }}</span>
        </div>
      </template>
    </q-uploader>
  </div>
</template>
<script>
export default {
  name: 'AddFile',
  data() {
    return {
      file: null,
    };
  },

  computed: {
    getSize() {
      if (this.file.size) {
        const bytes = this.file.size;
        const decimals = 2;
        if (bytes === 0) return '0 Bytes';

        const k = 1024;
        const dm = decimals < 0 ? 0 : decimals;
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

        const i = Math.floor(Math.log(bytes) / Math.log(k));

        return `${parseFloat((bytes / (k ** i)).toFixed(dm))} ${sizes[i]}`;
      }
      return null;
    },
  },

  methods: {
    async factoryFn(files) {
      this.file = {
        name: files[0].name,
        type: files[0].type,
        size: files[0].size,
      };
      const reader = await new FileReader();
      reader.onload = (evt) => {
        const input = Buffer.from(evt.target.result);
        const output = new Uint8Array(64);
        this.file.hash = this.$blake2b(output.length).update(input).digest('hex');
      };
      reader.readAsArrayBuffer(files[0]);
    },
  },
};
</script>
<style lang="scss">
.q-uploader {
  width: inherit;
  max-height: inherit;
  min-width: 25rem;
  min-height: 25rem;
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
    border: 2px dashed rgba(0, 0, 0, 0.12);
}
</style>
