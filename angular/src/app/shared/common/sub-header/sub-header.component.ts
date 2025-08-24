import { Component, Injector, Input } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from "./../../../services/dark-mode.service";

export class BreadcrumbItem {
    text: string;
    routerLink?: string;
    navigationExtras?: NavigationExtras;

    constructor(
        text: string, routerLink?: string, navigationExtras?: NavigationExtras
        ) 
        {
        this.text = text;
        this.routerLink = routerLink;
        this.navigationExtras = navigationExtras;
    }

    isLink(): boolean {
        return !!this.routerLink;
    }
}

@Component({
    selector: 'sub-header',
    templateUrl: './sub-header.component.html'
})
export class SubHeaderComponent extends AppComponentBase {
    theme: string;
    @Input() title: string;
    @Input() description: string;
    @Input() breadcrumbs: BreadcrumbItem[];
    @Input() showHomeIcon?: boolean = false;


    constructor(
        private _router: Router,
        injector: Injector,
        public darkModeService: DarkModeService,
    ) {
        super(injector);
    }
    ngOnInit(): void {

        this.theme = ThemeHelper.getTheme();

    }

    goToBreadcrumb(breadcrumb: BreadcrumbItem): void {
        if (!breadcrumb.routerLink) {
            return;
        }

        if (breadcrumb.navigationExtras) {
            this._router.navigate([breadcrumb.routerLink], breadcrumb.navigationExtras);
        } else {
            this._router.navigate([breadcrumb.routerLink]);
        }
    }
    goToDashboard() {
        this._router.navigate(['/app/main/dashboard']);
    }
}
