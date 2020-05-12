<template>
  <div>
    <div class="row text-weight-bold text-h6 justify-start text-weight-bold">
      <q-icon
        class="icon-spacing q-mr-sm"
        name="fas fa-history"
        size="1.25rem"
      />
      History
    </div>
    <q-card
      flat
      class="timestamp-list q-pa-md"
    >
      <div
        v-if="user.orderedTimestamps.length > 0"
      >
        <!-- <div class="row text-weight-bold justify-end q-mr-sm">
          {{ $t('totalTimestamps') }}: {{ user.timestamps.length }}
        </div> -->
        <q-scroll-area
          style="height: 15rem;"
          :thumb-style="thumbStyle"
          :bar-style="barStyle"
        >
          <div
            class="text-uppercase text-weight-bold text-secondary row no-wrap"
          >
            <div
              class="col-md-4 col-sm-6 col-xs-8"
            >
              <q-icon
                class="col-auto text-grey-6 q-ml-sm q-mr-md"
                name="fas fa-file"
                style="font-size: 1.2em"
              />
              {{ $t('file') }}
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6">
              {{ $t('proofId') }}
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6">
              {{ $t('date') }}
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6">
              {{ $t('time') }}
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6">
              {{ $t('certificate') }}
            </div>
          </div>
          <div
            v-for="stamp in user.orderedTimestamps"
            :key="stamp.txId"
            class="row no-wrap stamp-item2"
          >
            <div
              class="col-md-4 col-sm-6 col-xs-8 q-px-sm overflow"
            >
              <q-icon
                class="col-auto text-grey-6 q-pr-sm"
                :name="fileIcon(stamp.type)"
                style="font-size: 1.5em"
              />
              {{ stamp.name }}
            </div>
            <div
              class="col-md-2 col-sm-4 col-xs-6 row overflow"
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
              class="col-md-2 col-sm-4 col-xs-6 text-left"
            >
              <span v-if="stamp.status !== 0">
                {{ stamp.timestampDate.split(' ')[0] }}
              </span>
              <span v-else>
                __ / __ / ____
              </span>
            </div>
            <div
              class="col-md-2 col-sm-4 col-xs-6 text-left"
            >
              <span v-if="stamp.status !== 0">
                {{ stamp.timestampDate.split(' ')[1] }}
                {{ stamp.timestampDate.split(' ')[2] }}
              </span>
              <span v-else>
                __ : __ : __ __
              </span>
            </div>
            <div
              v-if="stamp.status === 0"
              class="col-md-2 col-sm-4 col-xs-6 q-pr-sm text-left"
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
              v-else-if="stamp.status === 2"
              class="col-md-2 col-sm-4 col-xs-6 q-pr-sm text-left"
            >
              <q-chip
                square
                color="red"
                text-color="white"
              >
                failed
              </q-chip>
            </div>
            <div
              v-else
              class="col-md-2 col-sm-4 col-xs-6 q-pr-sm text-blue cursor-pointer text-left"
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
      <q-card style="width:70%;max-width:1200px;backgroundColor:#4cbbc2;color:#fff">
        <div class="row justify-end">
          <q-icon
            v-close-popup
            name="close"
            style="cursor:pointer"
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
      fileIcon,
      thumbStyle: {
        right: '4px',
        borderRadius: '5px',
        backgroundColor: '#027be3',
        width: '5px',
        opacity: 0.75,
      },
      barStyle: {
        right: '2px',
        borderRadius: '9px',
        backgroundColor: '#027be3',
        width: '9px',
        opacity: 0.2,
      },
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },

    user() {
      return this.$auth.user(false, true, 'timestamps');
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
      const name = `Timescribe Certificate ${stamp.date}.pdf`;
      this.$pdf.create(name, stamp.certificate);
    },
  },
};
</script>
<style lang="scss" scoped>
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

.container {
  max-width: 100%;
  max-height: 100%;
  min-width: 50%;
  min-height: 50%;
  width: 100%;
  height: 50vw;
}


</style>
