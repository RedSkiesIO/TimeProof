/* eslint-disable */
/**
 * @jest-environment jsdom
 */

import { mount, createLocalVue, shallowMount } from '@vue/test-utils'
import Payment from '../../../../src/components/Payment/index.vue';
import * as All from 'quasar'
import store from '../../../../store';
import Vuex from 'vuex';
import flushPromises from 'flush-promises'

// import langEn from 'quasar/lang/en-us' // change to any language you wish! => this breaks wallaby :(
const { Quasar, date } = All

const components = Object.keys(All).reduce((object, key) => {
  const val = All[key]
  if (val && val.component && val.component.name != null) {
    object[key] = val
  }
  return object
}, {})

describe('Mount Payment', () => {
  const localVue = createLocalVue();

  let wrapper;
  let vm;

  beforeAll(() => {
    localVue.use(Quasar, { components }); // , lang: langEn
    localVue.use(Vuex);
    localVue.prototype.$auth = {
      user: (val) => {
          return {
              id: 1,
              userId: 2,
              email: 'veyseltosun.vt@gmail.com'
          }
      }
    }
    wrapper = mount(Payment, {
        localVue,
        mocks:{
            $router: {
                push: jest.fn()
            }
        },
        store,
    });

    vm = wrapper.vm;
  });

  test('passes the sanity check and creates a wrapper', () => {
    expect(wrapper.isVueInstance()).toBe(true)
  })

  it('has a created hook', () => {
    expect(typeof vm.setSellingProduct).toBe('function')
  })

  it('payment billing has exist', () => {
    expect(vm.$refs.paymentBilling).not.toBeNull();
  })

  it('make sure submit payment button is disabled before payment triggered', async () => {
    const submitButton = wrapper.find('button.payment-button');
    expect(submitButton.attributes().disabled).toBe('disabled');
  })

  it('make payment with random card number', async () => {

    vm.$refs.paymentBilling.name = "Veysel TOSUN";
    vm.$refs.paymentBilling.email = "veyseltosun.vt@gmail.com"
    vm.$refs.paymentBilling.address = "address 1";
    vm.$refs.paymentBilling.city = "London";
    vm.$refs.paymentBilling.state = "";
    vm.$refs.paymentBilling.postalCode = "HA9 8BB";
    vm.$refs.paymentBilling.country = "GB";

    await vm.$nextTick();
    await flushPromises();
    const submitButton = wrapper.find('button.payment-button');
    const cardNumber = wrapper.findAll('input.InputElement');
    expect(submitButton.attributes().disabled).not.toBe('disabled');
    await submitButton.trigger('click');
    await vm.$nextTick();
    expect(submitButton.text()).toBe('Processing...');
  });
})
