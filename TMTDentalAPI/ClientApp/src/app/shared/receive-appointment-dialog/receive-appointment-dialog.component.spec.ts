import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveAppointmentDialogComponent } from './receive-appointment-dialog.component';

describe('ReceiveAppointmentDialogComponent', () => {
  let component: ReceiveAppointmentDialogComponent;
  let fixture: ComponentFixture<ReceiveAppointmentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceiveAppointmentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceiveAppointmentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
