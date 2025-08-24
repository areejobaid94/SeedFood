import { ComponentFixture, TestBed } from '@angular/core/testing';

import { numberinput } from './number-input.component';

describe('NumberInputComponent', () => {
  let component: numberinput;
  let fixture: ComponentFixture<numberinput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ numberinput ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(numberinput);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
