<template>
  <q-page class="flex justify-center">
    <div class="q-pa-md">
      <a
        class="go-dashboard q-pb-sm cursor-pointer"
        @click="backToDashboard"
      >
        <span style="color:#336699">&nbsp;Go back to the dashboard</span>
      </a>
    </div>
    <div
      class="q-mt-lg"
    >
      <div class="column q-mb-md justify-center text-center text-secondary">
        <div class="col text-h5 text-weight-bold justify-center">
          {{ $t('verifyTimestamp') }}
        </div>
      </div>

      <div
        class="row q-gutter-x-lg"
      >
        <div class="verify">
          <AddFile :mode="'verify'" />
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import AddFile from '../components/AddFile';

export default {
  name: 'Verify',
  components: {
    AddFile,
  },
  methods: {
    backToDashboard() {
      this.$router.push('/dashboard');
    },
  },
};
</script>
<style lang="scss">
.verify .q-card {
  padding: 0;
}

.go-dashboard:before{
  content: url('../statics/icons/left-arrow.svg');
}

.go-dashboard {
  display: flex;
  position: fixed;
  top: 12%;
  left: 15%;
  right: 0;
  color: #4cbbc2;
}

.verify .q-uploader__list {
  background-color: white;
}

</style>
