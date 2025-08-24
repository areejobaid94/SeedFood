/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MessageValidityPeriodTemplateComponent } from './MessageValidityPeriodTemplate.component';

describe('MessageValidityPeriodTemplateComponent', () => {
  let component: MessageValidityPeriodTemplateComponent;
  let fixture: ComponentFixture<MessageValidityPeriodTemplateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MessageValidityPeriodTemplateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageValidityPeriodTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
