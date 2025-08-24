import { IThemeAssetContributor } from '../ThemeAssetContributor';
import { AppConsts } from '@shared/AppConsts';
import { StyleLoaderService } from '@shared/utils/style-loader.service';

export class Theme5ThemeAssetContributor implements IThemeAssetContributor {
    public getAssetUrls(): string[] {
        // return [AppConsts.appBaseUrl + '/assets/fonts/fonts-asap-condensed.min.css'];
        const styleLoaderService = new StyleLoaderService();
        let styleUrls = [
            AppConsts.appBaseUrl + '/assets/fonts/fonts-asap-condensed.min.css',
            AppConsts.appBaseUrl + '/assets/metronic/themes/theme12' + '/css/components.min.css',
            AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/pages/app-chat.css',
            AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/css/pages/app-chat-list.css',
            AppConsts.appBaseUrl + '/assets/metronic/themes/theme12'  + '/vendors/css/calendars/fullcalendar.min.css',


        ]
        return styleUrls
    }

    public getAdditionalBodyStle(): string {
        return '';
    }

    public getMenuWrapperStyle(): string {
        return '';
    }

    public getSubheaderStyle(): string {
        return 'subheader-title text-dark font-weight-bold my-1 mr-3';
    }

    public getFooterStyle(): string {
        return 'footer bg-white py-4 d-flex flex-lg-column';
    }
}
