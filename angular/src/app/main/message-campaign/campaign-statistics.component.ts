import {
    Component,
    EventEmitter,
    Injector,
    OnInit,
    Output,
    ViewChild,
    inject,
} from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
    CampaignStatisticsDto,
    TenantDashboardServiceProxy,
    WhatsAppCampaignModel,
    WhatsAppMessageTemplateServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
export const colors = {
    solid: {
        primary: "#7367F0",
        secondary: "#82868b",
        success: "#28C76F",
        info: "#00cfe8",
        warning: "#FF9F43",
        danger: "#EA5455",
        dark: "#4b4b4b",
        black: "#000",
        white: "#fff",
        body: "#f8f8f8",
    },
    light: {
        primary: "#7367F01a",
        secondary: "#82868b1a",
        success: "#28C76F1a",
        info: "#00cfe81a",
        warning: "#FF9F431a",
        danger: "#EA54551a",
        dark: "#4b4b4b1a",
    },
};
@Component({
    selector: "campaignStatistics",
    templateUrl: "./campaign-statistics.component.html",
    styleUrls: ["./campaign-statistics.component.css"],
})
export class CampaignStatisticsComponent extends AppComponentBase {
    @ViewChild("campaignStatistics", { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild("campaignStatistics", { static: true })
    campaignStatistics: CampaignStatisticsComponent;
    campaignDetails = new WhatsAppCampaignModel();
    campaignDetails2 = new CampaignStatisticsDto();
    whatsAppMessageTemplateServiceProxy: WhatsAppMessageTemplateServiceProxy =
        inject(WhatsAppMessageTemplateServiceProxy);

    showCharts = false;
    public sentChartoptions;
    public hangingChartoptions;
    public deliverdChartoptions;
    public readChartoptions;
    public failedChartoptions;
    public repliedChartoptions;

    // Private
    private $barColor = "#f3f3f3";
    private $trackBgColor = "#EBEBEB";
    private $textMutedColor = "#b9b9c3";
    private $budgetStrokeColor2 = "#dcdae3";
    private $goalStrokeColor2 = "#51e5a8";
    private $textHeadingColor = "#5e5873";
    private $strokeColor = "#ebe9f1";
    private $earningsStrokeColor2 = "#28c76f66";
    private $earningsStrokeColor3 = "#28c76f33";

    constructor(injector: Injector) {
        super(injector);

        this.sentChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.primary],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
        this.hangingChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.warning],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
        this.deliverdChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.secondary],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
        this.readChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.info],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
        this.failedChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.danger],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
        this.repliedChartoptions = {
            chart: {
                height: 60,
                width: 60,
                type: "radialBar",
            },
            grid: {
                show: false,
                padding: {
                    left: -15,
                    right: -15,
                    top: -12,
                    bottom: -15,
                },
            },
            colors: [colors.solid.success],
            series: [54.4],
            plotOptions: {
                radialBar: {
                    hollow: {
                        size: "22%",
                    },
                    track: {
                        background: this.$trackBgColor,
                    },
                    dataLabels: {
                        showOn: "always",
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                },
            },
            stroke: {
                lineCap: "round",
            },
        };
    }

    ngOnInit(): void {}

    show(campign: any) {
        this.campaignDetails = campign;
        this.campaignDetails2 = null;
        this.campaignDetails.title=campign.title;
        this.modal.show();

        this.whatsAppMessageTemplateServiceProxy
            .getDetailsWhatsAppCampaign(campign.id)
            .subscribe((res) => {
                this.campaignDetails2 = res;
                this.campaignDetails2.title=campign.title;
                console.log(this.campaignDetails2)
                this.showCharts = true;
              
            });

        // this.showCharts = true;
        // this.campaignDetails = new WhatsAppCampaignModel();
        // this.campaignDetails= campaignDetails;
        // this.modal.show();
    }

    close(): void {
        this.modal.hide();
        this.modalSave.emit(null);
    }
}
