import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstagramConnectComponent } from './instagram-connect.component';

describe('InstagramConnectComponent', () => {
  let component: InstagramConnectComponent;
  let fixture: ComponentFixture<InstagramConnectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InstagramConnectComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstagramConnectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
