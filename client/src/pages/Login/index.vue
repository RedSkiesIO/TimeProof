<template>
  <q-page class="flex flex-center">
    <q-card
      flat
    >
      <q-tabs
        v-model="tab"
        dense
        class="bg-white text-primary"
      >
        <q-tab
          name="login"
          :label="$t('login')"
        />
        <q-tab
          name="signUp"
          :label="$t('signUp')"
        />
      </q-tabs>
      <q-tab-panels
        v-model="tab"
        animated
      >
        <q-tab-panel
          name="login"
          class="q-gutter-y-md flex flex-center column text-center"
        >
          <q-input
            v-model="email"
            outlined
            label="email"
          />
          <q-input
            v-model="password"
            outlined
            label="password"
          />
          <q-btn
            class="q-mt-md"
            unelevated
            color="primary"
            :label="$t('signin')"
          />
        </q-tab-panel>

        <q-tab-panel
          name="signUp"
          class="q-gutter-y-md flex flex-center column text-center"
        >
          <q-input
            v-model="name"
            outlined
            label="Full name"
          />
          <q-input
            v-model="email"
            outlined
            label="email"
          />
          <q-input
            v-model="password"
            outlined
            label="password"
          />
          <q-btn
            class="q-mt-md"
            unelevated
            color="primary"
            :label="$t('register')"
          />
        </q-tab-panel>
      </q-tab-panels>
    </q-card>
  </q-page>
</template>

<script>

export default {
  name: 'Login',

  data() {
    return {
      tab: 'login',
    };
  },
};
</script>
<style lang="scss" scoped>
.q-tab-panel {
  padding: 0;
}

.q-card > div:first-child {
    border-top: 2px solid rgba(0, 0, 0, 0.12);
    border-left: 2px solid rgba(0, 0, 0, 0.12);
    border-right: 2px solid rgba(0, 0, 0, 0.12);

}

.q-tab-panel {
  width: inherit;
  max-height: inherit;
  min-width: 25rem;
  min-height: 25rem;
  border: 2px solid rgba(0, 0, 0, 0.12);
}

.q-tab-panel label {
    width: 75%;
}

.q-gutter-y-md, .q-gutter-md {
    margin-top: 0;
}
</style>
