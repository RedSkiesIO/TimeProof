<template>
  <div>
    <div class="row text-weight-bold text-h6 justify-start text-weight-bold">
      <q-icon
        class="icon-spacing q-mr-sm"
        name="fas fa-history"
        size="1.25rem"
      />
      Timestamp History
    </div>
    <q-card
      flat
      class="timestamp-list q-pa-md"
    >
      <div v-if="user.orderedTimestamps.length > 0">
        <div class="row text-weight-bold justify-end q-mr-sm">
          {{ $t('totalTimestamps') }}: {{ user.timestamps.length }}
        </div>
        <div class="text-uppercase text-weight-bold text-secondary row">
          <div
            class="col-5"
          >
            {{ $t('file') }}
          </div>
          <div class="col-1">
            {{ $t('proofId') }}
          </div>
          <div class="col-auto">
            {{ $t('date') }}
          </div>
        </div>
        <q-scroll-area style="height: 15rem;">
          <div
            v-for="stamp in user.orderedTimestamps"
            :key="stamp.txId"
            class="row stamp-item2"
          >
            <div
              class="col-5 q-px-sm overflow"
            >
              <q-icon
                class="col-auto text-grey-6 q-pr-sm"
                :name="fileIcon(stamp.type)"
                style="font-size: 1.5em"
              />
              {{ stamp.name }}
            </div>
            <div
              class="col-1 row overflow"
            >
              <div>
                <q-btn
                  flat
                  rounded
                  size="sm"
                  color="grey"
                  icon="filter_none"
                  class="copy-button"
                  @click="copy(stamp.txId)"
                >
                  <q-tooltip anchor="top middle">
                    {{ copyLabel }}
                  </q-tooltip>
                </q-btn>
              </div>
            </div>
            <div
              class=" col text-left"
            >
              <span v-if="stamp.blockNumber !== -1">
                {{ stamp.timestampDate }}

              </span>
            </div>
            <div
              v-if="stamp.blockNumber === -1"
              class="col q-pr-sm text-center"
            >
              <q-chip
                square
                color="orange"
                text-color="white"
              >
                pending
              </q-chip>
            </div>
            <div
              v-else
              class="col q-pr-sm text-right text-blue cursor-pointer"
              @click="timestampDialog(stamp)"
            >
              {{ $t('downloadCertificate') }}
            </div>
          </div>
        </q-scroll-area>
      </div>
      <div
        v-else
        class="flex column justify-center q-pa-md"
      >
        <img
          src="~assets/no-timestamps.svg"
          style="height: 12vw"
        >
        <span class="text-body1 text-center">
          You haven't created any timestamps yet.
          <br> Head over to the

          <span
            class="text-blue link"
            @click="goToStamp"
          >stamp</span>
          section to get started
        </span>
      </div>
    </q-card>
    <q-dialog
      v-model="confirmed"
    >
      <q-card style="width:70%;max-width:1200px">
        <div class="row justify-end">
          <q-icon
            v-close-popup
            name="close"
            size="md"
          />
        </div>

        <Proof
          v-if="confirmed"
          :file="file"
          :proof-id="txId"
          :scope="{dialog: true}"
        />
      </q-card>
    </q-dialog>
  </div>
</template>
<script>
import User from '../../store/User';
import Proof from '../Proof';
import { fileIcon } from '../../util';

export default {
  name: 'Timestamps',

  components: {
    Proof,
  },

  data() {
    return {
      copyLabel: 'copy',
      confirmed: false,
      file: {},
      txId: null,
      tiers: {
        free: 10,
        basic: 30,
        premium: 200,
      },
      fileIcon,
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
  },

  methods: {
    goToStamp() {
      this.$router.push('/stamp');
    },

    timestampDialog({ txId }) {
      this.txId = txId;
      this.confirmed = true;
    },

    copy(text) {
      navigator.clipboard.writeText(text.toLowerCase()).then(() => {
        this.copyLabel = this.$t('copied');
        setTimeout(() => {
          this.copyLabel = this.$t('copy');
        }, 1500);
      }, (err) => {
        console.error('Async: Could not copy text: ', err);
      });
    },

    getCertificate(stamp) {
      const name = `${stamp.date}.pdf`;
      this.$pdf(name, stamp.certificate);
    },
  },
};
</script>
<style lang="scss">
.link {
  text-decoration: none;
  color: inherit;
}

.stamp-item2 {
  border-top: 1px solid $grey-4;
  padding: 1rem 0;
}

.stamp-item2:hover {
  background: $grey-2;
}

.stamp-item2 .q-chip {
  margin: 0;
  height: 1.5em;
}

.overflow {
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}
</style>
