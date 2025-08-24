import { Component, ViewChild, Injector, Output,Input, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
    TemplateMessagesServiceProxy, CreateOrEditTemplateMessageDto, TemplateMessageTemplateMessagePurposeLookupTableDto
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { AppConsts } from "@shared/AppConsts";
import { IAjaxResponse, TokenService } from "@node_modules/abp-ng2-module";
import { HttpClient } from "@angular/common/http";
import { ThemeHelper } from '../../../shared/layout/themes/ThemeHelper';
import { DarkModeService } from './../../../services/dark-mode.service';


@Component({
    selector: 'createOrEditTemplateMessageModal',
    templateUrl: './create-or-edit-templateMessage-modal.component.html'
})
export class CreateOrEditTemplateMessageModalComponent extends AppComponentBase {
    theme:string;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Input() timp :any;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    url: any;
    active = false;
    saving = false;
    submitted=false;

    templateMessage: CreateOrEditTemplateMessageDto = new CreateOrEditTemplateMessageDto();

    templateMessagePurposePurpose = '';

    allTemplateMessagePurposes: TemplateMessageTemplateMessagePurposeLookupTableDto[];

    public uploader: FileUploader;
    private _uploaderOptions: FileUploaderOptions = {};

    constructor(
        injector: Injector,
        private _templateMessagesServiceProxy: TemplateMessagesServiceProxy,
        private _tokenService: TokenService,
        private http: HttpClient,
        public darkModeService : DarkModeService,
    ) {
        super(injector);
    }
    ngOnInit(): void {
    
        this.theme= ThemeHelper.getTheme();
    }


    show(templateMessageId?: number): void {


        if (!templateMessageId) {
            this.templateMessage = new CreateOrEditTemplateMessageDto();
            this.templateMessage.id = templateMessageId;
            this.templateMessage.messageCreationDate = moment().startOf('day');
            this.templateMessagePurposePurpose = '';
            this.url="";
            this.active = true;
            this.modal.show();
        } else {
            this._templateMessagesServiceProxy.getTemplateMessageForEdit(templateMessageId).subscribe(result => {
                this.templateMessage = result.templateMessage;
                this.templateMessagePurposePurpose = result.templateMessagePurposePurpose;
                this.http.get(AppConsts.remoteServiceBaseUrl + '/Chat/GetUploadedObject?fileId=' + 
                this.templateMessage.attachmentId)
                .subscribe();
                 this.active = true;  

                this.url="https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Image_created_with_a_mobile_phone.png/1200px-Image_created_with_a_mobile_phone.png";
                
                 this.modal.show();


               
               
            });
        }
        this._templateMessagesServiceProxy.getAllTemplateMessagePurposeForTableDropdown().subscribe(result => {
            this.allTemplateMessagePurposes = result;
        });

    }

    initFileUploader(): void {
        this.uploader = new FileUploader({url: AppConsts.remoteServiceBaseUrl + '/Chat/UploadFile'});
        this._uploaderOptions.autoUpload = false;
        this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        this._uploaderOptions.removeAfterUpload = true;
        this.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        this.uploader.setOptions(this._uploaderOptions);
    }
    
    handleFileInput(event: any): void {
        this.initFileUploader();
        this.uploader.clearQueue();
        this.uploader.addToQueue([event.target.files[0]]);
        if (event.target.files && event.target.files[0]) {
            var reader = new FileReader();
            
            reader.readAsDataURL(event.target.files[0]); // read file as data url
      
            reader.onload = (event) => { // called once readAsDataURL is completed
              this.url = event.target.result;
            }
          }
    }

    save(): void {
        this.saving = true;

        if(this.templateMessage.templateMessageName === null || this.templateMessage.templateMessageName === undefined || this.templateMessage.templateMessageName === '' ||
        this.templateMessage.messageText === null || this.templateMessage.messageText === undefined || this.templateMessage.messageText === ''
        ){
            this.submitted = true;
            this.saving =false;
            return;
        }
        this. templateMessage.templateMessagePurposeId=1;

        
        if (this.uploader) {
            this.uploader.uploadAll();
            this.uploader.onSuccessItem = (item, response) => {
                
                const resp = <IAjaxResponse>JSON.parse(response);
                this.templateMessage.attachmentId = resp.result.id;
                this._templateMessagesServiceProxy.createOrEdit(this.templateMessage)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('savedSuccessfully'));
                    this.submitted = false;
                    this.saving=false;
                    this.close();
                    this.modalSave.emit(null);
                });
            };
            
        } else {
            
            this._templateMessagesServiceProxy.createOrEdit(this.templateMessage)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                
                this.notify.info(this.l('savedSuccessfully'));
                this.submitted = false;
                this.saving=false;
                this.close();
                this.modalSave.emit(null);
            });
        }
        
    }

    close(): void {
        this.active = false;
        this.submitted = false;
        this.saving=false;
        this.modal.hide();
    }
}
