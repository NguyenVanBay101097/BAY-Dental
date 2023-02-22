import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentOverCancelComponent } from './appointment-over-cancel.component';

describe('AppontmentOverCancelComponent', () => {
  let component: AppointmentOverCancelComponent;
  let fixture: ComponentFixture<AppointmentOverCancelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AppointmentOverCancelComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentOverCancelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
