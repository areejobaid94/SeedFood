import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryLocationComponent } from './location.component';

describe('LocationComponent', () => {
  let component: DeliveryLocationComponent;
  let fixture: ComponentFixture<DeliveryLocationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeliveryLocationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeliveryLocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
