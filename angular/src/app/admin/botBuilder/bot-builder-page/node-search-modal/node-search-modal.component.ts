import {
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild,
    ElementRef,
    Renderer2,
} from "@angular/core";
import { GetBotFlowForViewDto } from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";

@Component({
    selector: "app-node-search-modal",
    templateUrl: "./node-search-modal.component.html",
    styleUrls: ["./node-search-modal.component.css"],
})
export class NodeSearchModalComponent implements OnInit {
    @ViewChild("NodeSearch", { static: true }) modal: ModalDirective;
    @Input() Nodes: GetBotFlowForViewDto[] = [];
    @Output() NodePostion = new EventEmitter<number[]>();

    nodesAfterSearch: GetBotFlowForViewDto[] = [];
    searchQuery: string;

    constructor(private el: ElementRef, private renderer: Renderer2) {}

    ngOnInit(): void {}

    close() {
        this.searchQuery="";
        this.search();
        this.modal.hide();
    }
    close2() {
        this.searchQuery = "";
        this.nodesAfterSearch = [];
        this.modal.hide();
    }

    open() {
        this.modal.show();
        if (this.searchQuery.length > 0) {
            this.search();
        }
    }

    onModalMouseDown(event: MouseEvent) {
        // Prevent the mouse down event from propagating to the backdrop
        event.stopPropagation();
    }

    search() {
        const term = this.searchQuery.toLowerCase();
        if (term.length <= 0) {
            this.nodesAfterSearch = [];
            return;
        }
        this.nodesAfterSearch = this.Nodes.filter((item) => {
            return (
                item.captionEn?.toLowerCase().includes(term) ||
                item.captionAr?.toLowerCase().includes(term)
            );
        });
    }

    clickNode(node: GetBotFlowForViewDto) {
        const el = document.getElementById(`content_${node.id}`);
        
        if (el) { // Ensure the element exists
            el.classList.add("flashing");
    
            setTimeout(() => {
                el.classList.remove("flashing");
            }, 2500); // Match this duration with the CSS animation duration
    
            this.NodePostion.emit([node.left, node.top]);
            // this.modal.hide();
        }
    }
    
    
}
