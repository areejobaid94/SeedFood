export default {
  /**
   * Set data selected Item
   * @param {any} state current state
   * @param {Object} item selectedMenu item
   */
  setSelectedItem (state, item) {
    state.selectedItem = item
  },
  /**
   * Set data selected Item
   * @param {any} state current state
   * @param {Object} items getting items
   */
  setSelectedItems (state, items) {
    state.items = items
  },
  /**
   * add item to cart
   * @param {any} state current state
   * @param {Object} item added item
   */
  addItemToCard (state, item) {
    state.cart.push(item)
    localStorage.setItem('cart', JSON.stringify(state.cart))
  },
  /**
   * add item to cart
   * @param {any} state current state
   */
  emptyCart (state) {
    state.cart = []
  },
  /**
   * add item to cart
   * @param {any} state current state
   * @param {any} data data
   */
  setCartData (state, data) {
    state.cart = data
    localStorage.setItem('cart', JSON.stringify(state.cart))
  }
}
