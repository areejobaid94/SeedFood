import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveableHeaderComponent } from './moveable-header.component';

describe('MoveableHeaderComponent', () => {
  let component: MoveableHeaderComponent;
  let fixture: ComponentFixture<MoveableHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MoveableHeaderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MoveableHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
