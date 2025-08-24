/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { UserPeroformanceTableTicketComponent } from './user-peroformance-table-ticket.component';

describe('UserPeroformanceTableTicketComponent', () => {
  let component: UserPeroformanceTableTicketComponent;
  let fixture: ComponentFixture<UserPeroformanceTableTicketComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserPeroformanceTableTicketComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPeroformanceTableTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
