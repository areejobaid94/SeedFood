import {
    PermissionCheckerService,
    FeatureCheckerService,
    LocalizationService,
    MessageService,
    AbpMultiTenancyService,
    NotifyService,
    SettingService
} from 'abp-ng2-module';
import { Component, Injector } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { UiCustomizationSettingsDto } from '@shared/service-proxies/service-proxies';
import { NgxSpinnerService } from 'ngx-spinner';
import { NgxSpinnerTextService } from '@app/shared/ngx-spinner-text.service';
import { io } from 'socket.io-client';
import { RoleService } from './session/role.service';

export abstract class AppComponentBase {

    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: AbpMultiTenancyService;
    appSession: AppSessionService;
    primengTableHelper: PrimengTableHelper;
    AllTicketRecordAfterFilter:any;
    ui: AppUiCustomizationService;
    appUrlService: AppUrlService;
    spinnerService: NgxSpinnerService;
    private ngxSpinnerTextService: NgxSpinnerTextService;
    roleService: RoleService;
    socket;
    isAdmin: boolean

    constructor(injector: Injector) {
        
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(AbpMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.ui = injector.get(AppUiCustomizationService);
        this.appUrlService = injector.get(AppUrlService);
        this.primengTableHelper = new PrimengTableHelper();
        this.spinnerService = injector.get(NgxSpinnerService);
        this.ngxSpinnerTextService = injector.get(NgxSpinnerTextService);
        this.roleService = injector.get(RoleService);
    }




    flattenDeep(array) {
        return array.reduce((acc, val) =>
            Array.isArray(val) ?
                acc.concat(this.flattenDeep(val)) :
                acc.concat(val),
            []);
    }

    l(key: string, ...args: any[]): string {
        args.unshift(key);
        args.unshift(this.localizationSourceName);
        return this.ls.apply(this, args);
    }

    ls(sourcename: string, key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, sourcename);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, this.flattenDeep(args));
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }

    isGrantedAny(...permissions: string[]): boolean {
        if (!permissions) {
            return false;
        }

        for (const permission of permissions) {
            if (this.isGranted(permission)) {
                return true;
            }
        }

        return false;
    }

    s(key: string): string {
        return abp.setting.get(key);
    }

    appRootUrl(): string {
        return this.appUrlService.appRootUrl;
    }

    get currentTheme(): UiCustomizationSettingsDto {
        return this.appSession.theme;
    }

    get containerClass(): string {
        if (this.appSession.theme.baseSettings.layout.layoutType === 'fluid') {
            return 'container-fluid';
        }

        return 'container';
    }

    showMainSpinner(text?: string): void {
        this.ngxSpinnerTextService.setText(text);
        this.spinnerService.show();
    }

    hideMainSpinner(text?: string): void {
        this.spinnerService.hide();
    }

    hasArabicCodepoints(text) {
        if (text != null) {
            let firstCharacter =text.charAt(0);
            if (/[\u0600-\u06FF]/.test(firstCharacter)) {
                return true;
            } else if ((/^[A-Za-z0-9]*$/.test(firstCharacter))) {
                return false;
            }
        }
    }

    async getIsAdmin(){
        if(this.isAdmin == null){
            this.isAdmin = await this.roleService.isAdmin();
        }
    }


    base64ToFile(base64String: string, filename: string, mimeType: string): File {
        const base64Data = base64String.split(',')[1];
        const byteChars = atob(base64Data);
        const byteNumbers = new Array(byteChars.length);
        for (let i = 0; i < byteChars.length; i++) {
            byteNumbers[i] = byteChars.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        return new File([byteArray], filename, { type: mimeType });
    }

    extractMimeType(base64String: string): string {
    const mimeMatch = base64String.match(/data:(image\/\w+);base64,/);
    return mimeMatch ? mimeMatch[1] : 'image/png'; // Default to PNG if MIME type is not found
    }

    getExtensionFromMimeType(mimeType: string): string {
        switch (mimeType) {
          case 'image/jpeg': return '.jpg';
          case 'image/png': return '.png';
          case 'image/gif': return '.gif';
          default: return '.jpg'; // Default to .jpg for unsupported MIME types
        }
    }

    setToStartOfDay(date: Date): Date {
        return new Date(date.setHours(0, 0, 0, 0));
    }

    setToEndOfDay(date: Date): Date {
        // Create a new date object to avoid modifying the original date
        const endOfDay = new Date(date);
        // Set the time to the last millisecond of the day
        endOfDay.setHours(23, 59, 59, 999);
        return endOfDay;
    }
}
