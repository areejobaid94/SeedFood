export default {
  /**
   * @param {Function} commit function to call mutation
   * @param {Object} item selected menu item
   */
  async setSelectedItem ({ commit }, item) {
    await commit('setSelectedItem', item)
  },
  /**
   * @param {Function} commit function to call mutation
   * @param {Object} item selected menu item
   */
  async setSelectedItems ({ commit }, items) {
    await commit('setSelectedItems', items)
  },
  /**
   * @param {Function} commit function to call mutation
   * @param {Object} item selected menu item
   */
  async addItemToCard ({ commit }, item) {
    await commit('addItemToCard', item)
  },
  /**
   * @param {Function} commit function to call mutation
   */
  async emptyCart ({ commit }) {
    await commit('emptyCart')
  },
  /**
   * @param {Function} commit function to call mutation
   * @param {Object} data selected data item
   */
  async setCartData ({ commit }, data) {
    await commit('setCartData', data)
  }
}
