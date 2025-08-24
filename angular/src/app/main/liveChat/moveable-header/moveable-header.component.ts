import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-moveable-header',
  templateUrl: './moveable-header.component.html',
  styleUrls: ['./moveable-header.component.css']
})
export class MoveableHeaderComponent implements OnInit {
  @Input() isActive: boolean = true;
  @Input() body: string = "";
  @Input() InnerClass;

  constructor() { }

  ngOnInit(): void {
  }

}
