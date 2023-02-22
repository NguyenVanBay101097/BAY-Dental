import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentDetailDialogComponent } from './appointment-detail-dialog.component';

describe('AppointmentDetailDialogComponent', () => {
  let component: AppointmentDetailDialogComponent;
  let fixture: ComponentFixture<AppointmentDetailDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentDetailDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
