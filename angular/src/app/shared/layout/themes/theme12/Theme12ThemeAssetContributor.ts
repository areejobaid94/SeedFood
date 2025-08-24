import { IThemeAssetContributor } from '../ThemeAssetContributor';
import { AppConsts } from '@shared/AppConsts';
import * as rtlDetect from 'rtl-detect';
import { StyleLoaderService } from '@shared/utils/style-loader.service';

export class Theme12ThemeAssetContributor implements IThemeAssetContributor {

    
    public getAssetUrls(): string[] {
        const isRtl = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);

        if (isRtl) {
            let rtlBody = document.getElementById('bodyID');
            if(rtlBody != null){
                rtlBody.classList.add('rtl');        
            }
            // document.documentElement.setAttribute('dir', 'rtl');
        }

       
        const styleLoaderService = new StyleLoaderService();
          let styleUrls = [
          AppConsts.appBaseUrl + '/assets/metronic/themes/theme12' + '/css/colors.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/bootstrap.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12' + '/vendors/css/vendors.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/bootstrap-extended.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/components.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/themes/dark-layout.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/themes/bordered-layout.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/themes/semi-dark-layout.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/core/menu/menu-types/vertical-menu.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/core/menu/menu-types/vertical-overlay-menu.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/plugins/forms/pickers/form-pickadate.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/plugins/forms/pickers/form-flat-pickr.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/pages/app-chat.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/pages/app-chat-list.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/forms/wizard/bs-stepper.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/forms/select/select2.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/plugins/forms/form-validation.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/plugins/forms/form-wizard.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/extensions/toastr.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/pages/app-calendar.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/plugins/forms/pickers/form-flat-pickr.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/tables/datatable/dataTables.bootstrap5.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/tables/datatable/responsive.bootstrap5.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/charts/apexcharts.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/calendars/fullcalendar.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/pickers/pickadate/pickadate.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/pickers/flatpickr/flatpickr.min.css',
        AppConsts.appBaseUrl + '/assets/css/plugins/forms/form-file-uploader.css',
    

       
    ]

    let styleRtlUrls = [
     //css/plugins/forms/form-file-uploader.css
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12' + '/vendors/css/vendors-rtl.min.css',


        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/bootstrap.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/bootstrap-extended.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12' + '/css-rtl/colors.min.css',
        
      
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/components.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/themes/dark-layout.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/themes/bordered-layout.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/themes/semi-dark-layout.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/core/menu/menu-types/vertical-menu.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css-rtl/pages/app-chat.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12/css-rtl/pages/app-chat-list.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/tables/datatable/dataTables.bootstrap5.min.css',
        AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/tables/datatable/responsive.bootstrap5.min.css',
        AppConsts.appBaseUrl + '/assets/css/plugins/forms/form-file-uploader.css',

    ]

      
          styleLoaderService.loadArray(styleUrls);

          return styleUrls
   
    }

    public getAdditionalBodyStle(): string {
        document.getElementById('bodyID').dataset.open = "click";
        document.getElementById('bodyID').dataset.menu = "vertical-menu-modern";
        document.getElementById('bodyID').dataset.col = "content-left-sidebar";
        const isRtl = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        if (isRtl) {
            return `rtl vertical-layout vertical-menu-modern content-left-sidebar navbar-floating footer-static` ;

        } else{
            return `vertical-layout vertical-menu-modern content-left-sidebar navbar-floating footer-static` ;

        }

    }


    public getMenuWrapperStyle(): string {
        return '';
    }

    public getSubheaderStyle(): string {
        return '';
    }

    public getFooterStyle(): string {
        return 'footer footer-static footer-light';
    }
    

}
