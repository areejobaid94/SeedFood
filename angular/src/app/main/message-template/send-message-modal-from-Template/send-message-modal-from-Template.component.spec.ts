/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';
import { SendMessageModalFromTeamplatComponent } from './send-message-modal-from-Template.component';


describe('SendMessageModalComponent', () => {
  let component: SendMessageModalFromTeamplatComponent;
  let fixture: ComponentFixture<SendMessageModalFromTeamplatComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SendMessageModalFromTeamplatComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SendMessageModalFromTeamplatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
