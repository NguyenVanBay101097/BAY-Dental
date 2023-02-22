import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentDateFilterComponent } from './appointment-date-filter.component';

describe('AppointmentDateFilterComponent', () => {
  let component: AppointmentDateFilterComponent;
  let fixture: ComponentFixture<AppointmentDateFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentDateFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentDateFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
