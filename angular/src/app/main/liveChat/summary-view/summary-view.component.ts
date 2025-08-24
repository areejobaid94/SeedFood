import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-summary-view',
  templateUrl: './summary-view.component.html',
  styleUrls: ['./summary-view.component.css']
})
export class SummaryViewComponent implements OnInit, OnChanges {

  Summary: string = "";
  @Input() SummaryBody: string = "";
  isPerant = true;
  isInner = false;

  constructor() { }

  ngOnInit(): void {
    this.updateSummary();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['SummaryBody']) {
      this.updateSummary();
    }
  }

  private updateSummary(): void {
    if (this.SummaryBody.length >= 7) {
      this.Summary = this.SummaryBody.slice(0, 7) + "...";
    } else {
      this.Summary = this.SummaryBody;
    }
  }

  handelClick(): void {
    this.isPerant = !this.isPerant;
    this.isInner = !this.isInner;
  }
}
