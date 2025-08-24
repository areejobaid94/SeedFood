/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SkeleltonComponent } from './skelelton.component';

describe('SkeleltonComponent', () => {
  let component: SkeleltonComponent;
  let fixture: ComponentFixture<SkeleltonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SkeleltonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SkeleltonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
