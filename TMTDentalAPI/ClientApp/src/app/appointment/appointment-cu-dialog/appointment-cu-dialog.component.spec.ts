import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentCuDialogComponent } from './appointment-cu-dialog.component';

describe('AppointmentCuDialogComponent', () => {
  let component: AppointmentCuDialogComponent;
  let fixture: ComponentFixture<AppointmentCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
