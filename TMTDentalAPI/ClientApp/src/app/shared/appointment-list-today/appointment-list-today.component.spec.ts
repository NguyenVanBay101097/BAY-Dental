import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentListTodayComponent } from './appointment-list-today.component';

describe('AppointmentListTodayComponent', () => {
  let component: AppointmentListTodayComponent;
  let fixture: ComponentFixture<AppointmentListTodayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentListTodayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentListTodayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
