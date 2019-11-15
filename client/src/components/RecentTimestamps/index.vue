<template>
  <div>
    <q-card
      flat
      class="usage-summary q-pa-sm"
    >
      <div class="row text-weight-bold text-h6">
        {{ $t('recentTimestamps') }}
      </div>
      <div class="row">
        <div
          class="col-7"
        >
          File
        </div>
        <div>Date</div>
      </div>
      <div
        v-for="stamp in timestamps"
        :key="stamp.txId"
        class="row q-py-sm stamp-item"
        @click="timestampDialog(stamp)"
      >
        <div
          class="col-7 q-pr-sm overflow"
        >
          <q-icon
            class="col-auto text-grey-6 q-pr-sm"
            :name="fileIcon(stamp.type)"
            style="font-size: 1.5em"
          />
          {{ stamp.name }}
        </div>
        <div class="text-right">
          {{ getDate(stamp.date) }}
        </div>
      </div>
      <div
        class="text-center text-blue"
        @click="$emit('open')"
      >
        {{ $t('viewAll') }}
      </div>
    </q-card>
    <q-dialog v-model="confirmed">
      <q-card>
        <div class="row justify-end">
          <q-icon
            v-close-popup
            size="md"
            name="close"
          />
        </div>
        <Proof
          v-if="confirmed"
          :proof="file"
          :scope="{dialog: true}"
        />
      </q-card>
    </q-dialog>
  </div>
</template>
<script>
import User from '../../store/User';
import Proof from '../Proof';

export default {
  name: 'RecentTimestamps',

  components: {
    Proof,
  },

  data() {
    return {
      confirmed: false,
      file: null,
      tiers: {
        free: 50,
        basic: 1000,
        standard: 10000,
        premium: 100000,
      },
    };
  },

  computed: {
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return null;
      }
      return account;
    },

    user() {
      if (this.account) {
        const user = User.query().whereId(this.account.accountIdentifier).with('timestamps').get();
        if (user) {
          return user[0];
        }
      }
      return null;
    },
    timestamps() {
      const { timestamps } = this.user;
      if (this.user.timestamps.length > 5) {
        return timestamps.slice(this.user.timestamps.length - 5).reverse();
      }
      return timestamps.slice(0).reverse();
    },
  },

  methods: {
    timestampDialog(stamp) {
      this.file = stamp;
      this.confirmed = true;
    },

    getDate(time) {
      const date = new Date(time);
      return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    },

    fileIcon(type) {
      if (type === 'application/pdf') {
        return 'fas fa-file-pdf';
      }

      if (type === 'application/zip') {
        return 'fas fa-file-archive';
      }

      if (type === 'image/png' || type === 'image/gif' || type === 'image/jpeg') {
        return 'fas fa-file-image';
      }

      return 'fas fa-file';
    },
  },
};
</script>
<style lang="scss">
.usage-summary {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 25em;
}
.stamp-item {
  border-top: 1px solid $grey-4;
}

.stamp-item:hover {
  background: $grey-2;
}

.overflow {
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}
</style>
