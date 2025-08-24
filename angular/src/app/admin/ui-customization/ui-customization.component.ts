import { Component, ViewEncapsulation, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeSettingsDto, UiCustomizationSettingsServiceProxy } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { ThemeHelper } from '../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../services/dark-mode.service';

@Component({
    templateUrl: './ui-customization.component.html',
    styleUrls: ['./ui-customization.component.less'],
    animations: [appModuleAnimation()],
    encapsulation: ViewEncapsulation.None
})
export class UiCustomizationComponent extends AppComponentBase implements OnInit {
    theme:string;

    themeSettings: ThemeSettingsDto[];
    currentThemeName = '';

    constructor(
        injector: Injector,
        private _uiCustomizationService: UiCustomizationSettingsServiceProxy,
        public darkModeService : DarkModeService,
    ) {
        super(injector);
    }

    getLocalizedThemeName(str: string): string {
        return this.l('Theme_' + abp.utils.toPascalCase(str));
    }

    ngOnInit(): void {
        this.theme= ThemeHelper.getTheme();
        this.currentThemeName = 'theme12' ||this.currentTheme.baseSettings.theme;
        this._uiCustomizationService.getUiManagementSettings().subscribe((settingsResult) => {
           // ThemeSettingsDto modele = 
           // settingsResult.push()
             
            this.themeSettings = 
            _.sortBy(settingsResult, setting => {
                return 12;
                // setting.theme === 'default' ? 0 : parseInt(setting.theme.replace('theme', ''));
            });
            this.themeSettings=this.themeSettings.filter(x => x.theme != "theme5");
        });
    }
}
