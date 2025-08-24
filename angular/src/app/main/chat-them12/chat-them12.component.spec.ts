import { ComponentFixture, TestBed, waitForAsync  } from '@angular/core/testing';

import { ChatThem12Component } from './chat-them12.component';

describe('ChatThem12Component', () => {
  let component: ChatThem12Component;
  let fixture: ComponentFixture<ChatThem12Component>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatThem12Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatThem12Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
