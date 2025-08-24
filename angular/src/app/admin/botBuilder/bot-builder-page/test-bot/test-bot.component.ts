import {
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    Injector,
    OnInit,
    Output,
    Renderer2,
    ViewChild,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { DarkModeService } from "@app/services/dark-mode.service";
import { AppComponentBase } from "@shared/common/app-component-base";
import { BotFlowServiceProxy } from "@shared/service-proxies/service-proxies";
import { Howl } from "howler";
import { ModalDirective } from "ngx-bootstrap/modal";

@Component({
    selector: "testBot",
    templateUrl: "./test-bot.component.html",
    styleUrls: ["./test-bot.component.scss"],
})
export class TestBotComponent extends AppComponentBase {
    @ViewChild("testBot", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("scrollMe") private myScrollContainer: ElementRef;
    date = "";
    botName = "";
    botId = 0;
    chatForm: FormGroup;
    listOfBotMessages = [];
    visibleMessages: number[] = [];
    showEndBot = false;
    sound: any;
    showTyping = false;
    public safeUrl: SafeResourceUrl;
    constructor(
        injector: Injector,
        public darkModeService: DarkModeService,
        private renderer: Renderer2,
        private el: ElementRef,
        private testBotService: BotFlowServiceProxy,
        private sanitizer: DomSanitizer
    ) {
        super(injector);
    }

    ngOnInit(): void {
        let date = new Date();
        this.date = date.toUTCString();
        this.initForm();
    }
    initForm() {
        this.chatForm = new FormGroup({
            text: new FormControl("", [Validators.required]),
            isVisible: new FormControl(true),
            type: new FormControl("default"),
        });
    }

    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent): void {
        event.stopPropagation(); // prevent this event from reaching the page drag handler
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mousemove", ["$event"])
    onMouseMove(event: MouseEvent): void {
        event.stopPropagation(); // prevent this event from reaching the page drag handler
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mouseup", ["$event"])
    onMouseUp(event: MouseEvent): void {
        event.stopPropagation(); // prevent this event from reaching the page drag handler
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    @HostListener("mouseleave", ["$event"])
    onMouseLeave(event: MouseEvent): void {
        event.stopPropagation(); // prevent this event from reaching the page drag handler
        this.renderer.setStyle(this.el.nativeElement, "cursor", "default");
    }

    show(botData) {
        this.restartTheBot();
        if (botData.getBotFlowForViewDto == undefined) {
            this.notify.error(this.l("Please Save Before Testing"));
            return;
        } else if (botData.getBotFlowForViewDto.length < 2) {
            this.notify.error(this.l("Please Add Nodes"));
            return;
        } else {
            this.listOfBotMessages = [];
            this.modal.show();
            this.showEndBot = false;
            this.botName = botData.flowName;
            this.botId = botData.id;
            this.chatForm.controls["text"].setValue("#");
        }
    }

    restartTheBot(){
        this.testBotService
        .updateBotReStart(this.appSession.tenantId).subscribe((result: any) => {
        });
    }

    close() {
        this.modal.hide();
    }

    async save() {
        if (this.chatForm.valid) {

            this.chatForm.controls["isVisible"].setValue(true);
            this.chatForm.controls["type"].setValue("default");
            this.listOfBotMessages.push(this.chatForm.value);
            this.scrollToBottom();
            let value = this.chatForm.controls["text"].value;
            this.chatForm.reset();
            this.testBotService
                .testBotStart(this.appSession.tenantId, value, this.botId)
                .subscribe(async (result: any) => {
                    if (result.length > 0) {
                        for (let i = 0; i < result.length; i++) {
                            this.listOfBotMessages.push(result[i]);
                            this.scrollToBottom();
                            if (result[i].type === "ping") {
                                await new Promise(resolve => setTimeout(resolve, result[i].speak));
                            }
                            if (result[i].type === "ping") {
                                let index = this.listOfBotMessages.findIndex(x => x.id === result[i].id);
                                this.listOfBotMessages.splice(-1);
                            }
                            result[i].isVisible = true;
                            if (this.sound != null) {
                                this.sound.stop();
                                this.sound.unload();
                                this.sound = null;
                            }

                            this.sound = new Howl({
                                src: "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/WhatsAppNotification.mp3",
                                html5: true,
                                volume: 1.0,
                            });
                            this.sound.play();
                        }
                    } else {
                        this.showEndBot = true;
                    }
                });
        }
    }
    startConversation() {
        this.chatForm.reset();
        this.listOfBotMessages = [];
        this.showEndBot = false;
        this.restartTheBot();
    }
    chooseButton(button) {
        this.chatForm.controls["text"].setValue(button);
        this.save();
    }

    chatBoxKeyup($event: KeyboardEvent) {
        if ($event.keyCode === 13) {
            $event.preventDefault();
            $event.stopPropagation();
            this.save();
        }
    }

    scrollToBottom(): void {
        setTimeout(() => {
            this.myScrollContainer.nativeElement.scroll({
                top: this.myScrollContainer.nativeElement.scrollHeight,
                left: 0,
                behavior: "smooth",
            });
        }, 100);
    }
    checkIfVideo(url) {
        const extension = url.split('.').pop();
        if (extension === 'mp4') {
            return true;
        }
    }
    checkIfDocOrPdf(url) {
        const extension = url.split('.').pop();
        if (extension === 'pdf' || extension === 'doc') {
            return true;
        }
    }

    maketheUrlSafe(url) {
        return (this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url));
    }
}
