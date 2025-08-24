import { ComponentFixture, TestBed, waitForAsync as  } from '@angular/core/testing';

import { TenantsAccountBillingsComponent } from './tenants-account-billings.component';

describe('TenantsAccountBillingsComponent', () => {
  let component: TenantsAccountBillingsComponent;
  let fixture: ComponentFixture<TenantsAccountBillingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantsAccountBillingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantsAccountBillingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
