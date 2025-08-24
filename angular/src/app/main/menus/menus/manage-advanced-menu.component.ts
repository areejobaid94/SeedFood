import {
    Component,
    Injector,
    ViewEncapsulation,
    ViewChild,
    ElementRef,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import {
    ItemDto,
    MenuServiceProxy,
    CreateOrEditMenuCategoryDto,
    ItemCategoryServiceProxy,
    CreateOrEditMenuSubCategoryDto,
    ItemsServiceProxy,
    LoyaltyServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/common/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { FileDownloadService } from "@shared/utils/file-download.service";
import { CreateOrEditctownModalComponent } from "@app/main/CtownUpdate/create-or-edit-ctown-modal.component";
import { CreateOrEditCategoryModalComponent } from "./create-or-edit-category-modal.component";
import { CreateOrEditSubCategoryModalComponent } from "./create-or-edit-subcategory-modal.component";
import { CreateOrEditCategoryItemComponent } from "./create-or-edit-category-item-modal.component";
import { TeamInboxService } from "@app/main/teamInbox/teaminbox.service";
import { ThemeHelper } from "../../../shared/layout/themes/ThemeHelper";
import { DarkModeService } from "./../../../services/dark-mode.service";
import { NgxSpinnerService } from "ngx-spinner";
import { getItem } from "localforage";

@Component({
    templateUrl: "./manage-advanced-menu.component.html",
    styleUrls: ["./manage-advanced-menu.component.less"],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
})
export class ManageAdvancedMenuComponent extends AppComponentBase {
    theme: string;
    selectedCategory: number;
    selectedSubCategory: number;
    startSubCatLoad = false;
    startCatLoad = false;

    @ViewChild("createOrEditctownModal", { static: true })
    createOrEditctownModal: CreateOrEditctownModalComponent;
    @ViewChild("createOrEditCategoryComponent", { static: true })
    createOrEditCategoryComponent: CreateOrEditCategoryModalComponent;
    @ViewChild("createOrEditSubCategoryComponent", { static: true })
    createOrEditSubCategoryComponent: CreateOrEditSubCategoryModalComponent;
    @ViewChild("createOrEditCategoryItemModalComponent", { static: true })
    createOrEditCategoryItemModalComponent: CreateOrEditCategoryItemComponent;

    catgory: CreateOrEditMenuCategoryDto[];
    subcatgory: CreateOrEditMenuSubCategoryDto[];
    items: ItemDto[];
    selectcatgory: CreateOrEditMenuCategoryDto;
    selectsubcatgory: CreateOrEditMenuSubCategoryDto;
    showMessageLoader = false;
    formDataFile: any;
    fileToUpload: any;
    menuID: number;
    currency = "JOD";
    isTenantLoyal: boolean;
    extFile: string;
    @ViewChild("file")
    element: ElementRef;

    constructor(
        injector: Injector,
        private teamService: TeamInboxService,
        private _MenuServiceProxy: MenuServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        private _menuCategoriesServiceProxy: ItemCategoryServiceProxy,
        private _itemsServiceProxy: ItemsServiceProxy,
        public darkModeService: DarkModeService,
        private router: Router,
        private spinner: NgxSpinnerService,
        private _loyaltyServiceProxy: LoyaltyServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.currency = this.appSession.tenant.currencyCode;
        this.theme = ThemeHelper.getTheme();
        this.fillDataFromRoute();
        this.getCatogeory();
        this.checkIsLoyality();
    }

    fillDataFromRoute(): void {
        if (this._activatedRoute.snapshot.queryParams["menuid"] != null) {
            this.menuID = this._activatedRoute.snapshot.queryParams["menuid"];
        }
    }
    UpdateItems() {
        this.getItemtSubCategory();
    }

    getCatogeory(): void {
        this._MenuServiceProxy.getCatogeory(this.menuID).subscribe((res) => {
            this.catgory = res;
            this.selectedCategory = null;
            this.subcatgory = null;
            this.items = null;
            //  if(res[0].lstMenuSubCategoryDto != undefined)
            //  {this.subcatgory=res[0].lstMenuSubCategoryDto;
            // }
            // this.selectcatgory=res[0];
            //   if(this.selectcatgory.lstMenuSubCategoryDto[0] != undefined ){
            //   this.selectsubcatgory= this.selectcatgory.lstMenuSubCategoryDto[0];
            // }
            //   this.getItemtSubCategory();
        });
    }

    getSubCatogeory(): void {
        this._MenuServiceProxy.getCatogeory(this.menuID).subscribe((res) => {
            this.catgory = res;
            this.subcatgory = res[0].lstMenuSubCategoryDto;

            this.selectcatgory = res[0];
            this.selectedCategory = 0;
            // if (this.selectcatgory.lstMenuSubCategoryDto[0] != undefined) {
            //   this.selectsubcatgory= this.selectcatgory.lstMenuSubCategoryDto[0];
            //   this.selectsubcatgory= this.selectcatgory.lstMenuSubCategoryDto[0];

            // }
            //   this.getItemtSubCategory();
            this.items = null;
        });
    }

    selectCategory(cat: CreateOrEditMenuCategoryDto, index): void {
        this.startCatLoad = true;
        this.subcatgory = cat.lstMenuSubCategoryDto;
        this.items = null;
        this.selectcatgory = cat;
        this.selectedCategory = index;
        if (
            this.selectcatgory.lstMenuSubCategoryDto &&
            this.selectcatgory.lstMenuSubCategoryDto[0] != undefined
        ) {
            this.selectsubcatgory = this.selectcatgory.lstMenuSubCategoryDto[0];
            this.selectedSubCategory = null;
        }
        this.startCatLoad = false;

        //this.getItemtSubCategory();
    }

    selectSubCategory(subcat: CreateOrEditMenuSubCategoryDto, index): void {
        this.startSubCatLoad = true;
        this.selectsubcatgory = subcat;
        this.selectedSubCategory = index;
        this.getItemtSubCategory();
    }

    getItemtSubCategory(): void {
        // this.isOneTime = true;
        this.showMessageLoader = true;
        this._MenuServiceProxy
            .getItems(
                this.menuID,
                this.selectcatgory.id,
                this.selectsubcatgory.id,0,20//should change it 


                
            )
            .subscribe((res) => {
                // this.pageNumberC=this.pageNumberC+1;
                this.showMessageLoader = false;
                this.startSubCatLoad = false;
                this.items = res;
            });
    }

    handleFileInput(event) {
        this.fileToUpload = event.target.files[0];
        this.extFile = this.fileToUpload.name.substring(
            this.fileToUpload.name.lastIndexOf(".") + 1
        );

        if (this.extFile == "xlsx") {
            this.fileToUpload = event.target.files[0];
        } else {
            this.element.nativeElement.value = "";
            this.message.error("", this.l("pleaseChooseExcelFile"));
            event.target.files[0] = null;
            return;
        }
    }

    uploadFileToActivity() {
        if (
            this.selectcatgory === null ||
            this.selectcatgory === undefined ||
            this.selectsubcatgory === null ||
            this.selectsubcatgory === undefined
        ) {
            this.message.error("", this.l("exportAdvMenuToExcelError"));
        } else {
            let formDataFile = new FormData();
            formDataFile.append("formFile", this.fileToUpload);
            this.showMessageLoader = true;
            this.spinner.show();
            this.teamService
                .ItemsUploadExcelFile(formDataFile, this.appSession.tenantId)
                .subscribe(
                    (result) => {

                        this.showMessageLoader = false;
                        this.notify.success(this.l("successfullyUpload"));
                        this.getItemtSubCategory();
                        // this.reloadPage();
                        this.spinner.hide();
                    },
                    (error: any) => {
                        if (error) {
                            this.spinner.hide();
                        }
                    }
                );
        }
    }
    reloadPage() {
        location.reload();
    }

    checkIsLoyality() {
        this._loyaltyServiceProxy.isLoyalTenant().subscribe((result) => {
            this.isTenantLoyal = result;
        });
    }

    exportToExcel(): void {
        if (
            this.selectcatgory === null ||
            this.selectcatgory === undefined ||
            this.selectsubcatgory === null ||
            this.selectsubcatgory === undefined
        ) {
            this.message.error("", this.l("exportAdvMenuToExcelError"));
        } else {
            this.spinner.show();
            this._itemsServiceProxy
                .exportItemsToExcel(
                    this.selectcatgory.id,
                    this.selectsubcatgory.id
                )
                .subscribe(
                    (result) => {

                        this._fileDownloadService.downloadTempFile(result);
                        this.spinner.hide();
                    },
                    (error: any) => {
                        if (error) {
                            this.spinner.hide();
                        }
                    }
                );
        }
    }

    // exportToExcel(): void {
    //   this._itemsServiceProxy.getItemsToExcel(
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     null,
    //     0,
    //     null,
    //     null
    //   )
    //     .subscribe(result => {

    //       this._fileDownloadService.downloadTempFile(result);
    //     });
    // }

    //   onScroll2() {

    //     let scroll = window.pageYOffset;
    //     if (scroll > this.currentPosition) {
    //     } else {
    //     }
    //     this.currentPosition = scroll;

    //        var d =  document.getElementById("DivcontactList");

    //        var height = d.offsetHeight;

    //

    //          if(this.prvHeight>height){
    //

    //           }else{
    //                     var h=this.scrollChatt.nativeElement.scrollHeight-30;//-20;//-height;
    //                     var t=this.scrollChatt.nativeElement.scrollTop + height;
    //                     if (t>= h  && this.isOneTime) {
    //

    //                            this.isOneTime=false;
    //                            this.prvHeight=height;

    //                           this.topnumber=this.scrollChatt.nativeElement.scrollTop;
    //                           this.heiget=this.scrollChatt.nativeElement.scrollHeight;
    //                            this.pageNumberC =this.pageNumberC+ 1;
    //                            this.loadMoreitem();
    //                       }

    //             }

    //  }

    deleteCategoryItem(id) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._menuCategoriesServiceProxy
                    .delete(id)
                    .subscribe((result) => {
                        if (result) {
                            this.notify.success(this.l("successfullyDeleted"));
                            this.getCatogeory();
                            this.selectedCategory = null;
                        } else {
                            this.notify.error(
                                this.l("categoryDeleteRejection")
                            );
                        }
                    });
            }
        });
    }

    deleteSubCategoryItem(id) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._MenuServiceProxy
                    .deleteSubCatogeory(id)
                    .subscribe((result) => {
                        if (result) {
                            this.notify.success(this.l("successfullydeleted"));
                            this.getSubCatogeory();
                        } else {
                            this.notify.error(
                                this.l("categoryDeleteRejection")
                            );
                        }

                        // this.selectedSubCategory = null;
                    });
            }
        });
    }

    deleteItem(id) {
        this.message.confirm("", this.l("AreYouSure"), (isConfirmed) => {
            if (isConfirmed) {
                this._itemsServiceProxy.deleteItem(id).subscribe(() => {
                    this.notify.success(this.l("SuccessfullyDeleted"));
                    this.getItemtSubCategory();
                });
            }
        });
    }

    // loadMoreitem() {
    //   this.showMessageLoader = true;
    // }

    back() {
        this.router.navigate(["/app/main/menus/menus"]);
    }
    goToDashboard() {
        this.router.navigate(["/app/main/dashboard"]);
    }
}
