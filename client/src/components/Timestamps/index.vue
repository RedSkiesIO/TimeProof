<template>
  <div>
    <q-card
      flat
      class="timestamp-list q-pa-md"
    >
      <div class="row text-weight-bold text-h6 q-pb-md justify-between">
        {{ $t('timestamps') }}
        <div class="flex text-body1 q-gutter-x-md">
          <div @click="$emit('close')">
            <q-icon
              size="sm"
              name="close"
            />
          </div>
        </div>
      </div>
      <div class="text-uppercase text-weight-bold text-primary row">
        <div
          class="col-2"
        >
          {{ $t('file') }}
        </div>
        <div class="col-6">
          {{ $t('proofId') }}
        </div>
        <div class="col-auto">
          {{ $t('date') }}
        </div>
      </div>
      <q-scroll-area style="height: 30rem;">
        <div
          v-for="stamp in timestamps"
          :key="stamp.txId"
          class="row stamp-item2"
        >
          <div
            class="col-2 q-px-sm overflow"
            @click="timestampDialog(stamp)"
          >
            <q-icon
              class="col-auto text-grey-6 q-pr-sm"
              :name="fileIcon(stamp.type)"
              style="font-size: 1.5em"
            />
            {{ stamp.name }}
          </div>
          <div
            class="col-6 overflow"
            @click="timestampDialog(stamp)"
          >
            {{ stamp.txId }}
          </div>
          <div
            class=" col text-left"
            @click="timestampDialog(stamp)"
          >
            {{ getDate(stamp.date) }}
          </div>
          <div
            class="col q-pr-sm text-right text-blue"
            @click="getCertificate(stamp)"
          >
            {{ $t('downloadCertificate') }}
          </div>
        </div>
      </q-scroll-area>
      <div class="row text-weight-bold justify-end q-mt-md">
        {{ $t('totalTimestamps') }}: {{ user.timestampsUsed }}
      </div>
    </q-card>
    <q-dialog v-model="confirmed">
      <q-card>
        <div class="row justify-end">
          <q-icon
            v-close-popup
            name="close"
            size="md"
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
  name: 'Timestamps',

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
      const ts = timestamps.slice(0);

      ts.sort((a, b) => new Date(b.date) - new Date(a.date));

      return ts.slice(0);
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
      if (type === 'pdf') {
        return 'fas fa-file-pdf';
      }

      if (type === 'zip') {
        return 'fas fa-file-archive';
      }

      if (type === 'png' || type === 'gif' || type === 'jpeg') {
        return 'fas fa-file-image';
      }

      return 'fas fa-file';
    },

    getCertificate(stamp) {
      const name = `${stamp.date}.pdf`;
      const splitString = (string, index) => ({
        one: string.substr(0, index),
        two: string.substr(index),
      });

      const hash = splitString(stamp.hash.toLowerCase(), 65);
      const proofId = splitString(stamp.txId.toLowerCase(), 65);
      const signature = splitString(stamp.signature.toLowerCase(), 65);
      const file = {
        file: stamp.name,
        hash,
        proofId,
        signature,
        user: this.user.name,
        timestamp: this.getDate(stamp.date),
      };
      this.$pdf(name, file);
    },
  },
};
</script>
<style lang="scss">
.timestamp-list {
    border: 2px solid rgba(0, 0, 0, 0.12);
    min-width: 52rem;
    width: 80vw;
}
.stamp-item2 {
  border-top: 1px solid $grey-4;
  padding: 1rem 0;
}

.stamp-item2:hover {
  background: $grey-2;
}

.overflow {
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}
</style>
