import {
    Component,
    EventEmitter,
    Injector,
    Input,
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
    TeamsServiceProxy,
    TeamsCreateDto,
    TeamsMembersDto,
    TeamsDtoModel,
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
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
interface assignedUser {
    id: number;
    fullName: string;
    isActive: boolean;
}
@Component({
    selector: "createOrEditTeamsModal",
    templateUrl: "./create-or-edit-teams-modal.component.html",
    styleUrls: ["./create-or-edit-teams-modal.component.less"],
})

export class CreateOrEditTeamsModalComponent extends AppComponentBase {
    theme: string;
    submitted = false;

    @ViewChild("createOrEditTeamsModal", { static: true }) modal: ModalDirective;
    @ViewChild("organizationUnitTree")
    organizationUnitTree: OrganizationUnitsTreeComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() users;
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
        groupNameModel: string = "";
        groupNameStatus: number = 2;
    user: UserEditDto = new UserEditDto();
    teamsMember: TeamsMembersDto = new TeamsMembersDto();
    teamsCreate: TeamsDtoModel = new TeamsDtoModel();

    roles: UserRoleDto[];
    sendActivationEmail = true;
    setRandomPassword = true;
    passwordComplexityInfo = "";
    profilePicture: string;

    allOrganizationUnits: OrganizationUnitDto[];
    memberedOrganizationUnits: string[];
    userPasswordRepeat = "";
    selectedUsersToAssign: assignedUser[] = [];
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
    isCreat=true;
    
    unChangedUsersToAssign: number[] = [];
    TeamsId: number = 0;
    isModified: boolean = false;


    constructor(
        injector: Injector, 
         private teamService: TeamInboxService,
        private _teamsService: TeamsServiceProxy,
        private _userService: UserServiceProxy,
        private _areasServiceProxy: AreasServiceProxy,
        private _profileService: ProfileServiceProxy,
        public darkModeService: DarkModeService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.theme = ThemeHelper.getTheme();

    }


    save(): void {
        
        let userIds: string = this.selectedUsersToAssign
        .filter((item) => item.isActive) // Filter out items where `active` is true
        .map((item) => item.id) // Map to the `id` property
        .join(",");
if(this.groupNameModel.length<=0){
    this.message.error("", this.l("TheGroupNameNotEmptyAndNotExceed"));
    return 
}

        this.saving = true;
        this._teamsService
        .validTeamsName(this.groupNameModel,this.isCreat)
        .subscribe((res) => {

            switch (res.state) {
                case 2:
                    this.teamsCreate.id=this.TeamsId;
                this.teamsCreate.teamsName=this.groupNameModel;
                this.teamsCreate.userIds=userIds;
                this.teamsMember.teamsModel=this.teamsCreate
                this.teamsMember.isCreate=this.isCreat
                this._teamsService
                .teamsCreateMembers(this.teamsMember)
                .subscribe((res) => {

                    this.notify.info(this.l("savedSuccessfully"));
                    this.submitted = false;
                    this.saving = false;
                    this.close();
                    this.modalSave.emit(null);
                })
                    
                    break;
                case 1 : 
                this.message.error("", this.l("TheGroupNameNotEmptyAndNotExceed"));
                this.saving = false;
                break;
                case 3 : 
                this.message.error("", this.l("groupNameisUsed"));
                this.saving = false;
                break;
                default:
                    this.message.error("", res?.message || ' internal Server Error');
                    this.saving = false;
            }
        });
    }

    close(): void {

        if(this.groupNameModel){
            this.groupNameModel="";
        }
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




    

    show() { 
        
        this.TeamsId=0;
        this.isCreat=true;
          this.active = true;
          this.isModified = false;
          this.selectedUsersToAssign = [];
          this.unChangedUsersToAssign = [];
          this.teamService.getUsers().subscribe((result: any) => {
            this.users = result.result.items;
            this.users.forEach((user) => {
                let temp: assignedUser = {
                    id: user.id,
                    fullName: user.fullName,
                    isActive: false,
                };
                this.isModified=false;
                this.selectedUsersToAssign.push(temp);
                this.modal.show();
            });
        });



          this.modal.show();
    }
    showEdit(model:TeamsDtoModel) {
        this.TeamsId=model.id;
        this.active = true;
       this.isCreat=false;
        this.groupNameModel=model.teamsName;
        this.isModified = false;
        let tempSelectedUsers;
        tempSelectedUsers = model.userIds.split(",");
        this.selectedUsersToAssign = [];
        this.unChangedUsersToAssign = [];

        this.teamService.getUsers().subscribe((result: any) => {
            this.users = result.result.items;
            
            this.users.forEach((user) => {
                let temp: assignedUser = {
                    id: user.id,
                    fullName: user.fullName,
                    isActive: false,
                };
                if (
                    tempSelectedUsers.findIndex(
                        (id) => parseInt(id, 10) === user.id
                    ) != -1
                ) {
                    temp.isActive = true;
                    this.unChangedUsersToAssign.push(temp.id);
                } else {
                    temp.isActive = false;
                }
                this.selectedUsersToAssign.push(temp);
            });
    

            this.modal.show();
        });


      
  }
    showInbox(ticketId: number) {
      
    }

    onCheckboxChange(user) {
        debugger
        let tempUserIndex = this.selectedUsersToAssign.findIndex(
            (assignedUser) => assignedUser.id === user.id
        );


        if(tempUserIndex==-1){

            this.isModified = false;
        }else{

            this.selectedUsersToAssign[tempUserIndex].isActive =
            !this.selectedUsersToAssign[tempUserIndex].isActive;

        if (
            this.listsAreDifferent(
                this.selectedUsersToAssign,
                this.unChangedUsersToAssign
            )
        ) {
            this.isModified = true;
        } else {
            this.isModified = false;
        }

        }
       
    }
    listsAreDifferent(list1, list2) {
        let tempTrue = list1.filter((x) => x.isActive === true).map(l => l.id);
    
        // Check if arrays have the same length
        if (tempTrue.length !== list2.length) {
            return true;
        }
    
        // Sort both arrays to make comparison easier
        tempTrue.sort();
        list2.sort();
    
        // Check if all elements in both arrays are the same
        for (let i = 0; i < tempTrue.length; i++) {
            if (tempTrue[i] !== list2[i]) {
                return true;
            }
        }
    
        // If none of the above conditions are met, the lists are the same
        return false;
    }
    
    assignUsers() {
       
    }








}
