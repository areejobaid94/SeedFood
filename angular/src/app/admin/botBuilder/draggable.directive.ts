import {
    Directive,
    ElementRef,
    EventEmitter,
    HostListener,
    Input,
    NgZone,
    Output,
    Renderer2,
} from "@angular/core";
import { GetBotFlowForViewDto } from "@shared/service-proxies/service-proxies";

@Directive({
    selector: "[appDraggable]",
})
export class DraggableDirective {
    @Output() onDrag: EventEmitter<any> = new EventEmitter<any>();
    @Input() node = new GetBotFlowForViewDto();
    private isDragging = false;
    private startX: number = 0;
    private startY: number = 0;
    private initialX: number = 0;
    private initialY: number = 0;
    static highestZIndex = 0;
    private onMouseMoveBound = this.onMouseMove.bind(this);
    private onMouseUpBound = this.onMouseUp.bind(this);

    constructor(
        private el: ElementRef,
        private renderer: Renderer2,
        private ngZone: NgZone
    ) {
        this.initializePosition();
    }

    private initializePosition() {
        const nodes = document.querySelectorAll(".node-root");
        const nodeCount = nodes.length;

        let topPosition = this.node.top;
        let leftPosition = this.node.left;

        this.renderer.setStyle(this.el.nativeElement, "position", "absolute");
        this.renderer.setStyle(
            this.el.nativeElement,
            "top",
            `${topPosition}px`
        );
        this.renderer.setStyle(
            this.el.nativeElement,
            "left",
            `${leftPosition}px`
        );
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grab");
    }

    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent): void {
        if (
            this.isUndraggble(event) ||
            this.isUndraggbleHiddenelemenmts(event)
        ) {
            this.isDragging = false;
            return;
        }
        
        this.isDragging = true;
        this.startX = event.clientX;
        this.startY = event.clientY;
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grabbing");

        DraggableDirective.highestZIndex++;
        this.renderer.setStyle(
            this.el.nativeElement,
            "z-index",
            DraggableDirective.highestZIndex.toString()
        );

        this.resetZIndex();

        document.addEventListener("mousemove", this.onMouseMoveBound, true);
        document.addEventListener("mouseup", this.onMouseUpBound, true);

        event.preventDefault();
        event.stopPropagation(); // Stop the event from bubbling up
    }

    private onMouseUp(event: MouseEvent): void {
        console.log("i am called from Draggable");
        this.isDragging = false;
        this.renderer.setStyle(this.el.nativeElement, "cursor", "grab");
        DraggableDirective.highestZIndex++;
        this.renderer.setStyle(
            this.el.nativeElement,
            "z-index",
            DraggableDirective.highestZIndex.toString()
        );

        this.resetZIndex();

        document.removeEventListener("mousemove", this.onMouseMoveBound, true);
        document.removeEventListener("mouseup", this.onMouseUpBound, true);

        event.preventDefault();
        event.stopPropagation();
    }

    @HostListener("mouseleave", ["$event"])
    onMouseLeave(event: MouseEvent): void {
        if (this.isDragging) {
            this.renderer.setStyle(this.el.nativeElement, "cursor", "grabbing");
        }
    }

    private onMouseMove(event: MouseEvent): void {
        if (!this.isDragging) return;
        event.stopPropagation();

        this.ngZone.runOutsideAngular(() => {
            requestAnimationFrame(() => {
                const dx = event.clientX - this.startX;
                const dy = event.clientY - this.startY;

                this.initialX = this.node.left + dx;
                this.initialY = this.node.top + dy;

                const newTransform = `translate(${this.initialX}px, ${this.initialY}px)`;
                this.renderer.setStyle(
                    this.el.nativeElement,
                    "transform",
                    newTransform
                );

                this.startX = event.clientX;
                this.startY = event.clientY;

                this.node.left = this.initialX;
                this.node.top = this.initialY;
            });
        });

        this.ngZone.run(() => {
            this.onDrag.emit({
                x: this.initialX,
                y: this.initialY,
                mouseEvent: event,
            });
        });

        // Prevent default behavior to avoid document movement
        event.preventDefault();
        event.stopPropagation();
    }

    private resetZIndex(): void {
        const allNodes = document.querySelectorAll(".node-root");
        allNodes.forEach((node) => {
            if (node !== this.el.nativeElement) {
                this.renderer.setStyle(node, "z-index", "1");
            }
        });
    }

    private isUndraggble(event: MouseEvent): boolean {
        const undraggble = this.el.nativeElement.querySelector(".undraggble");
        return undraggble != null && undraggble.contains(event.target);
    }

    private isUndraggbleHiddenelemenmts(event: MouseEvent): boolean {
        let target = event.target as HTMLElement;
        while (target && target !== this.el.nativeElement) {
            if (target.classList.contains("undraggble-hidden-elements")) {
                return true;
            }
            target = target.parentElement;
        }
        return false;
    }
}
