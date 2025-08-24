import { Component, ViewChild,Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { base64ToFile, ImageCroppedEvent } from 'ngx-image-cropper';


@Component({
    selector: 'ImageCroppers',
    templateUrl: "./image-croppers.component.html",

})
export class ImageCrppersComponent extends AppComponentBase {
    @ViewChild('ImageCrppersComponent', { static: true }) modal: ModalDirective;
    imageChangedEvent: any = '';
    uploader: any;

    constructor(
        injector: Injector,
    ) {
        super(injector);

    }

    ngOnInit(): void {

    }
    show(): void {
        this.modal.show();
        this.imageChangedEvent = event;

    }
    imageCroppedFile(event: ImageCroppedEvent) {
        
        this.uploader.clearQueue();
        this.uploader.addToQueue([<File>base64ToFile(event.base64)]);
    }
    close(): void {
        this.modal.hide();
      }
}


