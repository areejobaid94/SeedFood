/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SendtoCompaignToGroupComponent } from './sendtoCompaignToGroup.component';

describe('SendtoCompaignToGroupComponent', () => {
  let component: SendtoCompaignToGroupComponent;
  let fixture: ComponentFixture<SendtoCompaignToGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SendtoCompaignToGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SendtoCompaignToGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
