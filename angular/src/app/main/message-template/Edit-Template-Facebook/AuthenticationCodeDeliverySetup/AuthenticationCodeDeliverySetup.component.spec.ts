/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { AuthenticationCodeDeliverySetupComponent } from './AuthenticationCodeDeliverySetup.component';

describe('AuthenticationCodeDeliverySetupComponent', () => {
  let component: AuthenticationCodeDeliverySetupComponent;
  let fixture: ComponentFixture<AuthenticationCodeDeliverySetupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AuthenticationCodeDeliverySetupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthenticationCodeDeliverySetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
