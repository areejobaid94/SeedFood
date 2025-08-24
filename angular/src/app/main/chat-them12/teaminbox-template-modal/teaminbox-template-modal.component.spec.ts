import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeaminboxTemplateModalComponent } from './teaminbox-template-modal.component';

describe('TeaminboxTemplateModalComponent', () => {
  let component: TeaminboxTemplateModalComponent;
  let fixture: ComponentFixture<TeaminboxTemplateModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeaminboxTemplateModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeaminboxTemplateModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
