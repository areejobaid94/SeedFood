import { ComponentFixture, TestBed, waitForAsync  } from '@angular/core/testing';

import { Sidebarthem12Component } from './sidebarthem12.component';

describe('Sidebarthem12Component', () => {
  let component: Sidebarthem12Component;
  let fixture: ComponentFixture<Sidebarthem12Component>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ Sidebarthem12Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Sidebarthem12Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
