import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentAdvanceSearchComponent } from './appointment-advance-search.component';

describe('AppointmentAdvanceSearchComponent', () => {
  let component: AppointmentAdvanceSearchComponent;
  let fixture: ComponentFixture<AppointmentAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
