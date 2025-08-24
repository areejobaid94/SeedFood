import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookAuthDialogComponent } from './facebook-auth-dialog.component';

describe('FacebookAuthDialogComponent', () => {
  let component: FacebookAuthDialogComponent;
  let fixture: ComponentFixture<FacebookAuthDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FacebookAuthDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FacebookAuthDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
