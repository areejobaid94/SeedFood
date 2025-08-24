export default function ({
  isHMR,
  app,
  store,
  route,
  params,
  error,
  redirect
}) {
  const defaultLocale = app.i18n.fallbackLocale
  // If middleware is called from hot module replacement, ignore it
  if (isHMR) {
    return
  }
  const locale = route.query.lang || defaultLocale
  document.getElementsByTagName('html')[0].dir = locale === 'ar' ? 'rtl' : 'ltr'
  document.getElementsByTagName('html')[0].style.direction = locale === 'ar' ? 'rtl' : 'ltr'
  app.i18n.locale = locale
}
