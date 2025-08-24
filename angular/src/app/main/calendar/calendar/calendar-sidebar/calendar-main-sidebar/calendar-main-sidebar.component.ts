import { Component, Injector } from "@angular/core";
import { DarkModeService } from "@app/services/dark-mode.service";
import { CalendarService } from "../../calendar.service";
import { ThemeHelper } from "../../../../../shared/layout/themes/ThemeHelper";
import { AreasServiceProxy, GetAllDashboard, UserServiceProxy } from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { Branches, Users } from "../../calendar.model";
import { User } from "@app/auth/models";
import * as rtlDetect from 'rtl-detect';


@Component({
    selector: "app-calendar-main-sidebar",
    templateUrl: "./calendar-main-sidebar.component.html",
    styleUrls: ["./calendar-main-sidebar.component.css"],
})
export class CalendarMainSidebarComponent extends AppComponentBase {
    public calendarRef = [];
    public tempRef = [];
    branches: Branches[]=[];
    Branch:Branches;
    users : Users[]=[];
    user : Users;
    theme: string;
    public checkAll = true;
    public checkAllBranches = true;
    public checkAllUsers = true;
    public isCollapsed5 = true;
    Measurement: GetAllDashboard = new GetAllDashboard();
    TotalRemainingAdsConversation: number = 0;
    isArabic= false;

    constructor(
        public calendarService: CalendarService,
        public darkModeService: DarkModeService,
        private _areasServiceProxy: AreasServiceProxy,
        injector: Injector,
        private _userServiceProxy: UserServiceProxy,
    ) {
        super(injector);
    }

    allChecked() {
        return this.calendarRef.every((v) => v.checked === true);
    }

    
    allCheckedBranches() {
        return this.branches.every((v) => v.checked === true);
    }

    
    allCheckUsers(){
        return this.users.every((v) => v.checked === true);
    }

    getBranch() {
        this.branches = [];
        this._areasServiceProxy
            .getAvailableAreas(this.appSession.tenantId)
            .subscribe((result) => {
                result.forEach(branch => {
                    this.Branch=new Branches();
                    this.Branch.id=branch.id;
                    this.Branch.areaName= branch.areaName;
                    this.Branch.checked = true;
                    this.branches.push(this.Branch)
                })
            });
    }

    getUsers(){
        this._userServiceProxy
        .getBookingUsers(this.appSession.tenantId, null)
        .subscribe((result) => {
            result.forEach(user => {
                this.user =new Users();
                this.user.id=user.id;
                this.user.name= user.name;
                this.user.checked = true;
                this.users.push(this.user)
            })

        });
    }

    checkboxChange(event, id) {
        const index = this.calendarRef.findIndex((r) => {
            if (r.id === id) {
                return id;
            }
        });
        this.calendarRef[index].checked = event.target.checked;
        this.checkAll = this.allChecked();
        this.calendarService.calendarUpdate(this.calendarRef);
    }

    checkboxChangeBranch(event, id) {
        const index = this.branches.findIndex((r) => {
            if (r.id === id) {
                return id;
            }
        });
        this.branches[index].checked = event.target.checked;
        this.checkAllBranches = this.allCheckedBranches();
        this.calendarService.calendarUpdateBranches(this.branches);
    }
    checkboxChangeUser(event, id) {
        const index = this.users.findIndex((r) => {
            if (r.id === id) {
                return id;
            }
        });
        this.users[index].checked = event.target.checked;
        this.checkAllUsers = this.allCheckUsers();
        this.calendarService.calendarUpdateUsers(this.users);
    }




    toggleCheckboxAll(event) {
        this.checkAll = event.target.checked;
        if (this.checkAll) {
            this.calendarRef.map((res) => {
                res.checked = true;
            });
        } else {
            this.calendarRef.map((res) => {
                res.checked = false;
            });
        }
        this.calendarService.calendarUpdate(this.calendarRef);
    }

    toggleCheckboxAllBranches(event){
        
        this.checkAllBranches = event.target.checked;
        if (this.checkAllBranches) {
            this.branches.map((res) => {
                res.checked = true;
            });
        } else {
            this.branches.map((res) => {
                res.checked = false;
            });
        }
        this.calendarService.calendarUpdateBranches(this.branches);
    }

    toggleCheckboxAllUsers(event){
        this.checkAllUsers = event.target.checked;
        if (this.checkAllUsers) {
            this.users.map((res) => {
                res.checked = true;
            });
        } else {
            this.users.map((res) => {
                res.checked = false;
            });
        }
        this.calendarService.calendarUpdateUsers(this.users);
    }



    async ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.GetStatistics();
        this.getBranch();
        this.getUsers();
        await this.getIsAdmin();
        this.calendarRef = [
            { id: 1, filter: this.localization.localize('pending', this.localizationSourceName) ,color: "checkbox-pending", checked: true },
            { id: 2, filter: this.localization.localize('confirmed', this.localizationSourceName), color: "checkbox-Confirmed", checked: true },
            { id: 3, filter:  this.localization.localize('booked', this.localizationSourceName), color: "checkbox-Booked", checked: true },
            { id: 4, filter:  this.localization.localize('canceled', this.localizationSourceName), color: "checkbox-Canceled ", checked: true },
            { id: 5, filter:  this.localization.localize('deleted', this.localizationSourceName), color: "checkbox-Deleted ", checked: true },
        ];

        // this.calendarService.onCalendarChange.subscribe((res) => {
        //     this.calendarRef = res;
        // });
    }

    createOrEdit() {
        this.calendarService.addEventt();

    }

    GetStatistics() {
    // this._whatsAppMessageTemplateServiceProxy
    //   .getStatistics(null)
    //   .subscribe((result) => {
    //     this.Measurement = result;

    //             this.TotalRemainingAdsConversation =
    //                 this.Measurement.remainingFreeConversation +
    //                 this.Measurement.remainingBIConversation;
    //         });
    }
}
