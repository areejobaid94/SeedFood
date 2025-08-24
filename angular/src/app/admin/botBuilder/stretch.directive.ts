import {
    Directive,
    ElementRef,
    Renderer2,
    OnInit,
    OnDestroy,
    HostListener,
    Input,
    Output,
    EventEmitter,
} from "@angular/core";

@Directive({
    selector: "[appStretchNode]",
})
export class StretchNodeDirective implements OnInit, OnDestroy {
    @Input() nodeIdentifier: any | null = null;
    @Input() buttonIdentifier: any | null = null;
    @Output() stretchResult = new EventEmitter<{
        parent: any;
        parentButton: any;
        dropNodeId: any;
    }>();
    @Output() dragStateChange = new EventEmitter<boolean>();

    private tempPathLine: SVGElement | null = null;
    private isDragging = false;
    private isDragEvent = false;
    private svgStartX: number;
    private svgStartY: number;
    private svgContainer: SVGElement | null = null;
    private boundOnMouseUp: any;
    private boundOnPathDraw: any;
    private clickTimeout?: any;
    private hasMoved = false;
    private initialMouseX: number;
    private initialMouseY: number;

    private clickCount = 0; // Add a counter to keep track of clicks

    constructor(private el: ElementRef, private renderer: Renderer2) {}

    ngOnInit() {
    }

    ngOnDestroy() {
        document.removeEventListener("mouseup", this.boundOnMouseUp, true);
        if (this.isDragging) {
            document.removeEventListener(
                "mousemove",
                this.boundOnPathDraw,
                true
            );
        }
    }

    @HostListener("dragstart", ["$event"])
    onDragStart(event: Event) {
        this.isDragEvent = true; // Set a flag to indicate a drag event has started
        this.isDragging = true;
        this.dragStateChange.emit(this.isDragging);
        if (this.clickTimeout) {
            clearTimeout(this.clickTimeout);
        }
    }

    @HostListener("dragend", ["$event"])
    onDragEnd(event: Event) {
        this.isDragEvent = false; // Reset the drag event flag when drag ends
        this.isDragging = false;
        this.dragStateChange.emit(this.isDragging);
    }

    @HostListener("mousedown", ["$event"])
    onMouseDown(event: MouseEvent): void {
        event.stopPropagation();
        this.clickCount += 1;
        this.initialMouseX = event.clientX;
        this.initialMouseY = event.clientY;
        this.boundOnMouseUp = this.onMouseUp.bind(this);
        document.addEventListener("mouseup", this.boundOnMouseUp, true);

        this.clickTimeout = setTimeout(() => {
            if (!this.isDragging && this.clickCount === 1) {
                event.stopPropagation();
                event.preventDefault();
                this.svgStartX = event.clientX;
                this.svgStartY = event.clientY;
                this.initiatePathDrawing(this.svgStartX, this.svgStartY);
                this.boundOnPathDraw = this.onPathDraw.bind(this);
                document.addEventListener(
                    "mousemove",
                    this.boundOnPathDraw,
                    true
                );
                this.dragStateChange.emit(false);
            }
        }, 500);
    }

    private initiatePathDrawing(startX, startY) {
        if (this.isDragging) return;
        this.isDragging = true;
        this.dragStateChange.emit(this.isDragging);
        this.tempPathLine = this.renderer.createElement("path", "svg");
        this.renderer.setAttribute(this.tempPathLine, "stroke", "#b6b9bb");
        this.renderer.setAttribute(this.tempPathLine, "fill", "none");
        this.renderer.setAttribute(this.tempPathLine, "stroke-width", "3");

        // create an svg container for the path
        const svgContainer = this.renderer.createElement("svg", "svg");
        this.renderer.setAttribute(svgContainer, "width", "100%");
        this.renderer.setAttribute(svgContainer, "height", "100%");
        this.renderer.setStyle(svgContainer, "position", "absolute");
        this.renderer.setStyle(svgContainer, "top", "0");
        this.renderer.setStyle(svgContainer, "left", "0");
        this.renderer.setStyle(svgContainer, "pointer-events", "none");
        this.renderer.appendChild(document.body, svgContainer);
        this.svgContainer = svgContainer;

        const rect = svgContainer.getBoundingClientRect();
        const nodeRect = this.el.nativeElement.getBoundingClientRect();

        this.svgStartX = nodeRect.left + nodeRect.width / 2 - rect.left;
        this.svgStartY = nodeRect.top + nodeRect.height - rect.top;
        const svgEndX = startX - rect.left;
        const svgEndY = startY - rect.top;

        const controlPoint1X = this.svgStartX;
        const controlPoint1Y = (this.svgStartY + svgEndY) / 2;
        const controlPoint2X = svgEndX;
        const controlPoint2Y = (this.svgStartY + svgEndY) / 2;

        this.renderer.setAttribute(
            this.tempPathLine,
            "d",
            `M ${this.svgStartX} ${this.svgStartY} C ${controlPoint1X} ${controlPoint1Y}, ${controlPoint2X} ${controlPoint2Y}, ${svgEndX} ${svgEndY}`
        );
        this.renderer.appendChild(svgContainer, this.tempPathLine);

        // Temporarily disable pointer events on the line to correctly identify the element underneath
        this.renderer.setAttribute(this.tempPathLine, "pointer-events", "none");

        // Check for hovered element and change its background color
        const hoveredElement = document.elementFromPoint(startX, startY);
        if (
            hoveredElement &&
            hoveredElement.classList.contains("child-connection") &&
            hoveredElement.classList.contains("top-end-node")
        ) {
            this.renderer.addClass(hoveredElement, "hovered");
        }

        // Re-enable pointer events on the line
        this.renderer.setAttribute(
            this.tempPathLine,
            "pointer-events",
            "visiblePainted"
        );

        document.addEventListener(
            "mousemove",
            this.onPathDraw.bind(this),
            true
        );
    }

