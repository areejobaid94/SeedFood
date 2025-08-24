/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { UserPerformanceTableAppointmentComponent } from './user-performance-table-appointment.component';

describe('UserPerformanceTableAppointmentComponent', () => {
  let component: UserPerformanceTableAppointmentComponent;
  let fixture: ComponentFixture<UserPerformanceTableAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserPerformanceTableAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPerformanceTableAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
