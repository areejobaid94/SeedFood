import { ComponentFixture, TestBed, waitForAsync  } from '@angular/core/testing';

import { AssignToModalComponent } from './assign-to-modal.component';

describe('AssignToModalComponent', () => {
  let component: AssignToModalComponent;
  let fixture: ComponentFixture<AssignToModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignToModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignToModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
