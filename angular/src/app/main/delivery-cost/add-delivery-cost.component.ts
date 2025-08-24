import { Component, EventEmitter, Injector, Output, ViewChild } from "@angular/core";
import { UntypedFormArray, UntypedFormBuilder, UntypedFormGroup, Validators } from "@angular/forms";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { DeliveryCostDetailsDto, DeliveryCostDto, DeliveryCostServiceProxy, ItemDto, MenusServiceProxy, RType } from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs/operators";
import { ThemeHelper } from '../../..../../shared/layout/themes/ThemeHelper';
import * as rtlDetect from 'rtl-detect';



@Component({
    selector: 'addDeliveryCost',
    templateUrl: './add-delivery-cost.component.html',
    styleUrls: ['./add-delivery-cost.component.css']
})
export class AddDeliveryCostComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    theme: string;
    active = false;
    isDisabled = false;
    isFromValid: boolean;
    saving = false;
    saveButton= true;
    deliveryCost: DeliveryCostDto = new DeliveryCostDto();
    lstDeliveryCostDetailsDto: Array<DeliveryCostDetailsDto> = [];
    minValue: number;
    dropdownSettings = {};
    rType: RType[];
    imagSrc: string;
    item: ItemDto;
    selectedAreaIds: Array<any> = [];
    delCost: UntypedFormGroup;
    showDelete: boolean = true;
    aboveValue: any;
    submitted= false;
    submitted2=false;
   currency='';
   isArabic=false;
  
    constructor(
        injector: Injector,
        private _deliveryCostServiceProxy: DeliveryCostServiceProxy,
        private _menusServiceProxy: MenusServiceProxy,
        private fb: UntypedFormBuilder,
        public darkModeService: DarkModeService,
    ) { super(injector); }



    ngOnInit() {
        this.theme = ThemeHelper.getTheme();
        this.isArabic = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        this.delCost = this.fb.group({
            'cost': this.fb.array([this.initCost()])
        })
        this.dropdownSettings = {
            singleSelection: false,
            idField: 'id',
            textField: 'name',
            itemsShowLimit: 3,
            allowSearchFilter: false,
            maxHeight: 200,
            closeDropDownOnSelection: true
        };
    }
    initCost() {
        return this.fb.group({
            'from': [0],
            'to': [],
            'value': []
        });
    }

    addCost2(item: any) {
        return this.fb.group({
            'from': [item.to],
            'to': [],
            'value': []
        });


    }


    addDeliveryCost() {
        this.submitted=false;
        this.submitted2=false;
        let idx = 0;
        this.saveButton= true;
        const control = <UntypedFormArray>this.delCost.controls['cost'];
        if (this.delCost.value.cost.length - 1 == -1) {
            idx = 0;
        } else {
            idx = this.delCost.value.cost.length - 1;
        
        }

        let item = control.at(idx).getRawValue();

        const del = document.getElementById("Delete" + (idx));
        if (item.to != null && item.from != null && (item.to > item.from)   ) {
            if (item.value != null) {                
                this.minValue = item.to + 1;
                control.push(this.addCost2(item));
                (<UntypedFormArray>this.delCost.controls['cost']).controls[idx].get('to').disable();
                (<UntypedFormArray>this.delCost.controls['cost']).controls[idx].get('value').disable();
                del.setAttribute("hidden", "");
            }else{
                // this.message.error("", this.l("Please Enter Delivery Cost"));
                this.submitted2=true;
                return;
            }
        }else{
            // this.message.error("", this.l("To Must be grater than From"));            
            this.submitted2=true;
            return;
        }

    }


    save(): void {
        this.saving = true;
        this.selectedAreaIds;
        if(this.selectedAreaIds == null || this.selectedAreaIds.length == 0 || this.deliveryCost.aboveValue == null ||
            this.deliveryCost.aboveValue == undefined ||  this.selectedAreaIds == undefined ){
            this.submitted=true;
            this.saving=false;
            return;
        }
        const control = (<UntypedFormArray>this.delCost.controls['cost']) as UntypedFormArray;
      let  lastindex= control.length -1;
        let item = control.value[lastindex];
     
            if(item.from === null || item.from === undefined || item.from === '' ||
            item.to ===null ||  item.to === undefined || item.to === '' ||
            item.value === null ||   item.value === undefined ||   item.value === '' 
            ){
                 this.submitted2=true;
            this.saving=false;
            return;
            }
    
      
            if (this.deliveryCost.id == 0 || this.deliveryCost.id == null) {


                var date = new Date();

                this.deliveryCost.tenantId = this.appSession.tenant.id;

                this.deliveryCost.createdBy = this.appSession.user.name;
                this.deliveryCost.areaIds = this.selectedAreaIds.filter(f => f.id > 0).map(({ id }) => id).toString();
            
                this.lstDeliveryCostDetailsDto = this.delCost.getRawValue().cost;
                this.deliveryCost.lstDeliveryCostDetailsDto = new Array<DeliveryCostDetailsDto>();
                this.lstDeliveryCostDetailsDto.forEach(element => {

                    var objDeliveryCostDetailsDto = new DeliveryCostDetailsDto();
                    objDeliveryCostDetailsDto.from = element.from;
                    objDeliveryCostDetailsDto.to = element.to;
                    objDeliveryCostDetailsDto.value = element.value;
                    this.deliveryCost.lstDeliveryCostDetailsDto.push(objDeliveryCostDetailsDto);
                });



                this._deliveryCostServiceProxy.addDeliveryCost(this.deliveryCost).subscribe(() => {

                    this.notify.info(this.l('savedSuccessfully'));
                    this.saving=false;
                    this.submitted=false;
                    this.submitted2=false;
                    this.close();
                    this.modalSave.emit(null);

                });
            }
            else {

                this.deliveryCost.modifiedBy = this.appSession.user.name;
                this.deliveryCost.areaIds = this.selectedAreaIds.filter(f => f.id > 0).map(({ id }) => id).toString();
                this.lstDeliveryCostDetailsDto = this.delCost.getRawValue().cost;
                this.deliveryCost.lstDeliveryCostDetailsDto = new Array<DeliveryCostDetailsDto>();

                this.lstDeliveryCostDetailsDto.forEach(element => {
                    var objDeliveryCostDetailsDto = new DeliveryCostDetailsDto();
                    objDeliveryCostDetailsDto.from = element.from;
                    objDeliveryCostDetailsDto.to = element.to;
                    objDeliveryCostDetailsDto.value = element.value;
                    this.deliveryCost.lstDeliveryCostDetailsDto.push(objDeliveryCostDetailsDto);
                });
                this._deliveryCostServiceProxy.updateDeliveryCost(this.deliveryCost)
                    .pipe(finalize(() => { this.saving = false; }))
                    .subscribe(() => {
                        this.notify.info(this.l('savedSuccessfully'));
                        this.submitted=true;
                        this.saving=false;
                        this.close();
                        this.modalSave.emit(null);
                    });
            }

    }
    deleteDeliveryCost(index) {
        const control = (<UntypedFormArray>this.delCost.controls['cost']) as UntypedFormArray;
        let item = control.at(index).value;
        if (item.id) {
            this.message.confirm(
                '',
                this.l('AreYouSure'),
                (isConfirmed) => {
                    if (isConfirmed) {
                        this._deliveryCostServiceProxy.deleteDeliveryCost(item.id)
                            .subscribe(() => {
                                this.notify.success(this.l('successfullyDeleted'));
                                control.removeAt(index);
                            });
                    }
                }
            );
        } else {
            if (index == 0) {
                const del = document.getElementById("Delete" + (index)).setAttribute("disabled", "");
            }
            if (index != 0) {
                control.removeAt(index);
                let prevIndex= index-1;
                let item = control.at(prevIndex).getRawValue();

                (<UntypedFormArray>this.delCost.controls['cost']).controls[prevIndex].get('to').enable();          
                (<UntypedFormArray>this.delCost.controls['cost']).controls[prevIndex].get('value').enable();          
                const del = document.getElementById("Delete" + (index - 1));
                del.removeAttribute("hidden");

            }

        }
    }
    show() {
        this.saveButton = true
        this.minValue=0;
        this.currency = this.appSession.tenant.currencyCode;
        this.ngOnInit();
        this.deliveryCost = new DeliveryCostDto();
        this.selectedAreaIds = null
        this._menusServiceProxy.getRType(this.appSession.tenantId).
            subscribe(result => {
                this.rType = result.filter(x => x.id != 0)
                this.active = true;
                this.modal.show();
            });

    }
    checkTo(e, ix) {
        if (e.target.value <= this.delCost.controls['cost'].value[ix].from) {
            // this.message.error("", this.l("To Must be grater than From"));
            this.submitted2=true;
            return;
        }else{
            if (this.delCost.controls['cost'].value[ix].value != null) {
                this.saveButton= false;
            }
        }
    }
    checkDeliveryCost(e,ix) {
        if (this.delCost.controls['cost'].value[ix].value == null) {
            // this.message.error("", this.l("Please Enter Delivery Cost"));
            this.submitted=true;
            this.saveButton= true;
            return
        }else{
            if (this.delCost.controls['cost'].value[ix].to > this.delCost.controls['cost'].value[ix].from) {
                this.saveButton= false;
                this.submitted=false;
            }
        }
    }

    bindCost(cfrom, cto, cvalue) {

        return this.fb.group({
            'from': [cfrom, [Validators.required]],
            'to': [cto, [Validators.required]],
            'value': [cvalue, [Validators.required]]
        });
    }

    showEdit(record: DeliveryCostDto): void {
        this.ngOnInit();
        this.saveButton = false;
        this.currency = this.appSession.tenant.currencyCode;
        this._menusServiceProxy.getRType(this.appSession.tenantId).
            subscribe(result => {
                this.rType = result.filter(x => x.id != 0)
                this.selectedAreaIds = [];
                if (this.deliveryCost.areaIds != undefined) {
                    var array = this.deliveryCost.areaIds.split(',')
                    array.forEach(element => {
                        var branch = this.rType.find(x => x.id == parseInt(element));
                        if(branch){
                            this.selectedAreaIds.push(branch)
                          }
                    });
                }
            });

        //this.deliveryCost.lstDeliveryCostDetailsDto = new Array<DeliveryCostDetailsDto>();

        let data = [];
        record.lstDeliveryCostDetailsDto.forEach(element => {
         data.push(this.bindCost(element.from, element.to, element.value));
        });

        this.delCost = this.fb.group({ 'cost': this.fb.array(data), });
        for(let i=  0 ; i <  (<UntypedFormArray>this.delCost.controls['cost']).controls.length-1 ; i++){
                    (<UntypedFormArray>this.delCost.controls['cost']).controls[i].get('to').disable();
                    (<UntypedFormArray>this.delCost.controls['cost']).controls[i].get('value').disable();
                    const del = document.getElementById("Delete" + (i));
                    del.setAttribute("hidden", "");

                }
                // let i= (<UntypedFormArray>this.delCost.controls['cost']).controls.length-1;
                // (<UntypedFormArray>this.delCost.controls['cost']).controls[i].get('to').enable();

        this.deliveryCost.aboveValue = record.aboveValue;
        this.deliveryCost = record;
        this.modal.show();
    }


    close(): void {
        this.active = false;
        this.saving=false;
        this.submitted=false;
        this.submitted2=false;
        this.modalSave.emit(null);
        this.modal.hide();
    }





}
