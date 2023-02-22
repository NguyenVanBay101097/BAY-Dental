import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PopoverStateAppointmentComponent } from './popover-state-appointment.component';

describe('PopoverStateAppointmentComponent', () => {
  let component: PopoverStateAppointmentComponent;
  let fixture: ComponentFixture<PopoverStateAppointmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PopoverStateAppointmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PopoverStateAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
