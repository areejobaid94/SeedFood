import {
    Component,
    EventEmitter,
    Injector,
    Output,
    ViewChild,
} from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    CreateOrUpdateUserInput,
    OrganizationUnitDto,
    PasswordComplexitySetting,
    ProfileServiceProxy,
    UserEditDto,
    UserRoleDto,
    UserServiceProxy,
    AreasServiceProxy,
    Area,
    AreasEntity,
    AreaDto,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import {
    IOrganizationUnitsTreeComponentData,
    OrganizationUnitsTreeComponent,
} from "../shared/organization-unit-tree.component";
import * as _ from "lodash";
import { finalize } from "rxjs/operators";
import { parseInt } from "lodash";
import { ThemeHelper } from "../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "@app/services/dark-mode.service";

@Component({
    selector: "createOrEditUserModal",
    templateUrl: "./create-or-edit-user-modal.component.html",
    styleUrls: ["create-or-edit-user-modal.component.less"],
})
export class CreateOrEditUserModalComponent extends AppComponentBase {
    theme: string;
    submitted = false;

    @ViewChild("createOrEditModal", { static: true }) modal: ModalDirective;
    @ViewChild("organizationUnitTree")
    organizationUnitTree: OrganizationUnitsTreeComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    canChangeUserName = true;
    isTwoFactorEnabled: boolean = this.setting.getBoolean(
        "Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled"
    );
    isLockoutEnabled: boolean = this.setting.getBoolean(
        "Abp.Zero.UserManagement.UserLockOut.IsEnabled"
    );
    passwordComplexitySetting: PasswordComplexitySetting =
        new PasswordComplexitySetting();

    user: UserEditDto = new UserEditDto();
    roles: UserRoleDto[];
    sendActivationEmail = true;
    setRandomPassword = true;
    passwordComplexityInfo = "";
    profilePicture: string;

    allOrganizationUnits: OrganizationUnitDto[];
    memberedOrganizationUnits: string[];
    userPasswordRepeat = "";

    //branch: Area;
    area: AreaDto = new AreaDto();
    //branches: Area[];
    areas: AreaDto[] = [new AreaDto()];
    branchId = -1;

    dropdownSettings = {};
    dropdownSettings2 = {};
    selectedBranchIds: Array<any> = [];
    selectedBranches: Array<any> = [];
    dropdownList = [];
    selectedItems = [];

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _areasServiceProxy: AreasServiceProxy,
        private _profileService: ProfileServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.theme = ThemeHelper.getTheme();

