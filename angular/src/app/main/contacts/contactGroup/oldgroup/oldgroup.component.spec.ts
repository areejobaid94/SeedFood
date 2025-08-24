/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { OldgroupComponent } from './oldgroup.component';

describe('OldgroupComponent', () => {
  let component: OldgroupComponent;
  let fixture: ComponentFixture<OldgroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OldgroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OldgroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