    private onPathDraw(e: MouseEvent) {
        if (this.isDragging && !this.hasMoved) {
            if (
                Math.abs(e.clientX - this.initialMouseX) > 5 ||
                Math.abs(e.clientY - this.initialMouseY) > 5
            ) {
                this.hasMoved = true;
            }
        }

        if(this.isDragging && this.hasMoved) {
          const rect = this.el.nativeElement.getBoundingClientRect();

            const svgEndX = e.clientX;
            const svgEndY = e.clientY;

            const controlPoint1X = this.svgStartX;
            const controlPoint1Y = (this.svgStartY + svgEndY) / 2;
            const controlPoint2X = svgEndX;
            const controlPoint2Y = (this.svgStartY + svgEndY) / 2;

            this.tempPathLine.setAttribute(
                "d",
                `M ${this.svgStartX} ${this.svgStartY} C ${controlPoint1X} ${controlPoint1Y}, ${controlPoint2X} ${controlPoint2Y}, ${svgEndX} ${svgEndY}`
            );

            // Reset color of all divs
            const connectors = document.querySelectorAll(".child-connection");
            connectors.forEach((conn) => conn.classList.remove("hovered"));

            // Temporarily hide the SVG container to correctly identify the element underneath
            this.renderer.setStyle(this.svgContainer, "display", "none");

            // Check if the line end point is over any div and change its color
            const hoveredElement = document.elementFromPoint(
                e.clientX,
                e.clientY
            );
            if (
                hoveredElement &&
                hoveredElement.classList.contains("child-connection")
            ) {
                this.renderer.addClass(hoveredElement, "hovered");
            } else {
                // Reset color of all divs
                const connectors =
                    document.querySelectorAll(".child-connection");
                connectors.forEach((conn) => conn.classList.remove("hovered"));
            }

            // Unhide the SVG container
            this.renderer.setStyle(this.svgContainer, "display", "block");
        }
    }

    private onMouseUp(e: MouseEvent) {
        console.log("no way i am called");
        // if (this.clickCount === 1) {
        //   this.clickCount = 0;  // Reset the counter on mouseup
        //   return;
        // }
        this.hasMoved = false;
        if (this.isDragging) {
            this.isDragging = false;
            this.dragStateChange.emit(this.isDragging);
            document.removeEventListener(
                "mousemove",
                this.onPathDraw.bind(this),
                true
            );
            // Temporarily hide the SVG container to correctly identify the element underneath
            if (this.svgContainer) {
                this.renderer.setStyle(this.svgContainer, "display", "none");
            }

            // Check the element under the mouse pointer
            const hoveredElement = document.elementFromPoint(
                e.clientX,
                e.clientY
            );
            if (
                hoveredElement &&
                hoveredElement.classList.contains("child-connection")
            ) {
                const idOfHoveredElement = hoveredElement.getAttribute("id");
                const IdOfNode = idOfHoveredElement.split("_")[1];
                const stretchResult = {
                    parent: this.nodeIdentifier,
                    parentButton: this.buttonIdentifier,
                    dropNodeId: IdOfNode,
                };
                this.stretchResult.emit(stretchResult);
            } else {
                this.renderer.removeChild(this.svgContainer, this.tempPathLine);
            }

            // Delete the temp line
            if (this.tempPathLine) {
                this.renderer.removeChild(this.svgContainer, this.tempPathLine);
            }
            // Delete the svg container
            if (this.svgContainer) {
                this.renderer.removeChild(document.body, this.svgContainer);
            }
            // Reset color of all divs
            const connectors = document.querySelectorAll(".child-connection");
            connectors.forEach((conn) => conn.classList.remove("hovered"));
            document.removeEventListener("mousemove", this.boundOnPathDraw);
        }
        this.clickCount = 0; // Reset the counter at the end of mouseup function
        document.removeEventListener("mouseup", this.boundOnMouseUp, true);
    }
}
