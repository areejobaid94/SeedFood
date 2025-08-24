import {
    Component,
    EventEmitter,
    Injector,
    Input,
    OnInit,
    Output,
    SecurityContext,
    ViewChild,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { ModalDirective } from "ngx-bootstrap/modal";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { PlyrComponent } from "ngx-plyr";
@Component({
    selector: "viewVideo",
    templateUrl: "./video-model.component.html",
    styleUrls: ["./video-model.component.css"],
})
export class VideoModelComponent extends AppComponentBase {
    @ViewChild("viewVideo", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    video: HTMLVideoElement;
    videLink: "";
    isPlay: boolean = false;
    playVideo: any;

    // or get it from plyrInit event
    public plyr: PlyrComponent;
    public player: Plyr;
    public plyrOptions = { tooltips: { controls: true } };

    // video Sources
    public videoSources: Plyr.Source[] = [
        {
            src: "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-576p.mp4",
            type: "video/mp4",
            size: 576,
        },
        {
            src: "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-720p.mp4",
            type: "video/mp4",
            size: 720,
        },
        {
            src: "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1080p.mp4",
            type: "video/mp4",
            size: 1080,
        },
        {
            src: "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1440p.mp4",
            type: "video/mp4",
            size: 1440,
        },
    ];

    constructor(injector: Injector, private sanitizer: DomSanitizer) {
        super(injector);
    }

    ngOnInit(): void {}

    close(): void {
        this.modal.hide();
    }

    show(video) {
        this.videLink = video;
        this.modal.show();
        this.isPlay = true;
        this.modal.onHidden.subscribe(() => {
            // Get the iframe element and cast it to HTMLIFrameElement
            const iframe = document.getElementById("myIframe") as HTMLIFrameElement;
            iframe.src = iframe.src;
        });
        this.videoSources = [
            {
                src: video,
                type: "video/mp4",
                size: 576,
            },
            {
                src: video,
                type: "video/mp4",
                size: 720,
            },
            {
                src: video,
                type: "video/mp4",
                size: 1080,
            },
            {
                src: video,
                type: "video/mp4",
                size: 1440,
            },
        ];
    }
}