        this.dropdownSettings = {
            singleSelection: false,
            idField: "id",
            textField: "areaName",
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true,
        };
    }

    show(userId?: number): void {
        if (userId == 0) {
            this.selectedBranchIds = [];
        }
        this.area = new AreaDto();
        if (this.appSession.tenantId != null) {
            this._areasServiceProxy
                .getAvailableAreas(this.appSession.tenantId)
                .subscribe((branchList) => {
                    this.areas = branchList;
                });
        }

        this.active = true;
        this.setRandomPassword = true;
        this.sendActivationEmail = true;

        this._userService.getUserForEdit(userId).subscribe((userResult) => {
            if (userResult.areaId == -1) {
            } else {
                this.area = this.areas.find(
                    (element) => element.id == userResult.areaId
                );
            }

            this.user = userResult.user;
            this.roles = userResult.roles;
            this.canChangeUserName =
                this.user.userName !==
                AppConsts.userManagement.defaultAdminUserName;

            this.allOrganizationUnits = userResult.allOrganizationUnits;
            this.memberedOrganizationUnits =
                userResult.memberedOrganizationUnits;

            this.getProfilePicture(userId);

            if (userId) {
                this.active = true;

                setTimeout(() => {
                    this.setRandomPassword = false;
                }, 0);

                this.sendActivationEmail = false;
            }

            const areaIds = this.user.areaIds;
            if (areaIds != undefined && areaIds != null && areaIds != "") {
                var array = areaIds.split(",");
                this.selectedBranchIds = [];
                array.forEach((element) => {
                    var Area = this.areas.find(
                        (x) => x.id == parseInt(element)
                    );
                    if (Area != undefined) {
                        this.selectedBranchIds.push(Area);
                    }
                });
            }

            this._profileService
                .getPasswordComplexitySetting()
                .subscribe((passwordComplexityResult) => {
                    this.passwordComplexitySetting =
                        passwordComplexityResult.setting;
                    this.setPasswordComplexityInfo();
                    this.modal.show();
                });
        });
    }
    onChangeArea(event): void {
        const agantId = event.target.value;
    }

    setPasswordComplexityInfo(): void {
        this.passwordComplexityInfo = "<ul>";

        if (this.passwordComplexitySetting.requireDigit) {
            this.passwordComplexityInfo +=
                "<li>" +
                this.l("PasswordComplexity_RequireDigit_Hint") +
                "</li>";
        }

        if (this.passwordComplexitySetting.requireLowercase) {
            this.passwordComplexityInfo +=
                "<li>" +
                this.l("PasswordComplexity_RequireLowercase_Hint") +
                "</li>";
        }

        if (this.passwordComplexitySetting.requireUppercase) {
            this.passwordComplexityInfo +=
                "<li>" +
                this.l("PasswordComplexity_RequireUppercase_Hint") +
                "</li>";
        }

        if (this.passwordComplexitySetting.requireNonAlphanumeric) {
            this.passwordComplexityInfo +=
                "<li>" +
                this.l("PasswordComplexity_RequireNonAlphanumeric_Hint") +
                "</li>";
        }

        if (this.passwordComplexitySetting.requiredLength) {
            this.passwordComplexityInfo +=
                "<li>" +
                this.l(
                    "PasswordComplexity_RequiredLength_Hint",
                    this.passwordComplexitySetting.requiredLength
                ) +
                "</li>";
        }

        this.passwordComplexityInfo += "</ul>";
    }

    getProfilePicture(userId: number): void {
        if (!userId) {
            this.profilePicture =
                this.appRootUrl() +
                "assets/common/images/default-profile-picture.png";
            return;
        }

        this._profileService
            .getProfilePictureByUser(userId)
            .subscribe((result) => {
                if (result && result.profilePicture) {
                    this.profilePicture =
                        "data:image/jpeg;base64," + result.profilePicture;
                } else {
                    this.profilePicture =
                        this.appRootUrl() +
                        "assets/common/images/default-profile-picture.png";
                }
            });
    }

    onShown(): void {
        this.organizationUnitTree.data = <IOrganizationUnitsTreeComponentData>{
            allOrganizationUnits: this.allOrganizationUnits,
            selectedOrganizationUnits: this.memberedOrganizationUnits,
        };

        document.getElementById("Name").focus();
    }

    save(): void {
        this.saving = true;
        if (
            this.user.name === null ||
            this.user.name === undefined ||
            this.user.name === "" ||
            this.user.password != this.userPasswordRepeat ||
            this.user.surname === null ||
            this.user.surname === undefined ||
            this.user.surname === "" ||
            this.user.emailAddress === null ||
            this.user.emailAddress === undefined ||
            this.user.emailAddress === "" ||
            this.user.userName === null ||
            this.user.userName === undefined ||
            this.user.userName === ""
        ) {
            this.submitted = true;
            this.saving = false;
            return;
        }

        if (!this.user.id) {
            if (
                !this.setRandomPassword &&
                (this.user.password === null ||
                    this.user.password === undefined ||
                    this.user.password === "" ||
                    this.userPasswordRepeat === null ||
                    this.userPasswordRepeat === undefined ||
                    this.userPasswordRepeat === "")
            ) {
                this.submitted = true;
                this.saving = false;
                this.notify.error(this.l("pleaseFillPassField"));
                return;
            }
        }

        let input = new CreateOrUpdateUserInput();
        input.areaIds = this.selectedBranchIds
            .filter((f) => f.id > 0)
            .map(({ id }) => id)
            .toString();
        if (input.areaIds == "") {
            input.areaIds = null;
        }

        input.user = this.user;
        input.setRandomPassword = this.setRandomPassword;
        input.sendActivationEmail = false;
        input.user.shouldChangePasswordOnNextLogin = false;
        input.user.isActive = true;
        input.assignedRoleNames = _.map(
            _.filter(this.roles, {
                isAssigned: true,
                inheritedFromOrganizationUnit: false,
            }),
            (role) => role.roleName
        );

        // input.organizationUnits = this.organizationUnitTree.getSelectedOrganizations();

        this.saving = true;
        this._userService
            .createOrUpdateUser(input)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(() => {
                this.notify.info(this.l("savedSuccessfully"));
                this.submitted = false;
                this.saving = false;
                this.close();
                this.modalSave.emit(null);
            });
    }

    close(): void {
        this.active = false;
        this.submitted = false;
        this.saving = false;
        this.userPasswordRepeat = "";
        this.modal.hide();
    }

    getAssignedRoleCount(): number {
        return _.filter(this.roles, { isAssigned: true }).length;
    }

    onSelectAll(items: any) {
        this.selectedBranchIds = [];
        this.selectedBranchIds = items;
    }
}
