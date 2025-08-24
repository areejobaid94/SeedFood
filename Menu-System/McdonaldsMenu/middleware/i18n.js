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




//const elems = document.querySelectorAll('.arrow-icon');
// var elems = document.querySelector(".arrow-icon");

//var filename="style"


  if(locale === 'en')
  
   {
   // import('~/assets/style.css')
    
   // filename="style";

    //changeArrow();
  } 
else{
  
//import('~/assets/arrtl.css') 
//filename="arrtl";
//changeArrow();

}
//flipTheme('style_es.css')

function flipTheme(lang) 
{
   $('css[rel="stylesheet"]').each(function () 
{ 
  if (this.href.indexOf(theme)>=0) 
  { 
    this.href = this.href.replace(theme, lang); theme = lang; 
  } });
 }

  
  


function changeArrow(){
  var elems = document.getElementsByClassName("arrow-icon");
if (elems.length > 0){ 
  for (let i = 0; i < elems.length; i++) {
  var $this= elems[i];

if (locale == "en"){
  $this.classList.remove('fa-angle-left');
  $this.classList.add("fa-angle-right");
}
else{ 
  $this.classList.remove('fa-angle-right');
  $this.classList.add("fa-angle-left");
}
   };
  }
}


}
