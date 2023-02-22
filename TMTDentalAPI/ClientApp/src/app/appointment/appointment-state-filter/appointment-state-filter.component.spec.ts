import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentStateFilterComponent } from './appointment-state-filter.component';

describe('AppointmentStateFilterComponent', () => {
  let component: AppointmentStateFilterComponent;
  let fixture: ComponentFixture<AppointmentStateFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentStateFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentStateFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
