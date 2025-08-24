import {Component, Injector, OnInit} from '@angular/core';
import {FileUploader, FileUploaderOptions, FileItem} from 'ng2-file-upload';
import {AppComponentBase} from "@shared/common/app-component-base";
import {ProfileServiceProxy} from "@shared/service-proxies/service-proxies";
import {IAjaxResponse, TokenService} from "@node_modules/abp-ng2-module";
import {AppConsts} from "@shared/AppConsts";
import {appModuleAnimation} from "@shared/animations/routerTransition";

@Component({
    templateUrl: './file-upload.component.html',
    animations: [appModuleAnimation()]
})
export class FileUploadComponent extends AppComponentBase implements OnInit {
    public uploader: FileUploader;
    private _uploaderOptions: FileUploaderOptions = {};
    description: string;

    constructor(
        injector: Injector,
        private _profileService: ProfileServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.initFileUploader();
    }

    initFileUploader(): void {
        this.uploader = new FileUploader({url: AppConsts.remoteServiceBaseUrl + '/Chat/UploadFile'});
        this._uploaderOptions.autoUpload = false;
        this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        this._uploaderOptions.removeAfterUpload = true;
        this.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        this.uploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
            form.append('Description', this.description);
        };

        this.uploader.onSuccessItem = (item, response, status) => {
            const resp = <IAjaxResponse>JSON.parse(response);
            if (resp.success) {
                this.message.success(this.l("FileSavedSuccessfully", response));
            } else {
                this.message.error(resp.error.message);
            }
        };

        this.uploader.setOptions(this._uploaderOptions);
    }

    save(): void {
        this.uploader.uploadAll();
    }

    fileChangeEvent(event: any): void {
        this.uploader.clearQueue();
        this.uploader.addToQueue([event.target.files[0]]);
    }
}