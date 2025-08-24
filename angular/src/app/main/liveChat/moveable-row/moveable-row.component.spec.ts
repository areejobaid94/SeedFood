import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveableRowComponent } from './moveable-row.component';

describe('MoveableRowComponent', () => {
  let component: MoveableRowComponent;
  let fixture: ComponentFixture<MoveableRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MoveableRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MoveableRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
