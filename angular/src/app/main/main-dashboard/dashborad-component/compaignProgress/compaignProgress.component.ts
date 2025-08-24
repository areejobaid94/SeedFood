import { Component, OnInit } from "@angular/core";
import { NgbProgressbarModule } from '@ng-bootstrap/ng-bootstrap';


// import * as snippet from "app/main/components/progress/progress.snippetcode";

@Component({
    selector: "app-compaignProgress",
    templateUrl: "./compaignProgress.component.html",
    standalone: true,
	imports: [NgbProgressbarModule],
})
export class CompaignProgressComponent implements OnInit {
    // public
    public contentHeader: object;
    public progressbarHeight = ".857rem";
    progressValue = 89;

    // // snippet code variables
    // public _snippetCodeBasicProgress = snippet.snippetCodeBasicProgress;
    // public _snippetCodeColoredProgress = snippet.snippetCodeColoredProgress;
    // public _snippetCodeLabeledProgress = snippet.snippetCodeLabeledProgress;
    // public _snippetCodeStripedProgress = snippet.snippetCodeStripedProgress;
    // public _snippetCodeAnimatedProgress = snippet.snippetCodeAnimatedProgress;

    constructor() {}

    // Lifecycle Hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // content header
        this.contentHeader = {
            headerTitle: "Progress",
            actionButton: true,
            breadcrumb: {
                type: "",
                links: [
                    {
                        name: "Home",
                        isLink: true,
                        link: "/",
                    },
                    {
                        name: "Components",
                        isLink: true,
                        link: "/",
                    },
                    {
                        name: "Progress",
                        isLink: false,
                    },
                ],
            },
        };
    }
}
